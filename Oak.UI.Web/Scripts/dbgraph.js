
var graphComponent = {
    // properties
    graph: null,
    objects: [],
    metadata: [],
    smoothLinks: true,
    isDragging: false,
    container: null,
    origPaperWidth: 0,
    origPaperHeight: 0,
    panBox: null,
    panBoxPaddingX: 20,
    panBoxPaddingY: 10,
    paper: null,
    lastX: 0,
    lastY: 0,
    viewportMatrix: [1, 0, 0, 1, 0, 0],
    selectedObj: null,
    direction: 'LR',
    // events
    loadDefHandler: null,
    displayMessageHandler: null,
    // actions
    init: function (loadDefHandler, displayMessageHandler) {

        this.loadDefHandler = loadDefHandler;
        this.displayMessageHandler = displayMessageHandler;

        //setup graph api
        this.graph = new joint.dia.Graph;
        this.container = $("#paper");
        this.panBox = $("#pan-box");

        this.origPaperWidth = this.panBox.width();
        this.origPaperHeight = this.panBox.height();

        this.paper = new joint.dia.Paper({
            el: $('#paper'),
            width: (this.origPaperWidth - this.panBoxPaddingX),
            height: (this.origPaperHeight - this.panBoxPaddingY),
            gridSize: 1,
            model: this.graph,
            snapLinks: false
        });

        this.paper.setOrigin(300, 200);

        // resize support
        $(window).resize(this.onWindowResized);

        //zoom support
        this.paper.$el.on('mousewheel DOMMouseScroll', this.onMouseWheel);

        //panning support
        this.paper.on('blank:pointerdown', this.onPointerDown);
        this.paper.on('blank:pointerup', this.onPointerUp);
        $(document).mousemove(this.onMouseMovement);

    },
    displayGraph: function (data, objName) {

        //console.log(data);
        this.objects = data.objects;
        this.metadata = data.metadata;
        this.displayMessageHandler("Rendering tree...");

        try {
            //generate graph from data
            var cells = this.buildGraphFromAdjacencyList(this.objects);
            this.graph.resetCells(cells);
            joint.layout.DirectedGraph.layout(this.graph, { setLinkVertices: false, rankDir: this.direction });

            //count entries in data (they're declared as properties but used as a dictionary, this is due to requisite adjacency list format)
            var objCount = _.size(this.objects);
            var me = this;

            //add css class based on object type
            _.each(cells, function (cell) {
                var element = me.paper.findViewByModel(cell);
                var attrs = element.model.attributes;
                var data = attrs.data;
                if (data === undefined) {
                    return;
                }
                var type = data.type;
                if (type === undefined) {
                    return;
                }

                //apply css class based on type
                V(element.el).addClass("object-base");
                V(element.el).addClass("object-" + type); //type (P, T, V)

                //give tree root special root css class
                if (objName === element.model.id) {
                    V(element.el).addClass("object-root");
                }

                //add doubleclick handler
                $(element.el).dblclick(function (evnt) {

                    //grab inner text which is the object name
                    selectedObj = $(this).find("text").text();

                    //request and display definition
                    me.loadDefHandler(selectedObj);

                });

            });

            // display tree object count
            this.displayMessageHandler(objCount + " item(s) in tree");

            // reset any pan and zoom in viewer
            this.resetViewer();

            this.centerViewerOnRootObject();

        } catch (e) {
            this.displayMessageHandler("Rendering tree failed.");
            throw e;
        }
    },
    buildGraphFromAdjacencyList: function (adjacencyList) {

        var elements = [];
        var links = [];
        var me = this;

        _.each(adjacencyList, function (edges, parentElementLabel) {
            elements.push(me.makeElement(parentElementLabel));

            _.each(edges, function (childElementLabel) {
                links.push(me.makeLink(parentElementLabel, childElementLabel));
            });
        });

        // Links must be added after all the elements. This is because when the links
        // are added to the graph, link source/target
        // elements must be in the graph already.
        return elements.concat(links);
    },
    makeLink: function (parentElementLabel, childElementLabel) {

        var newLink = new joint.dia.Link({
            source: { id: parentElementLabel },
            target: { id: childElementLabel },
            attrs: { '.marker-target': { d: 'M 4 0 L 0 2 L 4 4 z' } },
            smooth: this.smoothLinks
        });

        return newLink;
    },
    makeElement: function (label) {

        var maxLineLength = _.max(label.split('\n'), function (l) { return l.length; }).length;

        //get metadata associated with this db object
        var meta = this.metadata[label];

        // Compute width/height of the rectangle based on the number
        // of lines in the label and the letter size. 0.6 * letterSize is
        // an approximation of the monospace font letter width.
        var letterSize = 11;
        var width = 1.2 * (letterSize * (0.6 * maxLineLength + 1));
        var height = 1.2 * ((label.split('\n').length + 1) * letterSize);

        var attributes = {
            text: { text: label },
            rect: {
                rx: 5, ry: 5
            }
        };

        var rect = new joint.shapes.basic.Rect({
            id: label,
            size: { width: width, height: height },
            attrs: attributes,
            data: { 'type': meta.type }
        });

        return rect;
    },
    applyZoom: function (scale) {
        //console.log("zoom " + scale);

        //scale matrix values
        for (var i = 0; i < this.viewportMatrix.length; i++) {
            this.viewportMatrix[i] *= scale;
        }
        this.viewportMatrix[4] += (1 - scale) * $(this.paper.svg).width() / 2;
        this.viewportMatrix[5] += (1 - scale) * $(this.paper.svg).height() / 2;
        this.updateSvgTransform();
    },
    applyPan: function (deltaX, deltaY) {
        //console.log("pan " + deltaX + ", " + deltaY);

        //update pan on matrix
        this.viewportMatrix[4] += deltaX;
        this.viewportMatrix[5] += deltaY;
        this.updateSvgTransform();
    },
    resetViewer: function () {
        //reset all pan and zoom from matrix
        this.viewportMatrix = [1, 0, 0, 1, 0, 0];
        this.updateSvgTransform();
    },
    centerViewerOnRootObject: function () {

        var screen = $(this.paper.el);
        var graph = V(this.paper.viewport).bbox();

        //center x
        var panX = (screen.width() / 2) - (graph.width / 2);
        //33% from top of available screen
        var panY = (screen.height() / 3) - (graph.height / 2);

        this.applyPan(panX, panY);

    },
    updateSvgTransform: function () {
        //build and update transform matrix
        var txValue = "matrix(" + this.viewportMatrix.join(' ') + ")";
        V(this.paper.viewport).attr('transform', txValue);
    },
    onWindowResized: function () {
        $(graphComponent.paper.svg).width(graphComponent.panBox.width() - graphComponent.panBoxPaddingX);
        $(graphComponent.paper.svg).height(graphComponent.panBox.height() - graphComponent.panBoxPaddingY);
    },
    onMouseWheel: function (e) {

        e.preventDefault();
        e = e.originalEvent;

        var zoomAmount;

        //determine mousewheel delta, is set on diff properties for diff browsers
        if (navigator.userAgent.toLowerCase().indexOf('firefox') > -1) {

            //firefox browsers
            //either +3 / -3, * by 33 to get +99 / -99
            zoomAmount = e.detail * 30;

        } else {

            //non-firefox browsers
            //either +100 / -100
            zoomAmount = e.deltaY;
        }

        var isZoomIn = (zoomAmount < 0);
        graphComponent.applyZoom(isZoomIn ? 1.25 : .80);
    },
    onPointerDown: function (evt, x, y) {
        document.getElementsByTagName("body")[0].style.cursor = "-webkit-grabbing";
        graphComponent.isDragging = true;
        graphComponent.lastX = evt.clientX;
        graphComponent.lastY = evt.clientY;
    },
    onPointerUp: function (evt, x, y) {
        document.getElementsByTagName("body")[0].style.cursor = "pointer";
        graphComponent.isDragging = false;
    },
    onMouseMovement: function (e) {

        if (!graphComponent.isDragging) {
            return;
        }

        var diffX = e.clientX - graphComponent.lastX;
        var diffY = e.clientY - graphComponent.lastY;
        graphComponent.lastX = e.clientX;
        graphComponent.lastY = e.clientY;

        graphComponent.applyPan(diffX, diffY);
    }
};
        // end of component

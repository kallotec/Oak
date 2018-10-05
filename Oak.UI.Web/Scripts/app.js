//configuration
var autoCompleteDataUrl = "api/schema/autocomplete2";
var callTreeDataUrlFormat = "api/schema/dependencytree/?objName={0}&direction={1}";
var definitionDataUrl = "api/schema/definition/?objName=";
var messageLabel = null;

var app = new Vue({
    el: '#app',
    data: {
        searchText: '',
        selectedWalkDirection: 0,
        selectedLayoutDirection: 'LR',
        message: '',
        showAutocomplete: true,
        autocompleteFilterType: -1,
        autocompleteListFull: [],
        autocompleteListFiltered: [],
        currDefinitionText: ''
    },
    created: function () {

        // init graph
        graphComponent.init(this.loadDef, this.displayMessage);

        // init autocomplete
        this.loadAutocomplete();

        // refresh viewport
        graphComponent.updateSvgTransform();

    },
    watch: {
        selectedLayoutDirection: function (val) {
            // update graph settings
            graphComponent.direction = val;
        },
        autocompleteFilterType: function (val) {
            // filter autocomplete list
            //autocompleteListFiltered = ;
        }
    },
    methods: {
        search: function () {

            // needs at least 2 characters in box to search on
            // and direction must be selected
            if (this.searchText < 2 && !this.selectedWalkDirection) {
                return;
            }

            //build url with parameter
            var dataUrl = callTreeDataUrlFormat.replace("{0}", encodeURIComponent(this.searchText)).replace("{1}", this.selectedWalkDirection);
            console.log(dataUrl);

            try {
                this.displayMessage("Getting tree data...");

                //get graph json
                $.getJSON(dataUrl, function (data) {
                    graphComponent.displayGraph(data, this.searchText);
                });

            } catch (e) {
                this.displayMessage("Getting tree data failed.");
                throw e;
            }

        },
        loadDef: function (objName) {

            //need at least 2 characters in box to search on
            if(objName.length < 2) {
                return;
            }

            //build url with parameter
            var dataUrl = definitionDataUrl + encodeURIComponent(objName);
            console.log(dataUrl);

            try {
                console.log("Getting definition data...");
                var me = this;

                //get graph json
                $.getJSON(dataUrl)
                    .done(function (data) {
                        //console.log(data);

                        me.currDefinitionText = data.DefinitionText;
                        console.log("Got definition");

                        //display definition
                        me.showDefinition("50%");

                    })
                    .fail(function () {

                        console.log("Getting definition data failed.");

                        //display definition
                        me.currDefinitionText = "-- Sorry! The definition for this object could not be retrieved...";
                        me.showDefinition("50%");
                    });

            } catch(e) {
                this.displayMessage("Getting definition data failed.");
                throw e;
            }
        },
        showDefinition: function (paneWidth) {

            //show panel
            var defPanel = $("#sp-definition-box");
            defPanel.css("width", paneWidth);
            defPanel.show(300);

            //display def
            var def = $("#sp-definition-box code");
            def.text(this.currDefinitionText);

            //apply highlighting
            $('pre code').each(function (i, block) {
                hljs.highlightBlock(block);
            });

        },
        resetDefinitionWindow: function () {

            //hide panel
            var defPanel = $("#sp-definition-box");
            defPanel.hide(100);

            //clear def
            var def = $("#sp-definition");
            def.text('');

        },
        loadAutocomplete: function () {

            var me = this;

            //var substringMatcher = function (strs) {
            //    return function findMatches(q, cb) {
            //        var matches, substringRegex;

            //        // an array that will be populated with substring matches
            //        matches = [];

            //        // regex used to determine if a string contains the substring 'q'
            //        substrRegex = new RegExp(q, 'i');

            //        // iterate through the pool of strings and for any string that
            //        // contains the substring 'q', add it to the 'matches' array
            //        $.each(strs, function (i, str) {
            //            if (substrRegex.test(str)) {
            //                matches.push(str);
            //            }
            //        });

            //        cb(matches);
            //    };
            //};

            this.displayMessage("Loading autocomplete data...");

            $.getJSON(autoCompleteDataUrl, function (data) {

                console.log('autocomplete data', data);

                me.autocompleteListFull = data;
                me.autocompleteListFiltered = data;

                try {
                    me.displayMessage(data.length + " database objects in schema");
                }
                catch (e) {
                    me.displayMessage("Error fetching autocomplete data.");
                    throw e;
                }
            });
        },
        displayMessage: function (msg) {
            console.log(msg);
            this.message = msg;
        }
    }
});





// [SVGElement.getTransformToElement()]
// This method required by joint.js is removed in new version of Chrome
// More info @ http://jointjs.com/blog/get-transform-to-element-polyfill.html

SVGElement.prototype.getTransformToElement = SVGElement.prototype.getTransformToElement || function (toElement) {
    return toElement.getScreenCTM().inverse().multiply(this.getScreenCTM());
};

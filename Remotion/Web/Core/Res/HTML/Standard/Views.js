function Views() {
}

Views.SetBodyHeightToWindowHeight = function(containerElement) {
    var body = $("body");
    var html = $("html");

    var topMargin = body.offset().top;
    body.height(html.height() - 2*topMargin);
}

Views.Adjust = function(containerElement, elementToAdjust) {
    Views.AdjustTop(containerElement, elementToAdjust);
    Views.AdjustHeight(containerElement, elementToAdjust);
};

Views.AdjustTop = function(containerElement, elementToAdjust) {
    ArgumentUtility.CheckNotNull('containerElement', containerElement);
    ArgumentUtility.CheckNotNull('elementToAdjust', elementToAdjust);
    var topControls = $(containerElement).children()[0];
    elementToAdjust.style.top = topControls.clientHeight;
};

Views.AdjustHeight = function(containerElement, elementToAdjust) {
    ArgumentUtility.CheckNotNull('containerElement', containerElement);
    ArgumentUtility.CheckNotNull('elementToAdjust', elementToAdjust);
    var height = $(containerElement).height();
    var children = $(containerElement).children();
    var siblings = children.filter(function(index) { return children[index] != elementToAdjust; });
    for (var iSibling = 0; iSibling < siblings.length; iSibling++) {
        height -= siblings[iSibling].clientHeight;
    }
    if( height > 0 )
        elementToAdjust.style.height = height + 'px';
};
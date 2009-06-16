function Views() {
}

Views.SetBodyHeightToWindowHeight = function(containerElement) {
    var parent = containerElement.parentNode;
    while (parent.tagName.toLowerCase() != "body") {
        parent = parent.parentNode;
    }
    var body = parent;
    var html = body.parentNode;

    var topMargin = body.getBoundingClientRect().top - html.getBoundingClientRect().top;
    body.style.height = document.documentElement.clientHeight - 2*topMargin + 'px';
}

Views.Adjust = function(containerElement, elementToAdjust, topElementsCount) {
    Views.AdjustTop(containerElement, elementToAdjust, topElementsCount);
    Views.AdjustHeight(containerElement, elementToAdjust);
};

Views.AdjustTop = function(containerElement, elementToAdjust, topElementsCount) {
    ArgumentUtility.CheckNotNull('containerElement', containerElement);
    ArgumentUtility.CheckNotNull('elementToAdjust', elementToAdjust);
    ArgumentUtility.CheckNotNull('topElementsCount', topElementsCount);
    var top = 0;
    for (var iChild = 0; iChild < topElementsCount; iChild++) {
        var rowDiv = containerElement.childNodes[iChild];
        var childHeight = rowDiv.getBoundingClientRect().bottom - rowDiv.getBoundingClientRect().top;
        top += childHeight;
    }
    elementToAdjust.style.top = top + 'px';
};

Views.AdjustHeight = function(containerElement, elementToAdjust) {
    ArgumentUtility.CheckNotNull('containerElement', containerElement);
    ArgumentUtility.CheckNotNull('elementToAdjust', elementToAdjust);
    var height = containerElement.getBoundingClientRect().bottom - containerElement.getBoundingClientRect().top;
    for (var iChild = 0; iChild < containerElement.childNodes.length; iChild++) {
        var rowDiv = containerElement.childNodes[iChild];
        if (rowDiv == elementToAdjust)
            continue;

        var childHeight = rowDiv.getBoundingClientRect().bottom - rowDiv.getBoundingClientRect().top;
        height -= childHeight;
    }
    elementToAdjust.style.height = height + 'px';
};
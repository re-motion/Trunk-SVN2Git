function ViewLayout()
{
}

ViewLayout.AdjustWidth = function(containerElement) {
    var children = $(containerElement).children();
    children.each(function(i) {
        var parentWidth = $(this).parent().width();
        var margin = parseInt($(this).css('left'));
        var borderWidth = parseInt($(this).css('border-width'));
        if (isNaN(borderWidth))
            borderWidth = 0;
        $(this).width(parentWidth - 2 * margin - 2 * borderWidth);
    });
}

ViewLayout.Adjust = function(containerElement, elementToAdjust) {
    ViewLayout.AdjustTop(containerElement, elementToAdjust);
    ViewLayout.AdjustHeight(containerElement, elementToAdjust);
};

ViewLayout.AdjustTop = function(containerElement, elementToAdjust)
{
    var topControls = $(containerElement).children()[0];
    $(elementToAdjust).css('top', $(topControls).outerHeight() + ViewLayout.GetTopControlMargin(containerElement) );
};

ViewLayout.AdjustHeight = function(containerElement, elementToAdjust) {
    ArgumentUtility.CheckNotNull('containerElement', containerElement);
    ArgumentUtility.CheckNotNull('elementToAdjust', elementToAdjust);
    var height = $(containerElement).height();

    var children = $(containerElement).children();
    // var siblings = children.filter(function(index) { return children[index] != elementToAdjust; });
    for (var iSibling = 0; iSibling < children.length-1; iSibling++) {
        var sibling = $(children[iSibling])
        height -= sibling.outerHeight();
    }

    height -= ViewLayout.GetTopControlMargin(containerElement);
    height -= ViewLayout.GetBottomControlMargin(containerElement);

    if (height > 0)
        $(elementToAdjust).css('height', height);
};

ViewLayout.GetTopControlMargin = function(containerElement) {
    var children = $(containerElement).children();
    var topControls = $(children[0]);
    var top = parseInt(topControls.css('top'));
    if (!isNaN(top))
        return top;
    return 0;
}

ViewLayout.GetBottomControlMargin = function(containerElement) {
    var children = $(containerElement).children();
    var bottomControls = $(children[children.length - 2]);
    var bottom = parseInt(bottomControls.css('bottom'));
    if (!isNaN(bottom))
        return bottom;
    return 0;
}



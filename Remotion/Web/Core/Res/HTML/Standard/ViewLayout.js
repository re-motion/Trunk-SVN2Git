function ViewLayout()
{
}

ViewLayout.AdjustWidth = function(containerElement) {
    var children = $(containerElement).children('div');
    children.each(function(i) {
        var parentWidth = $(this).parent().width();
        var margin = parseInt($(this).css('left'));
        var borderWidth = parseInt($(this).css('border-width'));
        if (isNaN(margin))
            margin = 0;
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
    var siblings = ViewLayout.GetLayoutParts(containerElement, elementToAdjust)
    $(elementToAdjust).css('top', $(topControls).outerHeight() + ViewLayout.GetTopControlMargin(siblings) );
};

ViewLayout.AdjustHeight = function(containerElement, elementToAdjust) {
    ArgumentUtility.CheckNotNull('containerElement', containerElement);
    ArgumentUtility.CheckNotNull('elementToAdjust', elementToAdjust);
    var height = $(containerElement).height();

    var siblings = ViewLayout.GetLayoutParts(containerElement, elementToAdjust)
    for (var iSibling = 0; iSibling < siblings.length; iSibling++) {
        var sibling = $(siblings[iSibling])
        height -= sibling.outerHeight();
    }

    height -= ViewLayout.GetTopControlMargin(siblings);
    height -= ViewLayout.GetBottomControlMargin(siblings);

    if (height > 0)
        $(elementToAdjust).css('height', height);
};

ViewLayout.GetTopControlMargin = function(siblings) {
var topControls = $(siblings[0]);
    var top = parseInt(topControls.css('top'));
    if (!isNaN(top))
        return top;
    return 0;
}

ViewLayout.GetBottomControlMargin = function(siblings) {
    var bottomControls = $(siblings[siblings.length - 1]);
    var bottom = parseInt(bottomControls.css('bottom'));
    if (!isNaN(bottom))
        return bottom;
    return 0;
}

ViewLayout.GetLayoutParts = function(containerElement, elementToIgnore)
{
    var children = $(containerElement).children();
    var siblings = children.filter(function(index) { return (children[index].nodeName == 'DIV') && (children[index] != elementToIgnore); });
    return siblings;
}



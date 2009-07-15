function ViewLayout() {
}

ViewLayout.AdjustWidth = function(contentElement) {
    var children = contentElement.children('div');
    children.each(function(i) {
        $(this).css('position', 'absolute');
        $(this).css('left', '0');
        $(this).css('right', '0');
    });
};

ViewLayout.AdjustHeight = function(contentElement) {
    var margin = contentElement.outerHeight(true) - contentElement.innerHeight();
    contentElement.height(contentElement.parent().height() - margin);
};

ViewLayout.AdjustTop = function(topSibling, elementToAdjust) {
    $(elementToAdjust).css('top', topSibling.position().top + topSibling.outerHeight(true));
};

ViewLayout.AdjustBottom = function(bottomSibling, elementToAdjust) {
    $(elementToAdjust).css('bottom', bottomSibling.parent().height() - bottomSibling.position().top);
};

ViewLayout.AdjustSingleView = function(containerElement) {
    var contentElement = containerElement.children('div:first');

    ViewLayout.AdjustHeight(contentElement);
    ViewLayout.AdjustWidth(contentElement);

    var top = contentElement.children('div:eq(0)');
    var view = contentElement.children('div:eq(1)');
    var bottom = contentElement.children('div:eq(2)');

    top.css('top', '0');
    bottom.css('bottom', '0');
    ViewLayout.AdjustTop(top, view);
    ViewLayout.AdjustBottom(bottom, view);

    // setting UpdatePanel height prevents horizontal scrollbar in IE7
    view.children('div').css('height', '100%');
};

ViewLayout.AdjustTabbedMultiView = function(containerElement) {
    var contentElement = containerElement.children('div:first');

    ViewLayout.AdjustHeight(contentElement);
    ViewLayout.AdjustWidth(contentElement);

    var top = contentElement.children('div:eq(0)');
    var tabs = contentElement.children('div:eq(1)');
    var view = contentElement.children('div:eq(2)');
    var bottom = contentElement.children('div:eq(3)');

    top.css('top', '0');
    bottom.css('bottom', '0');
    ViewLayout.AdjustTop(top, tabs);
    ViewLayout.AdjustTop(tabs, view);
    ViewLayout.AdjustBottom(bottom, view);

    // setting UpdatePanel height prevents horizontal scrollbar in IE7
    view.children('div').css('height', '100%');
};

function ViewLayout()
{
}

ViewLayout.AdjustLayoutIE6 = function(containerElement) 
{
    if (!$.browser.msie || parseInt($.browser.version) > 6)
        return;
}

ViewLayout.AdjustMultiViewLayoutIE6 = function(containerElement) 
{
    if (!$.browser.msie || parseInt($.browser.version) > 6)
        return;
}

ViewLayout.AdjustSingleViewLayoutIE6 = function(containerElement) 
{
    if (!$.browser.msie || parseInt($.browser.version) > 6)
        return;
}

ViewLayout.AdjustActiveViewContent = function(viewContent) 
{
    var viewContentBorder = viewContent.children().eq(0);
    var viewBottomControls = viewContent.next();

    var viewContentBorderHeight = viewContentBorder.outerHeight(true) - viewContentBorder.height();
    var viewBottomControlsBorderHeight = viewBottomControls.outerHeight(true) - viewBottomControls.height();

    var viewTop = viewContent.offset().top;
    var bottomTop = viewBottomControls.offset().top;
    var viewNewHeight = bottomTop - viewTop - viewContentBorderHeight - viewBottomControlsBorderHeight;

    viewContentBorder.height(viewNewHeight);
}

ViewLayout.AdjustSingleView = function(containerElement) 
{
    var viewContent = containerElement.children().eq(0).children().eq(1);
    ViewLayout.AdjustActiveViewContent(viewContent);
};

ViewLayout.AdjustTabbedMultiView = function(containerElement) 
{
    /* Temporary hack for missisng div.tabbedMultiViewUpdatePanel class name */
        var tabbedMultiViewUpdatePanel = containerElement.parent();
        tabbedMultiViewUpdatePanel.addClass("tabbedMultiViewUpdatePanel");
    /* END Temporary hack for missisng div.tabbedMultiViewUpdatePanel class name */

    var viewContent = containerElement.children().eq(0).children().eq(2);
    ViewLayout.AdjustActiveViewContent(viewContent);
};


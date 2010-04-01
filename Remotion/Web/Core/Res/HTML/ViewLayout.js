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

  var viewContentOffset = viewContent.offset();
  var viewTop = viewContentOffset == null ? 0 : viewContentOffset.top;
  
  var viewBottomControlsOffset = viewBottomControls.offset();
  var bottomTop = viewBottomControlsOffset == null ? 0 : viewBottomControlsOffset;
  
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
    var viewContent = containerElement.children().eq(0).children().eq(2);
    ViewLayout.AdjustActiveViewContent(viewContent);
};


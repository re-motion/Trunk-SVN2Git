function ViewLayout()
{
}

ViewLayout.AdjustWidth = function(contentElement)
{
  var children = contentElement.children('div');
  children.each(function(i)
  {
    $(this).css('position', 'absolute');
    $(this).css('left', '0');
    $(this).css('right', '0');
  });
};

ViewLayout.AdjustHeight = function(contentElement)
{
  var margin = contentElement.outerHeight(true) - contentElement.height();
  contentElement.height(contentElement.parent().height() - margin);
};

ViewLayout.AdjustTop = function(topSibling, elementToAdjust)
{
  $(elementToAdjust).css('top', topSibling.position().top + topSibling.outerHeight(true));
};

ViewLayout.AdjustBottom = function(bottomSibling, elementToAdjust)
{
  $(elementToAdjust).css('bottom', bottomSibling.parent().height() - bottomSibling.position().top);
};

ViewLayout.AdjustSingleView = function(containerElement)
{
  ViewLayout.SetParentUpdatePanelHeight(containerElement);

  var contentElement = containerElement.children('div:first');

  ViewLayout.AdjustHeight(contentElement);
  ViewLayout.AdjustWidth(contentElement);

  var top = contentElement.children('div:eq(0)');
  var view = contentElement.children('div:eq(1)');
  var bottom = contentElement.children('div:eq(2)');
  var viewContentBorder = view.children(':first');

  top.css('top', '0');
  bottom.css('bottom', '0');
  ViewLayout.AdjustTop(top, view);
  ViewLayout.AdjustBottom(bottom, view);
  ViewLayout.AdjustViewContentBorder(viewContentBorder);

  ViewLayout.FixIE6(view, bottom);
};

ViewLayout.AdjustTabbedMultiView = function(containerElement)
{
  ViewLayout.SetParentUpdatePanelHeight(containerElement);

  var contentElement = containerElement.children('div:first');

  ViewLayout.AdjustHeight(contentElement);
  ViewLayout.AdjustWidth(contentElement);

  var top = contentElement.children('div:eq(0)');
  var tabs = contentElement.children('div:eq(1)');
  var view = contentElement.children('div:eq(2)');
  var bottom = contentElement.children('div:eq(3)');
  var viewContentBorder = view.children(':first');

  top.css('top', '0');
  bottom.css('bottom', '0');
  ViewLayout.AdjustTop(top, tabs);
  ViewLayout.AdjustTop(tabs, view);
  ViewLayout.AdjustBottom(bottom, view);

  ViewLayout.AdjustViewContentBorder(viewContentBorder);

  ViewLayout.FixIE6(view, bottom);
  ViewLayout.FixIE7(view);
};

ViewLayout.AdjustViewContentBorder = function(viewContentBorder)
{
  viewContentBorder.css('position', 'absolute');
  viewContentBorder.css('top', 0);
  viewContentBorder.css('left', 0);
  viewContentBorder.css('bottom', 0);
  viewContentBorder.css('right', 0);
}

ViewLayout.SetParentUpdatePanelHeight = function(element)
{
  var pageRequestManager = Sys.WebForms.PageRequestManager.getInstance();
  var updatePanelIDs = pageRequestManager._updatePanelIDs;
  for (var i = updatePanelIDs.length - 1; i >= 0; i--)
  {
    var updatePanelClientID = updatePanelIDs[i].replace(/\$/g, '_');;
    element.parent('#' + updatePanelClientID).css('height', '100%');
  }
}


ViewLayout.FixIE7 = function(view) {
  if (!jQuery.browser.msie || parseInt(jQuery.browser.version) != 7)
    return;
  setTimeout(function() { $(view).children().css('display', 'block'); }, 1);
}


ViewLayout.FixIE6 = function(view, bottom)
{
  if (!jQuery.browser.msie || parseInt(jQuery.browser.version) > 6)
    return;

  var contentElement = view.parent();
  contentElement.height(1);
  ViewLayout.AdjustHeight(contentElement);
  view.height(bottom.offset().top - view.offset().top);

  view.width(contentElement.innerWidth());
  var margin = view.outerWidth(true) - view.outerWidth(false);
  view.width(view.width() - margin);

  var contentBorder = view.children(':first');
  contentBorder.css('top', 0);
  contentBorder.css('left', 0);
  contentBorder.height(view.innerHeight());
  contentBorder.width(view.innerWidth());

  // fix absolute positioning issues
  $('span.bocReferenceValue span.content').each(function()
  {
    $(this).css('width', $(this).parent().parent().width() - parseInt($(this).css('left')) - parseInt($(this).css('right')));
  });
  $('span.bocAutoCompleteReferenceValue span.content').each(function()
  {
    var newWidth = $(this).parent().width() - parseInt($(this).css('left')) - parseInt($(this).css('right'));
    $(this).css('width', newWidth);
  });

  // does not change style, but solves the problem that certain elements didn't show in standard mode
  $('.tabStripTabSeparator').css('display', $('.tabStripTabSeparator').css('display'));
  $('.DropDownMenuContainer').css('display', $('.DropDownMenuContainer').css('display'));
  $('.bocListNavigator').css('display', $('.bocListNavigator').css('display'));
  $('span.bocReferenceValueContent').css('display', $('span.bocReferenceValueContent').css('display'));
  $('span.bocAutoCompleteReferenceValueContent').css('display', $('span.bocAutoCompleteReferenceValueContent').css('display'));
  $('span.bocDateTimeValue').css('display', $('span.bocDateTimeValue').css('display'));
  $('a.DatePickerButton').css('display', $('a.DatePickerButton').css('display'));
}

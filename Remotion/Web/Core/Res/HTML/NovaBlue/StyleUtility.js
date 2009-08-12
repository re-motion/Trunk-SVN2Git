function StyleUtility()
{ }

StyleUtility.CreateBorderSpans = function(selector)
{
  var element = $(selector);
  if (element.length == 0)
    return;

  StyleUtility.CreateAndAppendBorderSpan(element, element.attr('id'), 'top');
  StyleUtility.CreateAndAppendBorderSpan(element, element.attr('id'), 'left');
  StyleUtility.CreateAndAppendBorderSpan(element, element.attr('id'), 'bottom');
  StyleUtility.CreateAndAppendBorderSpan(element, element.attr('id'), 'right');
  StyleUtility.CreateAndAppendBorderSpan(element, element.attr('id'), 'topLeft');
  var topRight = StyleUtility.CreateAndAppendBorderSpan(element, element.attr('id'), 'topRight');
  var bottomLeft = StyleUtility.CreateAndAppendBorderSpan(element, element.attr('id'), 'bottomLeft');
  var bottomRight = StyleUtility.CreateAndAppendBorderSpan(element, element.attr('id'), 'bottomRight');

  StyleUtility.ShowBorderSpans(element, topRight, bottomLeft, bottomRight);

  var resizeHandler = function() { StyleUtility.OnResize(selector); }
  $(document).ready(function() { $(window).resize(resizeHandler); });
}

StyleUtility.ShowBorderSpans = function(element, topRight, bottomLeft, bottomRight)
{
  var scrollDiv = element.children(':first');
  while ( (scrollDiv.parent().css('overflow') != 'auto') && (scrollDiv.length > 0) )
    scrollDiv = scrollDiv.children(':first');

  if ((scrollDiv.length == 1) && (scrollDiv.attr('nodeName').toLowerCase() == 'div'))
  {
    var hasVerticalScrollBar = scrollDiv[0].scrollHeight > scrollDiv.height(); //height includes the scrollbar, if it exists
    var hasHorizontalScrollbar = scrollDiv[0].scrollWidth > scrollDiv.width(); //width includes the scrollbar, if it exists
    var hasExactlyOneScrollbar = (hasVerticalScrollBar && !hasHorizontalScrollbar) || (!hasVerticalScrollBar && hasHorizontalScrollbar);

    if (hasVerticalScrollBar)
      $(topRight).css('display', 'none');
    else
      $(topRight).css('display', '');

    if (hasHorizontalScrollbar)
      $(bottomLeft).css('display', 'none');
    else
      $(bottomLeft).css('display', '');

    if (hasExactlyOneScrollbar)
      $(bottomRight).css('display', 'none');
    else
      $(bottomRight).css('display', '');
  }
}

StyleUtility.CreateAndAppendBorderSpan = function(elementBody, elementID, className)
{
  var borderSpan = document.createElement('SPAN');
  borderSpan.id = elementID + '_' + className;
  borderSpan.className = className;

  elementBody[0].appendChild(borderSpan);

  return borderSpan
}

StyleUtility.OnResize = function(selector)
{
  var element = $(selector);
  if (element.length > 0)
  {
    var topRight = element.find('#' + element.attr('id') + '_topRight');
    var bottomLeft = element.find('#' + element.attr('id') + '_bottomLeft');
    var bottomRight = element.find('#' + element.attr('id') + '_bottomRight');

    StyleUtility.ShowBorderSpans(element, topRight[0], bottomLeft[0], bottomRight[0]);
  }
}
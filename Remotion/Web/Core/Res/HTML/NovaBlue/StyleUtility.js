function StyleUtility()
{ }

StyleUtility.CreateBorderSpans = function(selector)
{
  var element = $(selector);
  if (element.length == 0)
    return;

  StyleUtility.CreateAndAppendBorderSpan(element, element.id, 'top');
  StyleUtility.CreateAndAppendBorderSpan(element, element.id, 'left');
  StyleUtility.CreateAndAppendBorderSpan(element, element.id, 'bottom');
  StyleUtility.CreateAndAppendBorderSpan(element, element.id, 'right');
  StyleUtility.CreateAndAppendBorderSpan(element, element.id, 'topLeft');
  var topRight = StyleUtility.CreateAndAppendBorderSpan(element, element.id, 'topRight');
  var bottomLeft = StyleUtility.CreateAndAppendBorderSpan(element, element.id, 'bottomLeft');
  var bottomRight = StyleUtility.CreateAndAppendBorderSpan(element, element.id, 'bottomRight');

  StyleUtility.ShowBorderSpans(element, topRight, bottomLeft, bottomRight);

  var resizeHandler = function() { StyleUtility.OnResize(selector); }
  $(window).resize(resizeHandler);
}

StyleUtility.ShowBorderSpans = function(element, topRight, bottomLeft, bottomRight)
{
  var scrollDiv = $(element).children(':first').children(':first');
  if ((scrollDiv.length == 1) && !TypeUtility.IsUndefined(scrollDiv[0].nodeName) && (scrollDiv[0].nodeName.toLowerCase() == 'div'))
  {
    if (scrollDiv[0].scrollHeight > scrollDiv.height())
      $(topRight).css('display', 'none');
    else
      $(topRight).css('display', 'inline');

    if (scrollDiv[0].scrollWidth > scrollDiv.width())
      $(bottomLeft).css('display', 'none');
    else
      $(bottomLeft).css('display', 'inline');

    if ((scrollDiv[0].scrollHeight > scrollDiv.height() && scrollDiv[0].scrollWidth == scrollDiv.width())
    || (scrollDiv[0].scrollHeight == scrollDiv.height() && scrollDiv[0].scrollWidth > scrollDiv.width()))
    {
      $(bottomRight).css('display', 'none');
    }
    else
    {
      $(bottomRight).css('display', 'inline');
    }
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
    var topRight = element.find('#' + element.id + '_topRight');
    var bottomLeft = element.find('#' + element.id + '_bottomLeft');
    var bottomRight = element.find('#' + element.id + '_bottomRight');

    StyleUtility.ShowBorderSpans(element[0], topRight[0], bottomLeft[0], bottomRight[0]);
  }
}
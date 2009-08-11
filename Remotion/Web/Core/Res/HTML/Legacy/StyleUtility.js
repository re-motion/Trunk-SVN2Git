function StyleUtility()
{ }

StyleUtility.CreateBorderSpans = function(selector)
{
  var elementBody = $(selector)[0];

  StyleUtility.CreateAndAppendBorderSpan(elementBody, element.id, 'top');
  StyleUtility.CreateAndAppendBorderSpan(elementBody, element.id, 'left');
  StyleUtility.CreateAndAppendBorderSpan(elementBody, element.id, 'bottom');
  StyleUtility.CreateAndAppendBorderSpan(elementBody, element.id, 'right');
  StyleUtility.CreateAndAppendBorderSpan(elementBody, element.id, 'topLeft');
  var topRight = StyleUtility.CreateAndAppendBorderSpan(elementBody, element.id, 'topRight');
  var bottomLeft = StyleUtility.CreateAndAppendBorderSpan(elementBody, element.id, 'bottomLeft');
  var bottomRight = StyleUtility.CreateAndAppendBorderSpan(elementBody, element.id, 'bottomRight');

  StyleUtility.CalculateBorderSpans(elementBody, topRight, bottomLeft, bottomRight);

  var elementID = element.id;
  var resizeHandler = function() { StyleUtility.OnResize(elementID); }
  $(window).resize(resizeHandler);
}

StyleUtility.CalculateBorderSpans = function(element, topRight, bottomLeft, bottomRight)
{
  var right = 0;  // $(element).position().left + $(element).width();
  var bottom = $(element).position().top + $(element).height();
  var topRightOffset = 0; // $(topRight).offset().left;
  var bottomLeftOffset = $(bottomLeft).offset().top;
  var bottomRightVerticalOffset = $(bottomRight).offset().top;
  var bottomRightHorizontalOffset = 0; // $(bottomRight).offset().left;

  // QuirksMode calculations for IE - Firefox places borders correctly with jQuery positioning
  var offsetParent = topRight.offsetParent;
  if (offsetParent)
  { // this is null in Firefox
    right = offsetParent.clientLeft + offsetParent.clientWidth;
    bottom = offsetParent.clientTop + offsetParent.clientHeight;
    topRightOffset = topRight.offsetWidth;
    bottomLeftOffset = bottomLeft.offsetHeight;
    bottomRightVerticalOffset = bottomRight.offsetHeight;
    bottomRightHorizontalOffset = bottomRight.offsetWidth;
  }

  $(topRight).css('left', right - topRightOffset);
  $(bottomLeft).css('top', bottom - bottomLeftOffset);
  $(bottomRight).css('top', bottom - bottomRightVerticalOffset);
  $(bottomRight).css('left', right - bottomRightHorizontalOffset);

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
  if (elementBody.nodeType != 1)
  {
    elementBody = elementBody.parentNode;
  }

  var borderSpan = document.createElement('SPAN');
  borderSpan.id = elementID + '_' + className;
  borderSpan.className = className;

  elementBody.appendChild(borderSpan);

  return borderSpan
}

StyleUtility.OnResize = function(elementID)
{
  var element = document.getElementById(elementID);
  if (element != null)
  {
    var topRight = document.getElementById(elementID + '_topRight');
    var bottomLeft = document.getElementById(elementID + '_bottomLeft');
    var bottomRight = document.getElementById(elementID + '_bottomRight');

    StyleUtility.CalculateBorderSpans(element, topRight, bottomLeft, bottomRight);
  }
}
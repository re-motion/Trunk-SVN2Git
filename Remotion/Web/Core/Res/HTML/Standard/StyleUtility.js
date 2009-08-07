function StyleUtility()
{ }

StyleUtility.CreateBorderSpans = function(element)
{
  StyleUtility.CreateAndAppendBorderSpan(element, element.id, 'top');
  StyleUtility.CreateAndAppendBorderSpan(element, element.id, 'left');
  StyleUtility.CreateAndAppendBorderSpan(element, element.id, 'bottom');
  StyleUtility.CreateAndAppendBorderSpan(element, element.id, 'right');
  StyleUtility.CreateAndAppendBorderSpan(element, element.id, 'topLeft');
  var topRight = StyleUtility.CreateAndAppendBorderSpan(element, element.id, 'topRight');
  var bottomLeft = StyleUtility.CreateAndAppendBorderSpan(element, element.id, 'bottomLeft');
  var bottomRight = StyleUtility.CreateAndAppendBorderSpan(element, element.id, 'bottomRight');

  StyleUtility.ShowBorderSpans(element, topRight, bottomLeft, bottomRight);

  var resizeHandler = function() { StyleUtility.OnResize(element.id); }
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
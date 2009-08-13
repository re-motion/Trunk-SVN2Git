function StyleUtility()
{ }

StyleUtility.CreateBorderSpans = function(selector)
{
  var element = $(selector);
  while (element.length>0 && element.attr('id').length == 0)
    element = element.parent();

  var elementBody = $(selector)[0];

  StyleUtility.CreateAndAppendBorderSpan(elementBody, element.attr('id'), 'top');
  StyleUtility.CreateAndAppendBorderSpan(elementBody, element.attr('id'), 'left');
  StyleUtility.CreateAndAppendBorderSpan(elementBody, element.attr('id'), 'bottom');
  StyleUtility.CreateAndAppendBorderSpan(elementBody, element.attr('id'), 'right');
  StyleUtility.CreateAndAppendBorderSpan(elementBody, element.attr('id'), 'topLeft');
  var topRight = StyleUtility.CreateAndAppendBorderSpan(elementBody, element.attr('id'), 'topRight');
  var bottomLeft = StyleUtility.CreateAndAppendBorderSpan(elementBody, element.attr('id'), 'bottomLeft');
  var bottomRight = StyleUtility.CreateAndAppendBorderSpan(elementBody, element.attr('id'), 'bottomRight');

  StyleUtility.CalculateBorderSpans(element[0], topRight, bottomLeft, bottomRight);

  var elementID = element.id;
  var resizeHandler = function() { StyleUtility.OnResize(elementID); }
  $(window).resize(resizeHandler);
}

StyleUtility.CalculateBorderSpans = function(element, topRight, bottomLeft, bottomRight)
{
  topRight.style.left = topRight.offsetParent.clientLeft + topRight.offsetParent.clientWidth - topRight.offsetWidth  + 'px';
  bottomLeft.style.top = bottomLeft.offsetParent.clientTop + bottomLeft.offsetParent.clientHeight - bottomLeft.offsetHeight  + 'px';
  bottomRight.style.top = bottomRight.offsetParent.clientTop + bottomRight.offsetParent.clientHeight - bottomRight.offsetHeight  + 'px';
  bottomRight.style.left = bottomRight.offsetParent.clientLeft + bottomRight.offsetParent.clientWidth - bottomRight.offsetWidth  + 'px';

  var scrollDiv = element.firstChild.firstChild;
  if (scrollDiv != null && !TypeUtility.IsUndefined (scrollDiv.tagName) && scrollDiv.tagName.toLowerCase() == 'div')
  {
    if (scrollDiv.scrollHeight > scrollDiv.clientHeight)
      topRight.style.display = 'none';
    else
      topRight.style.display = '';

    if (scrollDiv.scrollWidth > scrollDiv.clientWidth)
      bottomLeft.style.display = 'none';
    else
      bottomLeft.style.display = '';

    if (   (scrollDiv.scrollHeight > scrollDiv.clientHeight && scrollDiv.scrollWidth == scrollDiv.clientWidth) 
        || (scrollDiv.scrollHeight == scrollDiv.clientHeight && scrollDiv.scrollWidth > scrollDiv.clientWidth))
    {
      bottomRight.style.display = 'none';
    }
    else
    {
      bottomRight.style.display = '';
    }
  }
}

StyleUtility.CreateAndAppendBorderSpan = function(elementBody, elementID, className)
{
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
// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
function StyleUtility()
{
}

StyleUtility.CreateBorderSpans = function(selector)
{
  var element = $(selector);

  StyleUtility.CreateAndAppendBorderSpan(element, element.attr('id'), 'top');
  StyleUtility.CreateAndAppendBorderSpan(element, element.attr('id'), 'left');
  StyleUtility.CreateAndAppendBorderSpan(element, element.attr('id'), 'bottom');
  StyleUtility.CreateAndAppendBorderSpan(element, element.attr('id'), 'right');
  StyleUtility.CreateAndAppendBorderSpan(element, element.attr('id'), 'topLeft');
  var topRight = StyleUtility.CreateAndAppendBorderSpan(element, element.attr('id'), 'topRight');
  var bottomLeft = StyleUtility.CreateAndAppendBorderSpan(element, element.attr('id'), 'bottomLeft');
  var bottomRight = StyleUtility.CreateAndAppendBorderSpan(element, element.attr('id'), 'bottomRight');

  if (StyleUtility.ShowBorderSpans(element, topRight, bottomLeft, bottomRight))
    PageUtility.Instance.RegisterResizeHandler(selector, StyleUtility.OnResize);
}

StyleUtility.ShowBorderSpans = function(element, topRight, bottomLeft, bottomRight)
{
  var scrollDiv = element;
  while (scrollDiv.css('overflow') != 'auto' && scrollDiv.css('overflow') != 'scroll' && (scrollDiv.length > 0))
    scrollDiv = scrollDiv.children(':first');
  var scrolledDiv = scrollDiv.children(':first');

  var hasScrollbarsOnOverflow = scrollDiv.css('overflow') == 'auto' || scrollDiv.css('overflow') == 'scroll';

  if (scrolledDiv.length == 1 && scrolledDiv.attr('nodeName').toLowerCase() == 'div' && hasScrollbarsOnOverflow)
  {
    var offset = 1;
    var hasVerticalScrollBar = scrolledDiv[0].scrollHeight > (scrolledDiv.height() + offset); //height includes the scrollbar, if it exists
    var hasHorizontalScrollbar = scrolledDiv[0].scrollWidth > (scrolledDiv.outerWidth() + offset); //width includes the scrollbar, if it exists
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

    return true;
  }
  else
  {
    return false;
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

StyleUtility.OnResize = function(element)
{
  var topRight = element.find('#' + element.attr('id') + '_topRight');
  var bottomLeft = element.find('#' + element.attr('id') + '_bottomLeft');
  var bottomRight = element.find('#' + element.attr('id') + '_bottomRight');

  StyleUtility.ShowBorderSpans(element, topRight[0], bottomLeft[0], bottomRight[0]);
}

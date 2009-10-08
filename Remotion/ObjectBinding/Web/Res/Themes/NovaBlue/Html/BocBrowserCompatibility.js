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
//  BrowserCompatibility.js contains client side scripts used by the Objectbinding.Web library to fix browser compatibility issues.

function BocBrowserCompatibility()
{ }

BocBrowserCompatibility.AdjustAutoCompleteReferenceValueLayout = function(element)
{
  if (!jQuery.browser.msie || parseInt(jQuery.browser.version) > 6)
    return;
  var content = element.children(':first').children('.content');
  //alert(content.attr('class'));
  var prevElement = content.prev();
  var firstElement = content.children(':first');
  var seccondElement = content.next();
  var continerElement = content.parent();
  var thisWidth = continerElement.outerWidth(true) - seccondElement.outerWidth(true) - prevElement.outerWidth(true);

  firstElement.css('width', thisWidth);
  //$('#' + element.attr('id') +' input').css('width', 'auto');

}

BocBrowserCompatibility.AdjustReferenceValueLayout = function(element)
{
}


BocBrowserCompatibility.AdjustDateTimeValueLayout = function(element)
{
}
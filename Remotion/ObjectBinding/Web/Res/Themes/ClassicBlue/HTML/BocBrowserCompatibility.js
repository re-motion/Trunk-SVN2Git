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

BocBrowserCompatibility.ReferenceValueLayoutFixIE6 = function(element) {
  if (!jQuery.browser.msie || parseInt(jQuery.browser.version) > 6)
    return;


  var content = element.children(':first').children('.content');
  var prevElement = content.prev();
  var firstElement = content.children(':first');
  var seccondElement = content.next();
  var continerElement = content.parent();
  var thisWidth = continerElement.outerWidth(true) - seccondElement.outerWidth(true) - prevElement.outerWidth(true);

  firstElement.css('width', thisWidth);

  var firstElementInput = firstElement.children(':first').children(':first');
  var firstElementArrow = firstElement.children(':nth-child(2)');

  if (firstElementInput) {
    var newWidth = firstElementInput.width() - firstElementArrow.outerWidth(true);
    firstElementInput.css('width', newWidth);
    firstElementArrow.css('height', firstElementInput.outerHeight());
  }

  if (element.hasClass('disabled') || element.hasClass('readOnly')) {
    if (firstElement.className != 'undefined') {
      var firstElementSpan = element.children(':first').children(':first');
      firstElementSpan.css({ 'display': 'block' });
      var seccondElement = element.children().find('input, select');
      seccondElement.css({ 'width': 'auto' });
    }
  }
}




BocBrowserCompatibility.AdjustDateTimeValueLayout = function(element) {
  if (!jQuery.browser.msie || parseInt(jQuery.browser.version) > 6)
    return;
  var totalChildrens = element.children().size();
  if (totalChildrens > 2) {
    //is date-time control
    var dateField = element.children(':first').children(':first');
    var dateFieldWidth = dateField.attr('maxlength') / 2;
    dateField.css({ 'width': dateFieldWidth + 'em' });

    var calField = element.children(':nth-child(2)');
    var calFieldWidth = calField.outerWidth(true);
    calField.css({ 'position': 'absolute', 'left': dateField.outerWidth(true) });

    var timeField = element.children(':nth-child(3)').children(':first');
    var timeFieldWidth = timeField.attr('maxlength') / 2;
    timeField.css({ 'position': 'absolute', 'width': timeFieldWidth + 'em', 'left': calFieldWidth });


    element.width(dateField.outerWidth(true) + timeField.outerWidth(true));
  }
  else {
    //is date control
    var dateField = element.children(':first').children(':first');
    var dateFieldWidth = dateField.attr('maxlength') / 2;
    dateField.css({ 'width': dateFieldWidth + 'em' });

    var calField = element.children(':nth-child(2)');
    calField.css({ 'left': dateField.innerWidth() });
  }
}


BocBrowserCompatibility.AdjustAutoCompleteReferenceValueLayout = function(element)
{
  BocBrowserCompatibility.ReferenceValueLayoutFixIE6(element);
}

BocBrowserCompatibility.AdjustReferenceValueLayout = function(element)
{
  BocBrowserCompatibility.ReferenceValueLayoutFixIE6(element);
}



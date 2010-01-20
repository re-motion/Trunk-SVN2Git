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
  var intrinsicRatio = 8;
  var totalChildrens = element.children().size();
  if (totalChildrens > 2) {

    var maxWidth = element.innerWidth();
    var dateField = element.children(':first');
    var dateInputField = dateField.children(':first');

    var calField = element.children(':nth-child(2)');
    var calFieldWidth = calField.width();


    var timeField = element.children(':nth-child(3)');
    var timeFieldWidth = timeField.width();

    dateInputField.css('width', maxWidth - calFieldWidth - timeFieldWidth - intrinsicRatio);
    calField.css({ 'left': maxWidth - calFieldWidth - timeFieldWidth });
    timeField.css({ 'left': maxWidth - timeFieldWidth });

  } else {

    var maxWidth = element.innerWidth();
    var dateField = element.children(':first');
    var dateInputField = dateField.children(':first');

    var calField = element.children(':nth-child(2)');
    var calFieldWidth = calField.outerWidth(true);

    if (element.css('width') == 'auto') {
      dateInputField.css('width', '147px');
      calField.css({ 'left': '155px' });
    } else {
    dateInputField.css('width', maxWidth - calFieldWidth - intrinsicRatio);
      calField.css({ 'left': maxWidth - calFieldWidth });
    }

  }


}

BocBrowserCompatibility.AdjustAutoCompleteReferenceValueLayout = function(element) {
  BocBrowserCompatibility.ReferenceValueLayoutFixIE6(element);
}

BocBrowserCompatibility.AdjustReferenceValueLayout = function(element) {
  BocBrowserCompatibility.ReferenceValueLayoutFixIE6(element);
}



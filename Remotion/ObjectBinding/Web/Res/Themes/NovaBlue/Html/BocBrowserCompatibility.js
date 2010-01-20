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


  var controlContent = element.children(':first').children();
  var firstControlElement = $(controlContent.get(0));
  var seccondControlElement = $(controlContent.get(1));
  var thirdControlElement = $(controlContent.get(2));

  var thirdControlElementMenuArrowWidth = thirdControlElement.find('img').outerWidth(true);

  var disponibleWidth = element.innerWidth();
  var firstControlElementWidth = firstControlElement.outerWidth(true);

  if (thirdControlElement.hasClass('bocReferenceValueOptionsMenu')) {
    var thirdControlElementMenuText = thirdControlElement.find('a').outerWidth(true);
    var thirdControlElementWidth = thirdControlElementMenuText + thirdControlElementMenuArrowWidth;
  } else {

    var thirdControlElementWidth = thirdControlElementMenuArrowWidth;
  }

  var seccondControlElementWidth = disponibleWidth - firstControlElementWidth - thirdControlElementWidth;
  
  if (element.hasClass('bocAutoCompleteReferenceValue')) {
    var top = 1;
  } else {
    var top = 0;
  }

  seccondControlElement.css({ 'left': '', 'right': '', 'width': seccondControlElementWidth-50 });
  thirdControlElement.css({ 'left': '', 'right': '', 'top': top, 'width': thirdControlElementWidth });



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
      dateInputField.css('width', '147');
      calField.css({ 'left': '155' });
    } else {
    dateInputField.css('width', maxWidth - calFieldWidth - intrinsicRatio);
      calField.css({ 'left': maxWidth - calFieldWidth });
    }

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


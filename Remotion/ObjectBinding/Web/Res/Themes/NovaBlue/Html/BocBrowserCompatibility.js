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

  var maxWidth = element.innerWidth();
  var controlContentChildrens = element.children(':first').children();
  var totalChildrens = controlContentChildrens.size();

  var firstControlElement = $(controlContentChildrens.get(0));
  var firstControlElementWidth = firstControlElement.outerWidth(true);

  var seccondControlElement = $(controlContentChildrens.get(1));

  var thirdControlElement = $(controlContentChildrens.get(2));
  var thirdControlElementMenuArrowWidth = thirdControlElement.find('img').outerWidth(true);

  if (thirdControlElement.hasClass('bocReferenceValueOptionsMenu')) {
    var thirdControlElementMenuText = thirdControlElement.find('a').outerWidth(true);
    var thirdControlElementWidth = thirdControlElementMenuText + thirdControlElementMenuArrowWidth;
  } else {
    var thirdControlElementWidth = thirdControlElementMenuArrowWidth;
  }


  if (totalChildrens == 1) {
    var firstControlElementSelect = firstControlElement.find('.content');
    if (firstControlElementSelect) {

      var firstControlElementSelectMarginPadding = firstControlElementSelect.outerWidth(true) - firstControlElementSelect.width();

      firstControlElementSelectMaxWidth = maxWidth - firstControlElementSelectMarginPadding - firstControlElementSelect.prev().outerWidth(true) - firstControlElementSelect.next().outerWidth(true);
      firstControlElementSelect.css('width', firstControlElementSelectMaxWidth);
    }
  }

  if (totalChildrens == 2) {

    if (seccondControlElement.hasClass('bocReferenceValueOptionsMenu')) {
      var seccondControlElementMenuText = seccondControlElement.find('a').outerWidth(true);
      var seccondControlElementMenuArrowWidth = seccondControlElement.find('img').outerWidth(true);
      var seccondControlElementWidth = seccondControlElementMenuText + seccondControlElementMenuArrowWidth;
      var firstControlElementWidth = maxWidth - seccondControlElementWidth;

    } else {
      var seccondControlElementWidth = maxWidth - firstControlElementWidth;
    }
    seccondControlElement.css({ 'left': firstControlElementWidth, 'width': seccondControlElementWidth });

  } else if (totalChildrens == 3) {
    var seccondControlElementWidth = maxWidth - firstControlElementWidth - thirdControlElementWidth;
    seccondControlElement.css({ 'left': firstControlElementWidth, 'width': seccondControlElementWidth });
    thirdControlElement.css({ 'left': firstControlElementWidth + seccondControlElementWidth, 'width': thirdControlElementWidth });

  }
}

BocBrowserCompatibility.AdjustDateTimeValueLayout = function(element)
{
  var hasDateAndTimeFields = element.children().size() == 3;
  if (hasDateAndTimeFields)
  {
    var maxWidth = element.width();
    var dateField = element.children().eq(0);
    var dateInputField = dateField.children().eq(0);
    var dateFieldMarginPadding = dateField.outerWidth(true) - dateField.width();
    var dateInputFieldMaxLength = dateInputField.attr('maxLength');
    var dateInputFieldMinWidth = dateInputFieldMaxLength * 7.3; //7.3 is character width

    var calendarField = element.children().eq(1);
    var calendarFieldWidth = calendarField.width(true);

    var timeField = element.children().eq(2);
    var timeFieldWidth = timeField.outerWidth(true);

    var dateInputFieldNewWidth = maxWidth - calendarFieldWidth - timeFieldWidth - dateFieldMarginPadding; // 2 is a margin

    dateInputField.css('width', dateInputFieldNewWidth);
    calendarField.css({ 'left': dateInputFieldNewWidth });
    timeField.css({ 'left': maxWidth - timeFieldWidth });
  }
  else
  {
    var maxWidth = element.innerWidth();
    var dateField = element.children().eq(0);
    var dateInputField = dateField.children().eq(0);

    var calendarField = element.children().eq(1);
    var calendarFieldWidth = calendarField.width(true);

    if (element.css('width') != 'auto')
    {
      dateInputField.css('width', maxWidth - calendarFieldWidth - dateFieldMarginPadding );
      calendarField.css({ 'left': maxWidth - calendarFieldWidth });
    }
  }
}



BocBrowserCompatibility.AdjustAutoCompleteReferenceValueLayout = function(element) {
  BocBrowserCompatibility.AutoCompleteReferenceValueLayoutFixIE6(element);
}

BocBrowserCompatibility.AdjustReferenceValueLayout = function(element) {
  BocBrowserCompatibility.ReferenceValueLayoutFixIE6(element);
}


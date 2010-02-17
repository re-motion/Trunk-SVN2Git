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

BocBrowserCompatibility.ReferenceValueLayoutFixIE6 = function(element)
{
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

  if (thirdControlElement.hasClass('bocReferenceValueOptionsMenu'))
  {
    var thirdControlElementMenuText = thirdControlElement.find('a').outerWidth(true);
    var thirdControlElementWidth = thirdControlElementMenuText + thirdControlElementMenuArrowWidth;
  } 
  else
  {
    var thirdControlElementWidth = thirdControlElementMenuArrowWidth;
  }

  if (totalChildrens == 1)
  {
    var firstControlElementSelect = firstControlElement.find('.content');
    if (firstControlElementSelect)
    {
      var firstControlElementSelectMarginPadding = firstControlElementSelect.outerWidth(true) - firstControlElementSelect.width();

      firstControlElementSelectMaxWidth = maxWidth - firstControlElementSelectMarginPadding - firstControlElementSelect.prev().outerWidth(true) - firstControlElementSelect.next().outerWidth(true);
      firstControlElementSelect.css('width', firstControlElementSelectMaxWidth);
    }
  }

  if (totalChildrens == 2)
  {
    if (seccondControlElement.hasClass('bocReferenceValueOptionsMenu'))
    {
      var seccondControlElementMenuText = seccondControlElement.find('a').outerWidth(true);
      var seccondControlElementMenuArrowWidth = seccondControlElement.find('img').outerWidth(true);
      var seccondControlElementWidth = seccondControlElementMenuText + seccondControlElementMenuArrowWidth;
      var firstControlElementWidth = maxWidth - seccondControlElementWidth;
    } 
    else
    {
      var seccondControlElementWidth = maxWidth - firstControlElementWidth;
    }
    seccondControlElement.css({ 'left': firstControlElementWidth, 'width': seccondControlElementWidth });

  } 
  else if (totalChildrens == 3)
  {
    var seccondControlElementWidth = maxWidth - firstControlElementWidth - thirdControlElementWidth;
    seccondControlElement.css({ 'left': firstControlElementWidth, 'width': seccondControlElementWidth });
    thirdControlElement.css({ 'left': firstControlElementWidth + seccondControlElementWidth, 'width': thirdControlElementWidth });
  }
}

BocBrowserCompatibility.AdjustDateTimeValueLayoutFixIE6 = function(element) {

    if (!jQuery.browser.msie || parseInt(jQuery.browser.version) > 6)
        return;

    var controlContentChildrens = element.children();

    if (controlContentChildrens.size() > 1) {
        var dateElement = $(controlContentChildrens.get(0));
        var calendarElement = $(controlContentChildrens.get(1));
        var dateLeft = dateElement.offset().left;
        var calendarLeft = calendarElement.offset().left;
        var dateWidth = calendarLeft - dateLeft - 8; //8 compensate default borders of the input element.

        dateElement.width(dateWidth);
    }
    else {
        return;
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


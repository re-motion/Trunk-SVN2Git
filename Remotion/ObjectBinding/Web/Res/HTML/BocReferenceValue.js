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
var _bocReferenceValue_nullValue;

//  Initializes the strings used to represent the true, false and null values.
//  Call this method once in a startup script.
function BocReferenceValue_InitializeGlobals(nullValue)
{
  _bocReferenceValue_nullValue = nullValue;
}

//  Returns the number of rows selected for the specified BocList
function BocReferenceValue_GetSelectionCount(referenceValueDropDownListID)
{
  var dropDownList = $('#' + referenceValueDropDownListID);
  if (dropDownList.length == 0 || dropDownList.attr('selectedIndex') < 0)
    return 0;
  if (dropDownList.children()[dropDownList.attr('selectedIndex')].value == _bocReferenceValue_nullValue)
    return 0;
  return 1;
}

function BocReferenceValue_AdjustPosition(control, isEmbedded)
{

  var totalWidth = $(control).innerWidth();
  var totalHeight = $(control).innerHeight();

  var icon = $(control).find('img.bocReferenceValueContent').parent();

  var left = 0;
  if (icon.length > 0)
    left = icon.outerWidth(true);

  var optionsMenu = $(control).find('.bocReferenceValueOptionsMenu');
  var right = 0;
  if (!isEmbedded && optionsMenu.length > 0)
    right = optionsMenu.outerWidth(true);

  var contentSpan = $(control).find('span.content');
  contentSpan.css('left', left);
  if (isEmbedded)
  {
    var dropDownMenu = $(control).find('select').parent().parent();
    var dropDownButton = dropDownMenu.find('.DropDownMenuButton');
    var commandSpan = $(control).find('span.bocReferenceValueCommand');
    if (dropDownMenu.length > 0)
    {
      dropDownMenu.height($(control).find('select').parent().outerHeight(true));
      icon.css('top', (dropDownMenu.innerHeight() - icon.outerHeight()) / 2);
      contentSpan.css('right', dropDownButton.width());
    }
  }
  else
  {
    contentSpan.css('right', right);
    
    if($.browser.msie){
      var heightDifference = $(control).height() - optionsMenu.height();
      var offset = heightDifference / 2;
      optionsMenu.css('top', offset);
    }
  }
}

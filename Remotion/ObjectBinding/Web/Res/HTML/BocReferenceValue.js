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

function BocReferenceValue()
{
}

BocReferenceValue.Initialize = function (dropDownList, command, nullValueString, isAutoPostBackEnabled, iconServiceUrl, iconContext)
{
  ArgumentUtility.CheckNotNullAndTypeIsObject('dropDownList', dropDownList);
  ArgumentUtility.CheckNotNullAndTypeIsObject('command', command);
  ArgumentUtility.CheckNotNullAndTypeIsString('nullValueString', nullValueString);
  ArgumentUtility.CheckTypeIsBoolean('isAutoPostBackEnabled', isAutoPostBackEnabled);
  ArgumentUtility.CheckTypeIsString('iconServiceUrl', iconServiceUrl);
  ArgumentUtility.CheckTypeIsObject('iconContext', iconContext);

  dropDownList.change(function ()
  {
    if (command == null)
      return;

    if (isAutoPostBackEnabled)
    {
      command = BocReferenceValueBase.UpdateCommand(command, null, null, null);
    }
    else
    {
      var businessObject = BocReferenceValue.GetSelectedValue(dropDownList, nullValueString);
      command = BocReferenceValueBase.UpdateCommand(command, businessObject, iconServiceUrl, iconContext);
    }
  });
};

BocReferenceValue.GetSelectedValue = function (dropDownList, nullValueString)
{
  ArgumentUtility.CheckNotNullAndTypeIsObject('dropDownList', dropDownList);
  ArgumentUtility.CheckNotNullAndTypeIsString('nullValueString', nullValueString);

  if (dropDownList.length == 0 || dropDownList.attr('selectedIndex') < 0)
    return nullValueString;
  var selectedValue = dropDownList.children()[dropDownList.attr('selectedIndex')].value;
  if (selectedValue == nullValueString)
    return null;
  return selectedValue;
}

//  Returns the number of rows selected for the specified BocList
BocReferenceValue.GetSelectionCount = function (referenceValueDropDownListID, nullValueString)
{
  ArgumentUtility.CheckNotNullAndTypeIsString('referenceValueDropDownListID', referenceValueDropDownListID);
  ArgumentUtility.CheckNotNullAndTypeIsString('nullValueString', nullValueString);

  var dropDownList = $('#' + referenceValueDropDownListID);
  if (BocReferenceValue.GetSelectedValue(dropDownList, nullValueString) == null)
    return 0;
  return 1;
}

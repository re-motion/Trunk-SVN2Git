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
function BocReferenceValue_InitializeGlobals (nullValue) 
{
  _bocReferenceValue_nullValue = nullValue;
}

//  Returns the number of rows selected for the specified BocList
function BocReferenceValue_GetSelectionCount (referenceValueDropDownListID)
{
  var dropDownList = document.getElementById (referenceValueDropDownListID);
  if (dropDownList == null || dropDownList.selectedIndex < 0)
    return 0;
  if (dropDownList.children[dropDownList.selectedIndex].value == _bocReferenceValue_nullValue)
    return 0;
  return 1;
}

//function BocReferenceValue_OnMouseOver (context, cssClass) 
//{
//  var className = context.className;
//  className = className.substr (0, className.lastIndexOf (' '));
//  context.className = className + ' ' + cssClass;
//}

//function BocReferenceValue_OnMouseOut (context, cssClass) 
//{
//  var className = context.className;
//  className = className.substr (0, className.lastIndexOf (' '));
//  context.className = className + ' ' + cssClass;
//}

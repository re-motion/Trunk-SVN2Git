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
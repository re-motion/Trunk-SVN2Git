/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

//  BocCheckBox.js contains client side scripts used by BocCheckBox.

//  The descriptions used for the true, false, and null values
var _bocCheckBox_trueDescription;
var _bocCheckBox_falseDescription;

//  Initializes the strings used to represent the true, false and null values.
//  Call this method once in a startup script.
function BocCheckBox_InitializeGlobals (trueDescription, falseDescription)
{
  _bocCheckBox_trueDescription = trueDescription;
  _bocCheckBox_falseDescription = falseDescription;
}

// Toggle the value of the checkbox.
// checkBox: The check box.
// label: The label containing the description for the value. null for no description.
function BocCheckBox_ToggleCheckboxValue (checkBox, label, trueDescription, falseDescription)
{    
  checkBox.checked = !checkBox.checked;
  BocCheckBox_OnClick (checkBox, label, trueDescription, falseDescription);
}

//  Update the text-represention of the check-box value.
function BocCheckBox_OnClick (checkBox, label, trueDescription, falseDescription)
{    
 // Update the controls
  var checkBoxToolTip;
  var labelText;
  
  if (checkBox.checked)
  {
    var description;
    if (trueDescription == null)
      description = _bocCheckBox_trueDescription;
    else
      description = trueDescription;
    checkBoxToolTip = description;
    labelText = description;
  }
  else
  {
    var description;
    if (falseDescription == null)
      description = _bocCheckBox_falseDescription;
    else
      description = falseDescription;
    labelText = description;
  }
  if (label != null)
    label.innerHTML = labelText;
}

/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

//  BocBooleanValue.js contains client side scripts used by BocBooleanValue.

var _bocBooleanValue_Resources = new Object();

BocBooleanValue_Resource = function(
  trueValue,
  falseValue,
  nullValue,
  trueDescription,
  falseDescription,
  nullDescription,
  trueIconUrl,
  falseIconUrl,
  nullIconUrl)
{
  var _trueValue = trueValue;
  var _falseValue = falseValue;
  var _nullValue = nullValue;

  var _trueDescription = trueDescription;
  var _falseDescription = falseDescription;
  var _nullDescription = nullDescription;

  var _trueIconUrl = trueIconUrl;
  var _falseIconUrl = falseIconUrl;
  var _nullIconUrl = nullIconUrl;

  this.SelectNextCheckboxValue = function(
      icon,
      label,
      hiddenField,
      isRequired,
      trueDescription,
      falseDescription,
      nullDescription)
  {
    var trueValue = _trueValue;
    var falseValue = _falseValue;
    var nullValue =_nullValue;

    var oldValue = hiddenField.value;
    var newValue;

    //  Select the next value.
    //  true -> false -> null -> true
    if (isRequired)
    {
      if (oldValue == falseValue)
        newValue = trueValue;
      else
        newValue = falseValue;
    }
    else
    {
      if (oldValue == falseValue)
        newValue = nullValue;
      else if (oldValue == nullValue)
        newValue = trueValue;
      else
        newValue = falseValue;
    }

    // Update the controls
    hiddenField.value = newValue;
    var iconSrc;
    var iconAlt;
    var labelText;

    if (newValue == falseValue)
    {
      iconSrc = _falseIconUrl;
      var description;
      if (falseDescription == null)
        description = _falseDescription;
      else
        description = falseDescription;
      iconAlt = description;
      labelText = description;
    }
    else if (newValue == nullValue)
    {
      iconSrc = _nullIconUrl;
      var description;
      if (nullDescription == null)
        description = _nullDescription;
      else
        description = nullDescription;
      iconAlt = description;
      labelText = description;
    }
    else if (newValue == trueValue)
    {
      iconSrc = _trueIconUrl;
      var description;
      if (trueDescription == null)
        description = _trueDescription;
      else
        description = trueDescription;
      iconAlt = description;
      labelText = description;
    }
    icon.src = iconSrc;
    icon.alt = iconAlt;
    if (label != null)
      label.innerHTML = labelText;

    if (typeof (hiddenField.fireEvent) != 'undefined')
      hiddenField.fireEvent('onchange');
    else if (typeof (hiddenField.dispatchEvent) != 'undefined')
      hiddenField.dispatchEvent('change');
    else if (hiddenField.onchange != null)
      hiddenField.onchange();
  };
}

//  Initializes the strings used to represent the true, false and null values.
//  Call this method once in a startup script.
function BocBooleanValue_InitializeGlobals(
    key,
    trueValue, 
    falseValue, 
    nullValue, 
    trueDescription,
    falseDescription,
    nullDescription,
    trueIconUrl, 
    falseIconUrl, 
    nullIconUrl)
{
  _bocBooleanValue_Resources[key] = new BocBooleanValue_Resource(
      trueValue,
      falseValue,
      nullValue,
      trueDescription,
      falseDescription,
      nullDescription,
      trueIconUrl,
      falseIconUrl,
      nullIconUrl);
}

// Selected the next value of the tri-state checkbox, skipping the null value if isRequired is true.
// icon: The icon representing the tri-state checkbox.
// label: The label containing the description for the value. null for no description.
// hiddenField: The hidden input field used to store the value between postbacks.
// isRequired: true to enqable the null value, false to limit the choices to true and false.
function BocBooleanValue_SelectNextCheckboxValue (
  key,
  icon,
  label,
  hiddenField,
  isRequired,
  trueDescription,
  falseDescription,
  nullDescription)
{
  var resource = _bocBooleanValue_Resources[key];
  resource.SelectNextCheckboxValue(icon,
  label,
  hiddenField,
  isRequired,
  trueDescription,
  falseDescription,
  nullDescription);
}

function BocBooleanValue_OnKeyDown (context)
{
  if (event.keyCode == 32)
  {
    context.click();
    event.cancelBubble = true;
    event.returnValue = false;
  }
}

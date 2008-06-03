/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

function TypeUtility()
{
}

TypeUtility.IsObject = function (value)
{
  return typeof (value) == 'object';    
};

TypeUtility.IsString = function (value)
{
  return typeof (value) == 'string';    
};

TypeUtility.IsNumber = function (value)
{
  return typeof (value) == 'number';    
};

TypeUtility.IsBoolean = function (value)
{
  return typeof (value) == 'boolean';    
};

TypeUtility.IsFunction = function (value)
{
  return typeof (value) == 'function';    
};

TypeUtility.IsUndefined = function (value)
{
  return typeof (value) == 'undefined';    
};

TypeUtility.IsNull = function (value)
{
  return ! TypeUtility.IsUndefined (value) && value == null;    
};


function StringUtility()
{
}

StringUtility.IsNullOrEmpty = function (value)
{
  ArgumentUtility.CheckTypeIsString ('value', value);
  return TypeUtility.IsNull (value) || value.length == 0;    
};


function ArgumentUtility()
{
}

// Checks that value is not null.
ArgumentUtility.CheckNotNull = function (name, value)
{
  if (TypeUtility.IsNull (value))
    throw ('Error: The value of parameter "' + name + '" is null.');
};

// Checks that value is not null and of type string.
ArgumentUtility.CheckTypeIsString = function (name, value)
{
  if (TypeUtility.IsNull (value))
    return;
  if (! TypeUtility.IsString (value))
    throw ('Error: The value of parameter "' + name + '" is not a string.');
};

// Checks that value is not null and of type string.
ArgumentUtility.CheckNotNullAndTypeIsString = function (name, value)
{
  ArgumentUtility.CheckNotNull (name, value);
  ArgumentUtility.CheckTypeIsString (name, value);
};

// Checks that value is not null and of type string.
ArgumentUtility.CheckTypeIsObject = function (name, value)
{
  if (TypeUtility.IsNull (value))
    return;
  if (! TypeUtility.IsObject (value))
    throw ('Error: The value of parameter "' + name + '" is not an object.');
};

// Checks that value is not null and of type string.
ArgumentUtility.CheckNotNullAndTypeIsObject = function (name, value)
{
  ArgumentUtility.CheckNotNull (name, value);
  ArgumentUtility.CheckTypeIsObject (name, value);
};

// Checks that value is not null and of type number.
ArgumentUtility.CheckTypeIsNumber = function (name, value)
{
  if (TypeUtility.IsNull (value))
    return;
  if (! TypeUtility.IsNumber (value))
    throw ('Error: The value of parameter "' + name + '" is not a number.');
};

// Checks that value is not null and of type number.
ArgumentUtility.CheckNotNullAndTypeIsNumber = function (name, value)
{
  ArgumentUtility.CheckNotNull (name, value);
  ArgumentUtility.CheckTypeIsNumber (name, value);
};

// Checks that value is not null and of type boolean.
ArgumentUtility.CheckTypeIsBoolean = function (name, value)
{
  if (TypeUtility.IsNull (value))
    return;
  if (! TypeUtility.IsBoolean (value))
    throw ('Error: The value of parameter "' + name + '" is not a boolean.');
};

// Checks that value is not null and of type boolean.
ArgumentUtility.CheckNotNullAndTypeIsBoolean = function (name, value)
{
  ArgumentUtility.CheckNotNull (name, value);
  ArgumentUtility.CheckTypeIsBoolean (name, value);
};

// Checks that value is not null and of type function.
ArgumentUtility.CheckTypeIsFunction = function (name, value)
{
  if (TypeUtility.IsNull (value))
    return;
  if (! TypeUtility.IsFunction (value))
    throw ('Error: The value of parameter "' + name + '" is not a function.');
};

// Checks that value is not null and of type function.
ArgumentUtility.CheckNotNullAndTypeIsFunction = function (name, value)
{
  ArgumentUtility.CheckNotNull (name, value);
  ArgumentUtility.CheckTypeIsFunction (name, value);
};


function StyleUtility()
{}

StyleUtility.CreateBorderSpans = function (element)
{
  var elementBody = element.firstChild;
  
  StyleUtility.CreateAndAppendBorderSpan (elementBody, element.id, 'top');  
  StyleUtility.CreateAndAppendBorderSpan (elementBody, element.id, 'left');
  StyleUtility.CreateAndAppendBorderSpan (elementBody, element.id, 'bottom');
  StyleUtility.CreateAndAppendBorderSpan (elementBody, element.id, 'right');
  StyleUtility.CreateAndAppendBorderSpan (elementBody, element.id, 'topLeft');
  var topRight = StyleUtility.CreateAndAppendBorderSpan (elementBody, element.id, 'topRight');
  var bottomLeft = StyleUtility.CreateAndAppendBorderSpan (elementBody, element.id, 'bottomLeft');
  var bottomRight = StyleUtility.CreateAndAppendBorderSpan (elementBody, element.id, 'bottomRight');
  
  StyleUtility.CalculateBorderSpans (element, topRight, bottomLeft, bottomRight);
  
  var elementID = element.id;
  var resizeHandler =  function () { StyleUtility.OnResize (elementID); }
  if (! TypeUtility.IsUndefined (window.attachEvent))
  {
    // // Make the function part of the object to provide 'this' pointer to object inside the handler.
    // var uniqueKey = eventType + handler;
    // object['e' + uniqueKey] = handler;
    // object[uniqueKey] = function() { object['e' + uniqueKey](window.event); }
    // object.attachEvent ('on' + eventType, object[uniqueKey]);
    window.attachEvent ('onresize', resizeHandler);    
    element.attachEvent ('onresize', resizeHandler);    
    elementBody.attachEvent ('onresize', resizeHandler);    
  } 
  else if (! TypeUtility.IsUndefined (window.addEventListener))
  {
    window.addEventListener ('resize', resizeHandler, false);
    element.addEventListener ('resize', resizeHandler, false);
    elementBody.addEventListener ('resize', resizeHandler, false);
  }
}

StyleUtility.CalculateBorderSpans = function (element, topRight, bottomLeft, bottomRight)
{
  topRight.style.left = topRight.offsetParent.clientLeft + topRight.offsetParent.clientWidth - topRight.offsetWidth  + 'px';
  bottomLeft.style.top = bottomLeft.offsetParent.clientTop + bottomLeft.offsetParent.clientHeight - bottomLeft.offsetHeight  + 'px';
  bottomRight.style.top = bottomRight.offsetParent.clientTop + bottomRight.offsetParent.clientHeight - bottomRight.offsetHeight  + 'px';
  bottomRight.style.left = bottomRight.offsetParent.clientLeft + bottomRight.offsetParent.clientWidth - bottomRight.offsetWidth  + 'px';

  var scrollDiv = element.firstChild.firstChild;
  if (scrollDiv != null && !TypeUtility.IsUndefined (scrollDiv.tagName) && scrollDiv.tagName.toLowerCase() == 'div')
  {
    if (scrollDiv.scrollHeight > scrollDiv.clientHeight)
      topRight.style.display = 'none';
    else
      topRight.style.display = '';
    
    if (scrollDiv.scrollWidth > scrollDiv.clientWidth)
      bottomLeft.style.display = 'none';
    else
      bottomLeft.style.display = '';
    
    if (   (scrollDiv.scrollHeight > scrollDiv.clientHeight && scrollDiv.scrollWidth == scrollDiv.clientWidth) 
        || (scrollDiv.scrollHeight == scrollDiv.clientHeight && scrollDiv.scrollWidth > scrollDiv.clientWidth))
    {
      bottomRight.style.display = 'none';
    }
    else
    {
      bottomRight.style.display = '';
    }
  }  
}

StyleUtility.CreateAndAppendBorderSpan = function (elementBody, elementID, className)
{
  var borderSpan = document.createElement ('SPAN');
  borderSpan.id = elementID + '_' + className;
  borderSpan.className = className;
  elementBody.appendChild (borderSpan);
  
  return borderSpan
}

StyleUtility.OnResize = function (elementID)
{
  var element = document.getElementById (elementID);
  if (element != null)
  {
    var topRight = document.getElementById (elementID + '_topRight');
    var bottomLeft = document.getElementById (elementID + '_bottomLeft');
    var bottomRight = document.getElementById (elementID + '_bottomRight');
    
    StyleUtility.CalculateBorderSpans (element, topRight, bottomLeft, bottomRight);
  }
}

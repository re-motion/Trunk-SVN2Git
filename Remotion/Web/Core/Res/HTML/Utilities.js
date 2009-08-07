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
function TypeUtility() {
}

TypeUtility.IsObject = function(value) {
    return typeof (value) == 'object';
};

TypeUtility.IsString = function(value) {
    return typeof (value) == 'string';
};

TypeUtility.IsNumber = function(value) {
    return typeof (value) == 'number';
};

TypeUtility.IsBoolean = function(value) {
    return typeof (value) == 'boolean';
};

TypeUtility.IsFunction = function(value) {
    return typeof (value) == 'function';
};

TypeUtility.IsUndefined = function(value) {
    return typeof (value) == 'undefined';
};

TypeUtility.IsNull = function(value) {
    return !TypeUtility.IsUndefined(value) && value == null;
};


function StringUtility() {
}

StringUtility.IsNullOrEmpty = function(value) {
    ArgumentUtility.CheckTypeIsString('value', value);
    return TypeUtility.IsNull(value) || value.length == 0;
};


function ArgumentUtility() {
}

// Checks that value is not null.
ArgumentUtility.CheckNotNull = function(name, value) {
    if (TypeUtility.IsNull(value))
        throw ('Error: The value of parameter "' + name + '" is null.');
};

// Checks that value is not null and of type string.
ArgumentUtility.CheckTypeIsString = function(name, value) {
    if (TypeUtility.IsNull(value))
        return;
    if (!TypeUtility.IsString(value))
        throw ('Error: The value of parameter "' + name + '" is not a string.');
};

// Checks that value is not null and of type string.
ArgumentUtility.CheckNotNullAndTypeIsString = function(name, value) {
    ArgumentUtility.CheckNotNull(name, value);
    ArgumentUtility.CheckTypeIsString(name, value);
};

// Checks that value is not null and of type string.
ArgumentUtility.CheckTypeIsObject = function(name, value) {
    if (TypeUtility.IsNull(value))
        return;
    if (!TypeUtility.IsObject(value))
        throw ('Error: The value of parameter "' + name + '" is not an object.');
};

// Checks that value is not null and of type string.
ArgumentUtility.CheckNotNullAndTypeIsObject = function(name, value) {
    ArgumentUtility.CheckNotNull(name, value);
    ArgumentUtility.CheckTypeIsObject(name, value);
};

// Checks that value is not null and of type number.
ArgumentUtility.CheckTypeIsNumber = function(name, value) {
    if (TypeUtility.IsNull(value))
        return;
    if (!TypeUtility.IsNumber(value))
        throw ('Error: The value of parameter "' + name + '" is not a number.');
};

// Checks that value is not null and of type number.
ArgumentUtility.CheckNotNullAndTypeIsNumber = function(name, value) {
    ArgumentUtility.CheckNotNull(name, value);
    ArgumentUtility.CheckTypeIsNumber(name, value);
};

// Checks that value is not null and of type boolean.
ArgumentUtility.CheckTypeIsBoolean = function(name, value) {
    if (TypeUtility.IsNull(value))
        return;
    if (!TypeUtility.IsBoolean(value))
        throw ('Error: The value of parameter "' + name + '" is not a boolean.');
};

// Checks that value is not null and of type boolean.
ArgumentUtility.CheckNotNullAndTypeIsBoolean = function(name, value) {
    ArgumentUtility.CheckNotNull(name, value);
    ArgumentUtility.CheckTypeIsBoolean(name, value);
};

// Checks that value is not null and of type function.
ArgumentUtility.CheckTypeIsFunction = function(name, value) {
    if (TypeUtility.IsNull(value))
        return;
    if (!TypeUtility.IsFunction(value))
        throw ('Error: The value of parameter "' + name + '" is not a function.');
};

// Checks that value is not null and of type function.
ArgumentUtility.CheckNotNullAndTypeIsFunction = function(name, value) {
    ArgumentUtility.CheckNotNull(name, value);
    ArgumentUtility.CheckTypeIsFunction(name, value);
};


function StyleUtility()
{ }

StyleUtility.CreateBorderSpans = function(element, standardMode)
{

  var elementBody = element;
  if (!standardMode)
  {
    elementBody = $(element).children(':first')[0];
  }
  StyleUtility.CreateAndAppendBorderSpan(elementBody, element.id, 'top');
  StyleUtility.CreateAndAppendBorderSpan(elementBody, element.id, 'left');
  StyleUtility.CreateAndAppendBorderSpan(elementBody, element.id, 'bottom');
  StyleUtility.CreateAndAppendBorderSpan(elementBody, element.id, 'right');
  StyleUtility.CreateAndAppendBorderSpan(elementBody, element.id, 'topLeft');
  var topRight = StyleUtility.CreateAndAppendBorderSpan(elementBody, element.id, 'topRight');
  var bottomLeft = StyleUtility.CreateAndAppendBorderSpan(elementBody, element.id, 'bottomLeft');
  var bottomRight = StyleUtility.CreateAndAppendBorderSpan(elementBody, element.id, 'bottomRight');

  StyleUtility.CalculateBorderSpans(elementBody, topRight, bottomLeft, bottomRight, standardMode);

  var elementID = element.id;
  var resizeHandler = function() { StyleUtility.OnResize(elementID, standardMode); }
  $(window).resize(resizeHandler);
}

StyleUtility.CalculateBorderSpans = function(element, topRight, bottomLeft, bottomRight, standardMode)
{

  var right = 0;  // $(element).position().left + $(element).width();
  var bottom = $(element).position().top + $(element).height();
  var topRightOffset = 0; // $(topRight).offset().left;
  var bottomLeftOffset = $(bottomLeft).offset().top;
  var bottomRightVerticalOffset = $(bottomRight).offset().top;
  var bottomRightHorizontalOffset = 0; // $(bottomRight).offset().left;

  if (!standardMode)
  { // QuirksMode calculations for IE - Firefox places borders correctly with jQuery positioning
    var offsetParent = topRight.offsetParent;
    if (offsetParent)
    { // this is null in Firefox
      right = offsetParent.clientLeft + offsetParent.clientWidth;
      bottom = offsetParent.clientTop + offsetParent.clientHeight;
      topRightOffset = topRight.offsetWidth;
      bottomLeftOffset = bottomLeft.offsetHeight;
      bottomRightVerticalOffset = bottomRight.offsetHeight;
      bottomRightHorizontalOffset = bottomRight.offsetWidth;
    }
  }

  $(topRight).css(standardMode ? 'right' : 'left', right - topRightOffset);
  $(bottomLeft).css('top', bottom - bottomLeftOffset);
  $(bottomRight).css('top', bottom - bottomRightVerticalOffset);
  $(bottomRight).css(standardMode ? 'right' : 'left', right - bottomRightHorizontalOffset);

  var scrollDiv = $(element).children(':first').children(':first');
  if ((scrollDiv.length == 1) && !TypeUtility.IsUndefined(scrollDiv[0].nodeName) && (scrollDiv[0].nodeName.toLowerCase() == 'div'))
  {
    if (scrollDiv[0].scrollHeight > scrollDiv.height())
      $(topRight).css('display', 'none');
    else
      $(topRight).css('display', 'inline');

    if (scrollDiv[0].scrollWidth > scrollDiv.width())
      $(bottomLeft).css('display', 'none');
    else
      $(bottomLeft).css('display', 'inline');

    if ((scrollDiv[0].scrollHeight > scrollDiv.height() && scrollDiv[0].scrollWidth == scrollDiv.width())
    || (scrollDiv[0].scrollHeight == scrollDiv.height() && scrollDiv[0].scrollWidth > scrollDiv.width()))
    {
      $(bottomRight).css('display', 'none');
    }
    else
    {
      $(bottomRight).css('display', 'inline');
    }
  }

}

StyleUtility.CreateAndAppendBorderSpan = function(elementBody, elementID, className) {
    if (elementBody.nodeType != 1) {
        elementBody = elementBody.parentNode;
    }

    var borderSpan = document.createElement('SPAN');
    borderSpan.id = elementID + '_' + className;
    borderSpan.className = className;

    elementBody.appendChild(borderSpan);

    return borderSpan
}

StyleUtility.OnResize = function(elementID, standardMode) {
    var element = document.getElementById(elementID);
    if (element != null) {
        var topRight = document.getElementById(elementID + '_topRight');
        var bottomLeft = document.getElementById(elementID + '_bottomLeft');
        var bottomRight = document.getElementById(elementID + '_bottomRight');

        StyleUtility.CalculateBorderSpans(element, topRight, bottomLeft, bottomRight, standardMode);
    }
}

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

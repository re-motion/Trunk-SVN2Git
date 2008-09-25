/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using Remotion.Collections;
using Remotion.Utilities;

namespace Remotion.Web.ExecutionEngine
{

/// <summary> Specifies in which direction a parameter is passed. </summary>
public enum WxeParameterDirection
{
  /// <summary> The parameter is passed from the caller to the callee. </summary>
  In,
  /// <summary> The parameter is returned from the callee to the caller. </summary>
  Out,
  /// <summary> The parameter is passed from the caller to the callee and returned back to the caller. </summary>
  InOut
}

/// <summary> Declares a WXE parameter. </summary>
[Serializable]
public class WxeParameterDeclaration
{
  private readonly string _name;
  private readonly bool _required;
  private readonly WxeParameterDirection _direction;
  private readonly Type _type;
  [NonSerialized]
  private WxeParameterConverter _converter;

  public WxeParameterDeclaration (string name, bool required, WxeParameterDirection direction, Type type)
  {
    _name = name;
    _required = required;
    _direction = direction;
    _type = type;
  }
  
  public string Name
  {
    get { return _name; }
  }

  public bool Required
  {
    get { return _required; }
  }

  public WxeParameterDirection Direction
  {
    get { return _direction; }
  }

  public Type Type
  {
    get { return _type; }
  }

  /// <summary> Copy a single caller variable to a callee parameter. </summary>
  public void CopyToCallee (string actualParameterName, NameObjectCollection callerVariables, NameObjectCollection calleeVariables)
  {
    if (_direction != WxeParameterDirection.Out)
      CopyParameter (actualParameterName, callerVariables, _name, calleeVariables, _required);
  }

  /// <summary> Copy a value to a callee parameter. </summary>
  public void CopyToCallee (object parameterValue, NameObjectCollection calleeVariables)
  {
    if (_direction == WxeParameterDirection.Out)
      throw new ApplicationException ("Constant provided for output parameter.");

    SetParameter (_name, parameterValue, calleeVariables); 
  }

  /// <summary> Copy a single callee parameter back to a caller variable. </summary>
  public void CopyToCaller (string actualParameterName, NameObjectCollection calleeVariables, NameObjectCollection callerVariables)
  {
    if (_direction != WxeParameterDirection.In)
      CopyParameter (_name, calleeVariables, actualParameterName, callerVariables, false);
  }

  /// <summary> Copy fromVariables[fromName] to toVariables[toName]. </summary>
  private void CopyParameter (string fromName, NameObjectCollection fromVariables, string toName, NameObjectCollection toVariables, bool required)
  {
    object value = fromVariables[fromName];
    if (value == null && required)
      throw new ApplicationException ("Parameter '" + fromName + "' is missing.");
    SetParameter (toName, value, toVariables);
  }

  /// <summary> Set the parameter variables[parameterName] to the specified value. </summary>
  private void SetParameter (string parameterName, object value, NameObjectCollection variables)
  {
    if (value != null && _type != null && ! _type.IsAssignableFrom (value.GetType()))
      throw new ApplicationException ("Parameter '" + parameterName + "' has unexpected type " + value.GetType().FullName + " (" + _type.FullName + " was expected).");
    variables[parameterName] = value;
  }

  /// <summary>
  /// Gets the value of the parameter described by this declaration from a function's variable list.
  /// </summary>
  /// <param name="variables">The variable list to get the parameter value from.</param>
  public object GetValue (NameObjectCollection variables)
  {
    ArgumentUtility.CheckNotNull ("variables", variables);
    return variables[_name];
  }

  public WxeParameterConverter Converter
  {
    get
    {
      if (_converter == null)
        _converter = new WxeParameterConverter (this);
      return _converter;
    }
  }

  public bool IsIn
  {
    get { return Direction == WxeParameterDirection.In || Direction == WxeParameterDirection.InOut; }
  }

  public bool IsOut
  {
    get { return Direction == WxeParameterDirection.Out || Direction == WxeParameterDirection.InOut; }
  }
}
}

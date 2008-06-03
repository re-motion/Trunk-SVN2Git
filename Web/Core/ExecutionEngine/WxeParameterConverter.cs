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
using System.Globalization;
using Remotion.Collections;
using Remotion.Utilities;

namespace Remotion.Web.ExecutionEngine
{

public class WxeParameterConverter
{
  private WxeParameterDeclaration _parameter;

  public WxeParameterConverter (WxeParameterDeclaration parameter)
  {
    ArgumentUtility.CheckNotNull ("parameter", parameter);
    _parameter = parameter;
  }
  
  protected WxeParameterDeclaration Parameter
  {
    get { return _parameter; }
  }

  /// <summary> Converts a parameter's value to its string representation. </summary>
  /// <param name="value"> The value to be converted. Must be of assignable to the <see cref="Type"/>. </param>
  /// <param name="callerVariables"> 
  ///   The optional list of caller variables. Used to dereference a <see cref="WxeVariableReference"/>.
  /// </param>
  /// <returns> 
  ///   A <see cref="string"/> or <see langword="null"/> if the conversion is not possible but the parameter is not
  ///   required.
  /// </returns>
  /// <exception cref="WxeException"> Thrown if the <paramref name="value"/> could not be converted. </exception>
  public string ConvertToString (object value, NameObjectCollection callerVariables)
  {
    CheckForRequiredOutParameter();

    WxeVariableReference varRef = value as WxeVariableReference;
    if (varRef != null)
      return ConvertVarRefToString (varRef, callerVariables);

    return ConvertObjectToString (value);
  }

  /// <summary> Converts a <see cref="WxeVariableReference"/>'s value to its string representation. </summary>
  /// <param name="varRef"> 
  ///   The <see cref="WxeVariableReference"/> to be converted. The referenced value must be of assignable to the 
  ///   <see cref="WxeParameterDeclaration"/>'s <see cref="Type"/>. Must not be <see langword="null"/>.
  /// </param>
  /// <param name="callerVariables">
  ///   The optional list of caller variables. Used to dereference a <see cref="WxeVariableReference"/>.
  /// </param>
  /// <returns> 
  ///   A <see cref="string"/> or <see langword="null"/> if the conversion is not possible but the parameter is not
  ///   required.
  /// </returns>
  /// <exception cref="WxeException"> Thrown if the <paramref name="value"/> could not be converted. </exception>
  protected string ConvertVarRefToString (WxeVariableReference varRef, NameObjectCollection callerVariables)
  {
    ArgumentUtility.CheckNotNull ("varRef", varRef);

    if (callerVariables == null)
    {
      if (_parameter.Required)
      {
        throw new WxeException (string.Format (
            "Required IN parameter '{0}' is a Variable Reference but no caller variables have been provided.", 
            _parameter.Name));
      }
      return null;
    }

    object value = callerVariables[_parameter.Name];
    
    if (value is WxeVariableReference)
    {
      if (_parameter.Required)
      {
        throw new WxeException (string.Format (
            "Required IN parameter '{0}' is a Variable Reference but no caller variables have been provided.", 
            _parameter.Name));
      }
      return null;
    }

    return ConvertObjectToString (value);
  }

  /// <summary> Converts a parameter's value to its string representation. </summary>
  /// <param name="value"> The value to be converted. Must be of assignable to the <see cref="Type"/>. </param>
  /// <returns> 
  ///   A <see cref="string"/> or <see langword="null"/> if the conversion is not possible but the parameter is not
  ///   required.
  /// </returns>
  /// <exception cref="WxeException"> Thrown if the <paramref name="value"/> could not be converted. </exception>
  protected string ConvertObjectToString (object value)
  {
    if (value != null && ! _parameter.Type.IsAssignableFrom (value.GetType()))
      throw new ArgumentTypeException ("value", _parameter.Type, value.GetType());

    if (! _parameter.Required && value == null)
      return null;

    value = TryConvertObjectToString (value);
    if (value is string)
      return (string) value;

    if (_parameter.Required)
    {
      throw new WxeException (string.Format (
          "Only parameters that can be restored from their string representation may be converted to a string. Parameter: '{0}'.",
          _parameter.Name));
    }
    return null;
  }

  /// <summary> Tries to convert a parameter's value to its string representation. </summary>
  /// <param name="value"> The value to be converted. </param>
  /// <returns> A <see cref="string"/> or the <paramref name="value"/> if the conversion is not possible. </returns>
  protected object TryConvertObjectToString (object value)
  {
    Type sourceType = _parameter.Type;
    Type destinationType = typeof (string);

    //TODO: #if DEBUG
    if (! TypeConversionProvider.Current.CanConvert (sourceType, destinationType))
      return value;

    return TypeConversionProvider.Current.Convert (
        null, CultureInfo.InvariantCulture, sourceType, destinationType, value);
  }

  protected void CheckForRequiredOutParameter ()
  {
    if (_parameter.Required && _parameter.Direction == WxeParameterDirection.Out)
    {
      throw new WxeException (string.Format (
          "Required OUT parameters cannot be converted to a string. Parameter: '{0}'", _parameter.Name));
    }
  }
}

}

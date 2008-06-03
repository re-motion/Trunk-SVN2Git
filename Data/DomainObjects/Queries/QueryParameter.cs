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
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Queries
{
/// <summary>
/// Represents a parameter that is used in a query.
/// </summary>
[Serializable]
public class QueryParameter
{
  // types

  // static members and constants

  // member fields

  private string _name;
  private object _value;
  private QueryParameterType _parameterType;

  // construction and disposing

  /// <summary>
  /// Initializes a new instance of the <see cref="QueryParameter"/> class with a <see cref="ParameterType"/> of <see cref="QueryParameterType.Value"/>.
  /// </summary>
  /// <param name="name">The name of the parameter.</param>
  /// <param name="value">The value of the parameter.</param>
  public QueryParameter (string name, object value) : this (name, value, QueryParameterType.Value)
  {
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="QueryParameter"/> class.
  /// </summary>
  /// <param name="name">The name of the parameter. Must not be <see langword="null"/>.</param>
  /// <param name="value">The value of the parameter.</param>
  /// <param name="parameterType">The <see cref="QueryParameterType"/> of the parameter.</param>
  /// <exception cref="System.ArgumentNullException"><paramref name="name"/> is <see langword="null"/>.</exception>
  /// <exception cref="Remotion.Utilities.ArgumentEmptyException"><paramref name="name"/> is an empty string.</exception>
  /// <exception cref="System.ArgumentOutOfRangeException"><paramref name="parameterType"/> is not a valid enum value.</exception>
  public QueryParameter (string name, object value, QueryParameterType parameterType)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("name", name);
    ArgumentUtility.CheckValidEnumValue ("parameterType", parameterType);

    _name = name;
    _value = value;
    _parameterType = parameterType;
  }

  // methods and properties

  /// <summary>
  /// Gets the name of the <see cref="QueryParameter"/>.
  /// </summary>
  public string Name
  {
    get { return _name; }
  }

  /// <summary>
  /// Gets or sets the value of the <see cref="QueryParameter"/>.
  /// </summary>
  public object Value
  {
    get { return _value; }
    set { _value = value; }
  }

  /// <summary>
  /// Gets or sets the <see cref="QueryParameterType"/> of the <see cref="QueryParameter"/>.
  /// </summary>
  public QueryParameterType ParameterType
  {
    get { return _parameterType; }
    set { _parameterType = value; }
  }
}
}

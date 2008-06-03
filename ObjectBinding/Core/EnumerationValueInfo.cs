/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using Remotion.Utilities;

namespace Remotion.ObjectBinding
{
  /// <summary> Default implementation of the <see cref="IEnumerationValueInfo"/> interface. </summary>
  public class EnumerationValueInfo : IEnumerationValueInfo
  {
    private readonly object _value;
    private readonly string _identifier;
    private readonly string _displayName;
    private readonly bool _isEnabled;

    /// <summary> Initializes a new instance of the <b>EnumerationValueInfo</b> type. </summary>
    public EnumerationValueInfo (object value, string identifier, string displayName, bool isEnabled)
    {
      ArgumentUtility.CheckNotNull ("value", value);
      ArgumentUtility.CheckNotNullOrEmpty ("identifier", identifier);

      _value = value;
      _identifier = identifier;
      _displayName = displayName;
      _isEnabled = isEnabled;
    }

    /// <summary> Gets the object representing the original value, e.g. a System.Enum type. </summary>
    public object Value
    {
      get { return _value; }
    }

    /// <summary> Gets the string presented to the user. </summary>
    public string Identifier
    {
      get { return _identifier; }
    }

    /// <summary> Gets the string presented to the user. </summary>
    public virtual string DisplayName
    {
      get { return _displayName; }
    }

    /// <summary>
    ///   Gets a flag indicating whether this value should be presented as an option to the user. 
    ///   (If not, existing objects might still use this value.)
    /// </summary>
    public bool IsEnabled
    {
      get { return _isEnabled; }
    }
  }
}

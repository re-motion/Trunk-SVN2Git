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

namespace Remotion.ObjectBinding.BindableObject.Properties
{
  public class StringProperty : PropertyBase, IBusinessObjectStringProperty
  {
    private readonly int? _maxLength;


    public StringProperty (Parameters parameters, int? maxLength)
        : base (parameters)
    {
      _maxLength = maxLength;
    }

    /// <summary>
    ///   Getsthe the maximum length of a string assigned to the property, or <see langword="null"/> if no maximum length is defined.
    /// </summary>
    public int? MaxLength
    {
      get { return _maxLength; }
    }
  }
}

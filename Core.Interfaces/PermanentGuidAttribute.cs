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
using Remotion.Implementation;

namespace Remotion
{
  /// <summary>
  ///   Supplies an identifier that should remain constant even accross refactorings. Can be applied to reference types, properties and fields.
  /// </summary>
  [AttributeUsage (AttributeTargets.Class | AttributeTargets.Property |AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
  public class PermanentGuidAttribute : Attribute
  {
    private readonly Guid _value;

    /// <summary>
    ///   Initializes a new instance of the <see cref="PermanentGuidAttribute"/> class.
    /// </summary>
    /// <param name="value"> The <see cref="String"/> representation of a <see cref="Guid"/>. </param>
    public PermanentGuidAttribute (string value)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("value", value);

      _value = new Guid (value);
    }

    /// <summary>
    ///   Gets the <see cref="Guid"/> supplied during initialization.
    /// </summary>
    public Guid Value
    {
      get { return _value; }
    }
  }
}

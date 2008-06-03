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
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;

namespace Remotion.Data.DomainObjects
{
  /// <summary>
  /// Apply the <see cref="MandatoryAttribute"/> to properties of type <see cref="DomainObject"/>.
  /// </summary>
  [AttributeUsage (AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
  public sealed class MandatoryAttribute : Attribute, INullablePropertyAttribute
  {
    public MandatoryAttribute()
    {
    }

    bool INullablePropertyAttribute.IsNullable
    {
      get { return false; }
    }
  }
}

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

namespace Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader
{
  /// <summary>
  /// Declares a relation as bidirectional.
  /// </summary>
  [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
  public class BidirectionalRelationAttribute: Attribute, IMappingAttribute
  {
    private readonly string _oppositeProperty;

    /// <summary>
    /// Initializes a new instance of the <see cref="BidirectionalRelationAttribute"/> class with the name of the oppsite property.
    /// </summary>
    /// <param name="oppositeProperty">The name of the opposite property. Must not be <see langword="null" /> or empty.</param>
    public BidirectionalRelationAttribute (string oppositeProperty)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("oppositeProperty", oppositeProperty);

      _oppositeProperty = oppositeProperty;
    }

    public string OppositeProperty
    {
      get { return _oppositeProperty; }
    }
  }
}

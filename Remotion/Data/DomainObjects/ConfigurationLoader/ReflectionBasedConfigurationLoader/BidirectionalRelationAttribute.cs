// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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

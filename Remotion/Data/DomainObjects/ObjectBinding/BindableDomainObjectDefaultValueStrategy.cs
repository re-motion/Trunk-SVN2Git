// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
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
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.BindableObject.Properties;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.ObjectBinding
{
  /// <summary>
  /// Implements <see cref="IDefaultValueStrategy"/> for <see cref="DomainObject"/> instances. A <see cref="DomainObject"/> property is defined
  /// to have its default value set if it is a new object and the property has not been touched yet.
  /// </summary>
  public class BindableDomainObjectDefaultValueStrategy : IDefaultValueStrategy
  {
    public static readonly IDefaultValueStrategy Instance = new BindableDomainObjectDefaultValueStrategy ();

    private BindableDomainObjectDefaultValueStrategy () { }

    public bool IsDefaultValue (IBusinessObject obj, PropertyBase property)
    {
      var domainObject = ArgumentUtility.CheckNotNullAndType<DomainObject> ("obj", obj);
      ArgumentUtility.CheckNotNull ("property", property);
      
      if (domainObject.State != StateType.New)
        return false;

      var propertyDefinition = domainObject.ID.ClassDefinition.ResolveProperty (property.PropertyInfo);
      if (propertyDefinition != null)
        return !domainObject.Properties[propertyDefinition.PropertyName].HasBeenTouched;

      return BindableObjectDefaultValueStrategy.Instance.IsDefaultValue (obj, property);
    }
  }
}
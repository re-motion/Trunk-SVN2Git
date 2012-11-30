// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.BusinessObjectPropertyPaths
{
  public class StaticBusinessObjectPropertyPath : BusinessObjectPropertyPathBase
  {
    private readonly string _propertyPathIdentifier;
    private readonly IBusinessObjectProperty[] _properties;

    public StaticBusinessObjectPropertyPath (string propertyPathIdentifier, IBusinessObjectClass root)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("propertyPathIdentifier", propertyPathIdentifier);
      ArgumentUtility.CheckNotNull ("root", root);

      var properties = new List<IBusinessObjectProperty>();
      var currentClass = root;
      var propertyEnumerator = new StaticBusinessObjectPropertyPathPropertyEnumerator (propertyPathIdentifier);

      while (propertyEnumerator.MoveNext (currentClass))
      {
        var currentProperty = propertyEnumerator.Current;
        Assertion.IsNotNull (currentProperty, "StaticPropertyPathPropertyEnumerator never returns null on successful enumeration.");
        properties.Add (currentProperty);

        var currentReferenceProperty = currentProperty as IBusinessObjectReferenceProperty;
        if (currentReferenceProperty != null)
          currentClass = currentReferenceProperty.ReferenceClass;
        else
          currentClass = null;
      }

      _propertyPathIdentifier = propertyPathIdentifier;
      _properties = properties.ToArray();
    }

    public override bool IsDynamic
    {
      get { return false; }
    }

    public override string Identifier
    {
      get { return _propertyPathIdentifier; }
    }

    public override ReadOnlyCollection<IBusinessObjectProperty> Properties
    {
      get { return new ReadOnlyCollection<IBusinessObjectProperty> (_properties); }
    }

    protected override IBusinessObjectPropertyPathPropertyEnumerator GetPropertyEnumerator ()
    {
      return new EvaluatedBusinessObjectPropertyPathPropertyEnumerator (_properties);
    }
  }
}
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
using System.Collections.ObjectModel;
using Remotion.ObjectBinding.BusinessObjectPropertyPaths.Enumerators;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.BusinessObjectPropertyPaths
{
 public enum BusinessObjectPropertyPathListValueBehavior
{
   GetResultForFirstListEntry,
   FailForListProperties
}

  public enum BusinessObjectPropertyPathUnreachableValueBehavior
  {
    ReturnNullForUnreachableValue,
    FailForUnreachableValue
  }

  public abstract class BusinessObjectPropertyPathBase
  {
    protected BusinessObjectPropertyPathBase ()
    {
    }

    public abstract bool IsDynamic { get; }
    public abstract string Identifier { get; }
    public abstract ReadOnlyCollection<IBusinessObjectProperty> Properties { get; }

    protected abstract IBusinessObjectPropertyPathPropertyEnumerator GetResultPropertyEnumerator ();

    public IBusinessObjectPropertyPathResult GetResult (IBusinessObject root, 
        BusinessObjectPropertyPathUnreachableValueBehavior unreachableValueBehavior, 
        BusinessObjectPropertyPathListValueBehavior listValueBehavior)
    {
      ArgumentUtility.CheckNotNull ("root", root);

      var currentObject = root;
      var propertyEnumerator = GetResultPropertyEnumerator();
      int propertyIndex = 0;
      while (propertyEnumerator.MoveNext (currentObject.BusinessObjectClass))
      {
        var currentProperty = propertyEnumerator.Current;

        if (currentProperty == null)
          return new NullBusinessObjectPropertyPathResult();

        if (!(currentProperty is IBusinessObjectReferenceProperty))
          return new EvaluatedBusinessObjectPropertyPathResult (currentObject, currentProperty);

        if (!currentProperty.IsAccessible (currentObject.BusinessObjectClass, currentObject))
          return new NotAccessibleBusinessObjectPropertyPathResult (currentObject.BusinessObjectClass.BusinessObjectProvider);

        currentObject = (IBusinessObject) currentObject.GetProperty (currentProperty);
        if (currentObject == null)
        {
          if (unreachableValueBehavior == BusinessObjectPropertyPathUnreachableValueBehavior.FailForUnreachableValue)
          {
            throw new InvalidOperationException (
                string.Format (
                    "A null value was detected in element {0} of property path '{1}'. Cannot evaluate rest of path.", propertyIndex, Identifier));
          }

          return new NullBusinessObjectPropertyPathResult();
        }

        propertyIndex++;
      }

      throw new InvalidOperationException ("Property path enumeration can never fall through.");
    }
  }
}
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
  public sealed class DynamicBusinessObjectPropertyPath : BusinessObjectPropertyPathBase
  {
    private readonly string _propertyPathIdentifier;

    public DynamicBusinessObjectPropertyPath (string propertyPathIdentifier)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("propertyPathIdentifier", propertyPathIdentifier);
      
      _propertyPathIdentifier = propertyPathIdentifier;
    }

    public override bool IsDynamic
    {
      get { return true; }
    }

    public override string Identifier
    {
      get { return string.Empty; }
    }

    public override ReadOnlyCollection<IBusinessObjectProperty> Properties
    {
      get { throw new NotSupportedException(); }
    }

    protected override IBusinessObjectPropertyPathPropertyEnumerator GetResultPropertyEnumerator ()
    {
      return new DynamicBusinessObjectPropertyPathPropertyEnumerator (_propertyPathIdentifier);
    }

    public IBusinessObjectPropertyPathResult GetResult_Inline (IBusinessObject root)
    {
      ArgumentUtility.CheckNotNull ("root", root);

      var currentObject = root;
      var remainingPropertyPathIdentifier = _propertyPathIdentifier;

      do
      {
        var currentClass = currentObject.BusinessObjectClass;
        var propertyIdentifierAndRemainder =
            remainingPropertyPathIdentifier.Split (
                new[] { currentClass.BusinessObjectProvider.GetPropertyPathSeparator() }, 2, StringSplitOptions.None);
        var currentPropertyIdentifier = propertyIdentifierAndRemainder[0];
        var currentProperty = currentClass.GetPropertyDefinition (currentPropertyIdentifier);

        if (currentProperty == null)
          return new NullBusinessObjectPropertyPathResult();

        if (!currentProperty.IsAccessible (currentClass, currentObject))
          return new NotAccessibleBusinessObjectPropertyPathResult (currentClass.BusinessObjectProvider);

        if (propertyIdentifierAndRemainder.Length == 1)
          return new EvaluatedBusinessObjectPropertyPathResult (currentObject, currentProperty);

        if (! (currentProperty is IBusinessObjectReferenceProperty))
          return new NullBusinessObjectPropertyPathResult();

        remainingPropertyPathIdentifier = propertyIdentifierAndRemainder[1];
        currentObject = (IBusinessObject) currentObject.GetProperty (currentProperty);
      } while (currentObject != null);

      return new NullBusinessObjectPropertyPathResult();
    }
  }
}
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
using Remotion.Utilities;

namespace Remotion.ObjectBinding.BusinessObjectPropertyPaths
{
  public abstract class BusinessObjectPropertyPathPropertyEnumeratorBase : IBusinessObjectPropertyPathPropertyEnumerator
  {
    private string _remainingPropertyPathIdentifier;
    private IBusinessObjectProperty _currentProperty;

    protected BusinessObjectPropertyPathPropertyEnumeratorBase (string propertyPathIdentifier)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("propertyPathIdentifier", propertyPathIdentifier);

      _remainingPropertyPathIdentifier = propertyPathIdentifier;
    }

    protected abstract void HandlePropertyNotFound (
        IBusinessObjectClass businessObjectClass,
        string propertyIdentifier);

    protected abstract void HandlePropertyNotLastPropertyAndNotReferenceProperty (
        IBusinessObjectClass businessObjectClass,
        IBusinessObjectProperty property);

    public IBusinessObjectProperty Current
    {
      get { return _currentProperty; }
    }

    public bool MoveNext (IBusinessObjectClass businessObjectClass)
    {
      ArgumentUtility.CheckNotNull ("businessObjectClass", businessObjectClass);

      _currentProperty = null;
      if (!HasNext)
        return false;

      var propertyIdentifierAndRemainder =
          _remainingPropertyPathIdentifier.Split (
              new[] { businessObjectClass.BusinessObjectProvider.GetPropertyPathSeparator() }, 2, StringSplitOptions.None);

      if (propertyIdentifierAndRemainder.Length == 2)
        _remainingPropertyPathIdentifier = propertyIdentifierAndRemainder[1];
      else
        _remainingPropertyPathIdentifier = string.Empty;

      var propertyIdentifier = propertyIdentifierAndRemainder[0];
      var property = businessObjectClass.GetPropertyDefinition (propertyIdentifier);

      if (property == null)
        HandlePropertyNotFound (businessObjectClass, propertyIdentifier);
      else if (HasNext && ! (property is IBusinessObjectReferenceProperty))
        HandlePropertyNotLastPropertyAndNotReferenceProperty (businessObjectClass, property);
      else
        _currentProperty = property;

      return true;
    }

    private bool HasNext
    {
      get { return _remainingPropertyPathIdentifier.Length > 0; }
    }
  }
}
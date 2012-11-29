﻿// This file is part of the re-motion Core Framework (www.re-motion.org)
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
using Remotion.Utilities;

namespace Remotion.ObjectBinding
{
  public class DynamicPropertyPath
  {
    private readonly string _propertyPathIdentifier;

    public DynamicPropertyPath (string propertyPathIdentifier)
    {
      _propertyPathIdentifier = propertyPathIdentifier;
    }

    public bool IsDynamic
    {
      get { return true; }
    }

    public string Identifier
    {
      get { return string.Empty; }
    }

    public ReadOnlyCollection<IBusinessObjectProperty> Properties
    {
      get { throw new NotSupportedException(); }
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
          return new NullPropertyPathResult();

        if (!currentProperty.IsAccessible (currentClass, currentObject))
          return new NotAccessiblePropertyPathResult (currentClass.BusinessObjectProvider);

        if (propertyIdentifierAndRemainder.Length == 1)
          return new BusinessObjectPropertyPathResult (currentObject, currentProperty);

        if (! (currentProperty is IBusinessObjectReferenceProperty))
          return new NullPropertyPathResult();

        remainingPropertyPathIdentifier = propertyIdentifierAndRemainder[1];
        currentObject = (IBusinessObject) currentObject.GetProperty (currentProperty);
      } while (currentObject != null);

      return new NullPropertyPathResult();
    }

    public IBusinessObjectPropertyPathResult GetResult_Enumerator (IBusinessObject root)
    {
      ArgumentUtility.CheckNotNull ("root", root);

      var currentObject = root;
      var propertyEnumerator = new PropertyPathPropertyEnumerator (_propertyPathIdentifier);

      if (!propertyEnumerator.MoveNext (currentObject.BusinessObjectClass))
        throw new InvalidOperationException();

      while (true)
      {
        var currentProperty = propertyEnumerator.Current;

        if (currentProperty == null)
          return new NullPropertyPathResult();

        if (!currentProperty.IsAccessible (currentObject.BusinessObjectClass, currentObject))
          return new NotAccessiblePropertyPathResult (currentObject.BusinessObjectClass.BusinessObjectProvider);

        if (!propertyEnumerator.MoveNext (currentObject.BusinessObjectClass))
          return new BusinessObjectPropertyPathResult (currentObject, currentProperty);

        currentObject = (IBusinessObject) currentObject.GetProperty (currentProperty);
        if (currentObject == null)
          return new NullPropertyPathResult();
      }
    }
  }

  public class PropertyPathPropertyEnumerator
  {
    private string _remainingPropertyPathIdentifier;
    private IBusinessObjectProperty _currentProperty;

    public PropertyPathPropertyEnumerator (string propertyPathIdentifier)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("propertyPathIdentifier", propertyPathIdentifier);

      _remainingPropertyPathIdentifier = propertyPathIdentifier;
    }

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

    private void HandlePropertyNotFound (IBusinessObjectClass businessObjectClass, string propertyIdentifier)
    {
    }

    private void HandlePropertyNotLastPropertyAndNotReferenceProperty (IBusinessObjectClass businessObjectClass, IBusinessObjectProperty property)
    {
    }
  }


  public interface IBusinessObjectPropertyPathResult
  {
    bool IsEvaluated { get; }
    object GetValue ();
    string GetString (string format);
    IBusinessObjectProperty ResultProperty { get; }
    IBusinessObject ResultObject { get; }
  }

  public class NullPropertyPathResult : IBusinessObjectPropertyPathResult
  {
    public bool IsEvaluated
    {
      get { return false; }
    }

    public object GetValue ()
    {
      return null;
    }

    public string GetString (string format)
    {
      return string.Empty;
    }

    public IBusinessObjectProperty ResultProperty
    {
      get { throw new NotSupportedException(); }
    }

    public IBusinessObject ResultObject
    {
      get { throw new NotSupportedException(); }
    }
  }

  public class NotAccessiblePropertyPathResult : IBusinessObjectPropertyPathResult
  {
    private readonly IBusinessObjectProvider _businessObjectProvider;

    public NotAccessiblePropertyPathResult (IBusinessObjectProvider businessObjectProvider)
    {
      _businessObjectProvider = businessObjectProvider;
    }

    public bool IsEvaluated
    {
      get { return false; }
    }

    public object GetValue ()
    {
      return null;
    }

    public string GetString (string format)
    {
      return _businessObjectProvider.GetNotAccessiblePropertyStringPlaceHolder();
    }

    public IBusinessObjectProperty ResultProperty
    {
      get { throw new NotSupportedException(); }
    }

    public IBusinessObject ResultObject
    {
      get { throw new NotSupportedException(); }
    }
  }

  public class BusinessObjectPropertyPathResult : IBusinessObjectPropertyPathResult
  {
    private readonly IBusinessObject _resultObject;
    private readonly IBusinessObjectProperty _resultProperty;

    public BusinessObjectPropertyPathResult (IBusinessObject resultObject, IBusinessObjectProperty resultProperty)
    {
      ArgumentUtility.CheckNotNull ("resultObject", resultObject);
      ArgumentUtility.CheckNotNull ("resultProperty", resultProperty);

      _resultObject = resultObject;
      _resultProperty = resultProperty;
    }

    public bool IsEvaluated
    {
      get { return true; }
    }

    public object GetValue ()
    {
      return _resultObject.GetProperty (_resultProperty);
    }

    public string GetString (string format)
    {
      return _resultObject.GetPropertyString (_resultProperty, format);
    }

    public IBusinessObjectProperty ResultProperty
    {
      get { return _resultProperty; }
    }

    public IBusinessObject ResultObject
    {
      get { return _resultObject; }
    }
  }
}
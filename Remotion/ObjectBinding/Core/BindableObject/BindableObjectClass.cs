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
using System.Collections.Generic;
using Remotion.Mixins;
using Remotion.ObjectBinding.BindableObject.Properties;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.BindableObject
{
  //TODO: doc
  public class BindableObjectClass : IBusinessObjectClass
  {
    private readonly Type _targetType;
    private readonly Type _concreteType;
    private readonly BindableObjectProvider _businessObjectProvider;
    private readonly PropertyCollection _properties = new PropertyCollection();
    private readonly BusinessObjectProviderAttribute _businessObjectProviderAttribute;

    public BindableObjectClass (Type concreteType, BindableObjectProvider businessObjectProvider, IEnumerable<PropertyBase> properties)
    {
      ArgumentUtility.CheckNotNull ("concreteType", concreteType);
      Assertion.IsFalse (concreteType.IsValueType, "mixed types cannot be value types");
      ArgumentUtility.CheckNotNull ("businessObjectProvider", businessObjectProvider);
      ArgumentUtility.CheckNotNullOrItemsNull ("properties", properties);
      
      _targetType = MixinTypeUtility.GetUnderlyingTargetType (concreteType);
      _concreteType = concreteType;
      _businessObjectProvider = businessObjectProvider;
      
      var attribute = AttributeUtility.GetCustomAttribute<BusinessObjectProviderAttribute> (concreteType, true);
      _businessObjectProviderAttribute = attribute;

      SetPropertyDefinitions (properties);
    }

    /// <summary> Returns the <see cref="IBusinessObjectProperty"/> for the passed <paramref name="propertyIdentifier"/>. </summary>
    /// <param name="propertyIdentifier"> 
    ///   A <see cref="String"/> uniquely identifying an <see cref="IBusinessObjectProperty"/> in this
    ///   business object class.
    /// </param>
    /// <returns> Returns the <see cref="IBusinessObjectProperty"/>. </returns>
    /// <exception cref="KeyNotFoundException">Thrown if no property with the <paramref name="propertyIdentifier"/> was found.</exception>
    public IBusinessObjectProperty GetPropertyDefinition (string propertyIdentifier)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("propertyIdentifier", propertyIdentifier);

      if (!HasPropertyDefinition (propertyIdentifier))
      {
        throw new KeyNotFoundException (
            string.Format ("The property '{0}' was not found on business object class '{1}'.", propertyIdentifier, Identifier));
      }

      return _properties[propertyIdentifier];
    }

    //TODO: doc
    public bool HasPropertyDefinition (string propertyIdentifier)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("propertyIdentifier", propertyIdentifier);

      return _properties.Contains (propertyIdentifier);
    }

    /// <summary> 
    ///   Returns the <see cref="IBusinessObjectProperty"/> instances defined for this business object class.
    /// </summary>
    /// <returns> An array of <see cref="IBusinessObjectProperty"/> instances.</returns>
    public IBusinessObjectProperty[] GetPropertyDefinitions ()
    {
      return _properties.ToArray();
    }

    /// <summary> Gets the <see cref="IBusinessObjectProvider"/> for this business object class. </summary>
    /// <value> An instance of the <see cref="IBusinessObjectProvider"/> type.</value>
    public IBusinessObjectProvider BusinessObjectProvider
    {
      get { return _businessObjectProvider; }
    }

    /// <summary>
    ///   Gets a flag that specifies whether a referenced object of this business object class needs to be 
    ///   written back to its container if some of its values have changed.
    /// </summary>
    /// <value> <see langword="true"/> if the <see cref="IBusinessObject"/> must be reassigned to its container. </value>
    /// <example>
    ///   The following pseudo code shows how this value affects the binding behaviour.
    ///   <code><![CDATA[
    ///   Address address = person.Address;
    ///   address.City = "Vienna";
    ///   // the RequiresWriteBack property of the 'Address' business object class specifies 
    ///   // whether the following statement is required:
    ///   person.Address = address;
    ///   ]]></code>
    /// </example>
    public bool RequiresWriteBack
    {
      get { return false; }
    }

    /// <summary> Gets the identifier (i.e. the type name) for this business object class. </summary>
    /// <value> 
    ///   A string that uniquely identifies the business object class within the business object model. 
    /// </value>
    public string Identifier
    {
      get { return Utilities.TypeUtility.GetPartialAssemblyQualifiedName (_targetType); }
    }

    public Type TargetType
    {
      get { return _targetType; }
    }

    public Type ConcreteType
    {
      get { return _concreteType; }
    }

    public BusinessObjectProviderAttribute BusinessObjectProviderAttribute
    {
      get { return _businessObjectProviderAttribute; }
    }

    private void SetPropertyDefinitions (IEnumerable<PropertyBase> properties)
    {
      foreach (PropertyBase property in properties)
      {
        property.SetReflectedClass (this);
        _properties.Add (property);
      }
    }
  }
}

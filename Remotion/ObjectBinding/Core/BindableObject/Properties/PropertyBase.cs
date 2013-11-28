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
using System.Collections;
using Remotion.FunctionalProgramming;
using Remotion.Reflection;
using Remotion.Security;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.BindableObject.Properties
{
  public abstract class PropertyBase : IBusinessObjectProperty
  {
    public struct Parameters
    {
      public readonly BindableObjectProvider BusinessObjectProvider;
      public readonly IPropertyInformation PropertyInfo;
      public readonly Type UnderlyingType;
      public readonly Type ConcreteType;
      public readonly IListInfo ListInfo;
      public readonly bool IsRequired;
      public readonly bool IsReadOnly;
      public readonly IDefaultValueStrategy DefaultValueStrategy;

      public Parameters (
          BindableObjectProvider businessObjectProvider,
          IPropertyInformation propertyInfo,
          Type underlyingType,
          Type concreteType,
          IListInfo listInfo,
          bool isRequired,
          bool isReadOnly,
          IDefaultValueStrategy defaultValueStrategy)
      {
        ArgumentUtility.CheckNotNull ("businessObjectProvider", businessObjectProvider);
        ArgumentUtility.CheckNotNull ("propertyInfo", propertyInfo);
        ArgumentUtility.CheckNotNull ("underlyingType", underlyingType);
        ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom ("concreteType", concreteType, underlyingType);

        BusinessObjectProvider = businessObjectProvider;
        PropertyInfo = propertyInfo;
        UnderlyingType = underlyingType;
        ConcreteType = concreteType;
        ListInfo = listInfo;
        IsRequired = isRequired;
        IsReadOnly = isReadOnly;
        DefaultValueStrategy = defaultValueStrategy;
      }
    }

    private readonly BindableObjectProvider _businessObjectProvider;
    private readonly IPropertyInformation _propertyInfo;
    private readonly IListInfo _listInfo;
    private readonly bool _isRequired;
    private readonly Type _underlyingType;
    private readonly bool _isReadOnly;
    private readonly bool _isNullable;
    private BindableObjectClass _reflectedClass;
    private readonly IDefaultValueStrategy _defaultValueStrategy;
    private readonly Func<object, object> _valueGetter;
    private readonly Action<object, object> _valueSetter;

    protected PropertyBase (Parameters parameters)
    {
      if (parameters.PropertyInfo.GetIndexParameters().Length > 0)
        throw new InvalidOperationException ("Indexed properties are not supported.");

      _businessObjectProvider = parameters.BusinessObjectProvider;
      _propertyInfo = parameters.PropertyInfo;
      _underlyingType = parameters.UnderlyingType;
      _listInfo = parameters.ListInfo;
      _isRequired = parameters.IsRequired;
      _isReadOnly = parameters.IsReadOnly;
      _defaultValueStrategy = parameters.DefaultValueStrategy;
      _isNullable = GetNullability();
      _valueGetter = Maybe.ForValue (_propertyInfo.GetGetMethod (true)).Select (mi => mi.GetFastInvoker<Func<object, object>>()).ValueOrDefault();
      _valueSetter = Maybe.ForValue (_propertyInfo.GetSetMethod (true)).Select (mi => mi.GetFastInvoker<Action<object, object>>()).ValueOrDefault();
    }

    /// <summary> Gets a flag indicating whether this property contains multiple values. </summary>
    /// <value> <see langword="true"/> if this property contains multiple values. </value>
    /// <remarks> Multiple values are provided via any type implementing <see cref="IList"/>. </remarks>
    public bool IsList
    {
      get { return _listInfo != null; }
    }

    /// <summary>Gets the <see cref="IListInfo"/> for this <see cref="IBusinessObjectProperty"/>.</summary>
    /// <value>An onject implementing <see cref="IListInfo"/>.</value>
    /// <exception cref="InvalidOperationException">Thrown if the property is not a list property.</exception>
    public IListInfo ListInfo
    {
      get
      {
        if (_listInfo == null)
          throw new InvalidOperationException (string.Format ("Cannot access ListInfo for non-list properties.\r\nProperty: {0}", Identifier));
        return _listInfo;
      }
    }

    /// <summary> Gets the type of the property. </summary>
    /// <remarks> 
    ///   <para>
    ///     This is the type of elements returned by the <see cref="IBusinessObject.GetProperty(IBusinessObjectProperty)"/> method
    ///     and set via the <see cref="IBusinessObject.SetProperty(IBusinessObjectProperty,object)"/> method.
    ///   </para><para>
    ///     If <see cref="IsList"/> is <see langword="true"/>, the property type must implement the <see cref="IList"/> 
    ///     interface, and the items contained in this list must have a type of <see cref="ListInfo"/>.<see cref="IListInfo.ItemType"/>.
    ///   </para>
    /// </remarks>
    public Type PropertyType
    {
      get { return _propertyInfo.PropertyType; }
    }

    /// <summary> Gets an identifier that uniquely defines this property within its class. </summary>
    /// <value> A <see cref="String"/> by which this property can be found within its <see cref="IBusinessObjectClass"/>. </value>
    public string Identifier
    {
      get { return ShortenName (_propertyInfo.Name); }
    }

    // Truncates the name to the part to the right of the last '.', if any.
    private string ShortenName (string name)
    {
      return name.Substring (name.LastIndexOf ('.') + 1);
    }

    /// <summary> Gets the property name as presented to the user. </summary>
    /// <value> The human readable identifier of this property. </value>
    /// <remarks> The value of this property may depend on the current culture. </remarks>
    public string DisplayName
    {
      get
      {
        IBindableObjectGlobalizationService globalizationService = BusinessObjectProvider.GetService<IBindableObjectGlobalizationService>();
        if (globalizationService == null)
          return Identifier;

        if (_reflectedClass == null)
        {
          throw new InvalidOperationException (
              string.Format ("The reflected class for the property '{0}.{1}' is not set.", _propertyInfo.DeclaringType.Name, _propertyInfo.Name));
        }

        return globalizationService.GetPropertyDisplayName (TypeAdapter.Create(_reflectedClass.TargetType), _propertyInfo);
      }
    }

    /// <summary> Gets a flag indicating whether this property is required. </summary>
    /// <value> <see langword="true"/> if this property is required. </value>
    /// <remarks> Setting required properties to <see langword="null"/> may result in an error. </remarks>
    public bool IsRequired
    {
      get { return _isRequired; }
    }

    /// <summary> Indicates whether this property can be accessed by the user. </summary>
    /// <param name="objectClass"> The <see cref="IBusinessObjectClass"/> of the <paramref name="obj"/>. </param>
    /// <param name="obj"> The object to evaluate this property for, or <see langword="null"/>. </param>
    /// <returns> <see langword="true"/> if the user can access this property. </returns>
    /// <remarks> The result may depend on the class, the user's authorization and/or the instance value. </remarks>
    public bool IsAccessible (IBusinessObjectClass objectClass, IBusinessObject obj)
    {
      ISecurableObject securableObject = obj as ISecurableObject;
      if (securableObject == null)
        return true;

      IObjectSecurityAdapter objectSecurityAdapter = AdapterRegistry.Instance.GetAdapter<IObjectSecurityAdapter>();
      if (objectSecurityAdapter == null)
        return true;

      return objectSecurityAdapter.HasAccessOnGetAccessor (securableObject, _propertyInfo);
    }

    public object GetValue (IBusinessObject obj)
    {
      ArgumentUtility.CheckNotNull ("obj", obj);

      if (_valueGetter == null)
        throw new InvalidOperationException ("Property has no getter.");

      return _valueGetter (obj);
    }

    public void SetValue (IBusinessObject obj, object value)
    {
      ArgumentUtility.CheckNotNull ("obj", obj);

      if (_valueSetter == null)
        throw new InvalidOperationException ("Property has no setter.");

      _valueSetter (obj, value);
    }

    public bool IsDefaultValue (IBusinessObject obj)
    {
      ArgumentUtility.CheckNotNull ("obj", obj);

      return _defaultValueStrategy.IsDefaultValue (obj, this);
    }

    /// <summary> Indicates whether this property can be modified by the user. </summary>
    /// <param name="obj"> The object to evaluate this property for, or <see langword="null"/>. </param>
    /// <returns> <see langword="true"/> if the user can set this property. </returns>
    /// <remarks> The result may depend on the user's authorization and/or the object. </remarks>
    public bool IsReadOnly (IBusinessObject obj)
    {
      if (_isReadOnly)
        return true;

      ISecurableObject securableObject = obj as ISecurableObject;
      if (securableObject == null)
        return false;

      IObjectSecurityAdapter objectSecurityAdapter = AdapterRegistry.Instance.GetAdapter<IObjectSecurityAdapter>();
      if (objectSecurityAdapter == null)
        return false;

      return !objectSecurityAdapter.HasAccessOnSetAccessor (securableObject, _propertyInfo);
    }

    /// <summary> Gets the <see cref="BindableObjectProvider"/> for this property. </summary>
    /// <value> An instance of the <see cref="BindableObjectProvider"/> type. </value>
    public BindableObjectProvider BusinessObjectProvider
    {
      get { return _businessObjectProvider; }
    }

    /// <summary> Gets the <see cref="IBusinessObjectProvider"/> for this property. </summary>
    /// <value> An instance of the <see cref="IBusinessObjectProvider"/> type. </value>
    IBusinessObjectProvider IBusinessObjectProperty.BusinessObjectProvider
    {
      get { return BusinessObjectProvider; }
    }

    /// <summary>Gets the <see cref="IBusinessObjectClass"/> that was used to retrieve this property.</summary>
    /// <value>An instance of the <see cref="IBusinessObjectClass"/> type.</value>
    IBusinessObjectClass IBusinessObjectProperty.ReflectedClass
    {
      get { return ReflectedClass; }
    }

    /// <summary>Gets the <see cref="IBusinessObjectClass"/> that was used to retrieve this property.</summary>
    /// <value>An instance of the <see cref="BindableObjectClass"/> type.</value>
    public BindableObjectClass ReflectedClass
    {
      get
      {
        if (_reflectedClass == null)
        {
          throw new InvalidOperationException (
              string.Format (
                  "Accessing the ReflectedClass of a property is invalid until the property has been associated with a class.\r\nProperty '{0}'",
                  Identifier));
        }

        return _reflectedClass;
      }
    }

    public IPropertyInformation PropertyInfo
    {
      get { return _propertyInfo; }
    }

    public virtual object ConvertFromNativePropertyType (object nativeValue)
    {
      return nativeValue;
    }

    public virtual object ConvertToNativePropertyType (object publicValue)
    {
      return publicValue;
    }

    public void SetReflectedClass (BindableObjectClass reflectedClass)
    {
      ArgumentUtility.CheckNotNull ("reflectedClass", reflectedClass);
      if (BusinessObjectProvider != reflectedClass.BusinessObjectProvider)
      {
        throw new ArgumentException (
            string.Format (
                "The BusinessObjectProvider of property '{0}' does not match the BusinessObjectProvider of class '{1}'.",
                Identifier,
                reflectedClass.Identifier),
            "reflectedClass");
      }

      if (_reflectedClass != null)
      {
        throw new InvalidOperationException (
            string.Format (
                "The ReflectedClass of a property cannot be changed after it was assigned.\r\nClass '{0}'\r\nProperty '{1}'",
                _reflectedClass.Identifier,
                Identifier));
      }

      _reflectedClass = reflectedClass;
    }

    protected Type UnderlyingType
    {
      get { return _underlyingType; }
    }

    protected bool IsNullable
    {
      get { return _isNullable; }
    }

    private bool GetNullability ()
    {
      return Nullable.GetUnderlyingType (IsList ? ListInfo.ItemType : PropertyType) != null;
    }
  }
}
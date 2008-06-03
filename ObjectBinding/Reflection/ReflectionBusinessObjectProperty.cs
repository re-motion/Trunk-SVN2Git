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
using System.Collections;
using System.Diagnostics;
using System.Reflection;
using System.Xml.Serialization;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.Reflection.Legacy
{
  public class ReflectionBusinessObjectProperty: IBusinessObjectProperty
  {
    public static ReflectionBusinessObjectProperty Create (PropertyInfo propertyInfo)
    {
      ArgumentUtility.CheckNotNull ("propertyInfo", propertyInfo);

      bool isList = propertyInfo.PropertyType.IsArray;
      Type itemType = isList ? propertyInfo.PropertyType.GetElementType() : propertyInfo.PropertyType;
      bool isNullableType = false;
      if (IsNullableValueType (itemType))
      {
        isNullableType = true;
        itemType = GetNativeType (itemType);
      }

      XmlAttributeAttribute[] xmlAttributes = (XmlAttributeAttribute[]) propertyInfo.GetCustomAttributes (typeof (XmlAttributeAttribute), true);
      if (xmlAttributes.Length == 1)
      {
        XmlAttributeAttribute xmlAttribute = xmlAttributes[0];
        // create ReflectionBusinessObjectDateProperty for [XmlAttribute (DataType="date")] DateTime
        if (propertyInfo.PropertyType == typeof (DateTime) && xmlAttribute.DataType == "date")
          return new ReflectionBusinessObjectDateProperty (propertyInfo, itemType, isList, isNullableType);
      }

      if (itemType == typeof (string))
        return new ReflectionBusinessObjectStringProperty (propertyInfo, itemType, isList);
      else if (itemType == typeof (int))
        return new ReflectionBusinessObjectNumericProperty (propertyInfo, itemType, isList, isNullableType);
      else if (itemType == typeof (double))
        return new ReflectionBusinessObjectNumericProperty (propertyInfo, itemType, isList, isNullableType);
      else if (itemType == typeof (bool))
        return new ReflectionBusinessObjectBooleanProperty (propertyInfo, itemType, isList, isNullableType);
      else if (itemType == typeof (DateTime))
        return new ReflectionBusinessObjectDateTimeProperty (propertyInfo, itemType, isList, isNullableType);
      else if (itemType.IsEnum)
        return new ReflectionBusinessObjectEnumerationProperty (propertyInfo, itemType, isList);
      else if (typeof (IBusinessObjectWithIdentity).IsAssignableFrom (itemType))
        return new ReflectionBusinessObjectReferenceProperty (propertyInfo, itemType, isList);
      else
        return new ReflectionBusinessObjectProperty (propertyInfo, itemType, isList);
    }

    //TODO: Code duplication with Remotion.Data.DomainObjects.Mapping.TypeInfo
    private static Type GetNativeType (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);

      if (IsNullableValueType (type))
        return type.GetGenericArguments ()[0];

      return type;
    }

    //TODO: Code duplication with Remotion.Data.DomainObjects.Mapping.TypeInfo
    private static bool IsNullableValueType (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);

      return type.IsValueType && type.IsGenericType && !type.IsGenericTypeDefinition && type.GetGenericTypeDefinition () == typeof (Nullable<>);
    }

    private PropertyInfo _propertyInfo;
    private IListInfo _listInfo;


    protected ReflectionBusinessObjectProperty (PropertyInfo propertyInfo, Type itemType, bool isList)
    {
      _propertyInfo = propertyInfo;
      _listInfo = isList ? new ListInfo (propertyInfo.PropertyType, itemType) : null;
    }

    public bool IsList
    {
      get { return _listInfo != null; }
    }

    public IListInfo ListInfo
    {
      get
      {
        if (_listInfo == null)
          throw new InvalidOperationException ("Cannot retrieve ListInfo for non-list properties.");
        return _listInfo;
      }
    }

    public virtual Type PropertyType
    {
      get { return _propertyInfo.PropertyType; }
    }

    public string Identifier
    {
      get { return _propertyInfo.Name; }
    }

    public string DisplayName
    {
      get
      {
        DisplayNameAttribute displayNameAttribute = DisplayNameAttribute.GetDisplayNameAttribute (_propertyInfo);
        if (displayNameAttribute != null)
          return displayNameAttribute.DisplayName;
        else
          return _propertyInfo.Name;
      }
    }

    public virtual bool IsRequired
    {
      get { return false; }
    }

    public bool IsAccessible (IBusinessObjectClass objClass, IBusinessObject obj)
    {
      return true;
    }

    public bool IsReadOnly (IBusinessObject obj)
    {
      return false;
    }

    public PropertyInfo PropertyInfo
    {
      get { return _propertyInfo; }
    }

    protected internal virtual object FromInternalType (object internalValue)
    {
      return internalValue;
    }

    protected internal virtual object ToInternalType (object publicValue)
    {
      return publicValue;
    }

    public IBusinessObjectProvider BusinessObjectProvider
    {
      get { return ReflectionBusinessObjectProvider.Instance; }
    }
  }

  public class ReflectionBusinessObjectStringProperty: ReflectionBusinessObjectProperty, IBusinessObjectStringProperty
  {
    public ReflectionBusinessObjectStringProperty (PropertyInfo propertyInfo, Type itemType, bool isList)
        : base (propertyInfo, itemType, isList)
    {
    }

    public int? MaxLength
    {
      get { return null; }
    }
  }

  public class ReflectionBusinessObjectNullableProperty: ReflectionBusinessObjectProperty
  {
    private bool _isNullableType;

    public ReflectionBusinessObjectNullableProperty (PropertyInfo propertyInfo, Type itemType, bool isList, bool isNullableType)
        : base (propertyInfo, itemType, isList)
    {
      _isNullableType = isNullableType;
    }

    protected bool IsNullableType
    {
      get { return _isNullableType; }
    }

    public override bool IsRequired
    {
      get { return ! _isNullableType; }
    }
  }

  public class ReflectionBusinessObjectNumericProperty: ReflectionBusinessObjectNullableProperty, IBusinessObjectNumericProperty
  {
    private readonly Type _type;

    public ReflectionBusinessObjectNumericProperty (PropertyInfo propertyInfo, Type itemType, bool isList, bool isNullable)
        : base (propertyInfo, itemType, isList, isNullable)
    {
      _type = itemType;
    }

    public bool AllowNegative
    {
      get { return true; }
    }

    public Type Type
    {
      get { return _type; }
    }
  }

  public class ReflectionBusinessObjectBooleanProperty
      : ReflectionBusinessObjectNullableProperty, IBusinessObjectBooleanProperty, IBusinessObjectEnumerationProperty
  {
    private BooleanToEnumPropertyConverter _booleanToEnumConverter = null;

    public ReflectionBusinessObjectBooleanProperty (PropertyInfo propertyInfo, Type itemType, bool isList, bool isNullable)
        : base (propertyInfo, itemType, isList, isNullable)
    {
      _booleanToEnumConverter = new BooleanToEnumPropertyConverter (this);
    }

    public string GetDisplayName (bool value)
    {
      return value.ToString();
    }

    public bool? GetDefaultValue (IBusinessObjectClass objectClass)
    {
      return null;
    }

    //  public bool AllowNegative
    //  {
    //    get { return false; }
    //  }

    public IEnumerationValueInfo[] GetEnabledValues(IBusinessObject businessObject)
    {
      return _booleanToEnumConverter.GetValues();
    }

    public IEnumerationValueInfo[] GetAllValues(IBusinessObject businessObject)
    {
      return _booleanToEnumConverter.GetValues();
    }

    public IEnumerationValueInfo GetValueInfoByValue (object value, IBusinessObject businessObject)
    {
      return _booleanToEnumConverter.GetValueInfoByValue (value);
    }

    public IEnumerationValueInfo GetValueInfoByIdentifier (string identifier, IBusinessObject businessObject)
    {
      return _booleanToEnumConverter.GetValueInfoByIdentifier (identifier);
    }
  }

  public class ReflectionBusinessObjectDateProperty: ReflectionBusinessObjectNullableProperty, IBusinessObjectDateTimeProperty
  {
    public ReflectionBusinessObjectDateProperty (PropertyInfo propertyInfo, Type itemType, bool isList, bool isNullable)
        : base (propertyInfo, itemType, isList, isNullable)
    {
    }

    public DateTimeType Type
    {
      get { return DateTimeType.Date; }
    }

    protected internal override object FromInternalType (object internalValue)
    {
      if (!IsList && internalValue != null)
        return ((DateTime) internalValue).Date;
      return internalValue;
    }
  }

  public class ReflectionBusinessObjectDateTimeProperty: ReflectionBusinessObjectNullableProperty, IBusinessObjectDateTimeProperty
  {
    public ReflectionBusinessObjectDateTimeProperty (PropertyInfo propertyInfo, Type itemType, bool isList, bool isNullable)
        : base (propertyInfo, itemType, isList, isNullable)
    {
    }

    public DateTimeType Type
    {
      get { return DateTimeType.DateTime; }
    }
  }

  public class ReflectionBusinessObjectEnumerationProperty: ReflectionBusinessObjectProperty, IBusinessObjectEnumerationProperty
  {
    private const string c_disabledPrefix = "Disabled_";

    public ReflectionBusinessObjectEnumerationProperty (PropertyInfo propertyInfo, Type itemType, bool isList)
        : base (propertyInfo, itemType, isList)
    {
    }

    public IEnumerationValueInfo[] GetEnabledValues (IBusinessObject businessObject)
    {
      return GetValues (false);
    }

    public IEnumerationValueInfo[] GetAllValues(IBusinessObject businessObject)
    {
      return GetValues (true);
    }

    private IEnumerationValueInfo[] GetValues (bool includeDisabledValues)
    {
      Debug.Assert (PropertyInfo.PropertyType.IsEnum, "type.IsEnum");
      FieldInfo[] fields = PropertyInfo.PropertyType.GetFields (BindingFlags.Static | BindingFlags.Public);
      ArrayList valueInfos = new ArrayList (fields.Length);

      foreach (FieldInfo field in fields)
      {
        bool isEnabled = ! field.Name.StartsWith (c_disabledPrefix);

        if (! includeDisabledValues && isEnabled
            || includeDisabledValues)
        {
          valueInfos.Add (
              new EnumerationValueInfo (field.GetValue (null), field.Name, field.Name, isEnabled));
        }
      }

      return (IEnumerationValueInfo[]) valueInfos.ToArray (typeof (IEnumerationValueInfo));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value">
    ///   An enum value that belongs to the enum identified by <see cref="ReflectionBusinessObjectProperty.PropertyType"/>.
    /// </param>
    /// <returns></returns>
    public IEnumerationValueInfo GetValueInfoByValue (object value, IBusinessObject businessObject)
    {
      if (value == null)
      {
        return null;
      }
      else
      {
        string valueString = value.ToString();

        //  Test if enum value is correct type, throws an exception if not
        Enum.Parse (PropertyType, valueString, false);

        bool isEnabled = ! valueString.StartsWith (c_disabledPrefix);

        return new EnumerationValueInfo (value, value.ToString(), value.ToString(), isEnabled);
      }
    }

    public IEnumerationValueInfo GetValueInfoByIdentifier (string identifier, IBusinessObject businessObject)
    {
      object value = Enum.Parse (PropertyType, identifier, false);

      string valueString = value.ToString();

      bool isEnabled = ! valueString.StartsWith (c_disabledPrefix);

      return new EnumerationValueInfo (value, value.ToString(), value.ToString(), isEnabled);
    }

    public override bool IsRequired
    {
      get { return true; }
    }
  }

  public class ReflectionBusinessObjectInstanceEnumerationProperty: ReflectionBusinessObjectProperty, IBusinessObjectEnumerationProperty
  {
    private IEnumerationValueInfo[] _valueInfos;
    private bool _isRequired;

    public ReflectionBusinessObjectInstanceEnumerationProperty (
        PropertyInfo propertyInfo, Type itemType, bool isList, bool isRequired, params object[] values)
        : base (propertyInfo, itemType, isList)
    {
      _isRequired = isRequired;

      if (values == null)
      {
        _valueInfos = new IEnumerationValueInfo[0];
      }
      else
      {
        ArgumentUtility.CheckNotNullOrItemsNull ("values", values);
        _valueInfos = new IEnumerationValueInfo[values.Length];
        for (int i = 0; i < values.Length; i++)
          _valueInfos[i] = new EnumerationValueInfo (values[i], values[i].ToString(), values[i].ToString(), true);
      }
    }

    public IEnumerationValueInfo[] GetEnabledValues (IBusinessObject businessObject)
    {
      return _valueInfos;
    }

    public IEnumerationValueInfo[] GetAllValues (IBusinessObject businessObject)
    {
      return _valueInfos;
    }

    public IEnumerationValueInfo GetValueInfoByValue (object value, IBusinessObject businessObject)
    {
      foreach (IEnumerationValueInfo valueInfo in _valueInfos)
      {
        if (valueInfo.Value == value)
          return valueInfo;
      }
      throw new ArgumentException (
          string.Format ("The value '{0}' is not part of the possible values for this Instance Enumeration.", value), "value");
    }

    public IEnumerationValueInfo GetValueInfoByIdentifier (string identifier, IBusinessObject businessObject)
    {
      foreach (IEnumerationValueInfo valueInfo in _valueInfos)
      {
        if (valueInfo.Identifier == identifier)
          return valueInfo;
      }
      throw new ArgumentException (
          string.Format ("The identifier '{0}' does not identify a possible value for this Instance Enumeration.", identifier), "identifier");
    }

    public override bool IsRequired
    {
      get { return _isRequired; }
    }
  }

  public class ReflectionBusinessObjectReferenceProperty: ReflectionBusinessObjectProperty, IBusinessObjectReferenceProperty
  {
    public ReflectionBusinessObjectReferenceProperty (PropertyInfo propertyInfo, Type itemType, bool isList)
        : base (propertyInfo, itemType, isList)
    {
    }

    public IBusinessObjectClass ReferenceClass
    {
      get { return new ReflectionBusinessObjectClass (PropertyType); }
    }

    public IBusinessObject[] SearchAvailableObjects (IBusinessObject referencingObject, bool requiresIdentity, string searchStatement)
    {
      if (searchStatement == "*")
        return ReflectionBusinessObjectStorage.GetObjects (referencingObject.GetType());
      else
        return new IBusinessObjectWithIdentity[0];
    }

    public bool SupportsSearchAvailableObjects (bool supportsIdentity)
    {
      return true;
    }

    public bool CreateIfNull
    {
      get { return false; }
    }

    IBusinessObject IBusinessObjectReferenceProperty.Create (IBusinessObject referencingObject)
    {
      throw new NotSupportedException();
    }
  }

  [AttributeUsage (AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
  public class DisplayNameAttribute: Attribute
  {
    public static DisplayNameAttribute GetDisplayNameAttribute (PropertyInfo propertyInfo)
    {
      object[] attributes = propertyInfo.GetCustomAttributes (typeof (DisplayNameAttribute), false);
      if (attributes.Length == 0)
        return null;
      else
        return (DisplayNameAttribute) attributes[0];
    }

    private string _displayName;

    public DisplayNameAttribute (string displayName)
    {
      _displayName = displayName;
    }

    public string DisplayName
    {
      get { return _displayName; }
    }
  }
}

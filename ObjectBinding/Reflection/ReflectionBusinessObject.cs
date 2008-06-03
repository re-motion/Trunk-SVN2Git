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
using System.ComponentModel;
using System.Reflection;
using System.Xml.Serialization;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.Reflection.Legacy
{

/// <summary>
///   This class provides BusinessObject interfaces for simple .NET objects.
/// </summary>
[Serializable]
public abstract class ReflectionBusinessObject: BusinessObject, IBusinessObjectWithIdentity
{
  internal Guid _id;

  public ReflectionBusinessObject ()
  {
    _id = Guid.NewGuid(); // this id is replaced immediately if the object is loaded from xml
  }

  [XmlIgnore]
  [EditorBrowsable (EditorBrowsableState.Never)]
  public Guid ID
  {
    get { return _id; }
  }

//  public override IBusinessObjectProperty GetBusinessObjectProperty (string propertyIdentifier)
//  {
//    return BusinessObjectClass.GetPropertyDefinition (propertyIdentifier);
//  }

  [XmlIgnore]
  [EditorBrowsable (EditorBrowsableState.Never)]
  public override IBusinessObjectClass BusinessObjectClass
  {
    get { return new ReflectionBusinessObjectClass (this.GetType()); }
  }

  public override object GetProperty (IBusinessObjectProperty property)
  {
    ReflectionBusinessObjectProperty reflectionProperty = ArgumentUtility.CheckNotNullAndType<ReflectionBusinessObjectProperty> ("property", property);
    PropertyInfo propertyInfo = reflectionProperty.PropertyInfo;

    object internalValue = propertyInfo.GetValue (this, new object[0]);
    return reflectionProperty.FromInternalType (internalValue);
  }

  public override void SetProperty (IBusinessObjectProperty property, object value)
  {
    ReflectionBusinessObjectProperty reflectionProperty = ArgumentUtility.CheckNotNullAndType<ReflectionBusinessObjectProperty> ("property", property);
    PropertyInfo propertyInfo = reflectionProperty.PropertyInfo;

    object internalValue = reflectionProperty.ToInternalType (value);
    propertyInfo.SetValue (this, internalValue, new object[0]);
  }

  [EditorBrowsable (EditorBrowsableState.Never)]
  public override string DisplayName 
  { 
    get { return GetType().FullName; }
  }

  [EditorBrowsable (EditorBrowsableState.Never)]
  string IBusinessObject.DisplayNameSafe
  {
    get { return DisplayName; }
  }

  [EditorBrowsable (EditorBrowsableState.Never)]
  string IBusinessObjectWithIdentity.UniqueIdentifier
  {
    get { return _id.ToString(); }
  }

  public void SaveObject()
  {
    ReflectionBusinessObjectStorage.SaveObject (this);
  }

  protected override IBusinessObjectStringFormatterService StringFormatterService
  {
    get { return new BusinessObjectStringFormatterService(); }
  }
}

}

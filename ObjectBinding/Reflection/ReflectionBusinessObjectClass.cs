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
using System.ComponentModel;
using System.Reflection;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.Reflection.Legacy
{

public class ReflectionBusinessObjectClass: IBusinessObjectClassWithIdentity
{
  Type _type;

  public ReflectionBusinessObjectClass (Type type)
  {
    ArgumentUtility.CheckNotNull ("type", type);
    if (type.IsArray)
      _type = type.GetElementType ();
    else
      _type = type;
  }

  public IBusinessObjectProperty GetPropertyDefinition (string propertyIdentifier)
  {
    PropertyInfo propertyInfo = _type.GetProperty (propertyIdentifier);
    return (propertyInfo == null) ? null : ReflectionBusinessObjectProperty.Create (propertyInfo);
  }

  public IBusinessObjectProperty[] GetPropertyDefinitions()
  {
    PropertyInfo[] propertyInfos = _type.GetProperties ();
    if (propertyInfos == null)
      return new IBusinessObjectProperty[0];

    ArrayList properties = new ArrayList();
    for (int i = 0; i < propertyInfos.Length; ++i)
    {
      PropertyInfo propertyInfo = propertyInfos[i];
      EditorBrowsableAttribute[] editorBrowsableAttributes = (EditorBrowsableAttribute[]) propertyInfo.GetCustomAttributes (typeof (EditorBrowsableAttribute), true);
      if (editorBrowsableAttributes.Length == 1)
      {
        EditorBrowsableAttribute editorBrowsableAttribute = editorBrowsableAttributes[0];
        if (editorBrowsableAttribute.State == EditorBrowsableState.Never)
          continue;
      }

      //  Prevents the display of the indexers declared in BusinessObject.
      //  Adding "EditorBrowsable (EditorBrowsableState.Never)" to BusinessObject 
      //  might not be the best solution until the final way of hiding properties is established
      if (propertyInfo.Name == "Item")
        continue;

      properties.Add (ReflectionBusinessObjectProperty.Create (propertyInfo));
    }

    return (IBusinessObjectProperty[]) properties.ToArray (typeof (IBusinessObjectProperty));
  }

  public IBusinessObjectProvider BusinessObjectProvider 
  {
    get { return ReflectionBusinessObjectProvider.Instance; }
  }

  public IBusinessObjectWithIdentity GetObject (string identifier)
  {
    Guid id = Guid.Empty;
    if (! StringUtility.IsNullOrEmpty (identifier))
      id = new Guid (identifier);

    return ReflectionBusinessObjectStorage.GetObject (_type, id);
  }

  public bool RequiresWriteBack
  { 
    get { return false; }
  }

  public string Identifier
  {
    get { return _type.FullName; }
  }

  public Type Type
  {
    get { return _type; }
    set { _type = value; }
  }
}

}

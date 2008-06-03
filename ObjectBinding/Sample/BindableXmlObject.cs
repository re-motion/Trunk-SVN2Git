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
using System.Xml.Serialization;
using Remotion.Mixins;
using Remotion.ObjectBinding.BindableObject;

namespace Remotion.ObjectBinding.Sample
{
  [Serializable]
  [BindableObjectWithIdentity]
  public class BindableXmlObject
  {
    protected static T GetObject<T> (Guid id)
      where T:BindableXmlObject
    {
      return (T) XmlReflectionBusinessObjectStorageProvider.Current.GetObject (typeof (T), id);
    }

    protected static T CreateObject<T> ()
       where T : BindableXmlObject
    {
      return XmlReflectionBusinessObjectStorageProvider.Current.CreateObject<T> ();
    }

    protected static T CreateObject<T> (Guid id)
         where T : BindableXmlObject
    {
      return XmlReflectionBusinessObjectStorageProvider.Current.CreateObject<T> (id);
    }
  
    internal Guid _id;

    protected BindableXmlObject ()
    {
    }

    [XmlIgnore]
    [ObjectBinding (Visible = false)]
    public Guid ID
    {
      get { return _id; }
    }

    [XmlIgnore]
    [OverrideMixin]
    public virtual string DisplayName
    {
      get { return GetType().FullName; }
    }

    [XmlIgnore]
    [OverrideMixin]
    [ObjectBinding (Visible = false)]
    public string UniqueIdentifier
    {
      get { return _id.ToString(); }
    }

    public void SaveObject ()
    {
      XmlReflectionBusinessObjectStorageProvider.Current.SaveObject (this);
    }
  }
}

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
using System.IO;
using System.Reflection;
using System.Xml.Serialization;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.Reflection.Legacy
{

public class ReflectionBusinessObjectStorage
{
  private static string _rootPath = null;

  private static Hashtable _identityMap = new Hashtable(); 

  public static void Reset()
  {
    _identityMap = new Hashtable();
  }

  public static string RootPath 
  {
    get { return _rootPath; }
    set { _rootPath = value; }
  }
  
  public static ReflectionBusinessObject GetObject (Type concreteType, Guid id)
  {
    if (id == Guid.Empty)
      return null;

    ReflectionBusinessObject obj = GetFromIdentityMap (id);
    if (obj != null)
      return obj;

    XmlSerializer serializer = new XmlSerializer (concreteType);

    string typeDir = Path.Combine (_rootPath, concreteType.FullName);
    string fileName = Path.Combine (typeDir, id.ToString());
    if (! File.Exists (fileName))
      return null;
    
    using (FileStream stream = new FileStream (fileName, FileMode.Open, FileAccess.Read))
    {
      obj = (ReflectionBusinessObject) serializer.Deserialize (stream);
      obj._id = id;
      AddToIdentityMap (obj);
      return obj;
    }
  }

  public static ReflectionBusinessObject[] GetObjects (Type concreteType)
  {
    ArrayList objects = new ArrayList();

    string typeDir = Path.Combine (_rootPath, concreteType.FullName);
    string[] filenames = Directory.GetFiles (typeDir);

    foreach (string filename in filenames)
    {
      Guid id = Guid.Empty;
      try 
      {
        id = new Guid (new FileInfo(filename).Name);
      }
      catch (FormatException)
      {
        continue;
      }

      ReflectionBusinessObject obj = ReflectionBusinessObjectStorage.GetObject (concreteType, id);
      if (obj != null)
        objects.Add (obj);
    }

    return (ReflectionBusinessObject[])objects.ToArray (typeof (ReflectionBusinessObject));
  }

  public static Guid GetID (ReflectionBusinessObject obj)
  {
    if (obj == null)
      return Guid.Empty;
    else
      return obj.ID;
  }

  public static void SaveObject (ReflectionBusinessObject obj)
  {
    ArgumentUtility.CheckNotNull ("obj", obj);

    Type concreteType = obj.GetType();
    XmlSerializer serializer = new XmlSerializer (concreteType);

    string typeDir = Path.Combine (_rootPath, concreteType.FullName);
    if (! Directory.Exists (typeDir))
      Directory.CreateDirectory (typeDir);

    string fileName = Path.Combine (typeDir, obj.ID.ToString());

    using (FileStream stream = new FileStream (fileName, FileMode.Create, FileAccess.Write))
    {
      serializer.Serialize (stream, obj);
    }
  }

  public static ReflectionBusinessObject CreateObject (Type concreteType)
  {
    return CreateObject (concreteType, Guid.NewGuid());
  }

  public static ReflectionBusinessObject CreateObject (Type concreteType, Guid id)
  {

    ReflectionBusinessObject obj = 
        (ReflectionBusinessObject) TypesafeActivator.CreateInstance (concreteType, BindingFlags.NonPublic | BindingFlags.Instance).With ();
    obj._id = id;
    AddToIdentityMap (obj);
    return obj;
  }

  private static void AddToIdentityMap (ReflectionBusinessObject obj)
  {
    if (_identityMap.ContainsKey (obj.ID))
      return;

    WeakReference reference = new WeakReference (obj, false);
    _identityMap.Add (obj.ID, reference);
  }

  private static ReflectionBusinessObject GetFromIdentityMap (Guid id)
  {
    WeakReference reference = (WeakReference) _identityMap[id];
    if (reference == null)
      return null;
    return (ReflectionBusinessObject) reference.Target;
  }
}

}

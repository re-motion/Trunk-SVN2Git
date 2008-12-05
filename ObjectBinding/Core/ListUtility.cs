// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
//
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
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

namespace Remotion.ObjectBinding
{
  public delegate IList CreateListMethod (int count);

  /// <summary>
  /// Provides utility methods for processing IList instances. 
  /// </summary>
  public static class ListUtility
  {
    private static CreateListMethod GetCreateListMethod (IBusinessObjectProperty property)
    {
      if (property == null)
        return null;
      if (!property.IsList)
        throw new ArgumentException (string.Format ("BusinessObjectProperty '{0}' is not a list property.", property.Identifier), "property");
      return property.ListInfo.CreateList;
    }

    /// <summary>
    ///   Adds an objectto a list. The original list may be modified.
    /// </summary>
    public static IList AddRange (
        IList list, object obj, IBusinessObjectReferenceProperty property, bool mustCreateCopy, bool createIfNull)
    {
      return AddRange (list, obj, GetCreateListMethod (property), mustCreateCopy, createIfNull);
    }

    /// <summary>
    ///   Adds an objectto a list. The original list may be modified.
    /// </summary>
    public static IList AddRange (
        IList list, object obj, CreateListMethod createListMethod, bool mustCreateCopy, bool createIfNull)
    {
      if (list == null)
      {
        if (! createIfNull)
          throw new ArgumentNullException ("list");
        
        list = CreateList (createListMethod, list, 1);
        list[0] = obj;
        return list;
      }

      if (   list.IsFixedSize
          || (mustCreateCopy && ! (list is ICloneable)))
      {
        ArrayList arrayList = new ArrayList (list);
        arrayList.Add (obj);
        IList newList = CreateList (createListMethod, list, arrayList.Count);
        Utilities.ListUtility.CopyTo (arrayList, newList);
        return newList;
      }
      else
      {
        if (mustCreateCopy)
          list = (IList) ((ICloneable)list).Clone();

        list.Add (obj);
        return list;
      }
    }

    /// <summary>
    ///   Adds a range of objects to a list. The original list may be modified.
    /// </summary>
    public static IList AddRange (
        IList list, IList objects, IBusinessObjectReferenceProperty property, bool mustCreateCopy, bool createIfNull)
    {
      return AddRange (list, objects, GetCreateListMethod (property), mustCreateCopy, createIfNull);    
    }
    
    /// <summary>
    ///   Adds a range of objects to a list. The original list may be modified.
    /// </summary>
    public static IList AddRange (
        IList list, IList objects, CreateListMethod createListMethod, bool mustCreateCopy, bool createIfNull)
    {
      if (list == null)
      {
        if (! createIfNull)
          throw new ArgumentNullException ("list");
        
        list = CreateList (createListMethod, list, objects.Count);
        Utilities.ListUtility.CopyTo (objects, list);
        return list;
      }
      
      if (   list.IsFixedSize
          || (mustCreateCopy && ! (list is ICloneable)))
      {
        ArrayList arrayList = new ArrayList (list);
        arrayList.AddRange (objects);
        IList newList = CreateList (createListMethod, list, arrayList.Count);
        Utilities.ListUtility.CopyTo (arrayList, newList);
        return newList;
      }
      else
      {
        if (mustCreateCopy)
          list = (IList) ((ICloneable)list).Clone();

        foreach (object obj in objects)
          list.Add (obj);
        return list;
      }
    }

    public static IList CreateList (CreateListMethod createListMethod, IList template, int size)
    {
      if (createListMethod != null)
        return createListMethod (size);
      else if (template is Array)
        return Array.CreateInstance (template.GetType().GetElementType(), size);
      else 
        throw new NotSupportedException ("Cannot create instance if argument 'createListMethod' is null and 'template' is not an array.");
    }

    /// <summary>
    ///    Removes a range of values from a list and returns the resulting list. The original list may be modified.
    /// </summary>
    public static IList Remove (
        IList list, IList objects, IBusinessObjectReferenceProperty property, bool mustCreateCopy)
    {
      return Remove (list, objects, GetCreateListMethod (property), mustCreateCopy);
    }
    
    /// <summary>
    ///    Removes a range of values from a list and returns the resulting list. The original list may be modified.
    /// </summary>
    public static IList Remove (
        IList list, IList objects, CreateListMethod createListMethod, bool mustCreateCopy)
    {
      if (list == null)
        return null;
      
      if (   list.IsFixedSize 
          || (mustCreateCopy && ! (list is ICloneable)))
      {
        ArrayList arrayList = new ArrayList (list);
        foreach (object obj in objects)
          arrayList.Remove (obj);

        IList newList = CreateList (createListMethod, list, arrayList.Count);
        Utilities.ListUtility.CopyTo (arrayList, newList);
        return newList;
      }
      else
      {
        if (mustCreateCopy)
          list = (IList) ((ICloneable)list).Clone();

        foreach (object obj in objects)
          list.Remove (obj);
        return list;
      }
    }

    /// <summary>
    ///    Removes a range of values from a list and returns the resulting list. The original list may be modified.
    /// </summary>
    public static IList Remove (IList list, object obj, IBusinessObjectReferenceProperty property, bool mustCreateCopy)
    {
      return Remove (list, obj, GetCreateListMethod (property), mustCreateCopy);
    }

    /// <summary>
    ///    Removes a range of values from a list and returns the resulting list. The original list may be modified.
    /// </summary>
    public static IList Remove (IList list, object obj, CreateListMethod createListMethod, bool mustCreateCopy)
    {
      if (list == null)
        return null;

      if (   list.IsFixedSize 
          || (mustCreateCopy && ! (list is ICloneable)))
      {  
        int idx = list.IndexOf (obj);
        if (idx < 0)
          return list;
        
        IList newList = CreateList (createListMethod, list, list.Count - 1);

        for (int i = 0; i < idx; ++i)
          newList[i] = list[i];
        for (int i = idx; i < list.Count - 1; ++i)
          newList[i] = list[i + 1];
        return newList;
      }
      else
      {
        if (mustCreateCopy)
          list = (IList) ((ICloneable)list).Clone();

        list.Remove (obj);
        return list;
      }
    }
  }
}

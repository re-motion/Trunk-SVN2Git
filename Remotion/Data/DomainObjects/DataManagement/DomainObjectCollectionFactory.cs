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
using System.Reflection;
using Remotion.Data.DomainObjects.DataManagement.CollectionDataManagement;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement
{
  /// <summary>
  /// Creates <see cref="DomainObjectCollection"/> instances via reflection for use with the data management classes (mostly 
  /// <see cref="CollectionEndPoint"/>).
  /// </summary>
  public class DomainObjectCollectionFactory
  {
    public DomainObjectCollection CreateCollection (
        Type collectionType,
        IDomainObjectCollectionData data,
        Type requiredItemType)
    {
      ArgumentUtility.CheckNotNull ("collectionType", collectionType);
      ArgumentUtility.CheckNotNull ("data", data);

      var collection = CreateCollectionWithDataOnlyCtor (collectionType, data) 
          ?? CreateCollectionWithDataAndTypeCtor (collectionType, data, requiredItemType);

      if (collection == null)
        throw CreateMissingConstructorException (collectionType);

      if (requiredItemType != null && collection.RequiredItemType != requiredItemType)
      {
        var message = string.Format (
            "Cannot create an instance of '{0}' with required item type '{1}': The collection's constructor sets a different required item type.",
            collectionType,
            requiredItemType);
        throw new InvalidOperationException (message);
      }

      return collection;
    }

    private DomainObjectCollection CreateCollectionWithDataOnlyCtor (Type collectionType, IDomainObjectCollectionData data)
    {
      var ctor = GetCollectionConstructor (collectionType, typeof (IDomainObjectCollectionData));
      if (ctor != null)
        return (DomainObjectCollection) ctor.Invoke (new[] { data });
      else
        return null;
    }

    private DomainObjectCollection CreateCollectionWithDataAndTypeCtor (Type collectionType, IDomainObjectCollectionData data, Type requiredItemType)
    {
      var ctor = GetCollectionConstructor (collectionType, typeof (IDomainObjectCollectionData), typeof (Type));
      if (ctor != null)
        return (DomainObjectCollection) ctor.Invoke (new object[] { data, requiredItemType });
      else
        return null;
    }

    private ConstructorInfo GetCollectionConstructor (Type collectionType, params Type[] parameterTypes)
    {
      return collectionType.GetConstructor (BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, parameterTypes, null);
    }

    private MissingMethodException CreateMissingConstructorException (Type collectionType)
    {
      var message = string.Format (
          "Cannot create an instance of '{0}' because that type does not provide a constructor taking an IDomainObjectCollectionData object and "
          + "optionally a required item type." + Environment.NewLine
          + "Example: " + Environment.NewLine
          + "public class {1} : ObjectList<...>" + Environment.NewLine
          + "{{" + Environment.NewLine
          + "  public {1} (IDomainObjectCollectionData dataStrategy)" + Environment.NewLine
          + "    : base (dataStrategy)" + Environment.NewLine
          + "  {{" + Environment.NewLine
          + "  }}" + Environment.NewLine
          + "}}",
          collectionType,
          collectionType.Name);
      return new MissingMethodException (message);
    }

  }
}
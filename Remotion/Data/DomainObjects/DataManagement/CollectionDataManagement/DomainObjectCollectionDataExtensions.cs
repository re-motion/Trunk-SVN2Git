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
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement.CollectionDataManagement
{
  /// <summary>
  /// Provides extension methods for working with <see cref="IDomainObjectCollectionData"/> instances.
  /// </summary>
  public static class DomainObjectCollectionDataExtensions
  {
    public static void Add (this IDomainObjectCollectionData data, DomainObject domainObject)
    {
      ArgumentUtility.CheckNotNull ("data", data);
      ArgumentUtility.CheckNotNull ("domainObject", domainObject);

      data.Insert (data.Count, domainObject);
    }

    public static void AddRange (this IDomainObjectCollectionData data, IEnumerable<DomainObject> items)
    {
      ArgumentUtility.CheckNotNull ("data", data);
      ArgumentUtility.CheckNotNull ("items", items);

      foreach (var domainObject in items)
        Add (data, domainObject);
    }

    public static void AddRangeAndCheckItems (this IDomainObjectCollectionData data, IEnumerable<DomainObject> items, Type requiredItemType)
    {
      ArgumentUtility.CheckNotNull ("data", data);
      ArgumentUtility.CheckNotNull ("items", items);

      var index = 0;
      foreach (var domainObject in items)
      {
        if (domainObject == null)
          throw new ArgumentItemNullException ("domainObjects", index);
        if (data.ContainsObjectID (domainObject.ID))
          throw new ArgumentItemDuplicateException ("domainObjects", index, domainObject.ID);

        if (requiredItemType != null && !requiredItemType.IsInstanceOfType (domainObject))
        {
          throw new ArgumentItemTypeException ("domainObjects", index, requiredItemType, domainObject.GetPublicDomainObjectType());
        }

        data.Add (domainObject);

        ++index;
      }
    }

    public static void ReplaceContents (this IDomainObjectCollectionData data, IEnumerable<DomainObject> items)
    {
      ArgumentUtility.CheckNotNull ("data", data);
      ArgumentUtility.CheckNotNull ("items", items);

      data.Clear ();
      data.AddRange (items);
    }
  }
}
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

namespace Remotion.Data.DomainObjects
{
  public partial class DomainObjectCollection
  {
    [Obsolete (
    "This method is obsolete. Directly call the constructor DomainObjectCollection (IEnumerable<DomainObject>, Type) or use DomainObjectCollectionFactory.",
    true)]
    public static DomainObjectCollection Create (Type collectionType)
    {
      throw new NotImplementedException ();
    }

    [Obsolete (
        "This method is obsolete. Directly call the constructor DomainObjectCollection (IEnumerable<DomainObject>, Type) or use DomainObjectCollectionFactory.",
        true)]
    public static DomainObjectCollection Create (Type collectionType, Type requiredItemType)
    {
      throw new NotImplementedException ();
    }

    [Obsolete (
        "This method is obsolete. Directly call the constructor DomainObjectCollection (IEnumerable<DomainObject>, Type) or use DomainObjectCollectionFactory.",
        true)]
    public static DomainObjectCollection Create (Type collectionType, IEnumerable<DomainObject> contents)
    {
      throw new NotImplementedException ();
    }

    [Obsolete (
        "This method is obsolete. Directly call the constructor DomainObjectCollection (IEnumerable<DomainObject>, Type) or use DomainObjectCollectionFactory.",
        true)]
    public static DomainObjectCollection Create (
        Type collectionType,
        IEnumerable<DomainObject> contents,
        Type requiredItemType)
    {
      throw new NotImplementedException ();
    }

    [Obsolete ("This method has been renamed and moved. Use UnionWith (extension method declared on DomainObjectCollectionExtensions) instead.", true)]
    public void Combine (DomainObjectCollection domainObjects)
    {
      throw new NotImplementedException ();
    }

    [Obsolete (
        "This method has been renamed and moved. Use GetItemsExcept (extension method declared on DomainObjectCollectionExtensions) instead."
        + "Note that the comparison is now based on IDs and that the order of arguments has been reversed for clarity.", true)]
    public DomainObjectCollection GetItemsNotInCollection (DomainObjectCollection domainObjects)
    {
      throw new NotImplementedException ();
    }

    // ReSharper disable UnusedParameter.Local
    [Obsolete (
       "This constructor has been removed. Use the constructor taking IEnumerable<DomainObject> (or Clone) to copy a collection, use AsReadOnly to "
       + "get a read-only version of a collection.", true)]
    public DomainObjectCollection (DomainObjectCollection collection, bool makeCollectionReadOnly)
    {
      throw new NotImplementedException ();
    }

    [Obsolete (
        "This constructor has been removed. Use the constructor taking IEnumerable<DomainObject> (or Clone) to copy a collection, use AsReadOnly to "
        + "get a read-only version of a collection.", true)]
    public DomainObjectCollection (IEnumerable<DomainObject> domainObjects, Type requiredItemType, bool makeCollectionReadOnly)
    {
      throw new NotImplementedException ();
    }
    // ReSharper restore UnusedParameter.Local

    [Obsolete (
    "This method has been removed. Use SequenceEqual (extension method defined on DomainObjectCollectionExtensions) instead. Note that that "
    + "method does not handle null arguments.", true)]
    public static bool Compare (DomainObjectCollection collection1, DomainObjectCollection collection2)
    {
      throw new NotImplementedException ();
    }

    [Obsolete (
        "This method has been removed. Use SequenceEqual or SetEquals (extension methods defined on DomainObjectCollectionExtensions) instead. "
        + "Note that those methods do not handle null arguments.", true)]
    public static bool Compare (DomainObjectCollection collection1, DomainObjectCollection collection2, bool ignoreItemOrder)
    {
      throw new NotImplementedException ();
    }

    [Obsolete (
        "This method has been removed. GetNonNotifyingData in conjunction with DomainObjectCollectionDataExtensions.ReplaceContents to replace the "
        + "contents of a collection without raising events. To hook commit or rollback events, override Commit or Rollback.", true)]
    protected internal virtual void ReplaceItems (DomainObjectCollection domainObjects)
    {
    }
  }
}
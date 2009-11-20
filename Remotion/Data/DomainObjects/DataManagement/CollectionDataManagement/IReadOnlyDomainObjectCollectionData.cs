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

namespace Remotion.Data.DomainObjects.DataManagement.CollectionDataManagement
{
  /// <summary>
  /// Provides a read-only interface for objects holding <see cref="DomainObjectCollection"/> data. To convert from an 
  /// <see cref="IDomainObjectCollectionData"/> object to <see cref="IReadOnlyDomainObjectCollectionData"/>, use 
  /// <see cref="ReadOnlyCollectionDataAdapter"/>.
  /// </summary>
  public interface IReadOnlyDomainObjectCollectionData : IEnumerable<DomainObject>
  {
    int Count { get; }
    bool ContainsObjectID (ObjectID objectID);
    DomainObject GetObject (int index);
    DomainObject GetObject (ObjectID objectID);
    int IndexOf (ObjectID objectID);
  }
}
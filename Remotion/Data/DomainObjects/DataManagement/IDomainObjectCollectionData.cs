// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
// 
// This framework is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with this framework; if not, see http://www.gnu.org/licenses.
// 
using System.Collections.Generic;

namespace Remotion.Data.DomainObjects.DataManagement
{
  /// <summary>
  /// Provides an interface for an encapsulation of the data stored inside a <see cref="DomainObjectCollection"/>.
  /// </summary>
  public interface IDomainObjectCollectionData : IEnumerable<DomainObject>
  {
    int Count { get; }

    bool ContainsObjectID (ObjectID objectID);

    DomainObject GetObject (int index);
    DomainObject GetObject (ObjectID objectID);

    int IndexOf (ObjectID objectID);

    void Clear ();
    void Insert (int index, DomainObject domainObject);
    void Remove (ObjectID objectID);
    void Replace (ObjectID oldDomainObjectID, DomainObject newDomainObject);
  }
}
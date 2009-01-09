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

namespace Remotion.Data.DomainObjects.Queries
{
  public class CollectionQueryResult<T> : ICollectionQueryResult where T: DomainObject
  {
    public int Count
    {
      get { throw new System.NotImplementedException(); }
    }

    public IQuery Query
    {
      get { throw new System.NotImplementedException(); }
    }

    public bool ContainsDuplicates ()
    {
      throw new System.NotImplementedException();
    }

    public bool ContainsNulls ()
    {
      throw new System.NotImplementedException();
    }

    public IEnumerable<T> AsEnumerable ()
    {
      throw new System.NotImplementedException ();
    }

    DomainObject[] ICollectionQueryResult.ToArray ()
    {
      throw new System.NotImplementedException ();
    }

    IEnumerable<DomainObject> ICollectionQueryResult.AsEnumerable ()
    {
      throw new System.NotImplementedException();
    }

    public T[] ToArray ()
    {
      throw new System.NotImplementedException ();
    }

    public ObjectList<T> ToObjectList ()
    {
      throw new System.NotImplementedException ();
    }

    ObjectList<DomainObject> ICollectionQueryResult.ToObjectList ()
    {
      throw new System.NotImplementedException();
    }
  }
}
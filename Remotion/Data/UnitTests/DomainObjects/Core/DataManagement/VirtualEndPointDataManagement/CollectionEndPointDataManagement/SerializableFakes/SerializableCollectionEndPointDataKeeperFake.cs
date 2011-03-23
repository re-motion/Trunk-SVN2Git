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
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.CollectionDataManagement;
using Remotion.Data.DomainObjects.DataManagement.VirtualEndPointDataManagement.CollectionEndPointDataManagement;
using Remotion.Data.DomainObjects.Infrastructure.Serialization;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.VirtualEndPointDataManagement.CollectionEndPointDataManagement.SerializableFakes
{
  public class SerializableCollectionEndPointDataKeeperFake : ICollectionEndPointDataKeeper
  {
    public SerializableCollectionEndPointDataKeeperFake ()
    {
    }

    public IDomainObjectCollectionData CollectionData
    {
      get { throw new NotImplementedException(); }
    }

    public ReadOnlyCollectionDataDecorator OriginalCollectionData
    {
      get { throw new NotImplementedException(); }
    }

    public IObjectEndPoint[] OppositeEndPoints
    {
      get { throw new NotImplementedException(); }
    }

    public IRealObjectEndPoint[] OriginalOppositeEndPoints
    {
      get { return new IRealObjectEndPoint[0]; }
    }

    public DomainObject[] OriginalItemsWithoutEndPoints
    {
      get { throw new NotImplementedException(); }
    }

    public IRealObjectEndPoint[] CurrentOppositeEndPoints
    {
      get { throw new NotImplementedException(); }
    }

    public IComparer<DomainObject> SortExpressionBasedComparer
    {
      get { throw new NotImplementedException(); }
    }

    public RelationEndPointID EndPointID
    {
      get { throw new NotImplementedException(); }
    }

    public bool ContainsOriginalOppositeEndPoint (IRealObjectEndPoint oppositeEndPoint)
    {
      return false;
    }

    public void RegisterOriginalOppositeEndPoint (IRealObjectEndPoint oppositeEndPoint)
    {
      throw new NotImplementedException();
    }

    public void UnregisterOriginalOppositeEndPoint (IRealObjectEndPoint oppositeEndPoint)
    {
      throw new NotImplementedException();
    }

    public bool ContainsCurrentOppositeEndPoint (IRealObjectEndPoint oppositeEndPoint)
    {
      throw new NotImplementedException();
    }

    public void RegisterCurrentOppositeEndPoint (IRealObjectEndPoint oppositeEndPoint)
    {
      throw new NotImplementedException();
    }

    public void UnregisterCurrentOppositeEndPoint (IRealObjectEndPoint oppositeEndPoint)
    {
      throw new NotImplementedException();
    }

    public bool ContainsOriginalItemWithoutEndPoint (DomainObject domainObject)
    {
      throw new NotImplementedException();
    }

    public void RegisterOriginalItemWithoutEndPoint (DomainObject domainObject)
    {
      throw new NotImplementedException();
    }

    public void UnregisterOriginalItemWithoutEndPoint (DomainObject domainObject)
    {
      throw new NotImplementedException();
    }

    public bool HasDataChanged ()
    {
      throw new NotImplementedException();
    }

    public void SortCurrentAndOriginalData (IComparer<DomainObject> comparer)
    {
      throw new NotImplementedException();
    }

    public void Commit ()
    {
      throw new NotImplementedException();
    }

    public void Rollback ()
    {
      throw new NotImplementedException();
    }

    public void Insert (int index, IObjectEndPoint insertedEndPoint)
    {
      throw new NotImplementedException();
    }

    public void Remove (IObjectEndPoint removedEndPoint)
    {
      throw new NotImplementedException();
    }

    public void Clear ()
    {
      throw new NotImplementedException();
    }

    public SerializableCollectionEndPointDataKeeperFake (FlattenedDeserializationInfo info)
    {

    }

    void IFlattenedSerializable.SerializeIntoFlatStructure (FlattenedSerializationInfo info)
    {
     
    }
  }
}
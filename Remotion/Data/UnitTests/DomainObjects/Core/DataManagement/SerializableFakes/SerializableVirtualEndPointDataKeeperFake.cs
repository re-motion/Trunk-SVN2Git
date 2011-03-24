// Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
// All rights reserved.

using System;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.VirtualEndPoints;
using Remotion.Data.DomainObjects.Infrastructure.Serialization;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.SerializableFakes
{
  public class SerializableVirtualEndPointDataKeeperFake : IVirtualEndPointDataKeeper
  {
    public SerializableVirtualEndPointDataKeeperFake ()
    {
    }

    public SerializableVirtualEndPointDataKeeperFake (FlattenedDeserializationInfo info)
    {
    }

    public void SerializeIntoFlatStructure (FlattenedSerializationInfo info)
    {
    }

    public RelationEndPointID EndPointID
    {
      get { throw new NotImplementedException(); }
    }

    public bool ContainsOriginalObjectID (ObjectID objectID)
    {
      throw new NotImplementedException();
    }

    public bool ContainsOriginalOppositeEndPoint (IRealObjectEndPoint oppositeEndPoint)
    {
      throw new NotImplementedException();
    }

    public void RegisterOriginalOppositeEndPoint (IRealObjectEndPoint oppositeEndPoint)
    {
      throw new NotImplementedException();
    }

    public void UnregisterOriginalOppositeEndPoint (IRealObjectEndPoint oppositeEndPoint)
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

    public void RegisterCurrentOppositeEndPoint (IRealObjectEndPoint oppositeEndPoint)
    {
      throw new NotImplementedException();
    }

    public void UnregisterCurrentOppositeEndPoint (IRealObjectEndPoint oppositeEndPoint)
    {
      throw new NotImplementedException();
    }

    public bool HasDataChanged ()
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
  }
}
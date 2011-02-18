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
using Remotion.Data.DomainObjects.Infrastructure.Serialization;
using Remotion.Data.DomainObjects.Mapping;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.CollectionEndPointDataManagement.SerializableFakes
{
  [Serializable]
  public class SerializableObjectEndPointFake : IObjectEndPoint
  {
    public SerializableObjectEndPointFake ()
    {
    }

    public SerializableObjectEndPointFake (FlattenedDeserializationInfo info)
    {
    }

    public bool IsNull
    {
      get { throw new NotImplementedException(); }
    }

    public void SerializeIntoFlatStructure (FlattenedSerializationInfo info)
    {
    }

    public RelationEndPointID ID
    {
      get { throw new NotImplementedException(); }
    }

    public ClientTransaction ClientTransaction
    {
      get { throw new NotImplementedException(); }
    }

    public ObjectID ObjectID
    {
      get { throw new NotImplementedException(); }
    }

    public IRelationEndPointDefinition Definition
    {
      get { throw new NotImplementedException(); }
    }

    public RelationDefinition RelationDefinition
    {
      get { throw new NotImplementedException(); }
    }

    public bool IsDataComplete
    {
      get { throw new NotImplementedException(); }
    }

    public bool HasChanged
    {
      get { throw new NotImplementedException(); }
    }

    public bool HasBeenTouched
    {
      get { throw new NotImplementedException(); }
    }

    public DomainObject GetDomainObject ()
    {
      throw new NotImplementedException();
    }

    public DomainObject GetDomainObjectReference ()
    {
      throw new NotImplementedException();
    }

    public void EnsureDataComplete ()
    {
      throw new NotImplementedException();
    }

    public void SynchronizeOppositeEndPoint (IObjectEndPoint oppositeEndPoint)
    {
      throw new NotImplementedException();
    }

    public void Touch ()
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

    public IDataManagementCommand CreateRemoveCommand (DomainObject removedRelatedObject)
    {
      throw new NotImplementedException();
    }

    public IDataManagementCommand CreateDeleteCommand ()
    {
      throw new NotImplementedException();
    }

    public void CheckMandatory ()
    {
      throw new NotImplementedException();
    }

    public IEnumerable<IRelationEndPoint> GetOppositeRelationEndPoints (IDataManager dataManager)
    {
      throw new NotImplementedException();
    }

    public void SetValueFrom (IRelationEndPoint source)
    {
      throw new NotImplementedException();
    }

    public ObjectID OppositeObjectID
    {
      get { throw new NotImplementedException(); }
      set { throw new NotImplementedException(); }
    }

    public ObjectID OriginalOppositeObjectID
    {
      get { throw new NotImplementedException(); }
    }

    public bool IsSynchronized
    {
      get { throw new NotImplementedException(); }
    }

    public void MarkSynchronized ()
    {
      throw new NotImplementedException();
    }

    public void MarkUnsynchronized ()
    {
      throw new NotImplementedException();
    }

    public void Synchronize (IRelationEndPoint oppositeEndPoint)
    {
      throw new NotImplementedException();
    }

    public DomainObject GetOppositeObject (bool includeDeleted)
    {
      throw new NotImplementedException();
    }

    public DomainObject GetOriginalOppositeObject ()
    {
      throw new NotImplementedException();
    }

    public IDataManagementCommand CreateSetCommand (DomainObject newRelatedObject)
    {
      throw new NotImplementedException();
    }
  }
}
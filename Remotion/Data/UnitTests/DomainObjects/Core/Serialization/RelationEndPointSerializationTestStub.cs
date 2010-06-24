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

namespace Remotion.Data.UnitTests.DomainObjects.Core.Serialization
{
  [Serializable]
  public class RelationEndPointSerializationTestStub : RelationEndPoint
  {
    public RelationEndPointSerializationTestStub (ClientTransaction clientTransaction, RelationEndPointID id)
        : base (clientTransaction, id)
    {
    }

    public RelationEndPointSerializationTestStub (FlattenedDeserializationInfo info)
      : base (info)
    {
    }

    public override void SetValueFrom (RelationEndPoint source)
    {
      throw new NotImplementedException();
    }

    public override bool IsDataAvailable
    {
      get { throw new NotImplementedException(); }
    }

    public override bool HasChanged
    {
      get { throw new NotImplementedException(); }
    }

    public override bool HasBeenTouched
    {
      get { throw new NotImplementedException(); }
    }

    public override void EnsureDataAvailable ()
    {
      throw new NotImplementedException();
    }

    public override void Touch ()
    {
      throw new NotImplementedException();
    }

    public override IEnumerable<RelationEndPoint> GetOppositeRelationEndPoints (IDataManager dataManager)
    {
      throw new NotImplementedException();
    }

    public override void Commit ()
    {
      throw new NotImplementedException();
    }

    public override void Rollback ()
    {
      throw new NotImplementedException();
    }

    public override void CheckMandatory ()
    {
      throw new NotImplementedException();
    }

    public override IDataManagementCommand CreateRemoveCommand (DomainObject removedRelatedObject)
    {
      throw new NotImplementedException();
    }

    public override IDataManagementCommand CreateDeleteCommand ()
    {
      throw new NotImplementedException();
    }

    protected override void SerializeIntoFlatStructure (FlattenedSerializationInfo info)
    {
    }
  }
}

// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
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
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Infrastructure.Serialization;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Serialization
{
  [Serializable]
  public class RelationEndPointStub : RelationEndPoint
  {
    public RelationEndPointStub (ClientTransaction clientTransaction, RelationEndPointID id)
        : base (clientTransaction, id)
    {
    }

    public RelationEndPointStub (FlattenedDeserializationInfo info)
      : base (info)
    {
    }

    public override RelationEndPoint Clone (ClientTransaction clientTransaction)
    {
      throw new NotImplementedException();
    }

    protected override void AssumeSameState (RelationEndPoint source)
    {
      throw new NotImplementedException();
    }

    protected override void TakeOverCommittedData (RelationEndPoint source)
    {
      throw new NotImplementedException();
    }

    public override bool HasChanged
    {
      get { throw new NotImplementedException(); }
    }

    public override bool HasBeenTouched
    {
      get { throw new NotImplementedException(); }
    }

    protected override void Touch ()
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

    public override void PerformDelete ()
    {
      throw new NotImplementedException();
    }

    public override RelationEndPointModification CreateModification (IEndPoint oldEndPoint, IEndPoint newEndPoint)
    {
      throw new NotImplementedException();
    }

    protected override void SerializeIntoFlatStructure (FlattenedSerializationInfo info)
    {
    }
  }
}

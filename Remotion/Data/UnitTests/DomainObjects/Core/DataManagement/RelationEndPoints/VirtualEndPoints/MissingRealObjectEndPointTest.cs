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
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints;
using Remotion.Data.DomainObjects.DomainImplementation;
using Remotion.Data.UnitTests.DomainObjects.Core.Serialization;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.RelationEndPoints.VirtualEndPoints
{
  [TestFixture]
  public class MissingRealObjectEndPointTest : StandardMappingTest
  {
    private ClientTransaction _clientTransaction;
    private RelationEndPointID _endPointID;
    
    private MissingRealObjectEndPoint _endPoint;

    public override void SetUp ()
    {
      base.SetUp ();

      _clientTransaction = Data.DomainObjects.ClientTransaction.CreateRootTransaction();
      _endPointID = RelationEndPointID.Create (DomainObjectIDs.Order1, typeof (Order), "Customer");

      _endPoint = new MissingRealObjectEndPoint (_clientTransaction, _endPointID);
    }

    [Test]
    public void IsNull ()
    {
      Assert.That (_endPoint.IsNull, Is.True);
    }

    [Test]
    public void ID ()
    {
      Assert.That (_endPoint.ID, Is.EqualTo (_endPointID));
    }

    [Test]
    public void ClientTransaction ()
    {
      Assert.That (_endPoint.ClientTransaction, Is.SameAs (_clientTransaction));
    }

    [Test]
    public void ObjectID ()
    {
      Assert.That (_endPoint.ObjectID, Is.EqualTo (_endPointID.ObjectID));
    }

    [Test]
    public void Definition ()
    {
      Assert.That (_endPoint.Definition, Is.EqualTo (_endPointID.Definition));
    }

    [Test]
    public void RelationDefinition ()
    {
      Assert.That (_endPoint.RelationDefinition, Is.EqualTo (_endPointID.Definition.RelationDefinition));
    }

    [Test]
    public void HasChanged ()
    {
      Assert.That (_endPoint.HasChanged, Is.False);
    }

    [Test]
    public void HasBeenTouched ()
    {
      Assert.That (_endPoint.HasBeenTouched, Is.False);
    }

    [Test]
    public void GetDomainObject ()
    {
      Assert.That (ClientTransactionTestHelper.GetDataManager (_clientTransaction).GetDataContainerWithoutLoading (_endPointID.ObjectID), Is.Null);

      var result = _endPoint.GetDomainObject();

      Assert.That (ClientTransactionTestHelper.GetDataManager (_clientTransaction).GetDataContainerWithoutLoading (_endPointID.ObjectID), Is.Not.Null);
      Assert.That (result, Is.SameAs (LifetimeService.GetObject (_clientTransaction, _endPointID.ObjectID, false)));
    }

    [Test]
    public void GetDomainObjectReference ()
    {
      Assert.That (ClientTransactionTestHelper.GetDataManager (_clientTransaction).GetDataContainerWithoutLoading (_endPointID.ObjectID), Is.Null);

      var result = _endPoint.GetDomainObjectReference ();

      Assert.That (ClientTransactionTestHelper.GetDataManager (_clientTransaction).GetDataContainerWithoutLoading (_endPointID.ObjectID), Is.Null);
      Assert.That (result, Is.SameAs (LifetimeService.GetObjectReference (_clientTransaction, _endPointID.ObjectID)));
    }

    [Test]
    public void ThrowingMembers ()
    {
      Assert.That (() => _endPoint.OppositeObjectID, Throws.Exception.TypeOf<NotSupportedException> ());
      Assert.That (() => _endPoint.OriginalOppositeObjectID, Throws.Exception.TypeOf<NotSupportedException> ());
      Assert.That (() => _endPoint.GetOppositeObject (true), Throws.Exception.TypeOf<NotSupportedException> ());
      Assert.That (() => _endPoint.GetOriginalOppositeObject (), Throws.Exception.TypeOf<NotSupportedException> ());
      Assert.That (() => _endPoint.IsDataComplete, Throws.Exception.TypeOf<NotSupportedException> ());
      Assert.That (() => _endPoint.EnsureDataComplete(), Throws.Exception.TypeOf<NotSupportedException> ());
      Assert.That (() => _endPoint.IsSynchronized, Throws.Exception.TypeOf<NotSupportedException> ());
      Assert.That (() => _endPoint.Synchronize(), Throws.Exception.TypeOf<NotSupportedException> ());
      Assert.That (() => _endPoint.Touch (), Throws.Exception.TypeOf<NotSupportedException> ());
      Assert.That (() => _endPoint.Commit (), Throws.Exception.TypeOf<NotSupportedException> ());
      Assert.That (() => _endPoint.Rollback (), Throws.Exception.TypeOf<NotSupportedException> ());
      Assert.That (() => _endPoint.CheckMandatory (), Throws.Exception.TypeOf<NotSupportedException> ());
      Assert.That (() => _endPoint.SetDataFromSubTransaction (null), Throws.Exception.TypeOf<NotSupportedException> ());
      Assert.That (() => _endPoint.CreateSetCommand (null), Throws.Exception.TypeOf<NotSupportedException> ());
      Assert.That (() => _endPoint.CreateRemoveCommand (null), Throws.Exception.TypeOf<NotSupportedException> ());
      Assert.That (() => _endPoint.CreateDeleteCommand (), Throws.Exception.TypeOf<NotSupportedException> ());
      Assert.That (() => _endPoint.GetOppositeRelationEndPointIDs (), Throws.Exception.TypeOf<NotSupportedException> ());
      Assert.That (() => _endPoint.GetOppositeRelationEndPointID (), Throws.Exception.TypeOf<NotSupportedException> ());
      Assert.That (() => _endPoint.MarkSynchronized (), Throws.Exception.TypeOf<NotSupportedException> ());
      Assert.That (() => _endPoint.MarkUnsynchronized (), Throws.Exception.TypeOf<NotSupportedException> ());
      Assert.That (() => _endPoint.ResetSyncState (), Throws.Exception.TypeOf<NotSupportedException> ());
    }

    [Test]
    public void Serialization ()
    {
      var deserializedEndPoint = FlattenedSerializer.SerializeAndDeserialize (_endPoint);

      Assert.That (deserializedEndPoint.ID, Is.Not.Null);
      Assert.That (deserializedEndPoint.ClientTransaction, Is.Not.Null);
    }
  }
}
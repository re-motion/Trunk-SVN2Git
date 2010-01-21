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
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.Commands.EndPointModifications;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement
{
  [TestFixture]
  public class NullObjectEndPointTest : ClientTransactionBaseTest
  {
    private IRelationEndPointDefinition _definition;
    private NullObjectEndPoint _nullEndPoint;

    public override void SetUp ()
    {
      base.SetUp();
      _definition = DomainObjectIDs.OrderTicket1.ClassDefinition.GetRelationEndPointDefinition (typeof (OrderTicket).FullName + ".Order");
      _nullEndPoint = new NullObjectEndPoint (ClientTransactionMock, _definition);
    }

    [Test]
    public void Definition ()
    {
      Assert.That (_nullEndPoint.Definition, Is.SameAs (_definition));
    }

    [Test]
    public void ObjectID ()
    {
      Assert.That (_nullEndPoint.ObjectID, Is.Null);
    }

    [Test]
    public void ID ()
    {
      var id = _nullEndPoint.ID;
      Assert.That (id.Definition, Is.SameAs (_definition));
      Assert.That (id.ObjectID, Is.Null);
    }

    [Test]
    public void OppositeObjectID_Get ()
    {
      Assert.That (_nullEndPoint.OppositeObjectID, Is.Null);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException))]
    public void OppositeObjectID_Set ()
    {
      _nullEndPoint.OppositeObjectID = DomainObjectIDs.Order3;
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException))]
    public void OriginalOppositeObjectID_Get ()
    {
      Dev.Null = _nullEndPoint.OriginalOppositeObjectID;
    }

    [Test]
    public void IsDataAvailable_True ()
    {
      Assert.That (_nullEndPoint.IsDataAvailable, Is.True);
    }

    [Test]
    public void HasChanged ()
    {
      Assert.That (_nullEndPoint.HasChanged, Is.False);
    }

    [Test]
    public void HasBeenTouched ()
    {
      Assert.That (_nullEndPoint.HasBeenTouched, Is.False);
    }

    [Test]
    public void IsNull ()
    {
      Assert.That (_nullEndPoint.IsNull, Is.True);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException))]
    public void SetOppositeDomainObjectAndNotify ()
    {
      _nullEndPoint.SetOppositeObjectAndNotify (Order.NewObject ());
    }

    [Test]
    public void Touch ()
    {
      Assert.That (_nullEndPoint.HasBeenTouched, Is.False);
      _nullEndPoint.Touch ();
      Assert.That (_nullEndPoint.HasBeenTouched, Is.False);
    }

    [Test]
    public void CreateRemoveModification ()
    {
      var removedRelatedObject = Order.GetObject (DomainObjectIDs.Order3);
      var modification = (RelationEndPointModification) _nullEndPoint.CreateRemoveModification (removedRelatedObject);
      Assert.That (modification, Is.InstanceOfType (typeof (NullEndPointModification)));
      Assert.That (modification.OldRelatedObject, Is.SameAs (removedRelatedObject));
      Assert.That (modification.NewRelatedObject, Is.Null);
    }

    [Test]
    public void CreateSetModification ()
    {
      var newRelatedObject = Order.GetObject (DomainObjectIDs.Order3);
      var modification = (RelationEndPointModification) _nullEndPoint.CreateSetModification (newRelatedObject);
      Assert.That (modification, Is.InstanceOfType (typeof (NullEndPointModification)));
      Assert.That (modification.OldRelatedObject, Is.Null);
      Assert.That (modification.NewRelatedObject, Is.SameAs (newRelatedObject));
    }

    [Test]
    public void NotifyClientTransactionOfBeginRelationChange ()
    {
      var order = Order.NewObject ();

      ClientTransactionTestHelper.EnsureTransactionThrowsOnEvents (ClientTransactionMock);

      _nullEndPoint.NotifyClientTransactionOfBeginRelationChange (order, order);
    }

    [Test]
    public void NotifyClientTransactionOfEndRelationChange ()
    {
      ClientTransactionTestHelper.EnsureTransactionThrowsOnEvents (ClientTransactionMock);

      _nullEndPoint.NotifyClientTransactionOfEndRelationChange ();
    }

    [Test]
    public void EnsureDataAvailable_DoesNothing ()
    {
      ClientTransactionTestHelper.EnsureTransactionThrowsOnEvents (ClientTransactionMock);

      _nullEndPoint.EnsureDataAvailable ();
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException))]
    public void Commit ()
    {
      _nullEndPoint.Commit ();
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException))]
    public void Rollback ()
    {
      _nullEndPoint.Commit ();
    }
  }
}

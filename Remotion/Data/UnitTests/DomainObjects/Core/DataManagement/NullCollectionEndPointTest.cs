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
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.EndPointModifications;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Development.UnitTesting;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement
{
  [TestFixture]
  public class NullCollectionEndPointTest : ClientTransactionBaseTest
  {
    private IRelationEndPointDefinition _definition;
    private NullCollectionEndPoint _nullEndPoint;
    private OrderItem _relatedObject;

    public override void SetUp ()
    {
      base.SetUp();
      _definition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (Order))
          .GetRelationEndPointDefinition (typeof (Order).FullName + ".OrderItems");
      _nullEndPoint = new NullCollectionEndPoint (ClientTransactionMock, _definition);
      _relatedObject = OrderItem.NewObject();
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
    public void OppositeDomainObjects_Get ()
    {
      Assert.That (_nullEndPoint.OppositeDomainObjects, Is.Empty);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException))]
    public void OppositeDomainObjects_Set ()
    {
      _nullEndPoint.OppositeDomainObjects = new DomainObjectCollection ();
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException))]
    public void OriginalOppositeDomainObjectsContents ()
    {
      Dev.Null = _nullEndPoint.OriginalOppositeDomainObjectsContents;
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException))]
    public void OriginalOppositeDomainObjectsReference ()
    {
      Dev.Null = _nullEndPoint.OriginalOppositeDomainObjectsReference;
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
    public void SetOppositeCollectionAndNotify ()
    {
      _nullEndPoint.SetOppositeCollectionAndNotify (new DomainObjectCollection ());
    }

    [Test]
    public void Touch ()
    {
      Assert.That (_nullEndPoint.HasBeenTouched, Is.False);
      _nullEndPoint.Touch ();
      Assert.That (_nullEndPoint.HasBeenTouched, Is.False);
    }

    [Test]
    public void CreateInsertModification ()
    {
      Assert.That (_nullEndPoint.CreateInsertModification (_relatedObject, 12), Is.InstanceOfType (typeof (NullEndPointModification)));
    }

    [Test]
    public void CreateAddModification ()
    {
      Assert.That (_nullEndPoint.CreateAddModification (_relatedObject), Is.InstanceOfType (typeof (NullEndPointModification)));
    }

    [Test]
    public void CreateRemoveModification ()
    {
      Assert.That (_nullEndPoint.CreateRemoveModification (_relatedObject), Is.InstanceOfType (typeof (NullEndPointModification)));
    }

    [Test]
    public void CreateReplaceModification ()
    {
      Assert.That (_nullEndPoint.CreateReplaceModification (12, _relatedObject), Is.InstanceOfType (typeof (NullEndPointModification)));
    }

    [Test]
    public void NotifyClientTransactionOfBeginRelationChange ()
    {
      var order = Order.NewObject ();

      var listenerMock = new MockRepository ().StrictMock<IClientTransactionListener> ();
      ClientTransactionMock.AddListener (listenerMock);
      listenerMock.Replay ();

      _nullEndPoint.NotifyClientTransactionOfBeginRelationChange (order, order);

      listenerMock.AssertWasNotCalled (
          mock => mock.RelationChanging (
              Arg<DomainObject>.Is.Anything,
              Arg<string>.Is.Anything,
              Arg<DomainObject>.Is.Anything,
              Arg<DomainObject>.Is.Anything));
    }

    [Test]
    public void NotifyClientTransactionOfEndRelationChange ()
    {
      var listenerMock = new MockRepository ().StrictMock<IClientTransactionListener> ();
      ClientTransactionMock.AddListener (listenerMock);
      listenerMock.Replay ();

      _nullEndPoint.NotifyClientTransactionOfEndRelationChange ();

      listenerMock.AssertWasNotCalled (
          mock => mock.RelationChanged (
              Arg<DomainObject>.Is.Anything,
              Arg<string>.Is.Anything));
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

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
using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.EndPointModifications;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.EndPointModifications
{
  [TestFixture]
  public class CollectionEndPointInsertModificationTest : CollectionEndPointModificationTestBase
  {
    private Order _insertedRelatedObject;
    private CollectionEndPointInsertModification _modification;

    public override void SetUp ()
    {
      base.SetUp();

      _insertedRelatedObject = Order.GetObject (DomainObjectIDs.Order2);
      _modification = new CollectionEndPointInsertModification (CollectionEndPoint, _insertedRelatedObject, 12, CollectionDataMock);
    }

    [Test]
    public void Initialization ()
    {
      Assert.That (_modification.ModifiedEndPoint, Is.SameAs (CollectionEndPoint));
      Assert.That (_modification.OldRelatedObject, Is.Null);
      Assert.That (_modification.NewRelatedObject, Is.SameAs (_insertedRelatedObject));
      Assert.That (_modification.Index, Is.EqualTo (12));
      Assert.That (_modification.ModifiedCollection, Is.SameAs (CollectionEndPoint.OppositeDomainObjects));
      Assert.That (_modification.ModifiedCollectionData, Is.SameAs (CollectionDataMock));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Modified end point is null, a NullEndPointModification is needed.\r\n"
                                                                      + "Parameter name: modifiedEndPoint")]
    public void Initialization_FromNullEndPoint ()
    {
      var endPoint = new NullCollectionEndPoint (RelationEndPointID.Definition);
      new CollectionEndPointInsertModification (endPoint, _insertedRelatedObject, 0, CollectionDataMock);
    }

    [Test]
    public void Begin ()
    {
      bool relationChangingCalled = false;
      bool relationChangedCalled = false;

      DomainObject.RelationChanging += (sender, args) =>
      {
        relationChangingCalled = true;

        Assert.That (args.PropertyName, Is.EqualTo (CollectionEndPoint.PropertyName));
        Assert.That (args.NewRelatedObject, Is.SameAs (_insertedRelatedObject));
        Assert.That (args.OldRelatedObject, Is.Null);

        Assert.That (CollectionEventReceiver.AddingDomainObject, Is.SameAs (_insertedRelatedObject)); // collection got event first
      };
      DomainObject.RelationChanged += (sender, args) => relationChangedCalled = true;

      _modification.Begin ();

      Assert.That (relationChangingCalled, Is.True); // operation was started
      Assert.That (relationChangedCalled, Is.False); // operation was not finished
      Assert.That (CollectionEventReceiver.AddedDomainObject, Is.Null); // operation was not finished
    }

    [Test]
    public void End ()
    {
      bool relationChangingCalled = false;
      bool relationChangedCalled = false;

      DomainObject.RelationChanging += (sender, args) => relationChangingCalled = true;
      DomainObject.RelationChanged += (sender, args) =>
      {
        relationChangedCalled = true;

        Assert.That (args.PropertyName, Is.EqualTo (CollectionEndPoint.PropertyName));
        Assert.That (CollectionEventReceiver.AddedDomainObject, Is.SameAs (_insertedRelatedObject)); // collection got event first
      };

      _modification.End ();

      Assert.That (relationChangingCalled, Is.False); // operation was not started
      Assert.That (relationChangedCalled, Is.True); // operation was finished
      Assert.That (CollectionEventReceiver.RemovingDomainObjects, Is.Empty); // operation was not started
    }

    [Test]
    public void Perform ()
    {
      bool relationChangingCalled = false;
      bool relationChangedCalled = false;

      DomainObject.RelationChanging += (sender, args) => relationChangingCalled = true;
      DomainObject.RelationChanged += (sender, args) => relationChangedCalled = true;

      CollectionDataMock.BackToRecord ();
      CollectionDataMock.Expect (mock => mock.Insert (12, _insertedRelatedObject));
      CollectionDataMock.Replay ();

      _modification.Perform ();

      CollectionDataMock.VerifyAllExpectations ();

      Assert.That (relationChangingCalled, Is.False); // operation was not started
      Assert.That (relationChangedCalled, Is.False); // operation was not finished
      Assert.That (CollectionEventReceiver.AddingDomainObject, Is.Null); // operation was not started
      Assert.That (CollectionEventReceiver.AddedDomainObject, Is.Null); // operation was not finished
      Assert.That (CollectionEndPoint.HasBeenTouched, Is.True);
    }

    [Test]
    public void CreateBidirectionalModification ()
    {
      var bidirectionalModification = _modification.CreateBidirectionalModification ();
      Assert.That (bidirectionalModification, Is.InstanceOfType (typeof (NotifyingBidirectionalRelationModification)));

      // DomainObject.Orders.Insert (_insertedRelatedObject, 12)
      var steps = bidirectionalModification.GetModificationSteps ();
      Assert.That (steps.Count, Is.EqualTo (3));

      var oldCustomer = _insertedRelatedObject.Customer;

      // _insertedRelatedObject.Customer = DomainObject (previously oldCustomer)
      Assert.That (steps[0], Is.InstanceOfType (typeof (ObjectEndPointSetModificationBase)));
      Assert.That (steps[0].ModifiedEndPoint.ID.PropertyName, Is.EqualTo (typeof (Order).FullName + ".Customer"));
      Assert.That (steps[0].ModifiedEndPoint.ID.ObjectID, Is.EqualTo (_insertedRelatedObject.ID));
      Assert.That (steps[0].OldRelatedObject, Is.SameAs (oldCustomer));
      Assert.That (steps[0].NewRelatedObject, Is.SameAs (DomainObject));

      // DomainObject.Orders.Insert (_insertedRelatedObject, 12)
      Assert.That (steps[1], Is.SameAs (_modification));

      // oldCustomer.Orders.Remove (_insertedRelatedObject)
      Assert.That (steps[2], Is.InstanceOfType (typeof (CollectionEndPointRemoveModification)));
      Assert.That (steps[2].ModifiedEndPoint.ID.PropertyName, Is.EqualTo (typeof (Customer).FullName + ".Orders"));
      Assert.That (steps[2].ModifiedEndPoint.ID.ObjectID, Is.EqualTo (oldCustomer.ID));
      Assert.That (steps[2].OldRelatedObject, Is.SameAs (_insertedRelatedObject));
    }
  }
}
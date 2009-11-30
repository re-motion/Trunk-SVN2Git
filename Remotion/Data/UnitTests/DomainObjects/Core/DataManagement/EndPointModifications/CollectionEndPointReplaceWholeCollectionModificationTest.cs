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
using Remotion.Data.DomainObjects.DataManagement.CollectionDataManagement;
using Remotion.Data.DomainObjects.DataManagement.EndPointModifications;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Rhino.Mocks;
using System.Reflection;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.EndPointModifications
{
  [TestFixture]
  public class CollectionEndPointReplaceWholeCollectionModificationTest : CollectionEndPointModificationTestBase
  {
    private DomainObjectCollection _newCollection;

    private MockRepository _mockRepository;
    private IDomainObjectCollectionTransformer _oldTransformerMock;
    private IDomainObjectCollectionTransformer _newTransformerMock;

    private CollectionEndPointReplaceWholeCollectionModification _modification;

    private Order _order1;
    private Order _orderWithoutOrderItem;
    private Order _order2;

    public override void SetUp ()
    {
      base.SetUp();

      _newCollection = new OrderCollection();

      _mockRepository = new MockRepository ();
      _oldTransformerMock = _mockRepository.StrictMock<IDomainObjectCollectionTransformer> ();
      _newTransformerMock = _mockRepository.StrictMock<IDomainObjectCollectionTransformer> ();

      _modification = new CollectionEndPointReplaceWholeCollectionModification (
          CollectionEndPoint, 
          _newCollection,
          _oldTransformerMock,
          _newTransformerMock,
          CollectionDataMock);

      _order1 = Order.GetObject (DomainObjectIDs.Order1);
      _orderWithoutOrderItem = Order.GetObject (DomainObjectIDs.OrderWithoutOrderItem);
      _order2 = Order.GetObject (DomainObjectIDs.Order2);
    }

    [Test]
    public void Initialization ()
    {
      Assert.That (_modification.ModifiedEndPoint, Is.SameAs (CollectionEndPoint));
      Assert.That (_modification.OldRelatedObject, Is.Null);
      Assert.That (_modification.NewRelatedObject, Is.Null);
      Assert.That (_modification.NewOppositeCollection, Is.SameAs (_newCollection));
      Assert.That (_modification.OldOppositeCollectionTransformer, Is.SameAs (_oldTransformerMock));
      Assert.That (_modification.NewOppositeCollectionTransformer, Is.SameAs (_newTransformerMock));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Modified end point is null, a NullEndPointModification is needed.\r\n"
                                                                      + "Parameter name: modifiedEndPoint")]
    public void Initialization_FromNullEndPoint ()
    {
      var endPoint = new NullCollectionEndPoint (RelationEndPointID.Definition);
      new CollectionEndPointReplaceWholeCollectionModification (endPoint, _newCollection, _oldTransformerMock, _newTransformerMock, CollectionDataMock);
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
        Assert.That (args.NewRelatedObject, Is.Null);
        Assert.That (args.OldRelatedObject, Is.Null);
      };
      DomainObject.RelationChanged += (sender, args) => relationChangedCalled = true;

      _modification.Begin ();

      Assert.That (CollectionEventReceiver.RemovingDomainObjects, Is.Empty);
      Assert.That (CollectionEventReceiver.AddingDomainObject, Is.Null);

      Assert.That (relationChangingCalled, Is.True); // operation was started
      Assert.That (relationChangedCalled, Is.False); // operation was not finished
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
      };

      _modification.End ();

      Assert.That (relationChangingCalled, Is.False); // operation was not started
      Assert.That (relationChangedCalled, Is.True); // operation was finished
      Assert.That (CollectionEventReceiver.RemovedDomainObjects, Is.Empty);
      Assert.That (CollectionEventReceiver.AddedDomainObject, Is.Null);
    }

    [Test]
    public void Perform ()
    {
      _newCollection.Add (_order1);

      bool relationChangingCalled = false;
      bool relationChangedCalled = false;

      DomainObject.RelationChanging += (sender, args) => relationChangingCalled = true;
      DomainObject.RelationChanged += (sender, args) => relationChangedCalled = true;

      using (_mockRepository.Ordered ())
      {
        // Transform old collection to stand-alone
        _oldTransformerMock.Stub (mock => mock.Collection).Return (CollectionEndPoint.OppositeDomainObjects);
        _oldTransformerMock.Expect (mock => mock.TransformToStandAlone ());

        // Replace items of end point, _after_ old collection has been transformed to stand-alone
        CollectionDataMock.Expect (mock => mock.Clear ());
        CollectionDataMock.Expect (mock => mock.Count).Return (0);
        CollectionDataMock.Expect (mock => mock.Insert (0, _newCollection[0]));

        // Transform new collection to associated
        _newTransformerMock
            .Expect (mock => mock.TransformToAssociated (CollectionEndPoint))
            .WhenCalled (mi =>
            {
              Assert.That (CollectionEndPoint.OppositeDomainObjects != _newCollection); // transformations occur before SetOppositeCollection
              TransformToAssociated (_newCollection);
            });
      }
      _mockRepository.ReplayAll ();
      
      _modification.Perform ();

      _mockRepository.VerifyAll ();

      Assert.That (relationChangingCalled, Is.False); // operation was not started
      Assert.That (relationChangedCalled, Is.False); // operation was not finished

      Assert.That (CollectionEndPoint.OppositeDomainObjects, Is.SameAs (_newCollection));
      Assert.That (CollectionEndPoint.HasBeenTouched, Is.True);
    }

    [Test]
    public void Perform_DoesNotTransformOldCollectionToStandAlone_WhenOldCollectionAssociatedWithOtherEndPoint ()
    {
      _newCollection.Add (_order1);

      var endPointData = new EndPointDelegatingCollectionData (MockRepository.GenerateStub<ICollectionEndPoint> (), new DomainObjectCollectionData ());
      var collectionOfDifferentEndPoint = new DomainObjectCollection (endPointData);

      Assert.That (collectionOfDifferentEndPoint.AssociatedEndPoint != null);
      Assert.That (collectionOfDifferentEndPoint.AssociatedEndPoint != CollectionEndPoint);

      _oldTransformerMock.Stub (mock => mock.Collection).Return (collectionOfDifferentEndPoint);
      // _oldTransformerMock.TransformToStandAlone is not called because collectionOfDifferentEndPoint belongs to a different end point

      // Replace items of end point, _after_ old collection has been transformed to stand-alone
      CollectionDataMock.Expect (mock => mock.Clear ());
      CollectionDataMock.Expect (mock => mock.Count).Return (0);
      CollectionDataMock.Expect (mock => mock.Insert (0, _newCollection[0]));

      _newTransformerMock
          .Expect (mock => mock.TransformToAssociated (CollectionEndPoint))
          .WhenCalled (mi => TransformToAssociated (_newCollection));

      _mockRepository.ReplayAll ();

      _modification.Perform ();

      _oldTransformerMock.AssertWasNotCalled (mock => mock.TransformToStandAlone ());
      _newTransformerMock.VerifyAllExpectations ();
    }

    [Test]
    public void CreateBidirectionalModification ()
    {
      CollectionEndPoint.OppositeDomainObjects.Add (_order1);
      CollectionEndPoint.OppositeDomainObjects.Add (_orderWithoutOrderItem);

      Assert.That (_order1.Customer, Is.SameAs (CollectionEndPoint.GetDomainObject ()));
      Assert.That (_orderWithoutOrderItem.Customer, Is.SameAs (CollectionEndPoint.GetDomainObject ()));

      var customer3 = Customer.GetObject (DomainObjectIDs.Customer3);
      Assert.That (_order2.Customer, Is.SameAs (customer3));

      _newCollection.Add (_order1);
      _newCollection.Add (_order2);
      
      var bidirectionalModification = _modification.CreateBidirectionalModification ();
      Assert.That (bidirectionalModification, Is.InstanceOfType (typeof (CompositeRelationModificationWithEvents)));

      // DomainObject.Orders = _newCollection

      var steps = bidirectionalModification.GetModificationSteps ();
      Assert.That (steps.Count, Is.EqualTo (4));

      // orderWithoutOrderItem.Customer = null;
      // order2.Customer.Orders.Remove (order2);
      // order2.Customer = DomainObject;
      // DomainObject.Orders = _newCollection

      // orderWithoutOrderItem.Customer = null;
      Assert.That (steps[0], Is.InstanceOfType (typeof (ObjectEndPointSetModificationBase)));
      Assert.That (steps[0].ModifiedEndPoint.ID.PropertyName, Is.EqualTo (typeof (Order).FullName + ".Customer"));
      Assert.That (steps[0].ModifiedEndPoint.ID.ObjectID, Is.EqualTo (_orderWithoutOrderItem.ID));
      Assert.That (steps[0].OldRelatedObject, Is.SameAs (DomainObject));
      Assert.That (steps[0].NewRelatedObject, Is.Null);

      // order2.Customer.Orders.Remove (order2);
      Assert.That (steps[1], Is.InstanceOfType (typeof (CollectionEndPointRemoveModification)));
      Assert.That (steps[1].ModifiedEndPoint.ID.PropertyName, Is.EqualTo (typeof (Customer).FullName + ".Orders"));
      Assert.That (steps[1].ModifiedEndPoint.ID.ObjectID, Is.EqualTo (customer3.ID));
      Assert.That (steps[1].OldRelatedObject, Is.SameAs (_order2));
      Assert.That (steps[1].NewRelatedObject, Is.Null);

      // order2.Customer = DomainObject
      Assert.That (steps[2], Is.InstanceOfType (typeof (ObjectEndPointSetModificationBase)));
      Assert.That (steps[2].ModifiedEndPoint.ID.PropertyName, Is.EqualTo (typeof (Order).FullName + ".Customer"));
      Assert.That (steps[2].ModifiedEndPoint.ID.ObjectID, Is.EqualTo (_order2.ID));
      Assert.That (steps[2].OldRelatedObject, Is.SameAs (customer3));
      Assert.That (steps[2].NewRelatedObject, Is.SameAs (DomainObject));

      // DomainObject.Orders = _newCollection
      Assert.That (steps[3], Is.SameAs (_modification));
    }

    private void TransformToAssociated (DomainObjectCollection collection)
    {
      var transformerType = typeof (DomainObjectCollection).GetNestedType ("Transformer", BindingFlags.NonPublic);
      var transformer = (IDomainObjectCollectionTransformer) Activator.CreateInstance (transformerType, collection);
      transformer.TransformToAssociated (CollectionEndPoint);
    }
  }
}

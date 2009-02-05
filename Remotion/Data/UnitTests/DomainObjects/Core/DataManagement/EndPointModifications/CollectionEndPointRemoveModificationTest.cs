// Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
// All rights reserved.

using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.CollectionDataManagement;
using Remotion.Data.DomainObjects.DataManagement.EndPointModifications;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.Core.EventReceiver;
using Remotion.Data.UnitTests.DomainObjects.Core.Serialization;
using Rhino.Mocks;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.EndPointModifications
{
  [TestFixture]
  public class CollectionEndPointRemoveModificationTest : ClientTransactionBaseTest
  {
    private CollectionEndPoint _collectionEndPoint;
    private IEndPoint _removedEndPointStub;
    private IDomainObjectCollectionData _collectionDataMock;

    private Customer _affectedObject;
    private Order _removedRelatedObject;
    private CollectionEndPointRemoveModification _modification;
    private DomainObjectCollectionEventReceiver _oppositeDomainObjectsEventReceiver;

    public override void SetUp ()
    {
      base.SetUp ();

      _affectedObject = Customer.GetObject (DomainObjectIDs.Customer1);

      var id = new RelationEndPointID (_affectedObject.ID, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Customer.Orders");
      var oppositeDomainObjects = new DomainObjectCollection ();
      _oppositeDomainObjectsEventReceiver = new DomainObjectCollectionEventReceiver (oppositeDomainObjects);
      _collectionEndPoint = new CollectionEndPoint (ClientTransactionMock, id, oppositeDomainObjects, new FakeChangeDelegate ());

      _removedRelatedObject = Order.GetObject (DomainObjectIDs.Order1);

      _removedEndPointStub = MockRepository.GenerateStub<IEndPoint> ();
      _removedEndPointStub.Stub (stub => stub.GetDomainObject ()).Return (_removedRelatedObject);

      var relationEndPointDefinition = new RelationEndPointDefinition (
          _removedRelatedObject.ID.ClassDefinition,
          "Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.Customer", 
          false);
      _removedEndPointStub.Stub (stub => stub.Definition).Return (relationEndPointDefinition);

      _collectionDataMock = new MockRepository().StrictMock<IDomainObjectCollectionData> ();
      _collectionDataMock.Replay ();

      _modification = new CollectionEndPointRemoveModification (_collectionEndPoint, _removedEndPointStub, _collectionDataMock);
    }

    [Test]
    public void Initialization ()
    {
      Assert.That (_modification.AffectedEndPoint, Is.SameAs (_collectionEndPoint));
      Assert.That (_modification.OldEndPoint, Is.SameAs (_removedEndPointStub));
      Assert.That (_modification.RemovedObject, Is.SameAs (_removedRelatedObject));
      Assert.That (_modification.NewEndPoint.IsNull, Is.True);
      Assert.That (_modification.NewEndPoint.Definition, Is.EqualTo (_removedEndPointStub.Definition));
      Assert.That (_modification.ModifiedCollection, Is.SameAs (_collectionEndPoint.OppositeDomainObjects));
      Assert.That (_modification.ModifiedCollectionData, Is.SameAs (_collectionDataMock));
    }

    [Test]
    public void Begin ()
    {
      bool relationChangingCalled = false;
      bool relationChangedCalled = false;

      _affectedObject.RelationChanging += (sender, args) =>
      {
        relationChangingCalled = true;

        Assert.That (args.PropertyName, Is.EqualTo (_collectionEndPoint.PropertyName));
        Assert.That (args.NewRelatedObject, Is.Null);
        Assert.That (args.OldRelatedObject, Is.SameAs (_removedRelatedObject));

        Assert.That (_oppositeDomainObjectsEventReceiver.RemovingDomainObjects, Is.EqualTo (new[] { _removedRelatedObject })); // collection got event first
      };
      _affectedObject.RelationChanged += (sender, args) => relationChangedCalled = true;
      
      _modification.Begin ();

      Assert.That (relationChangingCalled, Is.True); // operation was started
      Assert.That (relationChangedCalled, Is.False); // operation was not finished
      Assert.That (_oppositeDomainObjectsEventReceiver.RemovedDomainObjects, Is.Empty); // operation was not finished
    }

    [Test]
    public void End ()
    {
      bool relationChangingCalled = false;
      bool relationChangedCalled = false;

      _affectedObject.RelationChanging += (sender, args) => relationChangingCalled = true;
      _affectedObject.RelationChanged += (sender, args) =>
      {
        relationChangedCalled = true;

        Assert.That (args.PropertyName, Is.EqualTo (_collectionEndPoint.PropertyName));
        Assert.That (_oppositeDomainObjectsEventReceiver.RemovedDomainObjects, Is.EqualTo (new[] { _removedRelatedObject })); // collection got event first
      };

      _modification.End ();

      Assert.That (relationChangingCalled, Is.False); // operation was not started
      Assert.That (relationChangedCalled, Is.True); // operation was finished
      Assert.That (_oppositeDomainObjectsEventReceiver.RemovingDomainObjects, Is.Empty); // operation was not started
    }

    [Test]
    public void Perform ()
    {
      bool relationChangingCalled = false;
      bool relationChangedCalled = false;

      _affectedObject.RelationChanging += (sender, args) => relationChangingCalled = true;
      _affectedObject.RelationChanged += (sender, args) => relationChangedCalled = true;

      _collectionDataMock.BackToRecord();
      _collectionDataMock.Expect (mock => mock.Remove (_removedRelatedObject.ID));
      _collectionDataMock.Replay ();
      
      _modification.Perform();

      _collectionDataMock.VerifyAllExpectations ();

      Assert.That (relationChangingCalled, Is.False); // operation was not started
      Assert.That (relationChangedCalled, Is.False); // operation was not finished
      Assert.That (_oppositeDomainObjectsEventReceiver.RemovingDomainObjects, Is.Empty); // operation was not started
      Assert.That (_oppositeDomainObjectsEventReceiver.RemovedDomainObjects, Is.Empty); // operation was not finished
    }

  }
}
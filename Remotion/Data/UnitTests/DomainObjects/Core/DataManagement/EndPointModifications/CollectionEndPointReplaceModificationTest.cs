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
  public class CollectionEndPointReplaceModificationTest : CollectionEndPointModificationTestBase
  {
    private CollectionEndPointReplaceModification _modification;
    private Order _replacedRelatedObject;
    private Order _replacementRelatedObject;

    public override void SetUp ()
    {
      base.SetUp();

      _replacedRelatedObject = Order.GetObject (DomainObjectIDs.Order1);
      _replacementRelatedObject = Order.GetObject (DomainObjectIDs.Order2);

      _modification = 
          new CollectionEndPointReplaceModification (CollectionEndPoint, _replacedRelatedObject, _replacementRelatedObject, CollectionDataMock);
    }

    [Test]
    public void Initialization ()
    {
      Assert.That (_modification.ModifiedEndPoint, Is.SameAs (CollectionEndPoint));
      Assert.That (_modification.OldRelatedObject, Is.SameAs (_replacedRelatedObject));
      Assert.That (_modification.NewRelatedObject, Is.SameAs (_replacementRelatedObject));
      Assert.That (_modification.ModifiedCollection, Is.SameAs (CollectionEndPoint.OppositeDomainObjects));
      Assert.That (_modification.ModifiedCollectionData, Is.SameAs (CollectionDataMock));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Modified end point is null, a NullEndPointModification is needed.\r\n"
                                                                      + "Parameter name: modifiedEndPoint")]
    public void Initialization_FromNullEndPoint ()
    {
      var endPoint = new NullCollectionEndPoint (RelationEndPointID.Definition);
      new CollectionEndPointReplaceModification (endPoint, _replacedRelatedObject, _replacementRelatedObject, CollectionDataMock);
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
        Assert.That (args.NewRelatedObject, Is.SameAs (_replacementRelatedObject));
        Assert.That (args.OldRelatedObject, Is.SameAs (_replacedRelatedObject));

        Assert.That (CollectionEventReceiver.RemovingDomainObjects, Is.EqualTo (new[] { _replacedRelatedObject })); // collection got event first
        Assert.That (CollectionEventReceiver.AddingDomainObject, Is.SameAs (_replacementRelatedObject)); // collection got event first
      };
      DomainObject.RelationChanged += (sender, args) => relationChangedCalled = true;

      _modification.Begin ();

      Assert.That (relationChangingCalled, Is.True); // operation was started
      Assert.That (relationChangedCalled, Is.False); // operation was not finished
      Assert.That (CollectionEventReceiver.AddedDomainObject, Is.Null); // operation was not finished
      Assert.That (CollectionEventReceiver.RemovedDomainObjects, Is.Empty); // operation was not finished
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
        Assert.That (CollectionEventReceiver.RemovedDomainObjects, Is.EqualTo (new[] { _replacedRelatedObject })); // collection got event first
        Assert.That (CollectionEventReceiver.AddedDomainObject, Is.SameAs (_replacementRelatedObject)); // collection got event first
      };

      _modification.End ();

      Assert.That (relationChangingCalled, Is.False); // operation was not started
      Assert.That (relationChangedCalled, Is.True); // operation was finished
      Assert.That (CollectionEventReceiver.RemovingDomainObjects, Is.Empty); // operation was not started
      Assert.That (CollectionEventReceiver.AddingDomainObject, Is.Null); // operation was not started
    }

    [Test]
    public void Perform ()
    {
      bool relationChangingCalled = false;
      bool relationChangedCalled = false;

      DomainObject.RelationChanging += (sender, args) => relationChangingCalled = true;
      DomainObject.RelationChanged += (sender, args) => relationChangedCalled = true;

      CollectionDataMock.BackToRecord ();
      CollectionDataMock.Expect (mock => mock.Replace(_replacedRelatedObject.ID, _replacementRelatedObject));
      CollectionDataMock.Replay ();

      _modification.Perform ();

      CollectionDataMock.VerifyAllExpectations ();

      Assert.That (relationChangingCalled, Is.False); // operation was not started
      Assert.That (relationChangedCalled, Is.False); // operation was not finished
      Assert.That (CollectionEventReceiver.AddingDomainObject, Is.Null); // operation was not started
      Assert.That (CollectionEventReceiver.AddedDomainObject, Is.Null); // operation was not finished
      Assert.That (CollectionEventReceiver.RemovingDomainObjects, Is.Empty); // operation was not started
      Assert.That (CollectionEventReceiver.RemovedDomainObjects, Is.Empty); // operation was not finished
      Assert.That (CollectionEndPoint.HasBeenTouched, Is.True);
    }

    [Test]
    public void CreateBidirectionalModification ()
    {
      var bidirectionalModification = _modification.CreateBidirectionalModification ();
      Assert.That (bidirectionalModification, Is.InstanceOfType (typeof (NotifyingBidirectionalRelationModification)));

      // DomainObject.Orders[indexof (_replacedRelatedObject)] = _replacementRelatedObject
      var steps = bidirectionalModification.GetModificationSteps ();
      Assert.That (steps.Count, Is.EqualTo (4));

      var oldCustomer = _replacementRelatedObject.Customer;

      // DomainObject.Orders[...].Customer = null
      Assert.That (steps[0], Is.InstanceOfType (typeof (ObjectEndPointSetModification)));
      Assert.That (steps[0].ModifiedEndPoint.ID.PropertyName, Is.EqualTo (typeof (Order).FullName + ".Customer"));
      Assert.That (steps[0].ModifiedEndPoint.ID.ObjectID, Is.EqualTo (_replacedRelatedObject.ID));
      Assert.That (steps[0].OldRelatedObject, Is.SameAs (DomainObject));
      Assert.That (steps[0].NewRelatedObject, Is.Null);

      // _replacementRelatedObject.Customer = DomainObject
      Assert.That (steps[1], Is.InstanceOfType (typeof (ObjectEndPointSetModification)));
      Assert.That (steps[1].ModifiedEndPoint.ID.PropertyName, Is.EqualTo (typeof (Order).FullName + ".Customer"));
      Assert.That (steps[1].ModifiedEndPoint.ID.ObjectID, Is.EqualTo (_replacementRelatedObject.ID));
      Assert.That (steps[1].OldRelatedObject, Is.SameAs (oldCustomer));
      Assert.That (steps[1].NewRelatedObject, Is.SameAs (DomainObject));

      // DomainObject.Orders[...] = _replacementRelatedObject
      Assert.That (steps[2], Is.SameAs (_modification));

      // oldCustomer.Orders.Remove (_replacementRelatedObject)
      Assert.That (steps[3], Is.InstanceOfType (typeof (CollectionEndPointRemoveModification)));
      Assert.That (steps[3].ModifiedEndPoint.ID.PropertyName, Is.EqualTo (typeof (Customer).FullName + ".Orders"));
      Assert.That (steps[3].ModifiedEndPoint.ID.ObjectID, Is.EqualTo (oldCustomer.ID));
      Assert.That (steps[3].OldRelatedObject, Is.SameAs (_replacementRelatedObject));
    }
  }
}
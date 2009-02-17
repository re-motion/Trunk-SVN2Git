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
using Rhino.Mocks;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.EndPointModifications
{
  [TestFixture]
  public class CollectionEndPointRemoveModificationTest : CollectionEndPointModificationTestBase
  {
    private Order _removedRelatedObject;
    private CollectionEndPointRemoveModification _modification;

    public override void SetUp ()
    {
      base.SetUp();
      _removedRelatedObject = Order.GetObject (DomainObjectIDs.Order1);
      _modification = new CollectionEndPointRemoveModification (CollectionEndPoint, _removedRelatedObject, CollectionDataMock);
    }

    [Test]
    public void Initialization ()
    {
      Assert.That (_modification.ModifiedEndPoint, Is.SameAs (CollectionEndPoint));
      Assert.That (_modification.OldRelatedObject, Is.SameAs (_removedRelatedObject));
      Assert.That (_modification.NewRelatedObject, Is.Null);
      Assert.That (_modification.ModifiedCollection, Is.SameAs (CollectionEndPoint.OppositeDomainObjects));
      Assert.That (_modification.ModifiedCollectionData, Is.SameAs (CollectionDataMock));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Modified end point is null, a NullEndPointModification is needed.\r\n"
                                                                      + "Parameter name: modifiedEndPoint")]
    public void Initialization_FromNullEndPoint ()
    {
      var endPoint = new NullCollectionEndPoint (RelationEndPointID.Definition);
      new CollectionEndPointRemoveModification (endPoint, _removedRelatedObject, CollectionDataMock);
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
        Assert.That (args.OldRelatedObject, Is.SameAs (_removedRelatedObject));

        Assert.That (CollectionEventReceiver.RemovingDomainObjects, Is.EqualTo (new[] { _removedRelatedObject }));
        // collection got event first
      };
      DomainObject.RelationChanged += (sender, args) => relationChangedCalled = true;

      _modification.Begin();

      Assert.That (relationChangingCalled, Is.True); // operation was started
      Assert.That (relationChangedCalled, Is.False); // operation was not finished
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
        Assert.That (CollectionEventReceiver.RemovedDomainObjects, Is.EqualTo (new[] { _removedRelatedObject }));
        // collection got event first
      };

      _modification.End();

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

      CollectionDataMock.BackToRecord();
      CollectionDataMock.Expect (mock => mock.Remove (_removedRelatedObject.ID));
      CollectionDataMock.Replay();

      _modification.Perform();

      CollectionDataMock.VerifyAllExpectations();

      Assert.That (relationChangingCalled, Is.False); // operation was not started
      Assert.That (relationChangedCalled, Is.False); // operation was not finished
      Assert.That (CollectionEventReceiver.RemovingDomainObjects, Is.Empty); // operation was not started
      Assert.That (CollectionEventReceiver.RemovedDomainObjects, Is.Empty); // operation was not finished
      Assert.That (CollectionEndPoint.HasBeenTouched, Is.True);
    }

    [Test]
    public void CreateBidirectionalModification ()
    {
      // DomainObject.Orders.Remove (_removedRelatedObject)
      var modification = _modification.CreateBidirectionalModification ().GetEndPointModifications ();
      Assert.That (modification.Count, Is.EqualTo (2));

      // _removedRelatedObject.Customer = null
      Assert.That (modification[0], Is.InstanceOfType (typeof (ObjectEndPointSetModification)));
      Assert.That (modification[0].OldRelatedObject, Is.SameAs (DomainObject));
      Assert.That (modification[0].NewRelatedObject, Is.Null);

      // DomainObject.Orders.Remove (_removedRelatedObject)
      Assert.That (modification[1], Is.SameAs (_modification));
    }
  }
}
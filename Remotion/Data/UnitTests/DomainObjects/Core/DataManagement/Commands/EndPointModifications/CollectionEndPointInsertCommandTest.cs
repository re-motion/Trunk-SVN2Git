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
using Remotion.Data.DomainObjects.DataManagement.Commands;
using Remotion.Data.DomainObjects.DataManagement.Commands.EndPointModifications;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.Commands.EndPointModifications
{
  [TestFixture]
  public class CollectionEndPointInsertCommandTest : CollectionEndPointModificationCommandTestBase
  {
    private Order _insertedRelatedObject;
    private CollectionEndPointInsertCommand _command;

    public override void SetUp ()
    {
      base.SetUp();

      _insertedRelatedObject = Order.GetObject (DomainObjectIDs.Order2);
      _command = new CollectionEndPointInsertCommand (CollectionEndPoint, 12, _insertedRelatedObject, CollectionDataMock);
    }

    [Test]
    public void Initialization ()
    {
      Assert.That (_command.ModifiedEndPoint, Is.SameAs (CollectionEndPoint));
      Assert.That (_command.OldRelatedObject, Is.Null);
      Assert.That (_command.NewRelatedObject, Is.SameAs (_insertedRelatedObject));
      Assert.That (_command.Index, Is.EqualTo (12));
      Assert.That (_command.ModifiedCollection, Is.SameAs (CollectionEndPoint.OppositeDomainObjects));
      Assert.That (_command.ModifiedCollectionData, Is.SameAs (CollectionDataMock));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Modified end point is null, a NullEndPointModificationCommand is needed.\r\n"
                                                                      + "Parameter name: modifiedEndPoint")]
    public void Initialization_FromNullEndPoint ()
    {
      var endPoint = new NullCollectionEndPoint (ClientTransactionMock, RelationEndPointID.Definition);
      new CollectionEndPointInsertCommand (endPoint, 0, _insertedRelatedObject, CollectionDataMock);
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

      _command.Begin ();

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

      _command.End ();

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

      _command.Perform ();

      CollectionDataMock.VerifyAllExpectations ();

      Assert.That (relationChangingCalled, Is.False); // operation was not started
      Assert.That (relationChangedCalled, Is.False); // operation was not finished
      Assert.That (CollectionEventReceiver.AddingDomainObject, Is.Null); // operation was not started
      Assert.That (CollectionEventReceiver.AddedDomainObject, Is.Null); // operation was not finished
      Assert.That (CollectionEndPoint.HasBeenTouched, Is.True);
    }

    [Test]
    public void ExtendToAllRelatedObjects ()
    {
      var bidirectionalModification = _command.ExtendToAllRelatedObjects ();
      Assert.That (bidirectionalModification, Is.InstanceOfType (typeof (CompositeDataManagementCommand)));

      // DomainObject.Orders.Insert (_insertedRelatedObject, 12)
      var steps = GetAllCommands (bidirectionalModification);
      Assert.That (steps.Count, Is.EqualTo (3));

      var oldCustomer = _insertedRelatedObject.Customer;

      // _insertedRelatedObject.Customer = DomainObject (previously oldCustomer)
      Assert.That (steps[0], Is.InstanceOfType (typeof (ObjectEndPointSetCommand)));
      Assert.That (steps[0].ModifiedEndPoint.ID.Definition.PropertyName, Is.EqualTo (typeof (Order).FullName + ".Customer"));
      Assert.That (steps[0].ModifiedEndPoint.ID.ObjectID, Is.EqualTo (_insertedRelatedObject.ID));
      Assert.That (steps[0].OldRelatedObject, Is.SameAs (oldCustomer));
      Assert.That (steps[0].NewRelatedObject, Is.SameAs (DomainObject));

      // DomainObject.Orders.Insert (_insertedRelatedObject, 12)
      Assert.That (steps[1], Is.SameAs (_command));

      // oldCustomer.Orders.Remove (_insertedRelatedObject)
      Assert.That (steps[2], Is.InstanceOfType (typeof (CollectionEndPointRemoveCommand)));
      Assert.That (steps[2].ModifiedEndPoint.ID.Definition.PropertyName, Is.EqualTo (typeof (Customer).FullName + ".Orders"));
      Assert.That (steps[2].ModifiedEndPoint.ID.ObjectID, Is.EqualTo (oldCustomer.ID));
      Assert.That (steps[2].OldRelatedObject, Is.SameAs (_insertedRelatedObject));
    }
  }
}

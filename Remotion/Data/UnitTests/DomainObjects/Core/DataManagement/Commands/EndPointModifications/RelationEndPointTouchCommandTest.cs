
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
using Remotion.Data.DomainObjects.DataManagement.Commands.EndPointModifications;
using Remotion.Data.UnitTests.DomainObjects.Core.EventReceiver;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.Commands.EndPointModifications
{
  [TestFixture]
  public class RelationEndPointTouchCommandTest : StandardMappingTest
  {
    private ClientTransactionMock _transaction;
    private RelationEndPoint _endPoint;
    private RelationEndPointTouchCommand _command;

    public override void SetUp ()
    {
      base.SetUp ();

      _transaction = new ClientTransactionMock ();

      var id = new RelationEndPointID (DomainObjectIDs.Order1, typeof (Order).FullName + ".Customer");
      _endPoint = _transaction.Execute (() => RelationEndPointObjectMother.CreateObjectEndPoint (id, null));

      _command = new RelationEndPointTouchCommand (_endPoint);
    }

    [Test]
    public void Initialization ()
    {
      Assert.That (_command.EndPoint, Is.SameAs (_endPoint));
    }

    [Test]
    public void NotifyClientTransactionOfBegin_DoesNothing ()
    {
      ClientTransactionTestHelper.EnsureTransactionThrowsOnEvents (_transaction);

      _command.NotifyClientTransactionOfBegin ();
    }

    [Test]
    public void NotifyClientTransactionOfEnd_DoesNothing ()
    {
      ClientTransactionTestHelper.EnsureTransactionThrowsOnEvents (_transaction);

      _command.NotifyClientTransactionOfEnd ();
    }

    [Test]
    public void Begin ()
    {
      var eventReceiver = new DomainObjectEventReceiver (_endPoint.GetDomainObject());
      _command.Begin ();

      Assert.IsFalse (eventReceiver.HasRelationChangingEventBeenCalled);
      Assert.IsFalse (eventReceiver.HasRelationChangedEventBeenCalled);
    }

    [Test]
    public void End ()
    {
      var eventReceiver = new DomainObjectEventReceiver (_endPoint.GetDomainObject ());

      _command.End ();

      Assert.IsFalse (eventReceiver.HasRelationChangingEventBeenCalled);
      Assert.IsFalse (eventReceiver.HasRelationChangedEventBeenCalled);
    }

    [Test]
    public void Perform ()
    {
      Assert.That (_endPoint.HasBeenTouched, Is.False);

      _command.Perform ();

      Assert.That (_endPoint.HasBeenTouched, Is.True);
    }

    [Test]
    public void ExpandToAllRelatedObjects ()
    {
      var result = ((IDataManagementCommand) _command).ExpandToAllRelatedObjects ();
      Assert.That (result.GetNestedCommands(), Is.EqualTo (new[] { _command }));
    }
  }
}

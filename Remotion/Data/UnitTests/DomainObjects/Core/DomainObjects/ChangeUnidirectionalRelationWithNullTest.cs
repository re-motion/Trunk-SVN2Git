// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using Remotion.Data.UnitTests.DomainObjects.Core.EventReceiver;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DomainObjects
{
  [TestFixture]
  public class ChangeUnidirectionalRelationWithNullTest : ClientTransactionBaseTest
  {
    [Test]
    public void SetRelatedObjectwithNewNullObject ()
    {
      Client oldClient = Client.GetObject (DomainObjectIDs.Client1);
      Location location = Location.GetObject (DomainObjectIDs.Location1);
      Assert.AreSame (oldClient, location.Client);

      location.Client = null;

      Assert.IsNull (location.Client);
      Assert.IsNull (location.Properties[typeof (Location), "Client"].GetRelatedObjectID());
      Assert.AreEqual (StateType.Changed, location.State);
      Assert.AreEqual (StateType.Unchanged, oldClient.State);
    }

    [Test]
    public void SetRelatedObjectWithOldNullObject ()
    {
      Client client = Client.GetObject (DomainObjectIDs.Client4);
      Client newClient = Client.GetObject (DomainObjectIDs.Client1);

      client.ParentClient = newClient;

      Assert.AreSame (newClient, client.ParentClient);
      Assert.AreEqual (newClient.ID, client.Properties[typeof (Client), "ParentClient"].GetRelatedObjectID ());
      Assert.AreEqual (StateType.Changed, client.State);
      Assert.AreEqual (StateType.Unchanged, newClient.State);
    }

    [Test]
    public void SetRelatedObjectWithOldAndNewNullObject ()
    {
      Client client = Client.GetObject (DomainObjectIDs.Client4);
      SequenceEventReceiver eventReceiver = new SequenceEventReceiver (client);

      client.ParentClient = null;

      eventReceiver.Check (new ChangeState[0]);
      Assert.IsNull (client.ParentClient);
      Assert.IsNull (client.Properties[typeof (Client), "ParentClient"].GetRelatedObjectID ());
      Assert.AreEqual (StateType.Unchanged, client.State);
    }
  }
}

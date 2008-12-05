// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
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
      Assert.IsNull (location.InternalDataContainer["Remotion.Data.UnitTests.DomainObjects.TestDomain.Location.Client"]);
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
			Assert.AreEqual (newClient.ID, client.InternalDataContainer["Remotion.Data.UnitTests.DomainObjects.TestDomain.Client.ParentClient"]);
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
			Assert.IsNull (client.InternalDataContainer["Remotion.Data.UnitTests.DomainObjects.TestDomain.Client.ParentClient"]);
      Assert.AreEqual (StateType.Unchanged, client.State);
    }
  }
}

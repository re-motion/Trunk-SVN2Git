using System;
using NUnit.Framework;
using Remotion.Data.DomainObjects.UnitTests.EventReceiver;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.DomainObjects
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
      Assert.IsNull (location.InternalDataContainer["Remotion.Data.DomainObjects.UnitTests.TestDomain.Location.Client"]);
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
			Assert.AreEqual (newClient.ID, client.InternalDataContainer["Remotion.Data.DomainObjects.UnitTests.TestDomain.Client.ParentClient"]);
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
			Assert.IsNull (client.InternalDataContainer["Remotion.Data.DomainObjects.UnitTests.TestDomain.Client.ParentClient"]);
      Assert.AreEqual (StateType.Unchanged, client.State);
    }
  }
}

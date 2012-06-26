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
using System.Runtime.Serialization;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.UnitTests.DomainObjects.Factories;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Serialization
{
  [TestFixture]
  public class DataContainerTest : ClientTransactionBaseTest
  {
    [Test]
    [ExpectedException (typeof (SerializationException), ExpectedMessage = "Type 'Remotion.Data.DomainObjects.DataManagement.DataContainer' in Assembly "
        + ".* is not marked as serializable.", MatchType = MessageMatch.Regex)]
    public void DataContainerIsNotSerializable ()
    {
      var objectID = new ObjectID ("Customer", Guid.NewGuid ());
      DataContainer dataContainer = DataContainer.CreateNew (objectID);

      Serializer.SerializeAndDeserialize (dataContainer);
    }

    [Test]
    public void DataContainerIsFlattenedSerializable ()
    {
      var objectID = new ObjectID ("Customer", Guid.NewGuid ());
      DataContainer dataContainer = DataContainer.CreateNew (objectID);
      DataContainer deserializedDataContainer = FlattenedSerializer.SerializeAndDeserialize (dataContainer);
      Assert.AreNotSame (dataContainer, deserializedDataContainer);
      Assert.AreEqual (dataContainer.ID, deserializedDataContainer.ID);
    }

    [Test]
    public void DataContainer_Contents ()
    {
      Employee employee = Employee.GetObject (DomainObjectIDs.Employee3);

      Computer computer = employee.Computer;
      computer.SerialNumber = "abc";

      DataContainer dataContainer = computer.InternalDataContainer;
      DataContainer deserializedDataContainer = FlattenedSerializer.SerializeAndDeserialize (dataContainer);

      Assert.AreEqual (dataContainer.ID, deserializedDataContainer.ID);
      Assert.IsNotNull (deserializedDataContainer.ClientTransaction);
      Assert.IsNotNull (deserializedDataContainer.EventListener);
      Assert.AreEqual (dataContainer.Timestamp, deserializedDataContainer.Timestamp);
      Assert.IsNotNull (deserializedDataContainer.DomainObject);
      Assert.AreEqual (dataContainer.DomainObject.ID, deserializedDataContainer.DomainObject.ID);
      Assert.AreEqual (StateType.Changed, deserializedDataContainer.State);
      Assert.AreEqual ("abc", GetPropertyValue (deserializedDataContainer, typeof (Computer), "SerialNumber"));
      Assert.AreEqual (employee.ID, GetPropertyValue (deserializedDataContainer, typeof (Computer), "Employee"));
    }

    [Test]
    public void DataContainer_MarkAsChanged_Contents()
    {
      Employee employee = Employee.GetObject (DomainObjectIDs.Employee3);

      Computer computer = employee.Computer;
      computer.MarkAsChanged ();

      DataContainer dataContainer = computer.InternalDataContainer;
      DataContainer deserializedDataContainer = FlattenedSerializer.SerializeAndDeserialize (dataContainer);

      Assert.AreEqual (dataContainer.ID, deserializedDataContainer.ID);
      Assert.AreEqual (StateType.Changed, deserializedDataContainer.State);
    }

    [Test]
    public void DataContainer_WithoutProperties_Contents ()
    {
      var objectID = new ObjectID (typeof (ClassWithoutProperties), Guid.NewGuid ());
      DataContainer dataContainer = DataContainer.CreateNew (objectID);
      DataContainer deserializedDataContainer = FlattenedSerializer.SerializeAndDeserialize (dataContainer);

      Assert.AreEqual (dataContainer.ID, deserializedDataContainer.ID);
    }

    [Test]
    public void DataContainer_Discarded_Contents ()
    {
      Computer computer = Computer.NewObject ();
      DataContainer dataContainer = computer.InternalDataContainer;
      computer.Delete ();
      Assert.IsTrue (dataContainer.IsDiscarded);

      DataContainer deserializedDataContainer = FlattenedSerializer.SerializeAndDeserialize (dataContainer);
      Assert.IsTrue (deserializedDataContainer.IsDiscarded);
      Assert.AreEqual (StateType.Invalid, deserializedDataContainer.State);
    }
  }
}

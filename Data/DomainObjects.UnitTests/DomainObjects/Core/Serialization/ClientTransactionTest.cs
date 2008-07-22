/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using System.Runtime.Serialization;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Serialization
{
  [TestFixture]
  public class ClientTransactionTest : SerializationBaseTest
  {
    [Test]
    public void ObjectIDTest ()
    {
      ObjectID objectID = new ObjectID ("Company", Guid.NewGuid ());

      ObjectID deserializedObjectID = (ObjectID) SerializeAndDeserialize (objectID);

      Assert.AreEqual (objectID, deserializedObjectID);
      Assert.AreEqual (objectID.Value.GetType (), deserializedObjectID.Value.GetType ());
      Assert.AreSame (objectID.ClassDefinition, deserializedObjectID.ClassDefinition);
      Assert.AreSame (MappingConfiguration.Current.ClassDefinitions.GetMandatory ("Company"), deserializedObjectID.ClassDefinition);
    }

    [Test]
    [ExpectedException (typeof (SerializationException), ExpectedMessage = "not marked as serializable", MatchType = MessageMatch.Contains)]
    public void PropertyValueTest ()
    {
      ClassDefinition classDefinition = MappingConfiguration.Current.ClassDefinitions["Customer"];
      PropertyDefinition propertyDefinition = classDefinition["Remotion.Data.UnitTests.DomainObjects.TestDomain.Customer.CustomerSince"];
      PropertyValue value = new PropertyValue (propertyDefinition);

      SerializeAndDeserialize (value);
    }

    [Test]
    [ExpectedException (typeof (SerializationException), ExpectedMessage = "not marked as serializable", MatchType = MessageMatch.Contains)]
    public void PropertyValueCollection ()
    {
      ClassDefinition classDefinition = MappingConfiguration.Current.ClassDefinitions["Customer"];
      PropertyDefinition propertyDefinition = classDefinition["Remotion.Data.UnitTests.DomainObjects.TestDomain.Customer.CustomerSince"];
      PropertyValue value = new PropertyValue (propertyDefinition);

      PropertyValueCollection collection = new PropertyValueCollection ();

      collection.Add (value);

      SerializeAndDeserialize (collection);
    }

    

    [Test]
    public void DomainObjectTest ()
    {
      DomainObject domainObject = Customer.GetObject (DomainObjectIDs.Customer1);

      DomainObject deserializedDomainObject = (DomainObject) SerializeAndDeserialize (domainObject);

      Assert.AreEqual (domainObject.ID, deserializedDomainObject.ID);
    }

    [Test]
    public void DomainObject_IDeserializationCallbackTest ()
    {
      Customer domainObject = Customer.GetObject (DomainObjectIDs.Customer1);

      Customer deserializedDomainObject = Serializer.SerializeAndDeserialize (domainObject);
      Assert.IsTrue (deserializedDomainObject.OnDeserializationCalled);
    }

    [Test]
    public void DomainObject_DeserializationCallbackAttributesTest ()
    {
      Customer domainObject = Customer.GetObject (DomainObjectIDs.Customer1);

      Customer deserializedDomainObject = Serializer.SerializeAndDeserialize (domainObject);
      Assert.IsTrue (deserializedDomainObject.OnDeserializingAttributeCalled);
      Assert.IsTrue (deserializedDomainObject.OnDeserializedAttributeCalled);
    }

    [Test]
    public void DomainObjectCollectionTest ()
    {
      Customer customer = Customer.GetObject (DomainObjectIDs.Customer1);

      DomainObjectCollection collection = new DomainObjectCollection (typeof (Customer));
      collection.Add (customer);

      DomainObjectCollection deserializedCollection = (DomainObjectCollection) SerializeAndDeserialize (collection);

      Assert.AreEqual (collection.Count, deserializedCollection.Count);
    }

    [Test]
    public void DataManagerTest ()
    {
      DataManager dataManager = new DataManager (ClientTransactionScope.CurrentTransaction);

      DataManager deserializedDataManager = (DataManager) SerializeAndDeserialize (dataManager);

      Assert.IsNotNull (deserializedDataManager);
    }

    [Test]
    public void QueryManagerTest ()
    {
      RootQueryManager queryManager = new RootQueryManager ((RootClientTransaction) ClientTransactionScope.CurrentTransaction);

      RootQueryManager deserializedQueryManager = (RootQueryManager) SerializeAndDeserialize (queryManager);

      Assert.IsNotNull (deserializedQueryManager);
    }

    [Test]
    public void ClientTransactionSerializationTest ()
    {
      ClientTransaction clientTransaction = ClientTransaction.NewRootTransaction();

      ClientTransaction deserializedClientTransaction = (ClientTransaction) SerializeAndDeserialize (clientTransaction);

      Assert.IsNotNull (deserializedClientTransaction);
    }

    [Test]
    public void SubClientTransactionSerializationTest ()
    {
      ClientTransaction clientTransaction = ClientTransaction.NewRootTransaction ().CreateSubTransaction();

      ClientTransaction deserializedClientTransaction = (ClientTransaction) SerializeAndDeserialize (clientTransaction);

      Assert.IsNotNull (deserializedClientTransaction);
      Assert.IsNotNull (deserializedClientTransaction.ParentTransaction);
    }
  }
}

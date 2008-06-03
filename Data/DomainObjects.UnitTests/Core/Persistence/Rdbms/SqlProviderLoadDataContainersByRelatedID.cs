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
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.UnitTests.Factories;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.Core.Persistence.Rdbms
{
  [TestFixture]
  public class SqlProviderLoadDataContainersByRelatedID : SqlProviderBaseTest
  {
    [Test]
    public void Loading ()
    {
      DataContainerCollection collection = Provider.LoadDataContainersByRelatedID (
          TestMappingConfiguration.Current.ClassDefinitions[typeof (Order)],
          "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer",
          DomainObjectIDs.Customer1);

      Assert.IsNotNull (collection);
      Assert.AreEqual (2, collection.Count, "DataContainerCollection.Count");
      Assert.IsNotNull (collection[DomainObjectIDs.Order1], "ID of Order with OrdnerNo 1");
      Assert.IsNotNull (collection[DomainObjectIDs.OrderWithoutOrderItem], "ID of Order with OrdnerNo 2");
    }

    [Test]
    public void LoadOverInheritedProperty ()
    {
      DataContainer personContainer = Provider.LoadDataContainer (DomainObjectIDs.Person6);

      DataContainerCollection collection = Provider.LoadDataContainersByRelatedID (
          TestMappingConfiguration.Current.ClassDefinitions[typeof (Distributor)],
          "Remotion.Data.DomainObjects.UnitTests.TestDomain.Partner.ContactPerson",
          DomainObjectIDs.Person6);

      Assert.AreEqual (1, collection.Count);
      Assert.AreEqual (DomainObjectIDs.Distributor2, collection[0].ID);
    }

    [Test]
    public void LoadWithOrderBy ()
    {
      ClassDefinition orderDefinition = TestMappingConfiguration.Current.ClassDefinitions["Order"];

      DataContainerCollection orderContainers = Provider.LoadDataContainersByRelatedID (
          orderDefinition, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer", DomainObjectIDs.Customer1);

      Assert.AreEqual (2, orderContainers.Count);
      Assert.AreEqual (DomainObjectIDs.Order1, orderContainers[0].ID);
      Assert.AreEqual (DomainObjectIDs.OrderWithoutOrderItem, orderContainers[1].ID);
    }

    [Test]
    public void LoadDataContainersByRelatedIDOfDifferentStorageProvider ()
    {
      ClassDefinition orderDefinition = TestMappingConfiguration.Current.ClassDefinitions["Order"];

      DataContainerCollection orderContainers = Provider.LoadDataContainersByRelatedID (orderDefinition, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Official", DomainObjectIDs.Official1);
      Assert.IsNotNull (orderContainers);
      Assert.AreEqual (5, orderContainers.Count);
    }
  }
}

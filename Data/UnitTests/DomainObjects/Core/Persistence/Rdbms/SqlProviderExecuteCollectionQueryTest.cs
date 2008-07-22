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
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.Queries.Configuration;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms
{
  [TestFixture]
  public class SqlProviderExecuteCollectionQueryTest : SqlProviderBaseTest
  {
    [Test]
    public void ExecuteCollectionQuery ()
    {
      Query query = new Query ("OrderQuery");
      query.Parameters.Add ("@customerID", DomainObjectIDs.Customer1.Value);

      DataContainerCollection orderContainers = Provider.ExecuteCollectionQuery (query);

      Assert.IsNotNull (orderContainers);
      Assert.IsTrue (orderContainers.Contains (DomainObjectIDs.Order1));
      Assert.IsTrue (orderContainers.Contains (DomainObjectIDs.OrderWithoutOrderItem));
    }

    [Test]
    public void AllDataTypes ()
    {
      Query query = new Query ("QueryWithAllDataTypes");
      query.Parameters.Add ("@boolean", false);
      query.Parameters.Add ("@byte", (byte) 85);
      query.Parameters.Add ("@date", new DateTime (2005, 1, 1));
      query.Parameters.Add ("@dateTime", new DateTime (2005, 1, 1, 17, 0, 0));
      query.Parameters.Add ("@decimal", (decimal) 123456.789);
      query.Parameters.Add ("@doubleLowerBound", 987654D);
      query.Parameters.Add ("@doubleUpperBound", 987655D);
      query.Parameters.Add ("@enum", ClassWithAllDataTypes.EnumType.Value1);
      query.Parameters.Add ("@guid", new Guid ("{236C2DCE-43BD-45ad-BDE6-15F8C05C4B29}"));
      query.Parameters.Add ("@int16", (short) 32767);
      query.Parameters.Add ("@int32", 2147483647);
      query.Parameters.Add ("@int64", (long) 9223372036854775807);
      query.Parameters.Add ("@singleLowerBound", (float) 6789);
      query.Parameters.Add ("@singleUpperBound", (float) 6790);
      query.Parameters.Add ("@string", "abcdeföäü");

      query.Parameters.Add ("@naBoolean", true);
      query.Parameters.Add ("@naByte", (byte) 78);
      query.Parameters.Add ("@naDate", new DateTime (2005, 2, 1));
      query.Parameters.Add ("@naDateTime", new DateTime (2005, 2, 1, 5, 0, 0));
      query.Parameters.Add ("@naDecimal", 765.098m);
      query.Parameters.Add ("@naDoubleLowerBound", 654321D);
      query.Parameters.Add ("@naDoubleUpperBound", 654322D);
      query.Parameters.Add ("@naEnum", ClassWithAllDataTypes.EnumType.Value2);
      query.Parameters.Add ("@naGuid", new Guid ("{19B2DFBE-B7BB-448e-8002-F4DBF6032AE8}"));
      query.Parameters.Add ("@naInt16", (short) 12000);
      query.Parameters.Add ("@naInt32", -2147483647);
      query.Parameters.Add ("@naInt64", 3147483647L);
      query.Parameters.Add ("@naSingleLowerBound", 12F);
      query.Parameters.Add ("@naSingleUpperBound", 13F);

      query.Parameters.Add ("@naBooleanWithNullValue", null);
      query.Parameters.Add ("@naByteWithNullValue", null);
      query.Parameters.Add ("@naDateWithNullValue", null);
      query.Parameters.Add ("@naDateTimeWithNullValue", null);
      query.Parameters.Add ("@naDecimalWithNullValue", null);
      query.Parameters.Add ("@naEnumWithNullValue", null);
      query.Parameters.Add ("@naDoubleWithNullValue", null);
      query.Parameters.Add ("@naGuidWithNullValue", null);
      query.Parameters.Add ("@naInt16WithNullValue", null);
      query.Parameters.Add ("@naInt32WithNullValue", null);
      query.Parameters.Add ("@naInt64WithNullValue", null);
      query.Parameters.Add ("@naSingleWithNullValue", null);
      query.Parameters.Add ("@stringWithNullValue", null);

      DataContainerCollection actualContainers = Provider.ExecuteCollectionQuery (query);

      Assert.IsNotNull (actualContainers);
      Assert.AreEqual (1, actualContainers.Count);

      DataContainer expectedContainer = TestDataContainerFactory.CreateClassWithAllDataTypesDataContainer ();
      DataContainerChecker checker = new DataContainerChecker ();
      checker.Check (expectedContainer, actualContainers[0]);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Expected query type is 'Collection', but was 'Scalar'.\r\nParameter name: query")]
    public void ScalarQuery ()
    {
      Provider.ExecuteCollectionQuery (new Query ("OrderNoSumByCustomerNameQuery"));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException))]
    public void DifferentStorageProviderID ()
    {
      QueryDefinition definition = new QueryDefinition (
          "QueryWithDifferentStorageProviderID",
          "DifferentStorageProviderID",
          "select 42",
          QueryType.Collection);

      Provider.ExecuteCollectionQuery (new Query (definition));
    }

    [Test]
    public void ObjectIDParameter ()
    {
      Query query = new Query ("OrderQuery");
      query.Parameters.Add ("@customerID", DomainObjectIDs.Customer1);

      DataContainerCollection orderContainers = Provider.ExecuteCollectionQuery (query);

      Assert.IsNotNull (orderContainers);
      Assert.IsTrue (orderContainers.Contains (DomainObjectIDs.Order1));
      Assert.IsTrue (orderContainers.Contains (DomainObjectIDs.OrderWithoutOrderItem));
    }

    [Test]
    public void ObjectIDOfDifferentStorageProvider ()
    {
      Query query = new Query ("OrderByOfficialQuery");
      query.Parameters.Add ("@officialID", DomainObjectIDs.Official1);

      DataContainerCollection orderContainers = Provider.ExecuteCollectionQuery (query);

      Assert.IsNotNull (orderContainers);
      Assert.AreEqual (5, orderContainers.Count);
      Assert.IsTrue (orderContainers.Contains (DomainObjectIDs.Order1));
      Assert.IsTrue (orderContainers.Contains (DomainObjectIDs.Order2));
      Assert.IsTrue (orderContainers.Contains (DomainObjectIDs.Order3));
      Assert.IsTrue (orderContainers.Contains (DomainObjectIDs.Order4));
      Assert.IsTrue (orderContainers.Contains (DomainObjectIDs.OrderWithoutOrderItem));

    }
  }
}

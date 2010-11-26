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
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Configuration;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.Queries.Configuration;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Factories
{
  public static class TestQueryFactory
  {
    public static QueryDefinition CreateOrderQueryWithCustomCollectionType()
    {
      return new QueryDefinition (
          "OrderQueryWithCustomCollectionType",
          "TestDomain",
          "select [Order].* from [Order] inner join [Company] where [Company].[ID] = @customerID order by [OrderNo] asc;",
          QueryType.Collection,
          typeof (OrderCollection));
    }

    public static QueryDefinition CreateOrderQueryDefinitionWithObjectListOfOrder ()
    {
      return new QueryDefinition (
          "OrderQueryWithObjectListOfOrder",
          "TestDomain",
          "select [Order].* from [Order] inner join [Company] where [Company].[ID] = @customerID order by [OrderNo] asc;",
          QueryType.Collection,
          typeof (ObjectList<Order>));
    }

    public static QueryDefinition CreateCustomerTypeQueryDefinition()
    {
      return new QueryDefinition (
          "CustomerTypeQuery",
          "TestDomain",
          "select [Company].* from [Company] where [CustomerType] = @customerType order by [Name] asc;",
          QueryType.Collection,
          typeof (DomainObjectCollection));
    }

    public static QueryDefinition CreateOrderSumQueryDefinition()
    {
      return new QueryDefinition (
          "OrderSumQuery",
          "TestDomain",
          "select sum(quantity) from [Order] where [CustomerID] = @customerID;",
          QueryType.Scalar);
    }

    public static QueryResult<T> CreateTestQueryResult<T> () where T : DomainObject
    {
      var collection = new T[0];
      return CreateTestQueryResult(collection);
    }

    public static QueryResult<T> CreateTestQueryResult<T> (T[] collection) where T: DomainObject
    {
      var classDefinition = MappingConfiguration.Current.ClassDefinitions[typeof (T)];
      var storageProviderID = classDefinition != null
                                  ? classDefinition.StorageEntityDefinition.StorageProviderDefinition.Name
                                  : DomainObjectsConfiguration.Current.Storage.DefaultStorageProviderDefinition.Name;
      var query = QueryFactory.CreateCollectionQuery ("test", storageProviderID, "TEST", new QueryParameterCollection (), typeof (DomainObjectCollection));
      return CreateTestQueryResult (query, collection);
    }

    public static QueryResult<T> CreateTestQueryResult<T> (IQuery query) where T : DomainObject
    {
      var collection = new T[0];
      return CreateTestQueryResult (query, collection);
    }

    public static QueryResult<T> CreateTestQueryResult<T> (IQuery query, T[] collection) where T : DomainObject
    {
      return new QueryResult<T> (query, collection);
    }
  }
}

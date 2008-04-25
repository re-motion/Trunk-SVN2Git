using System;
using Remotion.Data.DomainObjects.Queries.Configuration;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.Factories
{
  public static class QueryFactory
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
  }
}
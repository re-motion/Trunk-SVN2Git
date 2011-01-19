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
using System.Data;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping.SortExpressions;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Mixins;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms
{
  [TestFixture]
  public class SingleIDLookupCommandBuilderTest : SqlProviderBaseTest
  {
    [Test]
    public void Create_WithOrderClause ()
    {
      Provider.Connect ();
      var builder = new SingleIDLookupCommandBuilder (
          Provider,
          StorageNameProvider,
          "*", 
          "Order", 
          "CustomerID", 
          DomainObjectIDs.Customer1, 
          SortExpressionDefinitionObjectMother.ParseSortExpression (typeof (Order), "OrderNumber asc"));

      using (IDbCommand command = builder.Create ())
      {
        Assert.AreEqual (
            "SELECT * FROM [Order] WHERE [CustomerID] = @CustomerID ORDER BY [OrderNo] ASC;",
            command.CommandText);
      }
    }

    [Test]
    public void Create_WithoutOrderClause ()
    {
      Provider.Connect ();
      var builder = new SingleIDLookupCommandBuilder (
          Provider,
          StorageNameProvider,
          "*",
          "Order",
          "CustomerID",
          DomainObjectIDs.Customer1,
          null);

      using (IDbCommand command = builder.Create ())
      {
        Assert.AreEqual (
            "SELECT * FROM [Order] WHERE [CustomerID] = @CustomerID;",
            command.CommandText);
      }
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Provider must be connected first.\r\nParameter name: provider")]
    public void ConstructorChecksForConnectedProvider ()
    {
      new SingleIDLookupCommandBuilder (
          Provider,
          StorageNameProvider,
          "*",
          "Order",
          "CustomerID",
          DomainObjectIDs.Customer1,
          SortExpressionDefinitionObjectMother.ParseSortExpression (typeof (Order), "OrderNumber asc"));
    }

    [Test]
    public void WhereClauseBuilder_CanBeMixed ()
    {
      Provider.Connect ();
      using (MixinConfiguration.BuildFromActive().ForClass (typeof (WhereClauseBuilder)).Clear().AddMixins (typeof (WhereClauseBuilderMixin)).EnterScope())
      {
        var builder = new SingleIDLookupCommandBuilder (
            Provider,
            StorageNameProvider,
            "*",
            "Order",
            "CustomerID",
            DomainObjectIDs.Customer1,
            SortExpressionDefinitionObjectMother.ParseSortExpression (typeof (Order), "OrderNumber asc"));

        using (IDbCommand command = builder.Create())
        {
          Assert.IsTrue (command.CommandText.Contains ("Mixed!"));
        }
      }
    }
  }
}

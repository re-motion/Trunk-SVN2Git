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
using System.Data;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.UnitTests.DomainObjects.Factories;
using Remotion.Mixins;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms
{
  [TestFixture]
  public class SelectCommandBuilderTest : SqlProviderBaseTest
  {
    [Test]
    public void CreateWithOrderClause ()
    {
      ClassDefinition orderDefinition = TestMappingConfiguration.Current.ClassDefinitions["Order"];

      Provider.Connect ();
      SelectCommandBuilder builder = SelectCommandBuilder.CreateForRelatedIDLookup (
          Provider, orderDefinition, orderDefinition.GetMandatoryPropertyDefinition ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.Customer"), DomainObjectIDs.Customer1);

      using (IDbCommand command = builder.Create ())
      {
        Assert.AreEqual (
            "SELECT * FROM [Order] WHERE [CustomerID] = @CustomerID ORDER BY OrderNo asc;",
            command.CommandText);
      }
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Provider must be connected first.\r\nParameter name: provider")]
    public void ConstructorChecksForConnectedProvider ()
    {
      ClassDefinition orderDefinition = TestMappingConfiguration.Current.ClassDefinitions["Order"];
      SelectCommandBuilder.CreateForRelatedIDLookup (
          Provider, 
          orderDefinition, 
          orderDefinition.GetMandatoryPropertyDefinition ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.Customer"), 
          DomainObjectIDs.Customer1);
    }

    [Test]
    public void WhereClauseBuilder_CanBeMixed ()
    {
      ClassDefinition orderDefinition = TestMappingConfiguration.Current.ClassDefinitions["Order"];

      Provider.Connect ();
      using (MixinConfiguration.BuildFromActive().ForClass (typeof (WhereClauseBuilder)).Clear().AddMixins (typeof (WhereClauseBuilderMixin)).EnterScope())
      {
        SelectCommandBuilder builder = SelectCommandBuilder.CreateForRelatedIDLookup (
            Provider,
            orderDefinition,
            orderDefinition.GetMandatoryPropertyDefinition ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.Customer"),
            DomainObjectIDs.Customer1);

        using (IDbCommand command = builder.Create())
        {
          Assert.IsTrue (command.CommandText.Contains ("Mixed!"));
        }
      }
    }
  }
}

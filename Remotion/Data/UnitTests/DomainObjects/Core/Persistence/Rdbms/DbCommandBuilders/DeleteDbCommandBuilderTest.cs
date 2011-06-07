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
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Mixins;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.DbCommandBuilders
{
  [TestFixture]
  public class DeleteDbCommandBuilderTest : SqlProviderBaseTest
  {
    [Test]
    public void CreateWithoutForeignKeyColumn ()
    {
      ClassWithAllDataTypes classWithAllDataTypes = ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes1);

      classWithAllDataTypes.Delete();
      DataContainer deletedContainer = classWithAllDataTypes.InternalDataContainer;

      Provider.Connect();
      DbCommandBuilder commandBuilder = new DeleteDbCommandBuilder (
          Provider,
          StorageNameProvider,
          deletedContainer,
          Provider.SqlDialect,
          Provider,
          Provider.StorageProviderDefinition,
          Provider.CreateValueConverter());

      using (IDbCommand deleteCommand = commandBuilder.Create())
      {
        string expectedCommandText = "DELETE FROM [TableWithAllDataTypes] WHERE [ID] = @ID AND [Timestamp] = @Timestamp;";
        Assert.AreEqual (expectedCommandText, deleteCommand.CommandText);

        Assert.AreEqual (2, deleteCommand.Parameters.Count);

        IDataParameter idParameter = (IDataParameter) deleteCommand.Parameters["@ID"];
        IDataParameter timestampParameter = (IDataParameter) deleteCommand.Parameters["@Timestamp"];

        Assert.AreEqual (deletedContainer.ID.Value, idParameter.Value);
        Assert.AreEqual (deletedContainer.Timestamp, timestampParameter.Value);
      }
    }

    [Test]
    public void CreateWithForeignKeyColumn ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      order.Delete();
      DataContainer deletedOrderContainer = order.InternalDataContainer;

      Provider.Connect();
      DbCommandBuilder commandBuilder = new DeleteDbCommandBuilder (
          Provider,
          StorageNameProvider,
          deletedOrderContainer,
          Provider.SqlDialect,
          Provider,
          Provider.StorageProviderDefinition,
          Provider.CreateValueConverter());

      using (IDbCommand deleteCommand = commandBuilder.Create())
      {
        string expectedCommandText = "DELETE FROM [Order] WHERE [ID] = @ID;";
        Assert.AreEqual (expectedCommandText, deleteCommand.CommandText);

        Assert.AreEqual (1, deleteCommand.Parameters.Count);

        IDataParameter idParameter = (IDataParameter) deleteCommand.Parameters["@ID"];

        Assert.AreEqual (deletedOrderContainer.ID.Value, idParameter.Value);
      }
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "State of provided DataContainer must be 'Deleted', but is 'Unchanged'.\r\nParameter name: dataContainer")]
    public void InitializeWithDataContainerOfInvalidState ()
    {
      Provider.Connect();
      new DeleteDbCommandBuilder (
          Provider,
          StorageNameProvider,
          TestDataContainerFactory.CreateOrder1DataContainer(),
          Provider.SqlDialect,
          Provider,
          Provider.StorageProviderDefinition,
          Provider.CreateValueConverter());
    }

    [Test]
    public void WhereClauseBuilder_CanBeMixed ()
    {
      ClassWithAllDataTypes classWithAllDataTypes = ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes1);
      classWithAllDataTypes.Delete();
      using (
          MixinConfiguration.BuildFromActive().ForClass (typeof (WhereClauseBuilder)).Clear().AddMixins (typeof (WhereClauseBuilderMixin)).EnterScope(
              
              ))
      {
        DataContainer deletedContainer = classWithAllDataTypes.InternalDataContainer;

        Provider.Connect();
        DbCommandBuilder commandBuilder = new DeleteDbCommandBuilder (
            Provider,
            StorageNameProvider,
            deletedContainer,
            Provider.SqlDialect,
            Provider,
            Provider.StorageProviderDefinition,
            Provider.CreateValueConverter());

        using (IDbCommand deleteCommand = commandBuilder.Create())
        {
          Assert.IsTrue (deleteCommand.CommandText.Contains ("Mixed!"));
        }
      }
    }
  }
}
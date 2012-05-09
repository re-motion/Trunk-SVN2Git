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
using System.Data.SqlClient;
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Mapping.SortExpressions;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DataReaders;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Building;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.DbCommandBuilders;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.Model.Building;
using Remotion.Data.DomainObjects.Persistence.Rdbms.StorageProviderCommands.Factories;
using Remotion.Data.DomainObjects.Tracing;
using System.Linq;
using Remotion.Data.UnitTests.DomainObjects.TestDomain.TableInheritance;
using SortOrder = Remotion.Data.DomainObjects.Mapping.SortExpressions.SortOrder;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.SqlServer.IntegrationTests
{
  [TestFixture]
  public class SqlProviderTableInheritanceTest : TableInheritanceMappingTest
  {
    private RdbmsProvider _provider;

    public override void SetUp ()
    {
      base.SetUp ();

      var storageNameProvider = new ReflectionBasedStorageNameProvider ();
      var storageTypeInformationProvider = new SqlStorageTypeInformationProvider ();

      var rdbmsPersistenceModelProvider = new RdbmsPersistenceModelProvider ();
      var infrastructureStoragePropertyDefinitionProvider = new InfrastructureStoragePropertyDefinitionProvider (
          storageTypeInformationProvider, storageNameProvider);
      var dataStoragePropertyDefinitionFactory = new DataStoragePropertyDefinitionFactory (
          TableInheritanceTestDomainStorageProviderDefinition,
          storageTypeInformationProvider,
          storageNameProvider,
          new StorageEntityBasedStorageProviderDefinitionFinder ());

      var commandFactory = new RdbmsProviderCommandFactory (
          TableInheritanceTestDomainStorageProviderDefinition,
          new SqlDbCommandBuilderFactory (SqlDialect.Instance),
          rdbmsPersistenceModelProvider,
          new ObjectReaderFactory (rdbmsPersistenceModelProvider, infrastructureStoragePropertyDefinitionProvider, storageTypeInformationProvider),
          new TableDefinitionFinder (rdbmsPersistenceModelProvider),
          dataStoragePropertyDefinitionFactory);

      _provider = new RdbmsProvider (
          TableInheritanceTestDomainStorageProviderDefinition,
          NullPersistenceExtension.Instance,
          commandFactory,
          () => new SqlConnection ());
    }

    public override void TearDown ()
    {
      _provider.Dispose ();
      base.TearDown ();
    }
    [Test]
    public void LoadConcreteSingle ()
    {
      DataContainer customerContainer = _provider.LoadDataContainer (DomainObjectIDs.Customer).LocatedObject;
      Assert.IsNotNull (customerContainer);
      Assert.AreEqual (DomainObjectIDs.Customer, customerContainer.ID);
      Assert.AreEqual (
          "UnitTests", customerContainer.GetValue ("Remotion.Data.UnitTests.DomainObjects.TestDomain.TableInheritance.TIDomainBase.CreatedBy"));
      Assert.AreEqual (
          "Zaphod", customerContainer.GetValue ("Remotion.Data.UnitTests.DomainObjects.TestDomain.TableInheritance.TIPerson.FirstName"));
      Assert.AreEqual (
          CustomerType.Premium,
          customerContainer.GetValue ("Remotion.Data.UnitTests.DomainObjects.TestDomain.TableInheritance.TICustomer.CustomerType"));
    }

    [Test]
    public void LoadDataContainersByRelatedID_WithAbstractBaseClass ()
    {
      var relationEndPointDefinition = GetEndPointDefinition (typeof (TIDomainBase), "Client");
      var createdAtProperty = GetPropertyDefinition (typeof (TIDomainBase), "CreatedAt");
      var sortExpression = new SortExpressionDefinition (new[] { new SortedPropertySpecification (createdAtProperty, SortOrder.Ascending) });

      var loadedDataContainers = _provider.LoadDataContainersByRelatedID (
          (RelationEndPointDefinition) relationEndPointDefinition,
          sortExpression,
          DomainObjectIDs.Client).ToList();

      Assert.IsNotNull (loadedDataContainers);
      Assert.AreEqual (4, loadedDataContainers.Count);
      Assert.AreEqual (DomainObjectIDs.OrganizationalUnit, loadedDataContainers[0].ID);
      Assert.AreEqual (DomainObjectIDs.Person, loadedDataContainers[1].ID);
      Assert.AreEqual (DomainObjectIDs.PersonForUnidirectionalRelationTest, loadedDataContainers[2].ID);
      Assert.AreEqual (DomainObjectIDs.Customer, loadedDataContainers[3].ID);
    }

    [Test]
    public void LoadDataContainersByRelatedID_WithAbstractClassWithoutDerivations ()
    {
      var relationEndPointDefinition = GetEndPointDefinition (typeof (AbstractClassWithoutDerivations), "DomainBase");

      var result = _provider.LoadDataContainersByRelatedID (
          (RelationEndPointDefinition) relationEndPointDefinition,
          null,
          DomainObjectIDs.Customer);
      Assert.IsNotNull (result);
      Assert.AreEqual (0, result.Count());
    }
  }
}
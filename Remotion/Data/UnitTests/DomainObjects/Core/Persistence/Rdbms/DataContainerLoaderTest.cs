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
using System.Collections.Generic;
using System.Data;
using System.Linq;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Configuration;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.Tracing;
using Remotion.Data.UnitTests.DomainObjects.Core.TableInheritance.TestDomain;
using Remotion.Data.UnitTests.DomainObjects.Factories;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Development.UnitTesting;
using Rhino.Mocks;
using Rhino.Mocks.Interfaces;
using DomainObjectIDs = Remotion.Data.UnitTests.DomainObjects.Core.TableInheritance.DomainObjectIDs;
using Mocks_Is = Rhino.Mocks.Constraints.Is;
using Mocks_List = Rhino.Mocks.Constraints.List;
using Order = Remotion.Data.UnitTests.DomainObjects.TestDomain.Order;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms
{
  [TestFixture]
  public class DataContainerLoaderTest : SqlProviderBaseTest
  {
    private DataContainerLoader _loader;
    private MockRepository _mockRepository;

    public override void SetUp ()
    {
      base.SetUp();
      _loader = new DataContainerLoader (Provider);
      _mockRepository = new MockRepository();
      Provider.Connect();
    }

    public override void TearDown ()
    {
      base.TearDown();
      Provider.Disconnect();
    }

    [Test]
    public void Initialization ()
    {
      Assert.AreSame (Provider, _loader.Provider);
    }

    [Test]
    public void LoadDataContainerFromID ()
    {
      DataContainer dataContainer = _loader.LoadDataContainerFromID (DomainObjectIDs.Order1);
      Assert.AreEqual (DomainObjectIDs.Order1, dataContainer.ID);

      Assert.AreEqual (MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (Order)), dataContainer.ClassDefinition);
      Assert.AreEqual (dataContainer.ClassDefinition.GetPropertyDefinitions().Count, dataContainer.PropertyValues.Count);

      Assert.AreEqual (
          1, dataContainer.PropertyValues[ReflectionMappingHelper.GetPropertyName (typeof (Order), "OrderNumber")].Value);
      Assert.AreEqual (
          DomainObjectIDs.Customer1,
          dataContainer.PropertyValues[ReflectionMappingHelper.GetPropertyName (typeof (Order), "Customer")].Value);
    }

    [Test]
    public void LoadDataContainersFromID_CallsProviderExecuteReader ()
    {
      CreateLoaderAndExpectProviderExecuteReader (
          delegate (DataContainerLoader loader)
          {
            loader.Provider.Connect();

            loader.LoadDataContainerFromID (DomainObjectIDs.OrderItem1);
          });
    }

    [Test]
    public void LoadDataContainersFromIDs_Simple ()
    {
      IEnumerable<ObjectID> objectIDs = new[] { DomainObjectIDs.Order1, DomainObjectIDs.Order2, DomainObjectIDs.Order3 };
      DataContainerCollection dataContainers = _loader.LoadDataContainersFromIDs (objectIDs);
      Assert.AreEqual (3, dataContainers.Count);

      Assert.AreEqual (DomainObjectIDs.Order1, dataContainers[0].ID);
      Assert.AreEqual (DomainObjectIDs.Order2, dataContainers[1].ID);
      Assert.AreEqual (DomainObjectIDs.Order3, dataContainers[2].ID);

      Assert.AreEqual (MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (Order)), dataContainers[0].ClassDefinition);
      Assert.AreEqual (dataContainers[0].ClassDefinition.GetPropertyDefinitions().Count, dataContainers[0].PropertyValues.Count);

      Assert.AreEqual (
          1, dataContainers[0].PropertyValues[ReflectionMappingHelper.GetPropertyName (typeof (Order), "OrderNumber")].Value);
      Assert.AreEqual (
          DomainObjectIDs.Customer1,
          dataContainers[0].PropertyValues[ReflectionMappingHelper.GetPropertyName (typeof (Order), "Customer")].Value);
    }

    [Test]
    public void LoadDataContainersFromIDs_NonExistingID ()
    {
      IEnumerable<ObjectID> objectIDs = new[]
                                        {
                                            DomainObjectIDs.Order1, DomainObjectIDs.Order2,
                                            new ObjectID (typeof (Order), Guid.NewGuid()), DomainObjectIDs.Order3
                                        };
      DataContainerCollection dataContainers = _loader.LoadDataContainersFromIDs (objectIDs);
      Assert.AreEqual (3, dataContainers.Count);

      Assert.AreEqual (DomainObjectIDs.Order1, dataContainers[0].ID);
      Assert.AreEqual (DomainObjectIDs.Order2, dataContainers[1].ID);
      Assert.AreEqual (DomainObjectIDs.Order3, dataContainers[2].ID);
    }

    [Test]
    public void LoadDataContainersFromIDs_DifferentEntities_Ordering ()
    {
      IEnumerable<ObjectID> objectIDs = new[]
                                        {
                                            DomainObjectIDs.Order1, DomainObjectIDs.OrderItem1, DomainObjectIDs.Order2,
                                            DomainObjectIDs.Customer1, DomainObjectIDs.Customer2, DomainObjectIDs.Order3, DomainObjectIDs.OrderItem2
                                        };
      DataContainerCollection dataContainers = _loader.LoadDataContainersFromIDs (objectIDs);
      Assert.AreEqual (7, dataContainers.Count);

      Assert.AreEqual (DomainObjectIDs.Order1, dataContainers[0].ID);
      Assert.AreEqual (DomainObjectIDs.OrderItem1, dataContainers[1].ID);
      Assert.AreEqual (DomainObjectIDs.Order2, dataContainers[2].ID);
      Assert.AreEqual (DomainObjectIDs.Customer1, dataContainers[3].ID);
      Assert.AreEqual (DomainObjectIDs.Customer2, dataContainers[4].ID);
      Assert.AreEqual (DomainObjectIDs.Order3, dataContainers[5].ID);
      Assert.AreEqual (DomainObjectIDs.OrderItem2, dataContainers[6].ID);
    }

    [Test]
    public void LoadDataContainersFromIDs_DifferentEntities_Grouping ()
    {
      IEnumerable<ObjectID> objectIDs = new[]
                                        {
                                            DomainObjectIDs.Order1, DomainObjectIDs.OrderItem1, DomainObjectIDs.Order2,
                                            DomainObjectIDs.Company1, DomainObjectIDs.Company2, DomainObjectIDs.Order3, DomainObjectIDs.OrderItem2
                                        };

      IDataContainerLoaderHelper loaderHelperMock = _mockRepository.StrictMock<DataContainerLoaderHelper>();

      var loader = new DataContainerLoader (loaderHelperMock, Provider);

      using (_mockRepository.Unordered())
      {
        Expect.Call (loaderHelperMock.GetCommandBuilderForIDLookup (null, null, null))
            .Constraints (
            Mocks_Is.Same (Provider),
            Mocks_Is.Equal ("Order"),
            Mocks_List.Equal (new[] { DomainObjectIDs.Order1, DomainObjectIDs.Order2, DomainObjectIDs.Order3 }))
            .CallOriginalMethod (OriginalCallOptions.CreateExpectation);
        Expect.Call (loaderHelperMock.GetCommandBuilderForIDLookup (null, null, null))
            .Constraints (
            Mocks_Is.Same (Provider),
            Mocks_Is.Equal ("OrderItem"),
            Mocks_List.Equal (new[] { DomainObjectIDs.OrderItem1, DomainObjectIDs.OrderItem2 }))
            .CallOriginalMethod (OriginalCallOptions.CreateExpectation);
        Expect.Call (loaderHelperMock.GetCommandBuilderForIDLookup (null, null, null))
            .Constraints (
            Mocks_Is.Same (Provider),
            Mocks_Is.Equal ("Company"),
            Mocks_List.Equal (new[] { DomainObjectIDs.Company1, DomainObjectIDs.Company2 }))
            .CallOriginalMethod (OriginalCallOptions.CreateExpectation);
      }

      _mockRepository.ReplayAll();

      loader.LoadDataContainersFromIDs (objectIDs);

      _mockRepository.VerifyAll();
    }

    [Test]
    public void LoadDataContainersFromIDs_CallsProviderExecuteReader ()
    {
      CreateLoaderAndExpectProviderExecuteReader (
          delegate (DataContainerLoader loader)
          {
            loader.Provider.Connect();

            loader.LoadDataContainersFromIDs (new[] { DomainObjectIDs.OrderItem1, DomainObjectIDs.OrderItem2 });
          });
    }

    [Test]
    [ExpectedException (typeof (RdbmsProviderException),
        ExpectedMessage = "An object returned from the database had a NULL ID, which is not supported.")]
    public void LoadDataContainersFromIDs_NullIDs ()
    {
      var readerMock = MockRepository.GenerateMock<IDataReader>();
      using (readerMock.GetMockRepository().Ordered())
      {
        readerMock.Expect (mock => mock.Read()).Return (true);
        readerMock.Expect (mock => mock.GetOrdinal ("ID")).Return (0);
        readerMock.Expect (mock => mock.GetValue (0)).Return (DBNull.Value);
        readerMock.Expect (mock => mock.Read()).Return (false);
      }

      CreateLoaderAndExpectProviderCall (
          loader =>
          {
            loader.Provider.Connect();
            loader.LoadDataContainersFromIDs (new[] { DomainObjectIDs.OrderItem1 });
          },
          providerMock => providerMock
                              .Expect (mock => mock.ExecuteReader (Arg<IDbCommand>.Is.Anything, Arg<CommandBehavior>.Is.Anything))
                              .Return (readerMock));
    }

    [Test]
    public void LoadDataContainersFromCommandBuilder ()
    {
      ClassDefinition classDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (OrderItem));
      var builder = new MultiIDLookupCommandBuilder (
          Provider, 
          "*", 
          "OrderItem", 
          "ID", 
          "uniqueidentifier", 
          new[] { DomainObjectIDs.OrderItem1, DomainObjectIDs.OrderItem2 });

      List<DataContainer> sortedContainers = GetSortedContainers (_loader.LoadDataContainersFromCommandBuilder (builder, false));

      Assert.AreEqual (2, sortedContainers.Count);
      Assert.AreEqual (DomainObjectIDs.OrderItem1, sortedContainers[0].ID);
      Assert.AreEqual (DomainObjectIDs.OrderItem2, sortedContainers[1].ID);

      Assert.AreEqual (classDefinition.GetPropertyDefinitions().Count, sortedContainers[0].PropertyValues.Count);
      Assert.AreEqual (
          1, sortedContainers[0].PropertyValues[ReflectionMappingHelper.GetPropertyName (typeof (OrderItem), "Position")].Value);
      Assert.AreEqual (
          DomainObjectIDs.Order1,
          sortedContainers[0].PropertyValues[ReflectionMappingHelper.GetPropertyName (typeof (OrderItem), "Order")].Value);
    }

    [Test]
    public void LoadDataContainersFromCommandBuilder_CallsProviderExecuteReader ()
    {
      CreateLoaderAndExpectProviderExecuteReader (
          delegate (DataContainerLoader loader)
          {
            loader.Provider.Connect();

            var builder = new MultiIDLookupCommandBuilder (
                loader.Provider,
                "*",
                "Order",
                "ID",
                "uniqueidentifier",
                new[] { DomainObjectIDs.Order1, DomainObjectIDs.Order2 });
            loader.LoadDataContainersFromCommandBuilder (builder, false);
          });
    }

    [Test]
    public void LoadDataContainersFromCommandBuilder_NullIDs_AllowNullsTrue ()
    {
      var query = QueryFactory.CreateCollectionQuery (
          "test",
          Provider.StorageProviderDefinition,
          "SELECT NULL AS [ID] FROM [Order] WHERE [Order].[OrderNo] IN (1, 2)",
          new QueryParameterCollection(),
          typeof (DomainObjectCollection));
      var builder = new QueryCommandBuilder (Provider, query);

      var dcs = _loader.LoadDataContainersFromCommandBuilder (builder, true);

      Assert.That (dcs, Is.EqualTo (new DataContainer[] { null, null }));
    }

    [Test]
    [ExpectedException (typeof (RdbmsProviderException),
        ExpectedMessage = "An object returned from the database had a NULL ID, which is not supported.")]
    public void LoadDataContainersFromCommandBuilder_NullIDs_AllowNullsFalse ()
    {
      var query = QueryFactory.CreateCollectionQuery (
          "test",
          Provider.StorageProviderDefinition,
          "SELECT NULL AS [ID] FROM [Order] WHERE [Order].[OrderNo] IN (1, 2)",
          new QueryParameterCollection(),
          typeof (DomainObjectCollection));
      var builder = new QueryCommandBuilder (Provider, query);

      _loader.LoadDataContainersFromCommandBuilder (builder, false);
    }

    [Test]
    public void LoadDataContainersByRelatedID_WithEntityName ()
    {
      ClassDefinition classDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (OrderItem));
      Assert.IsNotNull (classDefinition.GetEntityName());

      DataContainerCollection dataContainers = _loader.LoadDataContainersByRelatedID (
          classDefinition, ReflectionMappingHelper.GetPropertyName (typeof (OrderItem), "Order"), DomainObjectIDs.Order1);
      List<DataContainer> sortedContainers = GetSortedContainers (dataContainers.Cast<DataContainer>());

      Assert.AreEqual (2, sortedContainers.Count);
      Assert.AreEqual (DomainObjectIDs.OrderItem1, sortedContainers[0].ID);
      Assert.AreEqual (DomainObjectIDs.OrderItem2, sortedContainers[1].ID);

      Assert.AreEqual (classDefinition.GetPropertyDefinitions().Count, sortedContainers[0].PropertyValues.Count);
      Assert.AreEqual (
          1, sortedContainers[0].PropertyValues[ReflectionMappingHelper.GetPropertyName (typeof (OrderItem), "Position")].Value);
      Assert.AreEqual (
          DomainObjectIDs.Order1,
          sortedContainers[0].PropertyValues[ReflectionMappingHelper.GetPropertyName (typeof (OrderItem), "Order")].Value);
    }

    [Test]
    public void LoadDataContainersByRelatedID_WithEntityName_CallsProviderLoadDataContainers_WithAllowNullsFalse ()
    {
      ClassDefinition classDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (OrderItem));
      Assert.IsNotNull (classDefinition.GetEntityName());

      CreateLoaderAndExpectProviderCall (
          delegate (DataContainerLoader loader)
          {
            loader.Provider.Connect();

            loader.LoadDataContainersByRelatedID (
                classDefinition, ReflectionMappingHelper.GetPropertyName (typeof (OrderItem), "Order"), DomainObjectIDs.Order1);
          },
          delegate (RdbmsProvider provider)
          {
            Expect.Call (PrivateInvoke.InvokeNonPublicMethod (provider, "LoadDataContainers", new object[] { null, false }))
                .IgnoreArguments().CallOriginalMethod (OriginalCallOptions.CreateExpectation);
          });
    }

    [Test]
    public void LoadDataContainersByRelatedID_WithEntityName_UsesSelectCommandBuilder ()
    {
      var classDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (OrderItem));
      Assert.IsNotNull (classDefinition.GetEntityName());

      var relatedClassDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (Order));
      var relationPropertyName = ReflectionMappingHelper.GetPropertyName (typeof (OrderItem), "Order");
      var relationPropertyDefinition = classDefinition.GetPropertyDefinition (relationPropertyName);

      IDataContainerLoaderHelper loaderHelperMock = _mockRepository.StrictMock<DataContainerLoaderHelper>();

      Expect.Call (
          loaderHelperMock.GetCommandBuilderForRelatedIDLookup (
              Provider,
              relatedClassDefinition.GetEntityName(),
              relationPropertyDefinition,
              DomainObjectIDs.Order1)).IgnoreArguments().CallOriginalMethod (OriginalCallOptions.CreateExpectation);

      _mockRepository.ReplayAll();

      var loader = new DataContainerLoader (loaderHelperMock, Provider);
      loader.LoadDataContainersByRelatedID (classDefinition, relationPropertyName, DomainObjectIDs.Order1);

      _mockRepository.VerifyAll();
    }

    [Test]
    public void LoadDataContainersByRelatedID_WithStorageClassTransaction ()
    {
      ClassDefinition classDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (Computer));
      Assert.IsNotNull (classDefinition.GetEntityName());

      DataContainerCollection dataContainers = _loader.LoadDataContainersByRelatedID (
          classDefinition,
          ReflectionMappingHelper.GetPropertyName (typeof (Computer), "EmployeeTransactionProperty"),
          DomainObjectIDs.Computer1);

      Assert.That (dataContainers, Is.Empty);
    }

    [Test]
    public void LoadDataContainersByRelatedID_WithoutEntityName ()
    {
      Provider.Disconnect();

      using (RdbmsProvider provider = new SqlProvider (
          (RdbmsProviderDefinition) DomainObjectsConfiguration.Current.Storage.StorageProviderDefinitions.GetMandatory ("TableInheritanceTestDomain"),
          NullPersistenceListener.Instance))
      {
        provider.Connect();

        ClassDefinition classDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (DomainBase));
        Assert.IsNull (classDefinition.GetEntityName());

        var loader = new DataContainerLoader (provider);
        var domainObjectIDs = new DomainObjectIDs(Configuration);
        DataContainerCollection dataContainers = loader.LoadDataContainersByRelatedID (
            classDefinition,
            ReflectionMappingHelper.GetPropertyName (typeof (DomainBase), "Client"),
            domainObjectIDs.Client);

        Assert.AreEqual (4, dataContainers.Count);
        Assert.IsTrue (dataContainers.Contains (domainObjectIDs.Customer));
        Assert.IsTrue (dataContainers.Contains (domainObjectIDs.Person));
        Assert.IsTrue (dataContainers.Contains (domainObjectIDs.OrganizationalUnit));
        Assert.IsTrue (dataContainers.Contains (domainObjectIDs.PersonForUnidirectionalRelationTest));
      }
    }

    [Test]
    public void LoadDataContainersByRelatedID_WithoutEntityName_UsesConcreteTableInheritanceRelationLoader ()
    {
      Provider.Disconnect();

      using (RdbmsProvider provider = new SqlProvider (
          (RdbmsProviderDefinition) DomainObjectsConfiguration.Current.Storage.StorageProviderDefinitions.GetMandatory ("TableInheritanceTestDomain"),
          NullPersistenceListener.Instance))
      {
        provider.Connect();

        ClassDefinition classDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (DomainBase));
        Assert.IsNull (classDefinition.GetEntityName());

        IDataContainerLoaderHelper loaderHelperMock = _mockRepository.StrictMock<DataContainerLoaderHelper>();

        Expect.Call (loaderHelperMock.GetConcreteTableInheritanceRelationLoader (null, null, null, null)).IgnoreArguments()
            .CallOriginalMethod (OriginalCallOptions.CreateExpectation);

        _mockRepository.ReplayAll();

        var loader = new DataContainerLoader (loaderHelperMock, provider);

        var domainObjectIDs = new DomainObjectIDs(Configuration);
        loader.LoadDataContainersByRelatedID (
            classDefinition,
            ReflectionMappingHelper.GetPropertyName (typeof (DomainBase), "Client"),
            domainObjectIDs.Client);

        _mockRepository.VerifyAll();
      }
    }

    private void CreateLoaderAndExpectProviderCall (Action<DataContainerLoader> action, Action<RdbmsProvider> expectationSetup)
    {
      Provider.Disconnect();

      using (RdbmsProvider providerMock = _mockRepository.PartialMock<SqlProvider> (TestDomainStorageProviderDefinition, NullPersistenceListener.Instance))
      {
        var loader = new DataContainerLoader (providerMock);

        expectationSetup (providerMock);

        _mockRepository.ReplayAll();

        action (loader);
      }
      _mockRepository.VerifyAll();
    }

    private void CreateLoaderAndExpectProviderExecuteReader (Action<DataContainerLoader> action)
    {
      CreateLoaderAndExpectProviderCall (
          action,
          delegate (RdbmsProvider providerMock)
          {
            Expect.Call (providerMock.ExecuteReader (null, CommandBehavior.Default)).IgnoreArguments()
                .CallOriginalMethod (OriginalCallOptions.CreateExpectation);
          });
    }

    private List<DataContainer> GetSortedContainers (IEnumerable<DataContainer> dataContainers)
    {
      var sortedContainers = new List<DataContainer> (dataContainers);
      sortedContainers.Sort ((one, two) => one.ID.Value.ToString().CompareTo (two.ID.Value.ToString()));
      return sortedContainers;
    }
  }
} // expect
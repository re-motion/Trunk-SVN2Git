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
using System.Linq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Building;
using Remotion.Data.UnitTests.DomainObjects.Factories;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.Model.Building
{
  [TestFixture]
  public class ForeignKeyConstraintDefinitionFactoryTest : StandardMappingTest
  {
    private IStorageNameProvider _storageNameProviderMock;
    private IRdbmsPersistenceModelProvider _persistenceModelProvider;
    private IInfrastructureStoragePropertyDefinitionProvider _infrastructureStoragePropertyDefintionProviderMock;
    private ForeignKeyConstraintDefinitionFactory _factory;
    private ObjectIDStoragePropertyDefinition _fakeObjectIDStoragePropertyDefinition;
    private ObjectIDStoragePropertyDefinition _fakeForeignKeyStoragePropertyDefinition;
    private IStorageProviderDefinitionFinder _storageProviderDefinitionFinderStub;

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp();

      _fakeObjectIDStoragePropertyDefinition = new ObjectIDStoragePropertyDefinition (
          SimpleStoragePropertyDefinitionObjectMother.IDProperty, SimpleStoragePropertyDefinitionObjectMother.ClassIDProperty);
      _fakeForeignKeyStoragePropertyDefinition = new ObjectIDStoragePropertyDefinition (
          SimpleStoragePropertyDefinitionObjectMother.CreateStorageProperty ("OrderID"),
          SimpleStoragePropertyDefinitionObjectMother.CreateStorageProperty ("ClassID"));

      _storageNameProviderMock = MockRepository.GenerateStrictMock<IStorageNameProvider>();
      _persistenceModelProvider = MockRepository.GenerateStrictMock<IRdbmsPersistenceModelProvider>();
      _infrastructureStoragePropertyDefintionProviderMock = MockRepository.GenerateStrictMock<IInfrastructureStoragePropertyDefinitionProvider>();
      _storageProviderDefinitionFinderStub = MockRepository.GenerateStub<IStorageProviderDefinitionFinder>();
      _factory = new ForeignKeyConstraintDefinitionFactory (
          _storageNameProviderMock,
          _persistenceModelProvider,
          _infrastructureStoragePropertyDefintionProviderMock,
          _storageProviderDefinitionFinderStub);
    }

    [Test]
    public void CreateForeignKeyConstraints_ObjectIDStoragePropertyDefinition ()
    {
      var orderClassDefinition = Configuration.GetClassDefinition ("Order");
      var customerClassDefintion = Configuration.GetClassDefinition ("Customer");
      var officialClassDefinition = Configuration.GetClassDefinition ("Official");

      _infrastructureStoragePropertyDefintionProviderMock
          .Expect (mock => mock.GetObjectIDStoragePropertyDefinition())
          .Return (_fakeObjectIDStoragePropertyDefinition);
      _infrastructureStoragePropertyDefintionProviderMock.Replay();

      _persistenceModelProvider
          .Expect (
              mock => mock.GetStoragePropertyDefinition (
                  orderClassDefinition.MyPropertyDefinitions["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.Customer"]))
          .Return (_fakeForeignKeyStoragePropertyDefinition);
      _persistenceModelProvider.Replay();

      _storageNameProviderMock
          .Expect (mock => mock.GetForeignKeyConstraintName (
              Arg.Is (orderClassDefinition), 
              Arg<IEnumerable<ColumnDefinition>>.Is.Equal (new[] { _fakeForeignKeyStoragePropertyDefinition.GetColumnForLookup() })))
          .Return ("FakeConstraintName");
      _storageNameProviderMock
          .Expect (mock => mock.GetTableName (customerClassDefintion))
          .Return (new EntityNameDefinition(null, "FakeTableName"))
          .Repeat.Twice();
      _storageNameProviderMock.Replay();

      _storageProviderDefinitionFinderStub
          .Stub (stub => stub.GetStorageProviderDefinition (orderClassDefinition.StorageGroupType, null))
          .Return (orderClassDefinition.StorageEntityDefinition.StorageProviderDefinition);
      _storageProviderDefinitionFinderStub
          .Stub (stub => stub.GetStorageProviderDefinition (customerClassDefintion.StorageGroupType, null))
          .Return (customerClassDefintion.StorageEntityDefinition.StorageProviderDefinition);
      _storageProviderDefinitionFinderStub
          .Stub (stub => stub.GetStorageProviderDefinition (officialClassDefinition.StorageGroupType, null))
          .Return (officialClassDefinition.StorageEntityDefinition.StorageProviderDefinition);

      var foreignKeyConstraintDefinitions = _factory.CreateForeignKeyConstraints (orderClassDefinition).ToArray();

      _infrastructureStoragePropertyDefintionProviderMock.VerifyAllExpectations();
      _persistenceModelProvider.VerifyAllExpectations();
      _storageNameProviderMock.VerifyAllExpectations();

      //OrderItem and OrderTicket endpoints are virtual and Official endpoint has different storage provider
      Assert.That (foreignKeyConstraintDefinitions.Length, Is.EqualTo (1));
      var foreignKeyConstraint = foreignKeyConstraintDefinitions[0];
      Assert.That (foreignKeyConstraint.ReferencedTableName.EntityName, Is.EqualTo ("FakeTableName"));
      Assert.That (foreignKeyConstraint.ReferencedTableName.SchemaName, Is.Null);
      Assert.That (foreignKeyConstraint.ConstraintName, Is.EqualTo ("FakeConstraintName"));
      Assert.That (foreignKeyConstraint.ReferencingColumns, Is.EqualTo (new[] { _fakeObjectIDStoragePropertyDefinition.GetColumnForLookup() }));
      Assert.That (foreignKeyConstraint.ReferencedColumns, Is.EqualTo (new[] { _fakeForeignKeyStoragePropertyDefinition.GetColumnForLookup() }));
    }
    
    [Test]
    public void CreateForeignKeyConstraints_StorageClassTransactionPropertiesAreIgnored ()
    {
      var computerClassDefinition = Configuration.GetClassDefinition ("Computer");
      var employeeClassDefinition = Configuration.GetClassDefinition ("Employee");

      _infrastructureStoragePropertyDefintionProviderMock
          .Expect (mock => mock.GetObjectIDStoragePropertyDefinition())
          .Return (_fakeObjectIDStoragePropertyDefinition).Repeat.Any();
      _infrastructureStoragePropertyDefintionProviderMock.Replay();

      _persistenceModelProvider
          .Expect (
              mock =>
              mock.GetStoragePropertyDefinition (
                  computerClassDefinition.MyPropertyDefinitions["Remotion.Data.UnitTests.DomainObjects.TestDomain.Computer.Employee"]))
          .Return (_fakeForeignKeyStoragePropertyDefinition);
      _persistenceModelProvider.Replay();

      _storageNameProviderMock
          .Expect (mock => mock.GetForeignKeyConstraintName (
              Arg.Is (computerClassDefinition),
              Arg<IEnumerable<ColumnDefinition>>.Is.Equal (new[] { _fakeForeignKeyStoragePropertyDefinition.GetColumnForLookup() })))
          .Return ("FakeConstraintName");
      _storageNameProviderMock
          .Expect (mock => mock.GetTableName (employeeClassDefinition))
          .Return (new EntityNameDefinition(null, "FakeTableName"))
          .Repeat.Times (2);
      _storageNameProviderMock.Replay();

      _storageProviderDefinitionFinderStub
          .Stub (stub => stub.GetStorageProviderDefinition (computerClassDefinition.StorageGroupType, null))
          .Return (computerClassDefinition.StorageEntityDefinition.StorageProviderDefinition);
      _storageProviderDefinitionFinderStub
          .Stub (stub => stub.GetStorageProviderDefinition (employeeClassDefinition.StorageGroupType, null))
          .Return (employeeClassDefinition.StorageEntityDefinition.StorageProviderDefinition);

      var foreignKeyConstraintDefinitions = _factory.CreateForeignKeyConstraints (computerClassDefinition).ToArray();

      _infrastructureStoragePropertyDefintionProviderMock.VerifyAllExpectations();
      _persistenceModelProvider.VerifyAllExpectations();
      _storageNameProviderMock.VerifyAllExpectations();
      Assert.That (foreignKeyConstraintDefinitions.Length, Is.EqualTo (1)); //EmployeeTransactionProperty relation property is filtered
    }

    [Test]
    public void CreateForeignKeyConstraints_NoEntityName_NoForeignKeyConstrainedIsCreated ()
    {
      var orderClassDefinition = Configuration.GetClassDefinition ("Order");

      _storageNameProviderMock
          .Expect (mock => mock.GetTableName (Arg<ClassDefinition>.Is.Anything))
          .Return (null)
          .Repeat.Any();
      _storageNameProviderMock.Replay();

      _storageProviderDefinitionFinderStub
          .Stub (stub => stub.GetStorageProviderDefinition (Arg<Type>.Is.Anything, Arg<string>.Is.Anything))
          .Return (orderClassDefinition.StorageEntityDefinition.StorageProviderDefinition);

      var result = _factory.CreateForeignKeyConstraints (orderClassDefinition).ToArray();

      Assert.That (result.Length, Is.EqualTo (0));
    }
  }
}
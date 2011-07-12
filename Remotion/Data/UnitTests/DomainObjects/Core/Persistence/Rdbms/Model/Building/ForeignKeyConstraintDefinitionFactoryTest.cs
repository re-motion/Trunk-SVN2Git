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
using System.Linq;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence;
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
    private IColumnDefinitionResolver _columnDefintionResolverMock;
    private IRdbmsStoragePropertyDefinitionFactory _rdbmsStoragePropertyDefintionFactoryMock;
    private ForeignKeyConstraintDefinitionFactory _factory;
    private ObjectIDStoragePropertyDefinition _fakeIdColumnDefinition;
    private SimpleStoragePropertyDefinition _fakeColumnDefintion;
    private ObjectIDStoragePropertyDefinition _fakeForeignColumnDefinition;
    private IStorageProviderDefinitionFinder _storageProviderDefinitionFinderStub;

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp();

      _fakeIdColumnDefinition = new ObjectIDStoragePropertyDefinition (
          SimpleStoragePropertyDefinitionObjectMother.ObjectIDProperty, SimpleStoragePropertyDefinitionObjectMother.ClassIDProperty);
      _fakeForeignColumnDefinition = new ObjectIDStoragePropertyDefinition (
          SimpleStoragePropertyDefinitionObjectMother.CreateStorageProperty ("OrderID"),
          SimpleStoragePropertyDefinitionObjectMother.CreateStorageProperty ("ClassID"));

      _fakeColumnDefintion = SimpleStoragePropertyDefinitionObjectMother.CreateStorageProperty();

      _storageNameProviderMock = MockRepository.GenerateStrictMock<IStorageNameProvider>();
      _columnDefintionResolverMock = MockRepository.GenerateStrictMock<IColumnDefinitionResolver>();
      _rdbmsStoragePropertyDefintionFactoryMock = MockRepository.GenerateStrictMock<IRdbmsStoragePropertyDefinitionFactory>();
      _storageProviderDefinitionFinderStub = MockRepository.GenerateStub<IStorageProviderDefinitionFinder>();
      _factory = new ForeignKeyConstraintDefinitionFactory (
          _storageNameProviderMock, _columnDefintionResolverMock, _rdbmsStoragePropertyDefintionFactoryMock, _storageProviderDefinitionFinderStub);
    }

    [Test]
    public void CreateForeignKeyConstraints ()
    {
      var orderClassDefinition = Configuration.GetClassDefinition ("Order");
      var customerClassDefintion = Configuration.GetClassDefinition ("Customer");
      var officialClassDefinition = Configuration.GetClassDefinition ("Official");

      _rdbmsStoragePropertyDefintionFactoryMock
          .Expect (mock => mock.CreateObjectIDColumnDefinition())
          .Return (_fakeIdColumnDefinition.ValueProperty.ColumnDefinition);
      _rdbmsStoragePropertyDefintionFactoryMock.Replay();

      _columnDefintionResolverMock
          .Expect (
              mock =>
              mock.GetColumnDefinition (orderClassDefinition.MyPropertyDefinitions["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.Customer"]))
          .Return (_fakeForeignColumnDefinition);
      _columnDefintionResolverMock.Replay();

      _storageNameProviderMock
          .Expect (mock => mock.GetForeignKeyConstraintName (orderClassDefinition, _fakeForeignColumnDefinition))
          .Return ("FakeConstraintName");
      _storageNameProviderMock
          .Expect (mock => mock.GetTableName (customerClassDefintion))
          .Return ("FakeTableName")
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

      _rdbmsStoragePropertyDefintionFactoryMock.VerifyAllExpectations();
      _columnDefintionResolverMock.VerifyAllExpectations();
      _storageNameProviderMock.VerifyAllExpectations();

      //OrderItem and OrderTicket endpoints are virtual and Official endpoint has different storage provider
      Assert.That (foreignKeyConstraintDefinitions.Length, Is.EqualTo (1));
      var foreignKeyConstraint = foreignKeyConstraintDefinitions[0];
      Assert.That (foreignKeyConstraint.ReferencedTableName.EntityName, Is.EqualTo ("FakeTableName"));
      Assert.That (foreignKeyConstraint.ReferencedTableName.SchemaName, Is.Null);
      Assert.That (foreignKeyConstraint.ConstraintName, Is.EqualTo ("FakeConstraintName"));
      Assert.That (foreignKeyConstraint.ReferencingColumns, Is.EqualTo (new[] { _fakeIdColumnDefinition.ValueProperty.ColumnDefinition }));
      Assert.That (foreignKeyConstraint.ReferencedColumns, Is.EqualTo (new[] { _fakeForeignColumnDefinition.ValueProperty.ColumnDefinition }));
    }

    [Test]
    public void CreateForeignKeyConstraints_StorageClassTransactionPropertiesAreIgnored ()
    {
      var computerClassDefinition = Configuration.GetClassDefinition ("Computer");
      var employeeClassDefinition = Configuration.GetClassDefinition ("Employee");

      _rdbmsStoragePropertyDefintionFactoryMock
          .Expect (mock => mock.CreateObjectIDColumnDefinition())
          .Return (_fakeIdColumnDefinition.ValueProperty.ColumnDefinition).Repeat.Any();
      _rdbmsStoragePropertyDefintionFactoryMock.Replay();

      _columnDefintionResolverMock
          .Expect (
              mock =>
              mock.GetColumnDefinition (
                  computerClassDefinition.MyPropertyDefinitions["Remotion.Data.UnitTests.DomainObjects.TestDomain.Computer.Employee"]))
          .Return (_fakeForeignColumnDefinition);
      _columnDefintionResolverMock.Replay();

      _storageNameProviderMock
          .Expect (mock => mock.GetForeignKeyConstraintName (computerClassDefinition, _fakeForeignColumnDefinition))
          .Return ("FakeConstraintName");
      _storageNameProviderMock
          .Expect (mock => mock.GetTableName (employeeClassDefinition))
          .Return ("FakeTableName")
          .Repeat.Times (2);
      _storageNameProviderMock.Replay();

      _storageProviderDefinitionFinderStub
          .Stub (stub => stub.GetStorageProviderDefinition (computerClassDefinition.StorageGroupType, null))
          .Return (computerClassDefinition.StorageEntityDefinition.StorageProviderDefinition);
      _storageProviderDefinitionFinderStub
          .Stub (stub => stub.GetStorageProviderDefinition (employeeClassDefinition.StorageGroupType, null))
          .Return (employeeClassDefinition.StorageEntityDefinition.StorageProviderDefinition);

      var foreignKeyConstraintDefinitions = _factory.CreateForeignKeyConstraints (computerClassDefinition).ToArray();

      _rdbmsStoragePropertyDefintionFactoryMock.VerifyAllExpectations();
      _columnDefintionResolverMock.VerifyAllExpectations();
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

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "The non virtual constraint column definition has to be an ID column definition.")]
    public void CreateForeignKeyConstraints_NonVirtualColumnDefinitionIsNoIDColumnDefinition_ThrowsException ()
    {
      var orderClassDefinition = Configuration.GetClassDefinition ("Order");
      var officialClassDefinition = Configuration.GetClassDefinition ("Official");

      _storageNameProviderMock
          .Expect (mock => mock.GetTableName (officialClassDefinition))
          .Return ("FakeTableName");
      _storageNameProviderMock.Replay();

      _columnDefintionResolverMock
          .Expect (
              mock =>
              mock.GetColumnDefinition (Arg<PropertyDefinition>.Is.Anything))
          .Return (_fakeColumnDefintion);
      _columnDefintionResolverMock.Replay();

      _rdbmsStoragePropertyDefintionFactoryMock
          .Expect (mock => mock.CreateObjectIDColumnDefinition())
          .Return (_fakeIdColumnDefinition.ValueProperty.ColumnDefinition);
      _rdbmsStoragePropertyDefintionFactoryMock.Replay();

      _storageProviderDefinitionFinderStub
          .Stub (stub => stub.GetStorageProviderDefinition (Arg<Type>.Is.Anything, Arg<string>.Is.Anything))
          .Return (orderClassDefinition.StorageEntityDefinition.StorageProviderDefinition);

      _columnDefintionResolverMock
          .Expect (
              mock =>
              mock.GetColumnDefinition (orderClassDefinition.MyPropertyDefinitions["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.Customer"]))
          .Return (_fakeColumnDefintion);
      _columnDefintionResolverMock.Replay();

      _factory.CreateForeignKeyConstraints (orderClassDefinition).ToArray();
    }
  }
}
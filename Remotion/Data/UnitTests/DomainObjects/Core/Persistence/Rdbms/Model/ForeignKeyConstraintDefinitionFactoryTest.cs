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
using System.Linq;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.Model
{
  [TestFixture]
  public class ForeignKeyConstraintDefinitionFactoryTest : StandardMappingTest
  {
    private IStorageNameCalculator _storageNameCalculatorMock;
    private IColumnDefinitionResolver _columnDefintionResolverMock;
    private IColumnDefinitionFactory _columnDefintionFactoryMock;
    private ForeignKeyConstraintDefinitionFactory _factory;
    private IDColumnDefinition _fakeIdColumnDefinition;
    private SimpleColumnDefinition _fakeColumnDefintion;
    private IDColumnDefinition _fakeForeignColumnDefinition;
    private IStorageProviderDefinitionFinder _storageProviderDefinitionFinderStub;

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp();

      _fakeIdColumnDefinition = new IDColumnDefinition (
          new SimpleColumnDefinition ("ID", typeof (ObjectID), "uniqueidentifier", false, true),
          new SimpleColumnDefinition ("ClassID", typeof (string), "varchar", false, false));
      _fakeForeignColumnDefinition = new IDColumnDefinition (
          new SimpleColumnDefinition ("OrderID", typeof (ObjectID), "uniqueidentifier", false, false),
          new SimpleColumnDefinition ("ClassID", typeof (string), "varchar", false, false));

      _fakeColumnDefintion = new SimpleColumnDefinition ("FakeColumn", typeof (string), "varchar", true, false);

      _storageNameCalculatorMock = MockRepository.GenerateStrictMock<IStorageNameCalculator>();
      _columnDefintionResolverMock = MockRepository.GenerateStrictMock<IColumnDefinitionResolver>();
      _columnDefintionFactoryMock = MockRepository.GenerateStrictMock<IColumnDefinitionFactory>();
      _storageProviderDefinitionFinderStub = MockRepository.GenerateStub<IStorageProviderDefinitionFinder>();
      _factory = new ForeignKeyConstraintDefinitionFactory (
          _storageNameCalculatorMock, _columnDefintionResolverMock, _columnDefintionFactoryMock, _storageProviderDefinitionFinderStub);
    }

    [Test]
    public void CreateForeignKeyConstraints ()
    {
      var orderClassDefinition = Configuration.ClassDefinitions["Order"];
      var customerClassDefintion = Configuration.ClassDefinitions["Customer"];
      var officialClassDefinition = Configuration.ClassDefinitions["Official"];
      
      _columnDefintionFactoryMock
          .Expect (mock => mock.CreateIDColumnDefinition())
          .Return (_fakeIdColumnDefinition);
      _columnDefintionFactoryMock.Replay();

      _columnDefintionResolverMock
          .Expect (
              mock =>
              mock.GetColumnDefinition (orderClassDefinition.MyPropertyDefinitions["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.Customer"]))
          .Return (_fakeForeignColumnDefinition);
      _columnDefintionResolverMock.Replay();

      _storageNameCalculatorMock
          .Expect (mock => mock.GetForeignKeyConstraintName (orderClassDefinition, _fakeForeignColumnDefinition))
          .Return ("FakeConstraintName");
      _storageNameCalculatorMock
          .Expect (mock => mock.GetTableName (customerClassDefintion))
          .Return ("FakeTableName");
      _storageNameCalculatorMock.Replay();

      _storageProviderDefinitionFinderStub
          .Stub (stub => stub.GetStorageProviderDefinition (orderClassDefinition))
          .Return (orderClassDefinition.StorageEntityDefinition.StorageProviderDefinition);
      _storageProviderDefinitionFinderStub
          .Stub (stub => stub.GetStorageProviderDefinition (customerClassDefintion))
          .Return (customerClassDefintion.StorageEntityDefinition.StorageProviderDefinition);
      _storageProviderDefinitionFinderStub
          .Stub (stub => stub.GetStorageProviderDefinition (officialClassDefinition))
          .Return (officialClassDefinition.StorageEntityDefinition.StorageProviderDefinition);

      var foreignKeyConstraintDefinitions = _factory.CreateForeignKeyConstraints (orderClassDefinition).ToArray();

      _columnDefintionFactoryMock.VerifyAllExpectations();
      _columnDefintionResolverMock.VerifyAllExpectations();
      _storageNameCalculatorMock.VerifyAllExpectations();

      //OrderItem and OrderTicket endpoints are virtual and Official endpoint has different storage provider
      Assert.That (foreignKeyConstraintDefinitions.Length, Is.EqualTo (1));
      var foreignKeyConstraint = foreignKeyConstraintDefinitions[0];
      Assert.That (foreignKeyConstraint.ReferencedTableName, Is.EqualTo ("FakeTableName"));
      Assert.That (foreignKeyConstraint.ConstraintName, Is.EqualTo ("FakeConstraintName"));
      Assert.That (foreignKeyConstraint.ReferencingColumns, Is.EqualTo (new[] { _fakeIdColumnDefinition.ObjectIDColumn }));
      Assert.That (foreignKeyConstraint.ReferencedColumns, Is.EqualTo (new[] { _fakeForeignColumnDefinition.ObjectIDColumn }));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "The non virtual constraint column definition has to be an ID column definition.")]
    public void CreateForeignKeyConstraints_NonVirtualColumnDefinitionIsNoIDColumnDefinition_ThrowsException ()
    {
      var classDefinition = Configuration.ClassDefinitions["Order"];

      _columnDefintionResolverMock
          .Expect (
              mock =>
              mock.GetColumnDefinition (Arg<PropertyDefinition>.Is.Anything))
          .Return (_fakeColumnDefintion);
      _columnDefintionResolverMock.Replay();

      _columnDefintionFactoryMock
          .Expect (mock => mock.CreateIDColumnDefinition())
          .Return (_fakeIdColumnDefinition);
      _columnDefintionFactoryMock.Replay();

      _storageProviderDefinitionFinderStub
          .Stub (stub => stub.GetStorageProviderDefinition (Arg<ClassDefinition>.Is.Anything))
          .Return (classDefinition.StorageEntityDefinition.StorageProviderDefinition);

      _columnDefintionResolverMock
          .Expect (
              mock =>
              mock.GetColumnDefinition (classDefinition.MyPropertyDefinitions["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.Customer"]))
          .Return (_fakeColumnDefintion);
      _columnDefintionResolverMock.Replay();

      _factory.CreateForeignKeyConstraints (classDefinition).ToArray();
    }
  }
}
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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Rhino.Mocks;
using System.Linq;

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
      _factory = new ForeignKeyConstraintDefinitionFactory(_storageNameCalculatorMock, _columnDefintionResolverMock, _columnDefintionFactoryMock);
    }

    [Test]
    public void CreateForeignKeyConstraints ()
    {
      var classDefintion = Configuration.ClassDefinitions["Order"];

      _columnDefintionFactoryMock
          .Expect (mock => mock.CreateIDColumnDefinition())
          .Return (_fakeIdColumnDefinition);
      _columnDefintionFactoryMock.Replay();

      _columnDefintionResolverMock
          .Expect (mock => mock.GetColumnDefinition (Arg<PropertyDefinition>.Is.Anything))
          .Return (_fakeForeignColumnDefinition);
      _columnDefintionResolverMock.Replay();

      _storageNameCalculatorMock
          .Expect (mock => mock.GetForeignKeyConstraintName (classDefintion, _fakeForeignColumnDefinition))
          .Return ("FakeConstraintName");
      _storageNameCalculatorMock
          .Expect (mock => mock.GetTableName (Configuration.ClassDefinitions["Customer"]))
          .Return ("FakeTableName");
      _storageNameCalculatorMock.Replay();

      var foreignKeyConstraintDefinitions = _factory.CreateForeignKeyConstraints (classDefintion).ToArray();
      
      _columnDefintionFactoryMock.VerifyAllExpectations();
      _columnDefintionResolverMock.VerifyAllExpectations();
      _storageNameCalculatorMock.VerifyAllExpectations();

      Assert.That (foreignKeyConstraintDefinitions.Length, Is.EqualTo (1));
      //TODO: check foreign key constraint defintion
    }

    //TODO: tests for exception if foreign key column is no IDColumn !
  }
}
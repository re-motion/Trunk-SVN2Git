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
using System.Reflection;
using NUnit.Framework;
using Remotion.Collections;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Mapping.SortExpressions;
using Remotion.Data.DomainObjects.Persistence.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders.Specifications;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping;
using Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.Model;
using Remotion.Data.UnitTests.DomainObjects.Factories;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.DbCommandBuilders.Specifications
{
  [TestFixture]
  public class OrderedColumnsSpecificationFactoryTest : StandardMappingTest
  {
    private OrderedColumnsSpecificationFactory _factory;
    private IRdbmsPersistenceModelProvider _rdbmsPersistenceModelProviderStub;
    private ColumnDefinition _column1;
    private ColumnDefinition _column2;
    private ColumnDefinition _column3;

    public override void SetUp ()
    {
      base.SetUp();

      _rdbmsPersistenceModelProviderStub = MockRepository.GenerateStub<IRdbmsPersistenceModelProvider>();
      _factory = new OrderedColumnsSpecificationFactory(_rdbmsPersistenceModelProviderStub);

      _column1 = ColumnDefinitionObjectMother.CreateColumn ("Column1");
      _column2 = ColumnDefinitionObjectMother.CreateColumn ("Column2");
      _column3 = ColumnDefinitionObjectMother.CreateColumn ("Column3");
    }

    [Test]
    public void CreateOrderedColumnsSpecification ()
    {
      var tableDefinition = TableDefinitionObjectMother.Create (TestDomainStorageProviderDefinition, new EntityNameDefinition (null, "Table1"));
      var classDefinition = CreateClassDefinition (tableDefinition);

      var rdbmsStoragePropertyDefinitionStub1 = MockRepository.GenerateStub<IRdbmsStoragePropertyDefinition>();
      rdbmsStoragePropertyDefinitionStub1.Stub (stub => stub.GetColumns()).Return (new[] { _column1, _column2 });
      var rdbmsStoragePropertyDefinitionStub2 = MockRepository.GenerateStub<IRdbmsStoragePropertyDefinition> ();
      rdbmsStoragePropertyDefinitionStub2.Stub (stub => stub.GetColumns ()).Return (new[] { _column3 });

      var spec1 = CreateSortedPropertySpecification (
          classDefinition,
          SortOrder.Descending,
          ColumnDefinitionObjectMother.IDColumn,
          ColumnDefinitionObjectMother.ClassIDColumn);
      var spec2 = CreateSortedPropertySpecification (classDefinition, SortOrder.Ascending, ColumnDefinitionObjectMother.TimestampColumn);

      _rdbmsPersistenceModelProviderStub.Stub (stub => stub.GetStoragePropertyDefinition (spec1.PropertyDefinition)).Return (
          rdbmsStoragePropertyDefinitionStub1);
      _rdbmsPersistenceModelProviderStub.Stub (stub => stub.GetStoragePropertyDefinition (spec2.PropertyDefinition)).Return (
          rdbmsStoragePropertyDefinitionStub2);

      var sortExpressionDefinition = new SortExpressionDefinition (new[] { spec1, spec2 });

      var result = _factory.CreateOrderedColumnsSpecification (sortExpressionDefinition);

      var expectedOrderedColumns = new[]
                                   {
                                       new OrderedColumn(_column1, SortOrder.Descending),
                                       new OrderedColumn(_column2, SortOrder.Descending),
                                       new OrderedColumn(_column3, SortOrder.Ascending)
                                   };

      Assert.That (result, Is.TypeOf(typeof(OrderedColumnsSpecification)));
      Assert.That (((OrderedColumnsSpecification) result).Columns, Is.EqualTo (expectedOrderedColumns));
    }

    private ClassDefinition CreateClassDefinition (IStorageEntityDefinition entityDefinition)
    {
      var classDefinition = ClassDefinitionFactory.CreateClassDefinitionWithoutStorageEntity (typeof (Order), null);
      classDefinition.SetStorageEntity (entityDefinition);
      return classDefinition;
    }

    private SortedPropertySpecification CreateSortedPropertySpecification (
        ClassDefinition classDefinition,
        SortOrder sortOrder,
        ColumnDefinition sortedColumn)
    {
      return CreateSortedPropertySpecification (
          classDefinition,
          typeof (Order).GetProperty ("OrderNumber"),
          new SimpleStoragePropertyDefinition (sortedColumn),
          sortOrder);
    }

    private SortedPropertySpecification CreateSortedPropertySpecification (
        ClassDefinition classDefinition,
        SortOrder sortOrder,
        ColumnDefinition sortedColumn1,
        ColumnDefinition sortedColumn2)
    {
      return CreateSortedPropertySpecification (
          classDefinition,
          typeof (Order).GetProperty ("OrderNumber"),
          new ObjectIDStoragePropertyDefinition (
              new SimpleStoragePropertyDefinition (sortedColumn1), new SimpleStoragePropertyDefinition (sortedColumn2)),
          sortOrder);
    }

    private SortedPropertySpecification CreateSortedPropertySpecification (
        ClassDefinition classDefinition, PropertyInfo propertyInfo, IStoragePropertyDefinition columnDefinition, SortOrder sortOrder)
    {
      var sortedPropertyDefinition = PropertyDefinitionFactory.Create (
          classDefinition, StorageClass.Persistent, propertyInfo, columnDefinition);
      return new SortedPropertySpecification (sortedPropertyDefinition, sortOrder);
    }
  }
}
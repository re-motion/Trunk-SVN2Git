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
using System;
using System.Data;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.UnitTests.DomainObjects.Factories;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.Model
{
  [TestFixture]
  public class SimpleStoragePropertyDefinitionTest
  {
    private IStorageTypeInformation _storageTypeInformationStub;
    private ColumnDefinition _innerColumnDefinition;
    private SimpleStoragePropertyDefinition _storagePropertyDefinition;

    private IDbCommand _dbCommandStub;
    private IDbDataParameter _dbDataParameterStub;

    [SetUp]
    public void SetUp ()
    {
      _storageTypeInformationStub = MockRepository.GenerateStub<IStorageTypeInformation>();
      _innerColumnDefinition = ColumnDefinitionObjectMother.CreateColumn (storageTypeInformation: _storageTypeInformationStub);
      _storagePropertyDefinition = new SimpleStoragePropertyDefinition (typeof (string), _innerColumnDefinition);

      _dbCommandStub = MockRepository.GenerateStub<IDbCommand>();
      _dbDataParameterStub = MockRepository.GenerateStub<IDbDataParameter> ();
      _dbCommandStub.Stub (stub => stub.CreateParameter ()).Return (_dbDataParameterStub);
    }

    [Test]
    public void ColumnDefinition ()
    {
      Assert.That (_storagePropertyDefinition.ColumnDefinition, Is.SameAs (_innerColumnDefinition));
    }

    [Test]
    public void GetColumns ()
    {
      Assert.That (_storagePropertyDefinition.GetColumns (), Is.EqualTo (new[] { _innerColumnDefinition }));
    }

    [Test]
    public void GetColumnsForComparison ()
    {
      Assert.That (_storagePropertyDefinition.GetColumnsForComparison(), Is.EqualTo (new[] { _innerColumnDefinition }));
    }

    [Test]
    public void SplitValue ()
    {
      var value = new object();

      var result = _storagePropertyDefinition.SplitValue (value);

      Assert.That (result, Is.EqualTo (new[] { new ColumnValue(_innerColumnDefinition, value) }));
    }

    [Test]
    public void SplitValue_NullValue ()
    {
      var result = _storagePropertyDefinition.SplitValue (null);

      Assert.That (result, Is.EqualTo (new[] { new ColumnValue (_innerColumnDefinition, null) }));
    }

    [Test]
    public void SplitValueForComparison ()
    {
      var value = new object ();

      var result = _storagePropertyDefinition.SplitValueForComparison (value);

      Assert.That (result, Is.EqualTo (new[] { new ColumnValue (_innerColumnDefinition, value) }));
    }

    [Test]
    public void SplitValueForComparison_NullValue ()
    {
      var result = _storagePropertyDefinition.SplitValueForComparison (null);

      Assert.That (result, Is.EqualTo (new[] { new ColumnValue (_innerColumnDefinition, null) }));
    }

    [Test]
    public void SplitValuesForComparison ()
    {
      var value1 = new object ();
      var value2 = new object ();

      var result = _storagePropertyDefinition.SplitValuesForComparison (new[] { value1, value2 });

      var expectedTable = new ColumnValueTable (
          new[] { _innerColumnDefinition }, 
          new[]
          {
              new ColumnValueTable.Row (new[] { value1 }), 
              new ColumnValueTable.Row (new[] { value2 }),
          });
      ColumnValueTableTestHelper.CheckTable (expectedTable, result);
    }

    [Test]
    public void SplitValuesForComparison_NullValue ()
    {
      var value2 = new object ();

      var result = _storagePropertyDefinition.SplitValuesForComparison (new[] { null, value2 });

      var expectedTable = new ColumnValueTable (
          new[] { _innerColumnDefinition },
          new[]
          {
              new ColumnValueTable.Row (new object[] { null }), 
              new ColumnValueTable.Row (new[] { value2 }),
          });
      ColumnValueTableTestHelper.CheckTable (expectedTable, result);
    }

    [Test]
    public void CombineValue ()
    {
      var columnValueProviderStub = MockRepository.GenerateStub<IColumnValueProvider>();
      columnValueProviderStub.Stub (stub => stub.GetValueForColumn (_innerColumnDefinition)).Return (12);

      var result = _storagePropertyDefinition.CombineValue (columnValueProviderStub);

      Assert.That (result, Is.EqualTo (12));
    }
  }
}
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
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.UnitTests.DomainObjects.Factories;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.Model
{
  [TestFixture]
  public class CompoundStoragePropertyDefinitionTest
  {
    private CompoundStoragePropertyDefinition _compoundStoragePropertyDefinition;
    private IRdbmsStoragePropertyDefinition _property1Stub;
    private IRdbmsStoragePropertyDefinition _property2Stub;
    private IRdbmsStoragePropertyDefinition _property3Stub;
    private CompoundStoragePropertyDefinition.NestedPropertyInfo _yearProperty;
    private CompoundStoragePropertyDefinition.NestedPropertyInfo _monthProperty;
    private CompoundStoragePropertyDefinition.NestedPropertyInfo _dayProperty;
    private ColumnDefinition _columnDefinition1;
    private ColumnDefinition _columnDefinition2;
    private ColumnDefinition _columnDefinition3;

    [SetUp]
    public void SetUp ()
    {
      var storageTypeInformation = StorageTypeInformationObjectMother.CreateStorageTypeInformation();
      _columnDefinition1 = ColumnDefinitionObjectMother.CreateColumn (storageTypeInformation: storageTypeInformation);
      _columnDefinition2 = ColumnDefinitionObjectMother.CreateColumn (storageTypeInformation: storageTypeInformation);
      _columnDefinition3 = ColumnDefinitionObjectMother.CreateColumn (storageTypeInformation: storageTypeInformation);

      _property1Stub = MockRepository.GenerateStub<IRdbmsStoragePropertyDefinition>();
      _property2Stub = MockRepository.GenerateStub<IRdbmsStoragePropertyDefinition>();
      _property3Stub = MockRepository.GenerateStub<IRdbmsStoragePropertyDefinition>();

      _yearProperty = new CompoundStoragePropertyDefinition.NestedPropertyInfo (_property1Stub, o => ((DateTime) o).Year);
      _monthProperty = new CompoundStoragePropertyDefinition.NestedPropertyInfo (_property2Stub, o => ((DateTime) o).Month);
      _dayProperty = new CompoundStoragePropertyDefinition.NestedPropertyInfo (_property3Stub, o => ((DateTime) o).Day);

      _compoundStoragePropertyDefinition = new CompoundStoragePropertyDefinition (
          typeof (DateTime),
          new[] { _yearProperty, _monthProperty, _dayProperty },
          objects => new DateTime ((int) objects[0], (int) objects[1], (int) objects[2]));
    }

    [Test]
    public void GetColumns ()
    {
      _property1Stub.Stub (stub => stub.GetColumns()).Return (new[] { _columnDefinition1 });
      _property2Stub.Stub (stub => stub.GetColumns()).Return (new[] { _columnDefinition2 });
      _property3Stub.Stub (stub => stub.GetColumns()).Return (new[] { _columnDefinition3 });

      var result = _compoundStoragePropertyDefinition.GetColumns();

      Assert.That (result, Is.EqualTo (new[] { _columnDefinition1, _columnDefinition2, _columnDefinition3 }));
    }

    [Test]
    public void GetColumnsForComparison ()
    {
      _property1Stub.Stub (stub => stub.GetColumnsForComparison ()).Return (new[] { _columnDefinition1 });
      _property2Stub.Stub (stub => stub.GetColumnsForComparison ()).Return (new[] { _columnDefinition2 });
      _property3Stub.Stub (stub => stub.GetColumnsForComparison ()).Return (new[] { _columnDefinition3 });

      var result = _compoundStoragePropertyDefinition.GetColumnsForComparison ();

      Assert.That (result, Is.EqualTo (new[] { _columnDefinition1, _columnDefinition2, _columnDefinition3 }));
    }

    [Test]
    public void SplitValue ()
    {
      var dateTime = new DateTime (2011, 7, 18);
      var columnValue1 = new ColumnValue (_columnDefinition1, dateTime);
      var columnValue2 = new ColumnValue (_columnDefinition2, dateTime);
      var columnValue3 = new ColumnValue (_columnDefinition3, dateTime);

      _property1Stub.Stub (stub => stub.SplitValue (2011)).Return (new[] { columnValue1 });
      _property2Stub.Stub (stub => stub.SplitValue (7)).Return (new[] { columnValue2 });
      _property3Stub.Stub (stub => stub.SplitValue (18)).Return (new[] { columnValue3 });

      var result = _compoundStoragePropertyDefinition.SplitValue (dateTime).ToArray();

      Assert.That (result, Is.EqualTo (new[] { columnValue1, columnValue2, columnValue3 }));
    }

    [Test]
    public void SplitValueForComparison ()
    {
      var dateTime = new DateTime (2011, 7, 18);
      var columnValue1 = new ColumnValue (_columnDefinition1, dateTime);
      var columnValue2 = new ColumnValue (_columnDefinition2, dateTime);
      var columnValue3 = new ColumnValue (_columnDefinition3, dateTime);

      _property1Stub.Stub (stub => stub.SplitValueForComparison (2011)).Return (new[] { columnValue1 });
      _property2Stub.Stub (stub => stub.SplitValueForComparison (7)).Return (new[] { columnValue2 });
      _property3Stub.Stub (stub => stub.SplitValueForComparison (18)).Return (new[] { columnValue3 });

      var result = _compoundStoragePropertyDefinition.SplitValueForComparison (dateTime).ToArray ();

      Assert.That (result, Is.EqualTo (new[] { columnValue1, columnValue2, columnValue3 }));
    }

    [Test]
    public void SplitValuesForComparison ()
    {
      var dateTime1 = new DateTime (2011, 7, 18);
      var dateTime2 = new DateTime (2012, 8, 19);

      var row1 = new ColumnValueTable.Row (new object[] { "2011" });
      var row2 = new ColumnValueTable.Row (new object[] { "2012" });
      var row3 = new ColumnValueTable.Row (new object[] { "7" });
      var row4 = new ColumnValueTable.Row (new object[] { "8" });
      var row5 = new ColumnValueTable.Row (new object[] { "18" });
      var row6 = new ColumnValueTable.Row (new object[] { "19" });

      _property1Stub
          .Stub (stub => stub.SplitValuesForComparison (Arg<IEnumerable<object>>.List.Equal (new object[] { 2011, 2012 })))
          .Return (new ColumnValueTable(new[] { _columnDefinition1}, new[] { row1, row2 }));
      _property2Stub
          .Stub (stub => stub.SplitValuesForComparison (Arg<IEnumerable<object>>.List.Equal (new object[] { 7, 8 })))
          .Return (new ColumnValueTable(new[] { _columnDefinition2}, new[] { row3, row4 }));
      _property3Stub
          .Stub (stub => stub.SplitValuesForComparison (Arg<IEnumerable<object>>.List.Equal (new object[] { 18, 19 })))
          .Return (new ColumnValueTable (new[] { _columnDefinition3 }, new[] { row5, row6 }));

      var result = _compoundStoragePropertyDefinition.SplitValuesForComparison (new object[] { dateTime1, dateTime2 });

      var expectedTable = new ColumnValueTable (
          new[] { _columnDefinition1, _columnDefinition2, _columnDefinition3 }, 
          new[]
          {
              new ColumnValueTable.Row (new object[] { "2011", "7", "18" }), 
              new ColumnValueTable.Row (new object[] { "2012", "8", "19" })
          });

      ColumnValueTableTestHelper.CheckTable (expectedTable, result);
    }

    [Test]
    public void CombineValue ()
    {
      var columnValueProviderStub = MockRepository.GenerateStub<IColumnValueProvider> ();

      _property1Stub.Stub (stub => stub.CombineValue (columnValueProviderStub)).Return (2011);
      _property2Stub.Stub (stub => stub.CombineValue (columnValueProviderStub)).Return (5);
      _property3Stub.Stub (stub => stub.CombineValue (columnValueProviderStub)).Return (17);

      var result = _compoundStoragePropertyDefinition.CombineValue (columnValueProviderStub);

      Assert.That (result, Is.EqualTo (new DateTime (2011, 5, 17)));
    }
  }
}
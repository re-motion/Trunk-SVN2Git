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
using NUnit.Framework;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.UnitTests.Core.BindableObject;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.UnitTests.Web.Domain;

namespace Remotion.ObjectBinding.UnitTests.Web.UI.Controls.BocListImplementation.Sorting
{
  [TestFixture]
  public class CellValueComparerStringValuesTest : CellValueComparerTestBase
  {
    private IBusinessObject _valueAA;
    private IBusinessObject _valueAB;
    private IBusinessObject _valueBA;

    private IBusinessObject _valueNullA;
    private IBusinessObject _valueNullB;
    private IBusinessObject _valueBNull;

    private BindableObjectClass _class;

    private IBusinessObjectPropertyPath _firstValuePath;
    private IBusinessObjectPropertyPath _secondValuePath;

    private BocSimpleColumnDefinition _firstValueSimpleColumn;
    private BocSimpleColumnDefinition _secondValueSimpleColumn;

    private BocCompoundColumnDefinition _firstValueFirstValueCompoundColumn;
    private BocCompoundColumnDefinition _firstValueSecondValueCompoundColumn;

    private BocCustomColumnDefinition _firstValueCustomColumn;
    private BocCustomColumnDefinition _secondValueCustomColumn;

    [SetUp]
    public virtual void SetUp ()
    {
      _valueAA = (IBusinessObject) TypeWithString.Create ("A", "A");
      _valueAB = (IBusinessObject) TypeWithString.Create ("A", "B");
      _valueBA = (IBusinessObject) TypeWithString.Create ("B", "A");

      _valueNullA = (IBusinessObject) TypeWithString.Create (null, "A");
      _valueNullB = (IBusinessObject) TypeWithString.Create (null, "B");
      _valueBNull = (IBusinessObject) TypeWithString.Create ("B", null);

      _class = BindableObjectProviderTestHelper.GetBindableObjectClass(typeof (TypeWithString));


      _firstValuePath = BusinessObjectPropertyPath.Parse (_class, "FirstValue");
      _secondValuePath = BusinessObjectPropertyPath.Parse (_class, "SecondValue");


      _firstValueSimpleColumn = new BocSimpleColumnDefinition();
      _firstValueSimpleColumn.SetPropertyPath (_firstValuePath);

      _secondValueSimpleColumn = new BocSimpleColumnDefinition();
      _secondValueSimpleColumn.SetPropertyPath (_secondValuePath);


      _firstValueFirstValueCompoundColumn = new BocCompoundColumnDefinition();
      _firstValueFirstValueCompoundColumn.PropertyPathBindings.Add (new PropertyPathBinding (_firstValuePath));
      _firstValueFirstValueCompoundColumn.PropertyPathBindings.Add (new PropertyPathBinding (_firstValuePath));
      _firstValueFirstValueCompoundColumn.FormatString = "{0}, {1}";

      _firstValueSecondValueCompoundColumn = new BocCompoundColumnDefinition();
      _firstValueSecondValueCompoundColumn.PropertyPathBindings.Add (new PropertyPathBinding (_firstValuePath));
      _firstValueSecondValueCompoundColumn.PropertyPathBindings.Add (new PropertyPathBinding (_secondValuePath));
      _firstValueSecondValueCompoundColumn.FormatString = "{0}, {1}";


      _firstValueCustomColumn = new BocCustomColumnDefinition();
      _firstValueCustomColumn.SetPropertyPath (_firstValuePath);
      _firstValueCustomColumn.IsSortable = true;

      _secondValueCustomColumn = new BocCustomColumnDefinition();
      _secondValueCustomColumn.SetPropertyPath (_secondValuePath);
      _secondValueCustomColumn.IsSortable = true;
    }

    [Test]
    public void CompareRowsWithSimpleColumns ()
    {
      CompareEqualValues (_firstValueSimpleColumn, _valueAA, _valueAB);
      CompareEqualValues (_firstValueSimpleColumn, _valueNullA, _valueNullB);

      CompareAscendingValues (_firstValueSimpleColumn, _valueAA, _valueBA);
      CompareAscendingValues (_firstValueSimpleColumn, _valueNullA, _valueAA);

      CompareDescendingValues (_firstValueSimpleColumn, _valueBA, _valueAA);
      CompareDescendingValues (_firstValueSimpleColumn, _valueAA, _valueNullA);
    }

    [Test]
    public void CompareRowsWithCompoundColumns ()
    {
      CompareEqualValues (_firstValueFirstValueCompoundColumn, _valueAA, _valueAB);
      CompareEqualValues (_firstValueFirstValueCompoundColumn, _valueNullA, _valueNullB);

      CompareAscendingValues (_firstValueSecondValueCompoundColumn, _valueAA, _valueBA);
      CompareAscendingValues (_firstValueSecondValueCompoundColumn, _valueAA, _valueAB);
      CompareAscendingValues (_firstValueSecondValueCompoundColumn, _valueNullA, _valueBNull);
      CompareAscendingValues (_firstValueSecondValueCompoundColumn, _valueNullA, _valueNullB);

      CompareDescendingValues (_firstValueSecondValueCompoundColumn, _valueBA, _valueAA);
      CompareDescendingValues (_firstValueSecondValueCompoundColumn, _valueAB, _valueAA);
      CompareDescendingValues (_firstValueSecondValueCompoundColumn, _valueBNull, _valueNullA);
      CompareDescendingValues (_firstValueSecondValueCompoundColumn, _valueNullB, _valueNullA);
    }


    [Test]
    public void CompareRowsWithCustomColumns ()
    {
      CompareEqualValues (_firstValueCustomColumn, _valueAA, _valueAB);
      CompareEqualValues (_firstValueCustomColumn, _valueNullA, _valueNullB);

      CompareAscendingValues (_firstValueCustomColumn, _valueAA, _valueBA);
      CompareAscendingValues (_firstValueCustomColumn, _valueNullA, _valueBA);

      CompareDescendingValues (_firstValueCustomColumn, _valueBA, _valueAA);
      CompareDescendingValues (_firstValueCustomColumn, _valueBA, _valueNullA);
    }
  }
}

/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using NUnit.Framework;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.UnitTests.Web.Domain;

namespace Remotion.ObjectBinding.UnitTests.Web.UI.Controls
{
  [TestFixture]
  public class BocListRowsCompareReferenceValuesTest : BaseBocListRowCompareValuesTest
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
      _valueAA = (IBusinessObject) TypeWithReference.Create (TypeWithReference.Create ("A"), TypeWithReference.Create ("A"));
      _valueAB = (IBusinessObject) TypeWithReference.Create (TypeWithReference.Create ("A"), TypeWithReference.Create ("B"));
      _valueBA = (IBusinessObject) TypeWithReference.Create (TypeWithReference.Create ("B"), TypeWithReference.Create ("A"));

      _valueNullA = (IBusinessObject) TypeWithReference.Create (null, TypeWithReference.Create ("A"));
      _valueNullB = (IBusinessObject) TypeWithReference.Create (null, TypeWithReference.Create ("B"));
      _valueBNull = (IBusinessObject) TypeWithReference.Create (TypeWithReference.Create ("B"), null);


      _class = BindableObjectProvider.GetBindableObjectClassFromProvider(typeof (TypeWithReference));


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
      CompareEqualValuesAscending (_firstValueSimpleColumn, _valueAA, _valueAB);
      CompareEqualValuesAscending (_firstValueSimpleColumn, _valueNullA, _valueNullB);

      CompareEqualValuesDescending (_firstValueSimpleColumn, _valueAA, _valueAB);
      CompareEqualValuesDescending (_firstValueSimpleColumn, _valueNullA, _valueNullB);

      CompareAscendingValuesAscending (_firstValueSimpleColumn, _valueAA, _valueBA);
      CompareAscendingValuesAscending (_firstValueSimpleColumn, _valueNullA, _valueAA);

      CompareAscendingValuesDescending (_firstValueSimpleColumn, _valueAA, _valueBA);
      CompareAscendingValuesDescending (_firstValueSimpleColumn, _valueNullA, _valueAA);
    }

    [Test]
    public void CompareRowsWithCompoundColumns ()
    {
      CompareEqualValuesAscending (_firstValueFirstValueCompoundColumn, _valueAA, _valueAB);
      CompareEqualValuesAscending (_firstValueFirstValueCompoundColumn, _valueNullA, _valueNullB);

      CompareEqualValuesDescending (_firstValueFirstValueCompoundColumn, _valueAA, _valueAB);
      CompareEqualValuesDescending (_firstValueFirstValueCompoundColumn, _valueNullA, _valueNullB);

      CompareAscendingValuesAscending (_firstValueSecondValueCompoundColumn, _valueAA, _valueBA);
      CompareAscendingValuesAscending (_firstValueSecondValueCompoundColumn, _valueAA, _valueAB);
      CompareAscendingValuesAscending (_firstValueSecondValueCompoundColumn, _valueNullA, _valueBNull);
      CompareAscendingValuesAscending (_firstValueSecondValueCompoundColumn, _valueNullA, _valueNullB);

      CompareAscendingValuesDescending (_firstValueSecondValueCompoundColumn, _valueAA, _valueBA);
      CompareAscendingValuesDescending (_firstValueSecondValueCompoundColumn, _valueAA, _valueAB);
      CompareAscendingValuesDescending (_firstValueSecondValueCompoundColumn, _valueNullA, _valueBNull);
      CompareAscendingValuesDescending (_firstValueSecondValueCompoundColumn, _valueNullA, _valueNullB);
    }


    [Test]
    public void CompareRowsWithCustomColumns ()
    {
      CompareEqualValuesAscending (_firstValueCustomColumn, _valueAA, _valueAB);
      CompareEqualValuesAscending (_firstValueCustomColumn, _valueNullA, _valueNullB);

      CompareEqualValuesDescending (_firstValueCustomColumn, _valueAA, _valueAB);
      CompareEqualValuesDescending (_firstValueCustomColumn, _valueNullA, _valueNullB);

      CompareAscendingValuesAscending (_firstValueCustomColumn, _valueAA, _valueBA);
      CompareAscendingValuesAscending (_firstValueCustomColumn, _valueNullA, _valueBA);

      CompareAscendingValuesDescending (_firstValueCustomColumn, _valueAA, _valueBA);
      CompareAscendingValuesDescending (_firstValueCustomColumn, _valueNullA, _valueBA);
    }
  }
}

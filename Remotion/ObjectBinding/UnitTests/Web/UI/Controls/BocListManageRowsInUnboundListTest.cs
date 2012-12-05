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
using NUnit.Framework;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.UnitTests.Core.BindableObject;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.UnitTests.Web.Domain;

namespace Remotion.ObjectBinding.UnitTests.Web.UI.Controls
{
  [TestFixture]
  public class BocListManageRowsInUnboundListTest : BocTest
  {
    private BocListMock _bocList;

    private IBusinessObject[] _values;
    private IBusinessObject[] _newValues;

    private BindableObjectClass _typeWithStringClass;

    private IBusinessObjectPropertyPath _typeWithStringFirstValuePath;
    private IBusinessObjectPropertyPath _typeWithStringSecondValuePath;

    private BocSimpleColumnDefinition _typeWithStringFirstValueSimpleColumn;
    private BocSimpleColumnDefinition _typeWithStringSecondValueSimpleColumn;

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp();

      _values = new IBusinessObject[5];
      _values[0] = (IBusinessObject) TypeWithString.Create ("0", "A");
      _values[1] = (IBusinessObject) TypeWithString.Create ("1", "A");
      _values[2] = (IBusinessObject) TypeWithString.Create ("2", "B");
      _values[3] = (IBusinessObject) TypeWithString.Create ("3", "B");
      _values[4] = (IBusinessObject) TypeWithString.Create ("4", "C");

      _newValues = new IBusinessObject[2];
      _newValues[0] = (IBusinessObject) TypeWithString.Create ("5", "C");
      _newValues[1] = (IBusinessObject) TypeWithString.Create ("6", "D");

      _typeWithStringClass = BindableObjectProviderTestHelper.GetBindableObjectClass(typeof (TypeWithString));

      _typeWithStringFirstValuePath = BusinessObjectPropertyPath.CreateStatic (_typeWithStringClass, "FirstValue");
      _typeWithStringSecondValuePath = BusinessObjectPropertyPath.CreateStatic (_typeWithStringClass, "SecondValue");

      _typeWithStringFirstValueSimpleColumn = new BocSimpleColumnDefinition();
      _typeWithStringFirstValueSimpleColumn.SetPropertyPath (_typeWithStringFirstValuePath);

      _typeWithStringSecondValueSimpleColumn = new BocSimpleColumnDefinition();
      _typeWithStringSecondValueSimpleColumn.SetPropertyPath (_typeWithStringSecondValuePath);

      _bocList = new BocListMock();
      _bocList.ID = "BocList";
      NamingContainer.Controls.Add (_bocList);

      _bocList.LoadUnboundValue (_values, false);
    }

    [Test]
    public void AddRow ()
    {
      int index = _bocList.AddRow (_newValues[0]);

      Assert.IsFalse (object.ReferenceEquals (_values, _bocList.Value));
      Assert.AreEqual (6, _bocList.Value.Count);
      Assert.AreSame (_values[0], _bocList.Value[0]);
      Assert.AreSame (_values[1], _bocList.Value[1]);
      Assert.AreSame (_values[2], _bocList.Value[2]);
      Assert.AreSame (_values[3], _bocList.Value[3]);
      Assert.AreSame (_values[4], _bocList.Value[4]);

      Assert.AreEqual (5, index);
      Assert.AreSame (_newValues[0], _bocList.Value[5]);
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException))]
    public void AddRowWithoutValue ()
    {
      _bocList.LoadUnboundValue (null, false);
      _bocList.AddRow (_newValues[0]);
    }

    [Test]
    public void AddRows ()
    {
      _bocList.AddRows (_newValues);

      Assert.IsFalse (object.ReferenceEquals (_values, _bocList.Value));
      Assert.AreEqual (7, _bocList.Value.Count);
      Assert.AreSame (_values[0], _bocList.Value[0]);
      Assert.AreSame (_values[1], _bocList.Value[1]);
      Assert.AreSame (_values[2], _bocList.Value[2]);
      Assert.AreSame (_values[3], _bocList.Value[3]);
      Assert.AreSame (_values[4], _bocList.Value[4]);

      Assert.AreSame (_newValues[0], _bocList.Value[5]);
      Assert.AreSame (_newValues[1], _bocList.Value[6]);
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException))]
    public void AddRowsWithoutValue ()
    {
      _bocList.LoadUnboundValue (null, false);
      _bocList.AddRows (_newValues);
    }

    [Test]
    public void RemoveRowWithIndex ()
    {
      _bocList.RemoveRow (2);

      Assert.IsFalse (object.ReferenceEquals (_values, _bocList.Value));
      Assert.AreEqual (4, _bocList.Value.Count);
      Assert.AreSame (_values[0], _bocList.Value[0]);
      Assert.AreSame (_values[1], _bocList.Value[1]);
      Assert.AreSame (_values[3], _bocList.Value[2]);
      Assert.AreSame (_values[4], _bocList.Value[3]);
    }

    [Test]
    public void RemoveRowWithBusinessObject ()
    {
      _bocList.RemoveRow (_values[2]);

      Assert.IsFalse (object.ReferenceEquals (_values, _bocList.Value));
      Assert.AreEqual (4, _bocList.Value.Count);
      Assert.AreSame (_values[0], _bocList.Value[0]);
      Assert.AreSame (_values[1], _bocList.Value[1]);
      Assert.AreSame (_values[3], _bocList.Value[2]);
      Assert.AreSame (_values[4], _bocList.Value[3]);
    }

    [Test]
    public void RemoveRowsWithNoRows ()
    {
      _bocList.RemoveRows (new IBusinessObject[0]);

      Assert.IsFalse (object.ReferenceEquals (_values, _bocList.Value));
      Assert.AreEqual (5, _bocList.Value.Count);
      Assert.AreSame (_values[0], _bocList.Value[0]);
      Assert.AreSame (_values[1], _bocList.Value[1]);
      Assert.AreSame (_values[2], _bocList.Value[2]);
      Assert.AreSame (_values[3], _bocList.Value[3]);
      Assert.AreSame (_values[4], _bocList.Value[4]);
    }

    [Test]
    public void RemoveRowsWithSingleRow ()
    {
      _bocList.RemoveRows (new IBusinessObject[] {_values[2]});

      Assert.IsFalse (object.ReferenceEquals (_values, _bocList.Value));
      Assert.AreEqual (4, _bocList.Value.Count);
      Assert.AreSame (_values[0], _bocList.Value[0]);
      Assert.AreSame (_values[1], _bocList.Value[1]);
      Assert.AreSame (_values[3], _bocList.Value[2]);
      Assert.AreSame (_values[4], _bocList.Value[3]);
    }

    [Test]
    public void RemoveRowsWithMultipleRows ()
    {
      _bocList.RemoveRows (new IBusinessObject[] {_values[1], _values[3]});

      Assert.IsFalse (object.ReferenceEquals (_values, _bocList.Value));
      Assert.AreEqual (3, _bocList.Value.Count);
      Assert.AreSame (_values[0], _bocList.Value[0]);
      Assert.AreSame (_values[2], _bocList.Value[1]);
      Assert.AreSame (_values[4], _bocList.Value[2]);
    }
  }
}

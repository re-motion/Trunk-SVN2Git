// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
//
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
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
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.UnitTests.Web.Domain;

namespace Remotion.ObjectBinding.UnitTests.Web.UI.Controls.BocTextValueTests
{
  [TestFixture]
  public class AllDataTypes : BocTest
  {
    private BocTextValueMock _bocTextValue;
    private IBusinessObject _businessObject;
    private BindableObjectDataSourceControl _dataSource;

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp();
      _bocTextValue = new BocTextValueMock();
      _bocTextValue.ID = "BocTextValue";
      NamingContainer.Controls.Add (_bocTextValue);

      _businessObject = (IBusinessObject) TypeWithAllDataTypes.Create();

      _dataSource = new BindableObjectDataSourceControl();
      _dataSource.Type = typeof (TypeWithAllDataTypes);
      _dataSource.BusinessObject = _businessObject;
    }

    [Test]
    public void LoadAndSaveValue_WithString ()
    {
      LoadAndSaveValue ("String", "Foo", "Bar");
    }

    [Test]
    public void LoadAndSaveValue_WithByte ()
    {
      LoadAndSaveValue ("Byte", (Byte) 1, (Byte) 2);
    }

    [Test]
    public void LoadAndSaveValue_WithInt16 ()
    {
      LoadAndSaveValue ("Int16", (Int16) 1, (Int16) 2);
    }

    [Test]
    public void LoadAndSaveValue_WithInt32 ()
    {
      LoadAndSaveValue ("Int32", 1, 2);
    }

    [Test]
    public void LoadAndSaveValue_WithInt64 ()
    {
      LoadAndSaveValue ("Int64", 1L, 2L);
    }

    [Test]
    public void LoadAndSaveValue_WithDecimal ()
    {
      LoadAndSaveValue ("Decimal", 1.1m, 2.1m);
    }

    [Test]
    public void LoadAndSaveValue_WithDouble ()
    {
      LoadAndSaveValue ("Double", 1.1, 2.1);
    }

    [Test]
    public void LoadAndSaveValue_WithSingle ()
    {
      LoadAndSaveValue ("Single", 1.1f, 2.1f);
    }

    [Test]
    public void LoadAndSaveValue_WithDate ()
    {
      LoadAndSaveValue ("Date", new DateTime (2000, 1, 1).Date, new DateTime (2000, 1, 2).Date);
    }

    [Test]
    public void LoadAndSaveValue_WithDateTime ()
    {
      LoadAndSaveValue ("DateTime", new DateTime (2000, 1, 1, 1, 1, 0), new DateTime (2000, 1, 2, 1, 1, 0));
    }
    
    private void LoadAndSaveValue<T> (string propertyidentifier, T initialValue, T newValue)
    {
      _businessObject.SetProperty (propertyidentifier, initialValue);
      _bocTextValue.DataSource = _dataSource;
      _bocTextValue.Property = _businessObject.BusinessObjectClass.GetPropertyDefinition (propertyidentifier);

      _bocTextValue.LoadValue (false);
      Assert.AreEqual (initialValue, _bocTextValue.Value);
      Assert.IsFalse (_bocTextValue.IsDirty);

      _bocTextValue.Text = newValue.ToString();
      Assert.IsTrue (_bocTextValue.IsDirty);

      _bocTextValue.SaveValue (false);
      Assert.AreEqual (newValue, _bocTextValue.Value);
      Assert.IsFalse (_bocTextValue.IsDirty);
      _bocTextValue.SaveValue (false);

      Assert.AreEqual (newValue, _businessObject.GetProperty (propertyidentifier));
    }
  }
}

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

namespace Remotion.ObjectBinding.UnitTests.Web.UI.Controls
{

[TestFixture]
public class BocListSortingOrderEntryConverterTest
{
  private const string c_empty = "empty";
  private BocListSortingOrderEntryConverter _converter;
  private Type _stringType;
  private BocListSortingOrderEntryMock _ascending0;
  private BocListSortingOrderEntryMock _descending1;
  private BocListSortingOrderEntryMock _none2;
  private string _ascending0String;
  private string _descending1String;
  private string _none2String;

  [SetUp]
  public virtual void SetUp()
  {
    _converter = new BocListSortingOrderEntryConverter();
    _stringType = typeof (string);
    
    _ascending0 = new BocListSortingOrderEntryMock (0, SortingDirection.Ascending);
    _descending1 = new BocListSortingOrderEntryMock (1, SortingDirection.Descending);
    _none2 = new BocListSortingOrderEntryMock (2, SortingDirection.None);

    _ascending0String = "0," + ((int) SortingDirection.Ascending).ToString();
    _descending1String = "1," + ((int) SortingDirection.Descending).ToString();
    _none2String = "2," + ((int) SortingDirection.None).ToString();
  }

  [Test]
  public void ConvertToStringWithEmpty()
  {
    string value = (string) _converter.ConvertTo (null, null, BocListSortingOrderEntry.Empty, _stringType);
    Assert.IsNotNull (value);
    Assert.AreEqual (c_empty, value);
  }

  [Test]
  public void ConvertFromStringWithEmpty()
  {
    BocListSortingOrderEntry value = 
        (BocListSortingOrderEntry) _converter.ConvertFrom (null, null, c_empty);
    Assert.IsNotNull (value);
    Assert.AreEqual (BocListSortingOrderEntry.Empty, value);
  }

  [Test]
  public void ConvertToStringWithAscending0()
  {
    string value = (string) _converter.ConvertTo (null, null, _ascending0, _stringType);
    Assert.IsNotNull (value);
    Assert.AreEqual (_ascending0String, value);
  }

  [Test]
  public void ConvertFromStringWithAscending0()
  {
    BocListSortingOrderEntry value = 
        (BocListSortingOrderEntry) _converter.ConvertFrom (null, null, _ascending0String);
    Assert.IsNotNull (value);
    Assert.AreEqual (_ascending0, value);
  }

  [Test]
  public void ConvertToStringWithDescending1()
  {
    string value = (string) _converter.ConvertTo (null, null, _descending1, _stringType);
    Assert.IsNotNull (value);
    Assert.AreEqual (_descending1String, value);
  }

  [Test]
  public void ConvertFromStringWithDescending1()
  {
    BocListSortingOrderEntry value = 
        (BocListSortingOrderEntry) _converter.ConvertFrom (null, null, _descending1String);
    Assert.IsNotNull (value);
    Assert.AreEqual (_descending1, value);
  }

  [Test]
  public void ConvertToStringWithNone2()
  {
    string value = (string) _converter.ConvertTo (null, null, _none2, _stringType);
    Assert.IsNotNull (value);
    Assert.AreEqual (_none2String, value);
  }

  [Test]
  public void ConvertFromStringWithNone2()
  {
    object value = _converter.ConvertFrom (null, null, _none2String);
    Assert.IsNotNull (value);
    Assert.AreEqual (_none2, value);
  }
}

}

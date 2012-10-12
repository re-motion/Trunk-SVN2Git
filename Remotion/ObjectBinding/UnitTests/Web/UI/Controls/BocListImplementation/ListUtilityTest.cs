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
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation;
using Rhino.Mocks;

namespace Remotion.ObjectBinding.UnitTests.Web.UI.Controls.BocListImplementation
{
  [TestFixture]
  public class ListUtilityTest
  {
    private List<IBusinessObject> _list;
    private List<IBusinessObject> _values;
    private IBusinessObject _objA;
    private IBusinessObject _objB;
    private IBusinessObject _objC;
    private IBusinessObject _objD;
    private IBusinessObject _objE;

    [SetUp]
    public void SetUp ()
    {
      _objA = CreateObject ("A");
      _objB = CreateObject ("B");
      _objC = CreateObject ("C");
      _objD = CreateObject ("D");
      _objE = CreateObject ("E");

      _list = new List<IBusinessObject>();
      _list.Add (_objA);
      _list.Add (_objB);
      _list.Add (_objC);
      _list.Add (_objD);

      _values = new List<IBusinessObject>();
      _values.Add (_objB);
      _values.Add (_objE);
      _values.Add (_objD);
      _values.Add (_objA);
    }

    [Test]
    public void IndicesOf_AscendingValues ()
    {
      var rows = ListUtility.IndicesOf (_list, _values).ToArray();

      Assert.IsNotNull (rows);
      Assert.AreEqual (3, rows.Length);
      Assert.AreEqual (1, rows[0].Index);
      Assert.AreEqual (3, rows[1].Index);
      Assert.AreEqual (0, rows[2].Index);

      Assert.AreEqual (_objB, rows[0].BusinessObject);
      Assert.AreEqual (_objD, rows[1].BusinessObject);
      Assert.AreEqual (_objA, rows[2].BusinessObject);
    }

    private IBusinessObject CreateObject (string value)
    {
      var businessObject = (IBusinessObject) Domain.TypeWithString.Create (value, null);
      return businessObject;
    }
  }
}
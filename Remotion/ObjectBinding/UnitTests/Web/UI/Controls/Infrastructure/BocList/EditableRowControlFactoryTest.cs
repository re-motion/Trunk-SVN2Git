// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.Infrastructure.BocList;
using Remotion.ObjectBinding.UnitTests.Web.Domain;

namespace Remotion.ObjectBinding.UnitTests.Web.UI.Controls.Infrastructure.BocList
{
  [TestFixture]
  public class EditableRowControlFactoryTest
  {
    private BindableObjectClass _stringValueClass;
    private IBusinessObjectPropertyPath _stringValuePropertyPath;
    private BocSimpleColumnDefinition _stringValueColumn;

    private EditableRowControlFactory _factory;

    [SetUp]
    public virtual void SetUp ()
    {
      _stringValueClass = BindableObjectProvider.GetBindableObjectClass(typeof (TypeWithString));

      _stringValuePropertyPath = BusinessObjectPropertyPath.Parse (_stringValueClass, "FirstValue");

      _stringValueColumn = new BocSimpleColumnDefinition();
      _stringValueColumn.SetPropertyPath (_stringValuePropertyPath);

      _factory = new EditableRowControlFactory();
    }

    [Test]
    public void CreateWithStringProperty ()
    {
      IBusinessObjectBoundEditableWebControl control = _factory.Create (_stringValueColumn, 0);

      Assert.IsNotNull (control);
      Assert.IsTrue (control is BocTextValue);
    }

    [Test]
    [ExpectedException (typeof (ArgumentOutOfRangeException))]
    public void CreateWithNegativeIndex ()
    {
      _factory.Create (_stringValueColumn, -1);
    }
  }
}

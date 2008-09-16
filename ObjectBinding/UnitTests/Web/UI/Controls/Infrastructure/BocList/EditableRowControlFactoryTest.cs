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

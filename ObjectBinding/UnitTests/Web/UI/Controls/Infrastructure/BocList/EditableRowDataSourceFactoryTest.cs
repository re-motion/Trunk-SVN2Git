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
using Remotion.ObjectBinding.Web.UI.Controls.Infrastructure.BocList;
using Remotion.ObjectBinding.UnitTests.Web.Domain;

namespace Remotion.ObjectBinding.UnitTests.Web.UI.Controls.Infrastructure.BocList
{

[TestFixture]
public class EditableRowDataSourceFactoryTest
{
  // types

  // static members and constants

  // member fields

  private IBusinessObject _value;
  private EditableRowDataSourceFactory _factory;

  // construction and disposing

  public EditableRowDataSourceFactoryTest ()
  {
  }

  // methods and properties

  [SetUp] 
  public virtual void SetUp()
  {
    _value = (IBusinessObject) TypeWithString.Create();

    _factory = new EditableRowDataSourceFactory ();
  }

  [Test]
  public void Create ()
  {
    IBusinessObjectReferenceDataSource dataSource = _factory.Create (_value);

    Assert.IsNotNull (dataSource);
    Assert.AreSame (_value, dataSource.BusinessObject);
  }
}

}

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
using System.Web.UI;
using NUnit.Framework;
using Remotion.Development.Web.UnitTesting.AspNetFramework;
using Remotion.Development.Web.UnitTesting.UI;
using Remotion.Development.Web.UnitTesting.UI.Controls;
using Remotion.Web.UI;

namespace Remotion.ObjectBinding.UnitTests.Web.UI.Controls
{

public class BocTest
{
  private WcagHelperMock _wcagHelperMock;
  private Page _page;
  private NamingContainerMock _namingContainer;
  private ControlInvoker _invoker;

  public BocTest()
  {
  }
  
  [SetUp]
  public virtual void SetUp()
  {
    _wcagHelperMock = new WcagHelperMock();
    WcagHelper.SetInstance (_wcagHelperMock);

    _page = new Page();

    _namingContainer = new NamingContainerMock();
    _namingContainer.ID = "NamingContainer";
    _page.Controls.Add (_namingContainer);

    _invoker = new ControlInvoker (_namingContainer);
  }

  [TearDown]
  public virtual void TearDown()
  {
    WcagHelper.SetInstance (new WcagHelperMock ());
    HttpContextHelper.SetCurrent (null);
  }

  protected WcagHelperMock WcagHelperMock
  {
    get { return _wcagHelperMock; }
  }

  public Page Page
  {
    get { return _page; }
  }

  public NamingContainerMock NamingContainer
  {
    get { return _namingContainer; }
  }

  public ControlInvoker Invoker
  {
    get { return _invoker; }
  }
}

}

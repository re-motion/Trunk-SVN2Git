// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using Remotion.ObjectBinding.Web.UI.Controls.Rendering.BocList;
using Remotion.ObjectBinding.Web.UI.Controls.Rendering.BocList.StandardMode;
using Remotion.ObjectBinding.Web.UI.Controls.Rendering.BocList.StandardMode.Factories;
using System.Web;
using Rhino.Mocks;

namespace Remotion.ObjectBinding.UnitTests.Web.UI.Controls.Rendering.BocList.StandardMode.Factories
{
  [TestFixture]
  public class BocListRendererFactoryTest
  {
    private HttpContextBase HttpContext { get; set; }
    private HtmlHelper Html { get; set; }
    private ObjectBinding.Web.UI.Controls.BocList List { get; set; }

    [SetUp]
    public void SetUp ()
    {
      Html = new HtmlHelper();

      HttpContext = MockRepository.GenerateMock<HttpContextBase> ();

      List = new ObjectBinding.Web.UI.Controls.BocList();
    }

    [Test]
    public void CreateTableBlockRenderer ()
    {
      IBocListTableBlockRendererFactory factory = new BocListRendererFactory();
      IBocListTableBlockRenderer renderer = factory.CreateRenderer (HttpContext, Html.Writer, List, new StubServiceLocator());

      Assert.IsInstanceOfType (typeof (BocListTableBlockRenderer), renderer);
      Assert.AreSame (List, ((BocListTableBlockRenderer)renderer).List);
    }

    [Test]
    public void CreateRowRenderer ()
    {
      IBocRowRendererFactory factory = new BocListRendererFactory();
      IBocRowRenderer renderer = factory.CreateRenderer (HttpContext, Html.Writer, List, new StubServiceLocator());

      Assert.IsInstanceOfType (typeof (BocRowRenderer), renderer);
      Assert.AreSame (List, ((BocRowRenderer)renderer).List);
    }

    [Test]
    public void CreateMenuBlockRenderer ()
    {
      IBocListMenuBlockRendererFactory factory = new BocListRendererFactory();
      IBocListMenuBlockRenderer renderer = factory.CreateRenderer (HttpContext, Html.Writer, List);

      Assert.IsInstanceOfType (typeof (BocListMenuBlockRenderer), renderer);
      Assert.AreSame (List, ((BocListMenuBlockRenderer) renderer).List);
    }

    [Test]
    public void CreateNavigationBlockRenderer ()
    {
      IBocListNavigationBlockRendererFactory factory = new BocListRendererFactory();
      IBocListNavigationBlockRenderer renderer = factory.CreateRenderer (HttpContext, Html.Writer, List);

      Assert.IsInstanceOfType (typeof (BocListNavigationBlockRenderer), renderer);
      Assert.AreSame (List, ((BocListNavigationBlockRenderer) renderer).List);
    }

    [Test]
    public void CreateBocListRenderer ()
    {
      IBocListRendererFactory factory = new BocListRendererFactory();
      IBocListRenderer renderer = factory.CreateRenderer (HttpContext, Html.Writer, List, new StubServiceLocator());

      Assert.IsInstanceOfType (typeof (BocListRenderer), renderer);
      Assert.AreSame (List, ((BocListRenderer) renderer).List);
    }
  }
}

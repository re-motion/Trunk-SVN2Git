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
using Remotion.ObjectBinding.Web.UI.Controls.Rendering.BocList;
using Remotion.ObjectBinding.Web.UI.Controls.Rendering.BocList.QuirksMode.Factories;
using Remotion.Web.Infrastructure;
using Rhino.Mocks;

namespace Remotion.ObjectBinding.UnitTests.Web.UI.Controls.Rendering.BocList.QuirksMode.Factories
{
  [TestFixture]
  public class BocListRendererFactoryTest
  {
    private IHttpContext HttpContext { get; set; }
    private HtmlHelper Html { get; set; }
    private ObjectBinding.Web.UI.Controls.BocList List { get; set; }

    [SetUp]
    public void SetUp ()
    {
      Html = new HtmlHelper();
      Html.InitializeStream();

      HttpContext = MockRepository.GenerateMock<IHttpContext> ();

      List = new ObjectBinding.Web.UI.Controls.BocList();
    }

    [Test]
    public void CreateTableBlockRenderer ()
    {
      IBocListTableBlockRendererFactory factory = new BocListRendererFactory();
      IBocListTableBlockRenderer renderer = factory.CreateRenderer (HttpContext, Html.Writer, List, new StubServiceLocator());

      Assert.IsNotNull (renderer);
      Assert.AreSame (Html.Writer, renderer.Writer);
      Assert.AreSame (List, renderer.List);
    }

    [Test]
    public void CreateRowRenderer ()
    {
      IBocRowRendererFactory factory = new BocListRendererFactory();
      IBocRowRenderer renderer = factory.CreateRenderer (HttpContext, Html.Writer, List, new StubServiceLocator());

      Assert.IsNotNull (renderer);
      Assert.AreSame (Html.Writer, renderer.Writer);
      Assert.AreSame (List, renderer.List);
    }

    [Test]
    public void CreateMenuBlockRenderer ()
    {
      IBocListMenuBlockRendererFactory factory = new BocListRendererFactory();
      IBocListMenuBlockRenderer renderer = factory.CreateRenderer (HttpContext, Html.Writer, List);

      Assert.IsNotNull (renderer);
      Assert.AreSame (Html.Writer, renderer.Writer);
      Assert.AreSame (List, renderer.List);
    }

    [Test]
    public void CreateNavigationBlockRenderer ()
    {
      IBocListNavigationBlockRendererFactory factory = new BocListRendererFactory();
      IBocListNavigationBlockRenderer renderer = factory.CreateRenderer (HttpContext, Html.Writer, List);

      Assert.IsNotNull (renderer);
      Assert.AreSame (Html.Writer, renderer.Writer);
      Assert.AreSame (List, renderer.List);
    }

    [Test]
    public void CreateBocListRenderer ()
    {
      IBocListRendererFactory factory = new BocListRendererFactory();
      IBocListRenderer renderer = factory.CreateRenderer (HttpContext, Html.Writer, List, new StubServiceLocator());

      Assert.IsNotNull (renderer);
      Assert.AreSame (Html.Writer, renderer.Writer);
      Assert.AreSame (List, renderer.List);
    }
  }
}
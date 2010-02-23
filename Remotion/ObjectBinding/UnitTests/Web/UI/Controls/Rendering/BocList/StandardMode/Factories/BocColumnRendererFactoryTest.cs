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
using System.Web.UI;
using NUnit.Framework;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.Rendering.BocList;
using Remotion.ObjectBinding.Web.UI.Controls.Rendering.BocList.StandardMode;
using Remotion.ObjectBinding.Web.UI.Controls.Rendering.BocList.StandardMode.Factories;
using System.Web;
using Rhino.Mocks;

namespace Remotion.ObjectBinding.UnitTests.Web.UI.Controls.Rendering.BocList.StandardMode.Factories
{
  [TestFixture]
  public class BocColumnRendererFactoryTest
  {
    protected HttpContextBase HttpContext { get; set; }
    private HtmlHelper Html { get; set; }
    private ObjectBinding.Web.UI.Controls.BocList List { get; set; }
    private Page Page { get; set; }

    [SetUp]
    public void SetUp ()
    {
      HttpContext = MockRepository.GenerateMock<HttpContextBase>();

      Html = new HtmlHelper();

      List = new ObjectBinding.Web.UI.Controls.BocList();
      Page = new Page();
      Page.Controls.Add (List);
    }

    [Test]
    public void CreateBocSimpleColumnRenderer ()
    {
      BocSimpleColumnDefinition column = new BocSimpleColumnDefinition();
      IBocColumnRendererFactory<BocSimpleColumnDefinition> factory = new BocColumnRendererFactory();
      CreateRenderer (column, factory);
    }

    [Test]
    public void CreateBocCompoundColumnRenderer ()
    {
      BocCompoundColumnDefinition column = new BocCompoundColumnDefinition();
      IBocColumnRendererFactory<BocCompoundColumnDefinition> factory = new BocColumnRendererFactory();
      CreateRenderer (column, factory);
    }

    [Test]
    public void CreateBocCommandColumnRenderer ()
    {
      BocCommandColumnDefinition column = new BocCommandColumnDefinition();
      IBocColumnRendererFactory<BocCommandColumnDefinition> factory = new BocColumnRendererFactory();
      CreateRenderer (column, factory);
    }

    [Test]
    public void CreateBocCustomColumnRenderer ()
    {
      BocCustomColumnDefinition column = new BocCustomColumnDefinition();
      IBocColumnRendererFactory<BocCustomColumnDefinition> factory = new BocColumnRendererFactory();
      CreateRenderer (column, factory);
    }

    [Test]
    public void CreateBocDropDownMenuColumnRenderer ()
    {
      BocDropDownMenuColumnDefinition column = new BocDropDownMenuColumnDefinition();
      IBocColumnRendererFactory<BocDropDownMenuColumnDefinition> factory = new BocColumnRendererFactory();
      CreateRenderer (column, factory);
    }

    [Test]
    public void CreateBocRowEditModeColumnRenderer ()
    {
      BocRowEditModeColumnDefinition column = new BocRowEditModeColumnDefinition();
      IBocColumnRendererFactory<BocRowEditModeColumnDefinition> factory = new BocColumnRendererFactory();
      CreateRenderer (column, factory);
    }

    [Test]
    public void CreateBocIndexColumnRenderer ()
    {
      IBocIndexColumnRendererFactory factory = new BocColumnRendererFactory();
      IBocIndexColumnRenderer renderer = factory.CreateRenderer (HttpContext, Html.Writer, List);

      Assert.IsInstanceOfType (typeof (BocIndexColumnRenderer), renderer);
      Assert.AreSame (List, ((BocIndexColumnRenderer) renderer).List);
    }

    [Test]
    public void CreateBocSelectorColumnRenderer ()
    {
      IBocSelectorColumnRendererFactory factory = new BocColumnRendererFactory();
      IBocSelectorColumnRenderer renderer = factory.CreateRenderer (HttpContext, Html.Writer, List);

      Assert.IsInstanceOfType (typeof (BocSelectorColumnRenderer), renderer);
      Assert.AreSame (List, ((BocSelectorColumnRenderer) renderer).List);
    }

    private void CreateRenderer<T> (T column, IBocColumnRendererFactory<T> rendererFactory)
        where T: BocColumnDefinition
    {
      column.ColumnTitle = "TestColumn1";
      List.FixedColumns.Add (column);

      IBocColumnRenderer renderer = rendererFactory.CreateRenderer (HttpContext, Html.Writer, List, column);

      Assert.IsInstanceOfType (typeof (BocColumnRendererBase<T>), renderer);
      Assert.AreSame (List, ((BocColumnRendererBase<T>) renderer).List);
      Assert.AreSame (column, ((BocColumnRendererBase<T>) renderer).Column);
    }
  }
}

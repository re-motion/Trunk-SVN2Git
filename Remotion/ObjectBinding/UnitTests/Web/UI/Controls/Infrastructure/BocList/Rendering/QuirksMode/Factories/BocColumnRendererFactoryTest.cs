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
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.Infrastructure.BocList.Rendering;
using Remotion.ObjectBinding.Web.UI.Controls.Infrastructure.BocList.Rendering.QuirksMode.Factories;

namespace Remotion.ObjectBinding.UnitTests.Web.UI.Controls.Infrastructure.BocList.Rendering.QuirksMode.Factories
{
  [TestFixture]
  public class BocColumnRendererFactoryTest
  {
    private HtmlHelper Html { get; set; }
    private ObjectBinding.Web.UI.Controls.BocList List { get; set; }
    private Page Page { get; set; }

    [SetUp]
    public void SetUp ()
    {
      Html = new HtmlHelper();
      Html.InitializeStream();

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

    private void CreateRenderer<T> (T column, IBocColumnRendererFactory<T> rendererFactory)
        where T: BocColumnDefinition
    {
      column.ColumnTitle = "TestColumn1";
      List.FixedColumns.Add (column);

      IBocColumnRenderer<T> renderer = rendererFactory.CreateRenderer (Html.Writer, List, column);

      Assert.IsNotNull (renderer);
      Assert.AreSame (Html.Writer, renderer.Writer);
      Assert.AreSame (List, renderer.List);
      Assert.AreSame (column, renderer.Column);
    }
  }
}
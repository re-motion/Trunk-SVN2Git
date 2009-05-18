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
using System.Web.UI.WebControls;
using HtmlAgilityPack;
using NUnit.Framework;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.Infrastructure.BocList.Rendering;
using Remotion.ObjectBinding.Web.UI.Controls.Infrastructure.BocList.Rendering.QuirksMode;
using Remotion.Web.UI.Controls;

namespace Remotion.ObjectBinding.UnitTests.Web.UI.Controls.Infrastructure.BocList.Rendering.QuirksMode
{
  [TestFixture]
  public class BocCommandColumnRendererTest : ColumnRendererTestBase<BocCommandColumnDefinition>
  {
    [SetUp]
    public override void SetUp ()
    {
      Column = new BocCommandColumnDefinition();
      Column.Command = new BocListItemCommand (CommandType.Event);
      Column.Command.EventCommand = new Command.EventCommandInfo();
      Column.Command.EventCommand.RequiresSynchronousPostBack = true;
      Column.Text = "TestCommand";
      Column.ColumnTitle = "FirstColumn";

      base.SetUp();
    }

    [Test]
    public void RenderBasicCell ()
    {
      IBocColumnRenderer<BocCommandColumnDefinition> renderer = new BocCommandColumnRenderer (HttpContext, Html.Writer, List, Column);
      renderer.RenderDataCell (0, false, EventArgs);

      HtmlDocument document = Html.GetResultDocument();

      HtmlNode td = Html.GetAssertedChildElement (document.DocumentNode, "td", 0, false);
      Html.AssertAttribute (td, "class", "bocListDataCellEven");

      HtmlNode a = Html.GetAssertedChildElement (td, "a", 0, false);
      Html.AssertAttribute (a, "href", "#");
      Html.AssertAttribute (a, "onclick", "__doPostBack('ctl00','ListCommand=0,0');");

      Html.AssertTextNode (a, "TestCommand", 0, false);
    }

    [Test]
    public void RenderIconCell ()
    {
      IBocColumnRenderer<BocCommandColumnDefinition> renderer = new BocCommandColumnRenderer (HttpContext, Html.Writer, List, Column);
      renderer.RenderDataCell (0, true, EventArgs);

      HtmlDocument document = Html.GetResultDocument();

      HtmlNode td = Html.GetAssertedChildElement (document.DocumentNode, "td", 0, false);
      Html.AssertAttribute (td, "class", "bocListDataCellEven");

      HtmlNode a = Html.GetAssertedChildElement (td, "a", 0, false);
      Html.AssertAttribute (a, "href", "#");
      Html.AssertAttribute (a, "onclick", "__doPostBack('ctl00','ListCommand=0,0');");

      Html.AssertIcon (a, EventArgs.BusinessObject, null);

      Html.AssertTextNode (a, HtmlHelper.WhiteSpace + "TestCommand", 1, false);
    }

    [Test]
    public void RenderCommandIconCell ()
    {
      Column.Icon.Url = "~/Images/CommandIcon.gif";
      Column.Icon.Width = new Unit (16, UnitType.Pixel);
      Column.Icon.Height = new Unit (16, UnitType.Pixel);

      IBocColumnRenderer<BocCommandColumnDefinition> renderer = new BocCommandColumnRenderer (HttpContext, Html.Writer, List, Column);
      renderer.RenderDataCell (0, false, EventArgs);

      HtmlDocument document = Html.GetResultDocument();

      HtmlNode td = Html.GetAssertedChildElement (document.DocumentNode, "td", 0, false);
      Html.AssertAttribute (td, "class", "bocListDataCellEven");

      HtmlNode a = Html.GetAssertedChildElement (td, "a", 0, false);
      Html.AssertAttribute (a, "href", "#");
      Html.AssertAttribute (a, "onclick", "__doPostBack('ctl00','ListCommand=0,0');");

      Html.AssertIcon (a, EventArgs.BusinessObject, Column.Icon.Url);

      Html.AssertTextNode (a, "TestCommand", 1, false);
    }
  }
}
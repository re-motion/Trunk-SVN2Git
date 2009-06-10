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
using System.Collections.Generic;
using System.Web.UI;
using System.Xml;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Development.UnitTesting;
using Remotion.Utilities;
using Remotion.Web.UI.Controls;

namespace Remotion.Web.UnitTests.UI.Controls.Rendering.WebTabStrip.QuirksMode
{
  [TestFixture]
  public class WebTabStripRendererTest : RendererTestBase
  {
    private Page _page;
    private WebTabStripMock _webTabStrip;

    [SetUp]
    public void SetUp ()
    {
      Initialize();

      _webTabStrip = new WebTabStripMock { ID = "MyWebTabStrip" };
      _page = new Page();
      _page.Controls.Add (_webTabStrip);
    }

    [Test]
    public void RenderEmptyStrip ()
    {
      AssertControl (false, true, false, 0);
    }

    [Test]
    public void RenderPopulatedStrip ()
    {
      PopulateTabStrip();
      ((WebTab) _webTabStrip.Tabs[1]).IsSelected = true;

      AssertControl (false, false, false, 4);
    }

    [Test]
    public void RenderPopulatedStripWithCssClass ()
    {
      _webTabStrip.CssClass = "SomeCssClass";
      PopulateTabStrip ();
      ((WebTab) _webTabStrip.Tabs[1]).IsSelected = true;

      AssertControl (true, false, false, 4);
    }

    [Test]
    public void RenderPopulatedStripWithEnableSelectedTab ()
    {
      _webTabStrip.EnableSelectedTab = true;

      PopulateTabStrip ();
      ((WebTab) _webTabStrip.Tabs[1]).IsSelected = true;

      AssertControl (false, false, false, 4);
    }

    [Test]
    public void RenderPopulatedStripWithDisabledTab ()
    {
      PopulateTabStrip ();
      ((WebTab) _webTabStrip.Tabs[1]).IsSelected = true;
      ((WebTab) _webTabStrip.Tabs[0]).IsDisabled = true;

      AssertControl (false, false, false, 4);
    }

    [Test]
    public void RenderPopulatedStripWithInvisibleTab ()
    {
      PopulateTabStrip ();
      ((WebTab) _webTabStrip.Tabs[1]).IsSelected = true;
      ((WebTab) _webTabStrip.Tabs[2]).IsVisible = false;

      AssertControl (false, false, false, 3);
    }

    [Test]
    public void RenderEmptyStripInDesignMode ()
    {
      _webTabStrip.SetDesignMode (true);

      AssertControl (false, true, true, 5);
    }

    private void PopulateTabStrip ()
    {
      var tab0 = new WebTab { ItemID = "Tab0", Text = "First Tab", Icon = new IconInfo ()};
      var tab1 = new WebTab { ItemID = "Tab1", Text = "Second Tab", Icon = new IconInfo ("myImageUrl")};
      var tab2 = new WebTab { ItemID = "Tab2", Text = "Third Tab", Icon = null };
      var tab3 = new WebTab { ItemID = "Tab3", Icon = null };
      _webTabStrip.Tabs.Add (tab0);
      _webTabStrip.Tabs.Add (tab1);
      _webTabStrip.Tabs.Add (tab2);
      _webTabStrip.Tabs.Add (tab3);
    }

    private void AssertControl (bool withCssClass, bool isEmpty, bool isDesignMode, int tabCount)
    {
      _webTabStrip.RenderControl (Html.Writer);

      var document = Html.GetResultDocument ();
      XmlNode list = GetAssertedTabList (document, withCssClass, isEmpty, tabCount, isDesignMode);
      AssertTabs (list, isDesignMode);
    }

    private void AssertTabs (XmlNode list, bool isDesignMode)
    {
      var tabs = (List<WebTab>)PrivateInvoke.InvokeNonPublicMethod (_webTabStrip, typeof(Web.UI.Controls.WebTabStrip), "GetVisibleTabs");
      int itemCount = list.ChildNodes.Count;
      if( !isDesignMode )
        Assert.That (itemCount, Is.EqualTo (tabs.Count));

      for (int i = 0; i < itemCount; i++)
      {
        WebTab tab = tabs[i];
        bool isLastItem = (i == itemCount - 1);

        var item = list.GetAssertedChildElement ("li", i);
        AssertItem (item, tab, isLastItem, isDesignMode);

        if (isLastItem)
        {
          var lastSpan = item.GetAssertedChildElement ("span", 1);
          lastSpan.AssertAttributeValueEquals ("class", "last");
        }
      }
    }

    private XmlNode GetAssertedTabList (XmlDocument document, bool withCssClass, bool isEmpty, int tabCount, bool isDesignMode)
    {
      var outerDiv = document.GetAssertedChildElement ("div", 0);
      outerDiv.AssertAttributeValueEquals ("class", withCssClass ? _webTabStrip.CssClass : _webTabStrip.CssClassBasePublic);
      outerDiv.AssertChildElementCount (1);

      var innerDiv = outerDiv.GetAssertedChildElement ("div", 0);
      innerDiv.AssertAttributeValueContains ("class", _webTabStrip.CssClassTabsPanePublic);
      if( isEmpty )
        innerDiv.AssertAttributeValueContains ("class", _webTabStrip.CssClassTabsPaneEmptyPublic);

      innerDiv.AssertChildElementCount (1);

      var list = innerDiv.GetAssertedChildElement ("ul", 0);
      if (isDesignMode)
      {
        list.AssertStyleAttribute ("list-style", "none");
        list.AssertStyleAttribute ("width", "100%");
        list.AssertStyleAttribute ("display", "inline");
      }
      list.AssertChildElementCount (tabCount);
      return list;
    }

    private void AssertItem (XmlNode item, WebTab webTab, bool isLastItem, bool isDesignMode)
    {
      if (isDesignMode)
      {
        item.AssertStyleAttribute ("float", "left");
        item.AssertStyleAttribute ("display", "block");
        item.AssertStyleAttribute ("white-space", "nowrap");
      }
      item.AssertChildElementCount (isLastItem ? 2 : 1);

      var wrapper = item.GetAssertedChildElement ("span", 0);
      wrapper.AssertAttributeValueEquals ("class", _webTabStrip.CssClassTapStripTabWrapperPublic);

      var separator = wrapper.GetAssertedChildElement ("span", 0);
      separator.AssertAttributeValueEquals ("class", _webTabStrip.CssClassSeparatorPublic);
      separator.AssertChildElementCount (1);

      var empty = separator.GetAssertedChildElement ("span", 0);
      empty.AssertChildElementCount (0);

      var tab = wrapper.GetAssertedChildElement ("span", 1);
      tab.AssertAttributeValueEquals ("id", _webTabStrip.ClientID + "_" + webTab.ItemID);
      tab.AssertAttributeValueContains ("class", webTab.IsSelected ? _webTabStrip.CssClassTabSelectedPublic : _webTabStrip.CssClassTabPublic);
      if( webTab.IsDisabled )
        tab.AssertAttributeValueContains ("class", _webTabStrip.CssClassDisabledPublic);
      var link = tab.GetAssertedChildElement ("a", 0);

      bool isDisabledBySelection = webTab.IsSelected && !_webTabStrip.EnableSelectedTab;
      if (!webTab.IsDisabled && !isDisabledBySelection)
      {
        link.AssertAttributeValueEquals ("href", "#");
        string clickScript = _page.ClientScript.GetPostBackClientHyperlink (_webTabStrip, webTab.ItemID);
        if( isDesignMode )
          clickScript = string.Empty;

        link.AssertAttributeValueEquals ("onclick", clickScript);
      }

      AssertAnchor(link, webTab);
    }

    private void AssertAnchor (XmlNode link, WebTab tab)
    {
      var anchorBody = link.GetAssertedChildElement ("span", 0);
      anchorBody.AssertAttributeValueEquals ("class", _webTabStrip.CssClassTabAnchorBodyPublic);

      string url = "/res/Remotion.Web/Image/Spacer.gif";
      string alt = "";
      string text = StringUtility.NullToEmpty(tab.Text);
      if (tab.Icon != null)
      {
        alt = StringUtility.NullToEmpty (tab.Icon.AlternateText);
        if (!string.IsNullOrEmpty (tab.Icon.Url))
        {
          url = tab.Icon.Url;
          text = HtmlHelper.WhiteSpace + text;
        }
      }

      var image = anchorBody.GetAssertedChildElement ("img", 0);
      image.AssertAttributeValueEquals ("src", url);
      image.AssertAttributeValueEquals ("alt", alt);

      if (string.IsNullOrEmpty (text))
        text = HtmlHelper.WhiteSpace;

      anchorBody.AssertTextNode (text, 1);
    }
  }
}
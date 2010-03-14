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
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Utilities;
using Remotion.Web.Infrastructure;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls;
using Remotion.Web.UI.Controls.TabbedMenuImplementation;
using Remotion.Web.UI.Controls.TabbedMenuImplementation.Rendering;
using Remotion.Web.UI.Controls.WebTabStripImplementation;
using Remotion.Web.UI.Controls.WebTabStripImplementation.Rendering;
using Rhino.Mocks;

namespace Remotion.Web.UnitTests.Core.UI.Controls.WebTabStripImplementation.Rendering
{
  [TestFixture]
  public class WebTabStripRendererTest : RendererTestBase
  {
    private IWebTabStrip _webTabStrip;
    private WebTabStripRenderer _renderer;
    private IPage _pageStub;
    private IMenuTab _tab0;
    private HttpContextBase _httpContext;
    private HtmlHelper _htmlHelper;

    [SetUp]
    public void SetUp ()
    {
      _htmlHelper = new HtmlHelper ();
      _httpContext = MockRepository.GenerateStub<HttpContextBase> ();

      _webTabStrip = MockRepository.GenerateStub<IWebTabStrip>();
      _webTabStrip.Stub (stub => stub.ClientID).Return ("WebTabStrip");

      _pageStub = MockRepository.GenerateStub<IPage>();
      var clientScriptStub = MockRepository.GenerateStub<IClientScriptManager>();
      clientScriptStub.Stub (stub => stub.GetPostBackClientHyperlink (_webTabStrip, null)).IgnoreArguments().Return ("PostBackHyperlink");
      _pageStub.Stub (stub => stub.ClientScript).Return (clientScriptStub);

      _webTabStrip.Stub (stub => stub.Page).Return (_pageStub);
      _webTabStrip.Stub (stub => stub.Tabs).Return (new WebTabCollection (_webTabStrip));
      _webTabStrip.Stub (stub => stub.GetVisibleTabs()).Return (new List<IWebTab>());

      StateBag stateBag = new StateBag();
      _webTabStrip.Stub (stub => stub.Attributes).Return (new AttributeCollection (stateBag));
      _webTabStrip.Stub (stub => stub.SelectedTabStyle).Return (new WebTabStyle());
      _webTabStrip.Stub (stub => stub.TabStyle).Return (new WebTabStyle());
      _webTabStrip.Stub (stub => stub.ControlStyle).Return (new Style (stateBag));
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
      _tab0.Stub (stub => stub.EvaluateEnabled()).Return (true);

      AssertControl (false, false, false, 4);
    }

    [Test]
    public void RenderPopulatedStripWithCssClass ()
    {
      _webTabStrip.CssClass = "SomeCssClass";
      PopulateTabStrip();
      _tab0.Stub (stub => stub.EvaluateEnabled()).Return (true);

      AssertControl (true, false, false, 4);
    }

    [Test]
    public void RenderPopulatedStripWithEnableSelectedTab ()
    {
      _webTabStrip.Stub (stub => stub.EnableSelectedTab).Return (true);

      PopulateTabStrip();
      _tab0.Stub (stub => stub.EvaluateEnabled()).Return (true);

      AssertControl (false, false, false, 4);
    }

    [Test]
    public void RenderPopulatedStripWithDisabledTab ()
    {
      PopulateTabStrip();
      _tab0.Stub (stub => stub.EvaluateEnabled()).Return (false);
      AssertControl (false, false, false, 4);
    }

    [Test]
    public void RenderPopulatedStripWithInvisibleTab ()
    {
      PopulateTabStrip();
      _webTabStrip.GetVisibleTabs().RemoveAt (2);

      AssertControl (false, false, false, 3);
    }

    [Test]
    public void RenderEmptyListInDesignMode ()
    {
      _webTabStrip.Stub (stub => stub.IsDesignMode).Return (true);

      AssertControl (false, true, true, 0);
    }

    [Test]
    public void RenderPopulatedListInDesignMode ()
    {
      _webTabStrip.Stub (stub => stub.IsDesignMode).Return (true);
      PopulateTabStrip();
      AssertControl (false, true, true, 4);
    }

    private void PopulateTabStrip ()
    {
      _tab0 = MockRepository.GenerateStub<IMenuTab>();
      _tab0.Stub (stub => stub.ItemID).Return ("Tab0");
      _tab0.Stub (stub => stub.Text).Return ("First Tab");
      _tab0.Stub (stub => stub.Icon).Return (new IconInfo());
      _tab0.Stub (stub => stub.EvaluateEnabled()).Return (true);
      _tab0.Stub (stub => stub.GetPostBackClientEvent()).Return (_pageStub.ClientScript.GetPostBackClientHyperlink (_webTabStrip, _tab0.ItemID));
      _tab0.Stub (stub => stub.GetActiveTab()).Return (_tab0);
      _tab0.Stub (stub => stub.Command).Return (new NavigationCommand (CommandType.Event));
      _tab0.Stub (stub => stub.GetRenderer (null, null)).IgnoreArguments().Return (
          new MenuTabRenderer (_httpContext, _webTabStrip, _tab0));

      var tab1 = MockRepository.GenerateStub<IMenuTab>();
      tab1.Stub (stub => stub.ItemID).Return ("Tab1");
      tab1.Stub (stub => stub.Text).Return ("Second Tab");
      tab1.Stub (stub => stub.Icon).Return (new IconInfo ("myImageUrl"));
      tab1.Stub (stub => stub.EvaluateEnabled()).Return (true);
      tab1.Stub (stub => stub.GetPostBackClientEvent()).Return (_pageStub.ClientScript.GetPostBackClientHyperlink (_webTabStrip, tab1.ItemID));
      tab1.Stub (stub => stub.GetActiveTab()).Return (tab1);
      tab1.Stub (stub => stub.Command).Return (new NavigationCommand (CommandType.Event));
      tab1.Stub (stub => stub.GetRenderer (null, null)).IgnoreArguments().Return (
          new MenuTabRenderer (_httpContext, _webTabStrip, tab1));

      var tab2 = MockRepository.GenerateStub<IMenuTab>();
      tab2.Stub (stub => stub.ItemID).Return ("Tab2");
      tab2.Stub (stub => stub.Text).Return ("Third Tab");
      tab2.Stub (stub => stub.Icon).Return (null);
      tab2.Stub (stub => stub.EvaluateEnabled()).Return (true);
      tab2.Stub (stub => stub.GetPostBackClientEvent()).Return (_pageStub.ClientScript.GetPostBackClientHyperlink (_webTabStrip, tab2.ItemID));
      tab2.Stub (stub => stub.GetActiveTab()).Return (tab2);
      tab2.Stub (stub => stub.Command).Return (new NavigationCommand (CommandType.Event));
      tab2.Stub (stub => stub.GetRenderer (null, null)).IgnoreArguments().Return (
          new MenuTabRenderer (_httpContext, _webTabStrip, tab2));

      var tab3 = MockRepository.GenerateStub<IMenuTab>();
      tab3.Stub (stub => stub.ItemID).Return ("Tab3");
      tab3.Stub (stub => stub.Text).Return (null);
      tab3.Stub (stub => stub.Icon).Return (null);
      tab3.Stub (stub => stub.EvaluateEnabled()).Return (true);
      tab3.Stub (stub => stub.GetPostBackClientEvent()).Return (_pageStub.ClientScript.GetPostBackClientHyperlink (_webTabStrip, tab3.ItemID));
      tab3.Stub (stub => stub.GetActiveTab()).Return (tab3);
      tab3.Stub (stub => stub.Command).Return (new NavigationCommand (CommandType.Event));
      tab3.Stub (stub => stub.GetRenderer (null, null)).IgnoreArguments().Return (
          new MenuTabRenderer (_httpContext, _webTabStrip, tab3));

      _webTabStrip.GetVisibleTabs().Add (_tab0);
      _webTabStrip.GetVisibleTabs().Add (tab1);
      _webTabStrip.GetVisibleTabs().Add (tab2);
      _webTabStrip.GetVisibleTabs().Add (tab3);
    }

    private void AssertControl (bool withCssClass, bool isEmpty, bool isDesignMode, int tabCount)
    {
      _renderer = new WebTabStripRenderer (_httpContext, _webTabStrip, MockRepository.GenerateStub<IResourceUrlFactory> ());
      _renderer.Render (_htmlHelper.Writer);

      var document = _htmlHelper.GetResultDocument();
      XmlNode list = GetAssertedTabList (document, withCssClass, isEmpty, tabCount, isDesignMode);
      AssertTabs (list, isDesignMode);
    }

    private void AssertTabs (XmlNode list, bool isDesignMode)
    {
      var tabs = _webTabStrip.GetVisibleTabs();
      int itemCount = list.ChildNodes.Count;
      if (!isDesignMode)
        Assert.That (itemCount, Is.EqualTo (tabs.Count));

      for (int i = 0; i < itemCount; i++)
      {
        IMenuTab tab = (IMenuTab) tabs[i];
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
      outerDiv.AssertAttributeValueEquals ("class", withCssClass ? _webTabStrip.CssClass : _renderer.CssClassBase);
      outerDiv.AssertChildElementCount (1);

      var innerDiv = outerDiv.GetAssertedChildElement ("div", 0);
      innerDiv.AssertAttributeValueContains ("class", _renderer.CssClassTabsPane);
      if (isEmpty)
        innerDiv.AssertAttributeValueContains ("class", _renderer.CssClassTabsPaneEmpty);

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

    private void AssertItem (XmlNode item, IMenuTab webTab, bool isLastItem, bool isDesignMode)
    {
      if (isDesignMode)
      {
        item.AssertStyleAttribute ("float", "left");
        item.AssertStyleAttribute ("display", "block");
        item.AssertStyleAttribute ("white-space", "nowrap");
      }
      item.AssertChildElementCount (isLastItem ? 2 : 1);

      var wrapper = item.GetAssertedChildElement ("span", 0);
      wrapper.AssertAttributeValueEquals ("class", _renderer.CssClassTabWrapper);

      var separator = wrapper.GetAssertedChildElement ("span", 0);
      separator.AssertAttributeValueEquals ("class", _renderer.CssClassSeparator);
      separator.AssertChildElementCount (1);

      var empty = separator.GetAssertedChildElement ("span", 0);
      empty.AssertChildElementCount (0);

      var tab = wrapper.GetAssertedChildElement ("span", 1);
      tab.AssertAttributeValueEquals ("id", _webTabStrip.ClientID + "_" + webTab.ItemID);
      tab.AssertAttributeValueContains ("class", webTab.IsSelected ? _renderer.CssClassTabSelected : _renderer.CssClassTab);
      if (webTab.IsDisabled)
        tab.AssertAttributeValueContains ("class", _renderer.CssClassDisabled);
      var link = tab.GetAssertedChildElement ("a", 0);

      bool isDisabledBySelection = webTab.IsSelected && !_webTabStrip.EnableSelectedTab;
      if (!webTab.IsDisabled && !isDisabledBySelection)
      {
        link.AssertAttributeValueEquals ("href", "#");
        string clickScript = _pageStub.ClientScript.GetPostBackClientHyperlink (_webTabStrip, webTab.ItemID);
        link.AssertAttributeValueEquals ("onclick", clickScript);
      }

      AssertAnchor (link, webTab);
    }

    private void AssertAnchor (XmlNode link, IMenuTab tab)
    {
      var anchorBody = link.GetAssertedChildElement ("span", 0);
      anchorBody.AssertAttributeValueEquals ("class", _renderer.CssClassTabAnchorBody);

      string url = "Spacer.gif";
      string alt = "";
      string text = StringUtility.NullToEmpty (tab.Text);
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
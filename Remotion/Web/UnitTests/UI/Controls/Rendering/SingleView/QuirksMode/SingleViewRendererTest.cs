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
using System.Xml;
using NUnit.Framework;
using Remotion.Development.Web.UnitTesting.UI.Controls;
using Remotion.Development.Web.UnitTesting.UI.Controls.Rendering;
using Remotion.Web.UI.Controls.Rendering.SingleView;
using Remotion.Web.UI.Controls.Rendering.SingleView.QuirksMode;

namespace Remotion.Web.UnitTests.UI.Controls.Rendering.SingleView.QuirksMode
{
  [TestFixture]
  public class SingleViewRendererTest : RendererTestBase
  {
    private const string c_cssClass = "CssClass";

    private SingleViewMock _singleView;
    private ControlInvoker _invoker;

    [SetUp]
    public void SetUp ()
    {
      Initialize();
      _singleView = new SingleViewMock { ID = "SingleView" };
      _invoker = new ControlInvoker (_singleView);
    }

    [Test]
    public void RenderView ()
    {
      PopulateControl();
      AssertRendering (false, false, false, false);
    }

    [Test]
    public void RenderViewWithCssClass ()
    {
      PopulateControl ();
      AssertRendering (false, true, false, false);
    }

    [Test]
    public void RenderViewWithCssClassInAttributes ()
    {
      PopulateControl ();
      AssertRendering (false, true, true, false);
    }

    [Test]
    public void RenderEmptyView ()
    {
      AssertRendering(true, false, false, false);
    }

    [Test]
    public void RenderEmptyViewWithCssClass ()
    {
      AssertRendering (true, true, false, false);
    }

    [Test]
    public void RenderEmptyViewWithCssClassInAttributes ()
    {
      AssertRendering (true, true, true, false);
    }

    [Test]
    public void RenderViewInDesignMode ()
    {
      PopulateControl ();
      _singleView.SetDesignMode (true);
      AssertRendering (false, false, false, true);
    }

    [Test]
    public void RenderViewWithCssClassInDesignMode ()
    {
      PopulateControl ();
      _singleView.SetDesignMode (true);
      AssertRendering (false, true, false, true);
    }

    [Test]
    public void RenderViewWithCssClassInAttributesInDesignMode ()
    {
      PopulateControl ();
      _singleView.SetDesignMode (true);
      AssertRendering (false, true, true, true);
    }

    [Test]
    public void RenderEmptyViewInDesignMode ()
    {
      _singleView.SetDesignMode (true);
      AssertRendering (true, false, false, true);
    }

    [Test]
    public void RenderEmptyViewWithCssClassInDesignMode ()
    {
      _singleView.SetDesignMode (true);
      AssertRendering (true, true, false, true);
    }

    [Test]
    public void RenderEmptyViewWithCssClassInAttributesInDesignMode ()
    {
      _singleView.SetDesignMode (true);
      AssertRendering (true, true, true, true);
    }

    private void PopulateControl ()
    {
      _singleView.TopControls.Add (new LiteralControl ("TopControl"));
      _singleView.BottomControls.Add (new LiteralControl ("BottomControl"));
      _singleView.View.Add (new LiteralControl ("View"));
    }

    private void AssertRendering (bool isEmpty, bool withCssClasses, bool inAttributes, bool isDesignMode)
    {
      string controlCssClass = _singleView.CssClassBasePublic;
      string topControlsCssClass = _singleView.CssClassTopControlsPublic;
      string bottomControlsCssClass = _singleView.CssClassBottomControlsPublic;

      string contentCssClass = _singleView.CssClassContentPublic;
      string viewCssClass = _singleView.CssClassViewPublic;
      string viewBodyCssClass = _singleView.CssClassViewBodyPublic;

      if (withCssClasses)
      {
        controlCssClass = c_cssClass;
        topControlsCssClass = c_cssClass;
        bottomControlsCssClass = c_cssClass;
        viewCssClass = c_cssClass;
        if (inAttributes)
          _singleView.Attributes["class"] = controlCssClass;
        else
          _singleView.CssClass = controlCssClass;
        
        _singleView.TopControlsStyle.CssClass = topControlsCssClass;
        _singleView.BottomControlsStyle.CssClass = bottomControlsCssClass;
        _singleView.ViewStyle.CssClass = viewCssClass;
      }

      _invoker.PreRenderRecursive();
      var renderer = new SingleViewRenderer (HttpContext, Html.Writer, _singleView);
      renderer.Render();
      //_singleView.RenderControl (Html.Writer);

      var document = Html.GetResultDocument();
      
      XmlNode outerDiv = GetAssertedOuterDiv(document, controlCssClass, isDesignMode);
      XmlNode table = GetAssertedTable(outerDiv, controlCssClass);


      XmlNode tdTop = GetAssertedTdTop (table, topControlsCssClass, isEmpty);
      XmlNode divTopControls = GetAssertedDivTopControls (tdTop, topControlsCssClass);

      var divTopContent = Html.GetAssertedChildElement (divTopControls, "div", 0);
      
      Html.AssertAttribute (divTopContent, "class", contentCssClass);

      
      XmlNode tdView = GetAssertedTdView(table, viewCssClass);
      XmlNode divViewControls = GetAssertedDivViewControls (tdView, viewCssClass);

      
      XmlNode divViewBody = GetAssertedDivViewBody (divViewControls, viewBodyCssClass);

      var divViewContent = Html.GetAssertedChildElement (divViewBody, "div", 0);
      Html.AssertAttribute (divViewContent, "class", contentCssClass);


      XmlNode tdBottom = GetAssertedTdBottom (table, bottomControlsCssClass, isEmpty);
      XmlNode divBottomControls = GetAssertedDivBottomControls(tdBottom, bottomControlsCssClass);

      var divBottomContent = Html.GetAssertedChildElement (divBottomControls, "div", 0);
      Html.AssertAttribute (divBottomContent, "class", contentCssClass);
    }

    private XmlNode GetAssertedDivViewBody (XmlNode divViewControls, string cssClass)
    {
      var divViewBody = Html.GetAssertedChildElement (divViewControls, "div", 0);
      Html.AssertAttribute (divViewBody, "class", cssClass);
      Html.AssertChildElementCount (divViewBody, 1);
      return divViewBody;
    }

    private XmlNode GetAssertedDivBottomControls (XmlNode tdBottom, string cssClass)
    {
      var divBottomControls = Html.GetAssertedChildElement (tdBottom, "div", 0);
      Html.AssertAttribute (divBottomControls, "id", ((ISingleView) _singleView).BottomControl.ClientID);
      Html.AssertAttribute (divBottomControls, "class", cssClass);
      Html.AssertChildElementCount (divBottomControls, 1);
      return divBottomControls;
    }

    private XmlNode GetAssertedDivViewControls (XmlNode tdView, string cssClass)
    {
      var divViewControls = Html.GetAssertedChildElement (tdView, "div", 0);
      Html.AssertAttribute (divViewControls, "class", cssClass);
      Html.AssertAttribute (divViewControls, "id", ((ISingleView) _singleView).ViewClientID);
      Html.AssertChildElementCount (divViewControls, 1);
      return divViewControls;
    }

    private XmlNode GetAssertedDivTopControls (XmlNode tdTop, string cssClass)
    {
      var divTopControls = Html.GetAssertedChildElement (tdTop, "div", 0);
      Html.AssertAttribute (divTopControls, "id", ((ISingleView) _singleView).TopControl.ClientID);
      Html.AssertAttribute (divTopControls, "class", cssClass);
      Html.AssertChildElementCount (divTopControls, 1);
      return divTopControls;
    }

    private XmlNode GetAssertedTdBottom (XmlNode table, string cssClass, bool isEmpty)
    {
      var trBottom = Html.GetAssertedChildElement (table, "tr", 2);
      Html.AssertChildElementCount (trBottom, 1);

      var tdBottom = Html.GetAssertedChildElement (trBottom, "td", 0);
      Html.AssertAttribute (tdBottom, "class", cssClass, HtmlHelperBase.AttributeValueCompareMode.Contains);
      if( isEmpty )
        Html.AssertAttribute (tdBottom, "class", _singleView.CssClassEmptyPublic, HtmlHelperBase.AttributeValueCompareMode.Contains);
      Html.AssertChildElementCount (tdBottom, 1);
      return tdBottom;
    }

    private XmlNode GetAssertedTdView (XmlNode table, string cssClass)
    {
      var trView = Html.GetAssertedChildElement (table, "tr", 1);
      Html.AssertChildElementCount (trView, 1);

      var tdView = Html.GetAssertedChildElement (trView, "td", 0);
      Html.AssertAttribute (tdView, "class", cssClass);
      if (((ISingleView) _singleView).IsDesignMode)
        Html.AssertStyleAttribute (tdView, "border", "solid 1px black");

      Html.AssertChildElementCount (tdView, 1);
      return tdView;
    }

    private XmlNode GetAssertedTdTop (XmlNode table, string cssClass, bool isEmpty)
    {
      var trTop = Html.GetAssertedChildElement (table, "tr", 0);
      Html.AssertChildElementCount (trTop, 1);

      var tdTop = Html.GetAssertedChildElement (trTop, "td", 0);
      Html.AssertAttribute (tdTop, "class", cssClass, HtmlHelperBase.AttributeValueCompareMode.Contains);
      if (isEmpty)
        Html.AssertAttribute (tdTop, "class", _singleView.CssClassEmptyPublic, HtmlHelperBase.AttributeValueCompareMode.Contains);
      Html.AssertChildElementCount (tdTop, 1);
      return tdTop;
    }

    private XmlNode GetAssertedTable (XmlNode outerDiv, string cssClass)
    {
      var table = Html.GetAssertedChildElement (outerDiv, "table", 0);
      Html.AssertAttribute (table, "class", cssClass);
      Html.AssertChildElementCount (table, 3);
      return table;
    }

    private XmlNode GetAssertedOuterDiv (XmlDocument document, string cssClass, bool isDesignMode)
    {
      var outerDiv = Html.GetAssertedChildElement (document, "div", 0);
      Html.AssertAttribute (outerDiv, "class", cssClass);
      if (isDesignMode)
      {
        Html.AssertStyleAttribute (outerDiv, "width", "100%");
        Html.AssertStyleAttribute (outerDiv, "height", "75%");
      }
      Html.AssertChildElementCount (outerDiv, 1);
      return outerDiv;
    }
  }
}
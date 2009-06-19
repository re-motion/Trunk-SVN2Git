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
using System.Web.UI.WebControls;
using System.Xml;
using Microsoft.Practices.ServiceLocation;
using NUnit.Framework;
using Remotion.Web.Infrastructure;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls;
using Remotion.Web.UI.Controls.Rendering.TabbedMultiView;
using Remotion.Web.UI.Controls.Rendering.TabbedMultiView.StandardMode;
using Remotion.Web.UI.Controls.Rendering.WebTabStrip;
using Rhino.Mocks;

namespace Remotion.Web.UnitTests.UI.Controls.Rendering.TabbedMultiView.StandardMode
{
  [TestFixture]
  public class TabbedMultiViewRendererTest : RendererTestBase
  {
    private const string c_cssClass = "SomeCssClass";

    private ITabbedMultiView _control;

    [SetUp]
    public void SetUp ()
    {
      ServiceLocator.SetLocatorProvider (() => new StubServiceLocator());
      Initialize();
      _control = MockRepository.GenerateStub<ITabbedMultiView>();
      _control.Stub (stub => stub.ClientID).Return ("MyTabbedMultiView");
      _control.Stub (stub => stub.TopControl).Return (new PlaceHolder { ID = "MyTabbedMultiView_TopControl" });
      _control.Stub (stub => stub.BottomControl).Return (new PlaceHolder { ID = "MyTabbedMultiView_BottomControl" });

      var tabStrip = MockRepository.GenerateStub<IWebTabStrip>();
      _control.Stub (stub => stub.TabStrip).Return (tabStrip);

      _control.Stub (stub => stub.ActiveViewClientID).Return (_control.ClientID + "_ActiveView");
      _control.Stub (stub => stub.TabStripContainerClientID).Return (_control.ClientID + "_TabStripContainer");

      StateBag stateBag = new StateBag ();
      _control.Stub (stub => stub.Attributes).Return (new AttributeCollection (stateBag));
      _control.Stub (stub => stub.TopControlsStyle).Return (new Style (stateBag));
      _control.Stub (stub => stub.BottomControlsStyle).Return (new Style (stateBag));
      _control.Stub (stub => stub.ActiveViewStyle).Return (new WebTabStyle ());
      _control.Stub (stub => stub.ControlStyle).Return (new Style (stateBag));

      _control.Stub (stub => stub.CssClassActiveView).Return ("CssClassActiveView");
      _control.Stub (stub => stub.CssClassBase).Return ("CssClassBase");
      _control.Stub (stub => stub.CssClassBottomControls).Return ("CssClassBottomControls");
      _control.Stub (stub => stub.CssClassContent).Return ("CssClassContent");
      _control.Stub (stub => stub.CssClassEmpty).Return ("CssClassEmpty");
      _control.Stub (stub => stub.CssClassTabStrip).Return ("CssClassTabStrip");
      _control.Stub (stub => stub.CssClassTopControls).Return ("CssClassTopControls");
      _control.Stub (stub => stub.CssClassViewBody).Return ("CssClassViewBody");

      var clientScriptStub = MockRepository.GenerateStub<IClientScriptManager> ();

      var pageStub = MockRepository.GenerateStub<IPage> ();
      pageStub.Stub (stub => stub.ClientScript).Return (clientScriptStub);

      _control.Stub (stub => stub.Page).Return (pageStub);
    }

    [Test]
    public void RenderEmptyControl ()
    {
      AssertControl (false, false, false, true);
    }

    [Test]
    public void RenderEmptyControlWithCssClass ()
    {
      _control.CssClass = c_cssClass;
      _control.TopControlsStyle.CssClass = c_cssClass;
      _control.ActiveViewStyle.CssClass = c_cssClass;
      _control.BottomControlsStyle.CssClass = c_cssClass;

      AssertControl (true, false, false, true);
    }

    [Test]
    public void RenderEmptyControlWithCssClassInAttributes ()
    {
      _control.Attributes["class"] = c_cssClass;
      _control.TopControlsStyle.CssClass = c_cssClass;
      _control.ActiveViewStyle.CssClass = c_cssClass;
      _control.BottomControlsStyle.CssClass = c_cssClass;

      AssertControl (true, true, false, true);
    }

    [Test]
    public void RenderEmptyControlInDesignMode ()
    {
      _control.Stub (stub => stub.IsDesignMode).Return (true);

      AssertControl (false, false, true, true);
    }

    [Test]
    public void RenderEmptyControlWithCssClassInDesignMode ()
    {
      _control.Stub (stub => stub.IsDesignMode).Return (true);
      _control.CssClass = c_cssClass;
      _control.TopControlsStyle.CssClass = c_cssClass;
      _control.ActiveViewStyle.CssClass = c_cssClass;
      _control.BottomControlsStyle.CssClass = c_cssClass;

      AssertControl (true, false,true, true);
    }

    [Test]
    public void RenderPopulatedControl ()
    {
      PopulateControl();

      AssertControl (false, false, false, false);
    }

    [Test]
    public void RenderPopulatedControlWithCssClass ()
    {
      PopulateControl();

      _control.CssClass = c_cssClass;
      _control.TopControlsStyle.CssClass = c_cssClass;
      _control.ActiveViewStyle.CssClass = c_cssClass;
      _control.BottomControlsStyle.CssClass = c_cssClass;

      AssertControl (true, false, false, false);
    }

    [Test]
    public void RenderPopulatedControlInDesignMode ()
    {
      PopulateControl ();
      _control.Stub (stub => stub.IsDesignMode).Return (true);

      AssertControl (false, false, true, false);
    }

    [Test]
    public void RenderPopulatedControlWithCssClassInDesignMode ()
    {
      PopulateControl();

      _control.Stub (stub => stub.IsDesignMode).Return (true);
      _control.CssClass = c_cssClass;
      _control.TopControlsStyle.CssClass = c_cssClass;
      _control.ActiveViewStyle.CssClass = c_cssClass;
      _control.BottomControlsStyle.CssClass = c_cssClass;

      AssertControl (true, false, true, false);
    }

    private void PopulateControl ()
    {
      _control.TopControl.Controls.Add (new LiteralControl ("TopControls"));
      
      var view1 = new TabView { ID="View1ID", Title = "View1Title" };
      view1.LazyControls.Add (new LiteralControl ("View1Contents"));
      _control.Stub(stub=>stub.GetActiveView()).Return (view1);

      _control.BottomControl.Controls.Add (new LiteralControl ("BottomControls"));
    }

    private void AssertControl (bool withCssClass, bool inAttributes, bool isDesignMode, bool isEmpty)
    {
      var renderer = new TabbedMultiViewRenderer (HttpContext, Html.Writer, _control);
      renderer.Render();

      var container = GetAssertedContainerElement (withCssClass, inAttributes, isDesignMode);
      AssertTopControls (container, withCssClass, isEmpty);
      AssertTabStrip (container);
      AssertView (container, withCssClass, isDesignMode);
      AssertBottomControls (container, withCssClass, isEmpty);
    }

    private XmlNode GetAssertedContainerElement (bool withCssClass, bool inAttributes, bool isDesignMode)
    {
      string cssClass = _control.CssClassBase;
      if (withCssClass)
      {
        cssClass = inAttributes ? _control.Attributes["class"] : _control.CssClass;
      }

      var document = Html.GetResultDocument();
      var outerDiv = document.GetAssertedChildElement ("div", 0);
      
      outerDiv.AssertAttributeValueEquals ("class", cssClass);
      if (isDesignMode)
      {
        outerDiv.AssertStyleAttribute ("width", "100%");
        outerDiv.AssertStyleAttribute ("height", "75%");
      }
      outerDiv.AssertChildElementCount (4);

      return outerDiv;
    }

    private void AssertBottomControls (XmlNode container, bool withCssClass, bool isEmpty)
    {
      string cssClass = _control.CssClassBottomControls;
      if (withCssClass)
        cssClass = c_cssClass;

      var divBottomControls = container.GetAssertedChildElement ("div", 2);
      divBottomControls.AssertChildElementCount (0);

      divBottomControls.AssertAttributeValueEquals ("id", _control.BottomControl.ClientID);
      divBottomControls.AssertAttributeValueContains ("class", cssClass);
      if( isEmpty )
        divBottomControls.AssertAttributeValueContains ("class", _control.CssClassEmpty);

      if (!isEmpty)
        divBottomControls.AssertTextNode ("BottomControls", 0);
    }

    private void AssertView (XmlNode container, bool withCssClass, bool isDesignMode)
    {
      string cssClassActiveView = _control.CssClassActiveView;
      if (withCssClass)
        cssClassActiveView = c_cssClass;

      var divActiveView = container.GetAssertedChildElement ("div", 3);
      divActiveView.AssertChildElementCount (0);

      divActiveView.AssertAttributeValueEquals ("id", _control.ActiveViewClientID);
      divActiveView.AssertAttributeValueEquals ("class", cssClassActiveView);
      if( isDesignMode )
        divActiveView.AssertStyleAttribute ("border", "solid 1px black");
    }

    private void AssertTabStrip (XmlNode container)
    {
      var divTabStrip = container.GetAssertedChildElement ("div", 1);
      divTabStrip.AssertChildElementCount (0);

      divTabStrip.AssertAttributeValueEquals ("class", _control.CssClassTabStrip);
    }

    private void AssertTopControls (XmlNode container, bool withCssClass, bool isEmpty)
    {
      string cssClass = _control.CssClassTopControls;
      if (withCssClass)
        cssClass = c_cssClass;

      var divTopControls = container.GetAssertedChildElement ("div", 0);
      divTopControls.AssertChildElementCount (0);
      if (!isEmpty)
        divTopControls.AssertTextNode ("TopControls", 0);

      divTopControls.AssertAttributeValueEquals ("id", _control.TopControl.ClientID);
      divTopControls.AssertAttributeValueContains ("class", cssClass);
      if (isEmpty)
        divTopControls.AssertAttributeValueContains ("class", _control.CssClassEmpty);
    }
  }
}
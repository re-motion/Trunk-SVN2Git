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
using NUnit.Framework;
using Remotion.Web.Infrastructure;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls;
using Remotion.Web.UI.Controls.Rendering.DropDownMenu;
using Remotion.Web.UI.Controls.Rendering.DropDownMenu.StandardMode;
using Rhino.Mocks;

namespace Remotion.Web.UnitTests.UI.Controls.Rendering.DropDownMenu.StandardMode
{
  [TestFixture]
  public class DropDownMenuRendererTest : RendererTestBase
  {
    private const string c_MenuTitle = "MenuTitle";
    private const string c_Icon_Url = "/Image/icon.gif";
    private const string c_IconAlternateText = "Icon_AlternateText";
    private const string c_Icon_ToolTip = "Icon_ToolTip";

    private static readonly Unit s_iconWidth = Unit.Pixel (16);
    private static readonly Unit s_iconHeight = Unit.Pixel (12);
    private static readonly IconInfo s_titleIcon = new IconInfo (c_Icon_Url, c_IconAlternateText, c_Icon_ToolTip, s_iconWidth, s_iconHeight);

    private IDropDownMenu _control;
    

    [SetUp]
    public void SetUp ()
    {
      Initialize ();
      _control = MockRepository.GenerateStub<IDropDownMenu> ();
      _control.ID = "DropDownMenu1";
      _control.Stub (stub => stub.Enabled).Return (true);
      _control.Stub (stub => stub.UniqueID).Return ("DropDownMenu1");
      _control.Stub (stub => stub.ClientID).Return ("DropDownMenu1");
      _control.Stub (stub => stub.MenuItems).Return (new WebMenuItemCollection (_control));
      _control.Stub (stub => stub.GetOpenDropDownMenuEventReference (null)).IgnoreArguments ().Return ("OpenDropDownMenuEventReference");

      IPage pageStub = MockRepository.GenerateStub<IPage> ();
      _control.Stub (stub => stub.Page).Return (pageStub);

      StateBag stateBag = new StateBag ();
      _control.Stub (stub => stub.Attributes).Return (new AttributeCollection (stateBag));
      _control.Stub (stub => stub.ControlStyle).Return (new Style (stateBag));

      IClientScriptManager scriptManagerMock = MockRepository.GenerateMock<IClientScriptManager> ();
      _control.Page.Stub (stub => stub.ClientScript).Return (scriptManagerMock);
      
    }

    [Test]
    public void RenderEmptyMenuWithoutTitle ()
    {
      XmlNode containerDiv = GetAssertedContainerDiv();
      AssertTitleDiv(containerDiv, false, false);
    }

    [Test]
    public void RenderEmptyMenuWithTitle ()
    {
      _control.Stub(stub=>stub.TitleText).Return(c_MenuTitle);

      XmlNode containerDiv = GetAssertedContainerDiv ();
      AssertTitleDiv (containerDiv, true, false);
    }

    [Test]
    public void RenderEmptyMenuWithTitleAndIcon ()
    {
      _control.Stub (stub => stub.TitleText).Return (c_MenuTitle);
      _control.Stub (stub => stub.TitleIcon).Return (s_titleIcon);

      XmlNode containerDiv = GetAssertedContainerDiv ();
      AssertTitleDiv (containerDiv, true, true);
    }

    [Test]
    public void RenderPopulatedMenu ()
    {
      PopulateMenu();

      XmlNode containerDiv = GetAssertedContainerDiv ();
      AssertTitleDiv (containerDiv, false, false);
    }

    private void AssertTitleDiv (XmlNode containerDiv, bool withTitle, bool withIcon)
    {
      var titleDiv = containerDiv.GetAssertedChildElement ("div", 0);
      titleDiv.AssertAttributeValueEquals ("class", "DropDownMenuSelect");
      titleDiv.AssertChildElementCount (2);

      AssertTitleAnchor(titleDiv, withTitle, withIcon);
      AssertDropDownButton(titleDiv);
    }

    private void AssertDropDownButton (XmlNode titleDiv)
    {
      var span = titleDiv.GetAssertedChildElement ("span", 1);

      var image = span.GetAssertedChildElement ("img", 0);
      image.AssertAttributeValueEquals ("src", IconInfo.Spacer.Url);
      image.AssertAttributeValueEquals ("alt", "");
      image.AssertStyleAttribute ("vertical-align", "middle");
      image.AssertStyleAttribute ("border-style", "none");
    }

    private void AssertTitleAnchor (XmlNode titleDiv, bool withTitle, bool withIcon)
    {
      var titleAnchor = titleDiv.GetAssertedChildElement ("a", 0);
      titleAnchor.AssertAttributeValueEquals ("href", "#");
      titleAnchor.AssertAttributeValueEquals ("onclick", "return false;");
      titleAnchor.AssertChildElementCount (withIcon ? 1 : 0);
      if (withTitle)
        titleAnchor.AssertTextNode (c_MenuTitle + HtmlHelper.WhiteSpace, withIcon ? 1 : 0);

      if (withIcon)
      {
        var icon = titleAnchor.GetAssertedChildElement ("img", 0);
        icon.AssertAttributeValueEquals ("src", c_Icon_Url);
        icon.AssertAttributeValueEquals ("alt", c_IconAlternateText);
        icon.AssertAttributeValueEquals ("width", s_iconWidth.ToString ());
        icon.AssertAttributeValueEquals ("height", s_iconHeight.ToString ());
        icon.AssertStyleAttribute ("vertical-align", "middle");
        icon.AssertStyleAttribute ("border-style", "none");
      }
    }

    private XmlNode GetAssertedContainerDiv ()
    {
      var renderer = new DropDownMenuRenderer (HttpContext, Html.Writer, _control);
      renderer.Render ();
      var document = Html.GetResultDocument();
      var containerDiv = document.GetAssertedChildElement ("div", 0);
      containerDiv.AssertAttributeValueEquals ("id", _control.ClientID);
      containerDiv.AssertAttributeValueEquals ("class", "DropDownMenuContainer");
      containerDiv.AssertAttributeValueEquals ("onclick", _control.GetOpenDropDownMenuEventReference ("eventReference"));
      containerDiv.AssertChildElementCount (1);

      return containerDiv;
    }

    private void PopulateMenu ()
    {
      AddItem (0, "Category1", CommandType.Event, false, true);
      AddItem (1, "Category1", CommandType.Href, false, true);
      AddItem (2, "Category2", CommandType.WxeFunction, false, true);
      AddItem (3, "Category2", CommandType.WxeFunction, true, true);
      AddItem (4, "Category2", CommandType.WxeFunction, false, false);
    }

    private void AddItem (int index, string category, CommandType commandType, bool isDisabled, bool isVisible)
    {
      string id = "item" + index;
      string text = "Item" + index;
      const string height = "16";
      const string width = "16";
      const string iconUrl = "~/Images/Icon.gif";
      const string disabledIconUrl = "~/Images/DisabledIcon.gif";
      const RequiredSelection requiredSelection = RequiredSelection.Any;

      Command command = new Command (commandType);
      if (commandType == CommandType.Href)
      {
        command.HrefCommand.Href = "~/Target.aspx?index={0}&itemID={1}";
        command.HrefCommand.Target = "_blank";
      }

      _control.MenuItems.Add (
          new WebMenuItem (
              id,
              category,
              text,
              new IconInfo (iconUrl, text, text, width, height),
              new IconInfo (disabledIconUrl, text, text, width, height),
              WebMenuItemStyle.IconAndText,
              requiredSelection,
              isDisabled,
              command) { IsVisible = isVisible });

      string link;
      if (commandType == CommandType.Href)
        link = string.Format ("~/Target.aspx?index={0}&itemID={1}", null, null);
      else
      {
        if (isVisible && _control.Enabled)
        {
          _control.Page.ClientScript.Expect (
              mock => mock.GetPostBackClientHyperlink (Arg.Is<IControl> (_control), Arg.Text.Like (index.ToString ())))
              .Return ("PostBackHyperLink:" + index);
        }
        link = "PostBackHyperLink:" + index;
      }
    }
  }
}
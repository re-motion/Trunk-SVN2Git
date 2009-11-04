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
using System.Web.UI.WebControls;
using System.Xml;
using Microsoft.Practices.ServiceLocation;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.ObjectBinding.UnitTests.Web.Domain;
using Remotion.ObjectBinding.UnitTests.Web.UI.Controls.Rendering.BocReferenceValue;
using Remotion.ObjectBinding.Web;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.Rendering.BocAutoCompleteReferenceValue.StandardMode;
using Remotion.Web.Infrastructure;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls;
using Rhino.Mocks;

namespace Remotion.ObjectBinding.UnitTests.Web.UI.Controls.Rendering.BocAutoCompleteRefenceValue.StandardMode
{
  [TestFixture]
  public class BocAutoCompleteReferenceValueRendererTest : RendererTestBase
  {
    private static readonly Unit s_width = Unit.Pixel (250);
    private static readonly Unit s_height = Unit.Point (12);

    private BusinessObjectReferenceDataSource _dataSource;
    private IBusinessObjectProvider _provider;
    private IBocAutoCompleteReferenceValue Control { get; set; }
    private DropDownMenu OptionsMenu { get; set; }
    private IClientScriptManager ClientScriptManagerMock { get; set; }
    private TypeWithReference BusinessObject { get; set; }
    private StubTextBox TextBox { get; set; }
    

    [SetUp]
    public void SetUp ()
    {
      Initialize();

      OptionsMenu = new StubDropDownMenu ();
      TextBox = new StubTextBox();

      Control = MockRepository.GenerateStub<IBocAutoCompleteReferenceValue> ();
      Control.Stub (stub => stub.ClientID).Return ("MyReferenceValue");
      Control.Stub (stub => stub.TextBoxUniqueID).Return ("MyReferenceValue_Boc_TextBox");
      Control.Stub (stub => stub.TextBoxClientID).Return ("MyReferenceValue_Boc_TextBox");
      Control.Stub (stub => stub.HiddenFieldUniqueID).Return ("MyReferenceValue_Boc_HiddenField");
      Control.Stub (stub => stub.HiddenFieldClientID).Return ("MyReferenceValue_Boc_HiddenField");
      Control.Stub (stub => stub.DropDownButtonClientID).Return ("MyReferenceValue_Boc_DropDownButton");
      Control.Stub (stub => stub.Command).Return (new BocCommand ());
      Control.Command.Type = CommandType.Event;
      Control.Command.Show = CommandShow.Always;

      Control.Stub (stub => stub.OptionsMenu).Return (OptionsMenu);

      IPage pageStub = MockRepository.GenerateStub<IPage> ();
      Control.Stub (stub => stub.Page).Return (pageStub);

      ClientScriptManagerMock = MockRepository.GenerateMock<IClientScriptManager> ();
      pageStub.Stub (stub => stub.ClientScript).Return (ClientScriptManagerMock);

      BusinessObject = TypeWithReference.Create ("MyBusinessObject");
      BusinessObject.ReferenceList = new[]
                                      {
                                          TypeWithReference.Create ("ReferencedObject 0"),
                                          TypeWithReference.Create ("ReferencedObject 1"),
                                          TypeWithReference.Create ("ReferencedObject 2")
                                      };
      _dataSource = new BusinessObjectReferenceDataSource ();
      _dataSource.BusinessObject = (IBusinessObject) BusinessObject;

      _provider = ((IBusinessObject) BusinessObject).BusinessObjectClass.BusinessObjectProvider;
      _provider.AddService<IBusinessObjectWebUIService> (new ReflectionBusinessObjectWebUIService ());

      StateBag stateBag = new StateBag ();
      Control.Stub (mock => mock.Attributes).Return (new AttributeCollection (stateBag));
      Control.Stub (mock => mock.Style).Return (Control.Attributes.CssStyle);
      Control.Stub (mock => mock.CommonStyle).Return (new Style (stateBag));
      Control.Stub (mock => mock.LabelStyle).Return (new Style (stateBag));
      Control.Stub (mock => mock.TextBoxStyle).Return (new SingleRowTextBoxStyle());
      Control.Stub (mock => mock.ControlStyle).Return (new Style (stateBag));

      Control.Stub (stub => stub.GetLabelText()).Return ("MyText");
    }

    [TearDown]
    public void TearDown ()
    {
      ClientScriptManagerMock.VerifyAllExpectations ();
    }

    [Test]
    public void RenderNullReferenceValue ()
    {
      Control.Stub (stub => stub.Enabled).Return (true);

      XmlNode containerDiv = GetAssertedContainerSpan (false);
      AssertControl (containerDiv, false, false);
    }

    [Test]
    public void RenderNullReferenceValueWithOptionsMenu ()
    {
      Control.Stub (stub => stub.Enabled).Return (true);
      SetUpClientScriptExpectations ();
      Control.Stub (stub => stub.HasOptionsMenu).Return (true);

      XmlNode containerDiv = GetAssertedContainerSpan (false);
      AssertControl (containerDiv, false, false);
    }

    [Test]
    public void RenderNullReferenceValueWithEmbeddedOptionsMenu ()
    {
      Control.Stub (stub => stub.Enabled).Return (true);
      Control.Stub (stub => stub.HasValueEmbeddedInsideOptionsMenu).Return (true);
      Control.Stub (stub => stub.HasOptionsMenu).Return (true);

      XmlNode span = GetAssertedContainerSpan (false);
      AssertControl (span, false, false);
    }

    [Test]
    public void RenderNullReferenceValueWithEmbeddedOptionsMenuAndStyle ()
    {
      Control.Stub (stub => stub.Enabled).Return (true);
      Control.Stub (stub => stub.HasValueEmbeddedInsideOptionsMenu).Return (true);
      Control.Stub (stub => stub.HasOptionsMenu).Return (true);
      AddStyle ();

      XmlNode span = GetAssertedContainerSpan (true);
      AssertControl (span, true, false);
    }

    [Test]
    public void RenderNullReferenceValueReadOnly ()
    {
      Control.Stub (stub => stub.Enabled).Return (true);
      Control.Stub (stub => stub.EnableIcon).Return (true);
      Control.Stub (stub => stub.IsReadOnly).Return (true);

      XmlNode span = GetAssertedContainerSpan (false);
      AssertReadOnlyContent (span, false);
    }

    [Test]
    public void RenderNullReferenceValueReadOnlyWithStyle ()
    {
      Control.Stub (stub => stub.Enabled).Return (true);
      Control.Stub (stub => stub.EnableIcon).Return (true);
      Control.Stub (stub => stub.IsReadOnly).Return (true);
      AddStyle ();

      XmlNode span = GetAssertedContainerSpan (true);
      AssertReadOnlyContent (span, true);
    }

    [Test]
    public void RenderNullReferenceValueReadOnlyWithOptionsMenu ()
    {
      Control.Stub (stub => stub.Enabled).Return (true);
      Control.Stub (stub => stub.EnableIcon).Return (true);
      Control.Stub (stub => stub.HasOptionsMenu).Return (true);
      Control.Stub (stub => stub.IsReadOnly).Return (true);

      XmlNode span = GetAssertedContainerSpan (false);
      AssertReadOnlyContent (span, false);

      Assert.That (OptionsMenu.Style["width"], Is.Null);
      Assert.That (OptionsMenu.Style["height"], Is.Null);
    }

    [Test]
    public void RenderNullReferenceValueWithStyle ()
    {
      Control.Stub (stub => stub.Enabled).Return (true);
      AddStyle ();

      XmlNode span = GetAssertedContainerSpan (true);
      AssertControl (span, true, false);
    }

    [Test]
    public void RenderNullReferenceValueWithOptionsAndStyle ()
    {
      Control.Stub (stub => stub.Enabled).Return (true);
      Control.Stub (stub => stub.HasOptionsMenu).Return (true);
      AddStyle ();

      XmlNode span = GetAssertedContainerSpan (true);
      AssertControl (span, true, false);

      Assert.That (OptionsMenu.Style["width"], Is.Null);
      Assert.That (OptionsMenu.Style["height"], Is.Null);
    }

    [Test]
    public void RenderNullReferenceValueWithIcon ()
    {
      Control.Stub (stub => stub.Enabled).Return (true);
      Control.Stub (stub => stub.EnableIcon).Return (true);
      Control.Stub (stub => stub.Property).Return (
          (IBusinessObjectReferenceProperty) ((IBusinessObject) BusinessObject).BusinessObjectClass.GetPropertyDefinition ("ReferenceValue"));
      SetUpGetIconExpectations ();

      XmlNode span = GetAssertedContainerSpan (false);
      AssertControl (span, false, true);
    }

    [Test]
    public void RenderReferenceValue ()
    {
      Control.Stub (stub => stub.Enabled).Return (true);
      SetUpClientScriptExpectations ();
      SetValue ();
      XmlNode span = GetAssertedContainerSpan (false);
      AssertControl (span, false, false);
    }

    [Test]
    public void RenderReferenceValueAutoPostback ()
    {
      TextBox.AutoPostBack = false;
      Control.Stub (stub => stub.Enabled).Return (true);
      SetUpClientScriptExpectations ();
      SetValue ();

      Control.Stub (stub => stub.TextBoxStyle).Return (new SingleRowTextBoxStyle ());
      Control.TextBoxStyle.AutoPostBack = true;

      XmlNode span = GetAssertedContainerSpan (false);
      AssertControl (span, false, false);
      Assert.IsTrue (TextBox.AutoPostBack);
    }

    private void SetValue ()
    {
      BusinessObject.ReferenceValue = BusinessObject.ReferenceList[0];
      Control.Stub (stub => stub.Value).Return ((IBusinessObjectWithIdentity) BusinessObject.ReferenceValue);
    }

    [Test]
    public void RenderReferenceValueWithOptionsMenu ()
    {
      Control.Stub (stub => stub.Enabled).Return (true);
      SetUpClientScriptExpectations ();
      SetValue ();
      Control.Stub (stub => stub.HasOptionsMenu).Return (true);

      XmlNode span = GetAssertedContainerSpan (false);
      AssertControl (span, false, false);

      Assert.That (OptionsMenu.Style["width"], Is.Null);
      Assert.That (OptionsMenu.Style["height"], Is.Null);
    }

    [Test]
    public void RenderReferenceValueWithEmbeddedOptionsMenu ()
    {
      Control.Stub (stub => stub.Enabled).Return (true);
      SetValue ();
      Control.Stub (stub => stub.HasValueEmbeddedInsideOptionsMenu).Return (true);
      Control.Stub (stub => stub.HasOptionsMenu).Return (true);

      XmlNode span = GetAssertedContainerSpan (false);
      AssertControl (span, false, false);
    }

    [Test]
    public void RenderReferenceValueWithEmbeddedOptionsMenuAndStyle ()
    {
      Control.Stub (stub => stub.Enabled).Return (true);
      SetValue ();
      Control.Stub (stub => stub.HasValueEmbeddedInsideOptionsMenu).Return (true);
      Control.Stub (stub => stub.HasOptionsMenu).Return (true);
      AddStyle ();

      XmlNode span = GetAssertedContainerSpan (false);
      AssertControl (span, true, false);
    }

    [Test]
    public void RenderReferenceValueReadOnly ()
    {
      Control.Stub (stub => stub.Enabled).Return (true);
      SetValue ();
      Control.Stub (stub => stub.EnableIcon).Return (true);
      Control.Stub (stub => stub.IsReadOnly).Return (true);

      XmlNode span = GetAssertedContainerSpan (false);
      AssertReadOnlyContent (span, false);
    }

    [Test]
    public void RenderReferenceValueReadOnlyWithStyle ()
    {
      Control.Stub (stub => stub.Enabled).Return (true);
      SetValue ();
      Control.Stub (stub => stub.EnableIcon).Return (true);
      Control.Stub (stub => stub.IsReadOnly).Return (true);
      AddStyle ();

      XmlNode span = GetAssertedContainerSpan (true);
      AssertReadOnlyContent (span, true);
    }

    [Test]
    public void RenderReferenceValueReadOnlyWithOptionsMenu ()
    {
      Control.Stub (stub => stub.Enabled).Return (true);
      SetValue ();
      Control.Stub (stub => stub.EnableIcon).Return (true);
      Control.Stub (stub => stub.HasOptionsMenu).Return (true);
      Control.Stub (stub => stub.IsReadOnly).Return (true);

      XmlNode span = GetAssertedContainerSpan (false);
      AssertReadOnlyContent (span, false);

      Assert.That (OptionsMenu.Style["width"], Is.Null);
      Assert.That (OptionsMenu.Style["height"], Is.Null);
    }

    [Test]
    public void RenderReferenceValueWithStyle ()
    {
      Control.Stub (stub => stub.Enabled).Return (true);
      SetValue ();
      AddStyle ();

      XmlNode span = GetAssertedContainerSpan (true);
      AssertControl (span, true, false);
    }

    [Test]
    public void RenderReferenceValueWithOptionsAndStyle ()
    {
      Control.Stub (stub => stub.Enabled).Return (true);
      SetValue ();
      Control.Stub (stub => stub.HasOptionsMenu).Return (true);
      AddStyle ();

      XmlNode span = GetAssertedContainerSpan (true);
      AssertControl (span, true, false);

      Assert.That (OptionsMenu.Style["width"], Is.Null);
      Assert.That (OptionsMenu.Style["height"], Is.Null);
    }

    [Test]
    public void RenderReferenceValueWithIcon ()
    {
      Control.Stub (stub => stub.Enabled).Return (true);
      SetValue ();
      Control.Stub (stub => stub.EnableIcon).Return (true);
      Control.Stub (stub => stub.Property).Return (
          (IBusinessObjectReferenceProperty) ((IBusinessObject) BusinessObject).BusinessObjectClass.GetPropertyDefinition ("ReferenceValue"));
      SetUpGetIconExpectations ();

      XmlNode span = GetAssertedContainerSpan (false);
      AssertControl (span, false, true);
    }

    [Test]
    public void RenderOptions ()
    {
      var renderer = new BocAutoCompleteReferenceValueRenderer (HttpContext, Html.Writer, Control, () => new StubTextBox ());

      Html.Writer.AddAttribute (HtmlTextWriterAttribute.Class, "bocAutoCompleteReferenceValueContent");
      Html.Writer.RenderBeginTag (HtmlTextWriterTag.Span);
      renderer.RenderOptionsMenuTitle ();
      Html.Writer.RenderEndTag ();

      var document = Html.GetResultDocument ();
      AssertControl (document, false, false);
    }

    [Test]
    public void RenderOptionsReadOnly ()
    {
      Control.Stub (stub => stub.EnableIcon).Return (true);
      Control.Stub (stub => stub.IsReadOnly).Return (true);

      var renderer = new BocAutoCompleteReferenceValueRenderer (HttpContext, Html.Writer, Control, () => new StubTextBox ());
      Html.Writer.AddAttribute (HtmlTextWriterAttribute.Class, "bocAutoCompleteReferenceValueContent");
      Html.Writer.RenderBeginTag (HtmlTextWriterTag.Span);
      renderer.RenderOptionsMenuTitle ();
      Html.Writer.RenderEndTag ();


      var document = Html.GetResultDocument ();
      AssertReadOnlyContent (document, false);
    }

    protected void AddStyle ()
    {
      Control.Height = s_height;
      Control.Width = s_width;
      Control.Style["height"] = Control.Height.ToString ();
      Control.Style["width"] = Control.Width.ToString ();
    }

    protected void SetUpGetIconExpectations ()
    {
      Control.Expect (mock => mock.GetIcon ()).IgnoreArguments ().Return (new IconInfo ("~/Images/NullIcon.gif"));
    }

    protected void SetUpClientScriptExpectations ()
    {
      ClientScriptManagerMock.Expect (mock => mock.GetPostBackEventReference (Control, string.Empty)).Return ("PostBackEventReference");
    }

    protected void AssertIcon (XmlNode parent, bool wrapNonCommandIcon)
    {
      if (Control.IsCommandEnabled (Control.IsReadOnly))
      {
        var link = parent.GetAssertedChildElement ("a", 0);
        link.AssertAttributeValueEquals ("class", "bocReferenceValueCommand");
        link.AssertAttributeValueEquals ("href", "#");
        link.AssertAttributeValueEquals ("onclick", "");
        link.AssertChildElementCount (1);

        var icon = link.GetAssertedChildElement ("img", 0);
        icon.AssertAttributeValueEquals ("src", "~/Images/Remotion.ObjectBinding.UnitTests.Web.Domain.TypeWithReference.gif");
        icon.AssertStyleAttribute ("border-width", "0px");
      }
      else
      {
        var iconParent = parent;
        if (wrapNonCommandIcon)
        {
          var span = parent.GetAssertedChildElement ("span", 0);
          span.AssertAttributeValueEquals ("class", "bocAutoCompleteReferenceValueCommand");

          iconParent = span;
        }

        var icon = iconParent.GetAssertedChildElement ("img", 0);
        icon.AssertAttributeValueEquals ("src", "~/Images/NullIcon.gif");
        icon.AssertStyleAttribute ("border-width", "0px");
      }
    }

    private void AssertReadOnlyContent (XmlNode parent, bool withStyle)
    {
      var span = parent.GetAssertedChildElement ("span", 0);
      span.AssertAttributeValueEquals ("class", "bocAutoCompleteReferenceValueContent");
      span.AssertChildElementCount (Control.HasOptionsMenu ? 2 : 1);

      var commandSpan = span.GetAssertedChildElement ("span", 0);
      commandSpan.AssertAttributeValueEquals ("class", "bocAutoCompleteReferenceValueCommand");
      commandSpan.AssertChildElementCount (1);

      var innerSpan = commandSpan.GetAssertedChildElement ("span", 0);
      innerSpan.AssertAttributeValueEquals ("id", Control.TextBoxClientID);
      innerSpan.AssertChildElementCount (0);
      innerSpan.AssertTextNode ("MyText", 0);

      if (withStyle)
      {
        span.AssertStyleAttribute ("width", "100%");
      }

      if (Control.HasOptionsMenu)
      {
        var wrapperSpan = span.GetAssertedChildElement ("span", 1);
        wrapperSpan.AssertAttributeValueEquals ("class", "bocAutoCompleteReferenceValueOptionsMenu");
        wrapperSpan.AssertChildElementCount (0);
        wrapperSpan.AssertTextNode ("DropDownMenu", 0);
      }
    }

    private void AssertDropDownListSpan (XmlNode contentSpan)
    {
      var dropDownListSpan = contentSpan.GetAssertedChildElement ("span", 0);
      dropDownListSpan.AssertAttributeValueEquals ("class", "bocAutoCompleteReferenceValueDropDownList");

      var inputSpan = dropDownListSpan.GetAssertedChildElement ("span", 0);
      inputSpan.AssertChildElementCount (0);
      inputSpan.AssertTextNode ("TextBox", 0);

      int hiddenFieldIndex = 1;
      if (Control.Enabled)
      {
        var dropDownButton = dropDownListSpan.GetAssertedChildElement ("span", 1);
        dropDownButton.AssertAttributeValueEquals ("class", "bocAutoCompleteReferenceValueButton");
        dropDownButton.AssertChildElementCount (1);

        var dropDownSpacer = dropDownButton.GetAssertedChildElement ("img", 0);
        dropDownSpacer.AssertAttributeValueEquals ("src", Control.ResolveClientUrl (IconInfo.Spacer.Url));
        dropDownSpacer.AssertChildElementCount (0);

        hiddenFieldIndex++;
      }

      var hiddenField = dropDownListSpan.GetAssertedChildElement ("input", hiddenFieldIndex);
      hiddenField.AssertAttributeValueEquals ("id", Control.HiddenFieldClientID);
      hiddenField.AssertAttributeValueEquals ("type", "hidden");
      hiddenField.AssertChildElementCount (0);
    }

    private void AssertControl (XmlNode containerDiv, bool withStyle, bool withIcon)
    {
      var contentDiv = containerDiv.GetAssertedChildElement ("span", 0);
      contentDiv.AssertAttributeValueEquals ("class", "bocAutoCompleteReferenceValueContent");

      if (withStyle)
      {
        contentDiv.AssertStyleAttribute ("width", "100%");

        if (!Control.IsReadOnly)
          contentDiv.AssertStyleAttribute ("height", "100%");
      }

      if (withIcon)
        AssertIcon (contentDiv, true);

      var contentSpan = contentDiv.GetAssertedChildElement ("span", withIcon ? 1 : 0);
      contentSpan.AssertAttributeValueEquals ("class", "content");

      AssertDropDownListSpan (contentSpan);

      if (Control.HasOptionsMenu)
      {
        var optionsMenuDiv = contentDiv.GetAssertedChildElement ("span", 1);
        optionsMenuDiv.AssertAttributeValueEquals ("class", "bocAutoCompleteReferenceValueOptionsMenu");
        optionsMenuDiv.AssertTextNode ("DropDownMenu", 0);
      }
    }

    private XmlNode GetAssertedContainerSpan (bool withStyle)
    {
      var renderer = new BocAutoCompleteReferenceValueRenderer (HttpContext, Html.Writer, Control, () => TextBox);
      renderer.Render ();

      var document = Html.GetResultDocument ();
      var containerDiv = document.GetAssertedChildElement ("span", 0);

      containerDiv.AssertAttributeValueContains ("class", "bocAutoCompleteReferenceValue");
      if (Control.IsReadOnly)
        containerDiv.AssertAttributeValueContains ("class", "readOnly");
      if (!Control.Enabled)
        containerDiv.AssertAttributeValueContains ("class", "disabled");

      // containerDiv.AssertChildElementCount (1);

      if (withStyle)
      {
        containerDiv.AssertStyleAttribute ("width", s_width.ToString ());
        containerDiv.AssertStyleAttribute ("height", s_height.ToString ());
      }

      return containerDiv;
    }
  }
}

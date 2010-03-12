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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Development.Web.UnitTesting.AspNetFramework;
using Remotion.ObjectBinding.UnitTests.Web.Domain;
using Remotion.ObjectBinding.Web;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.BocReferenceValueImplementation;
using Remotion.ObjectBinding.Web.UI.Controls.BocReferenceValueImplementation.Rendering;
using Remotion.Web.Infrastructure;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls;
using Rhino.Mocks;

namespace Remotion.ObjectBinding.UnitTests.Web.UI.Controls.BocReferenceValueImplementation.Rendering
{
  [TestFixture]
  public class BocReferenceValueRendererTest : RendererTestBase
  {
    private IBusinessObjectProvider _provider;
    private BusinessObjectReferenceDataSource _dataSource;
    protected static readonly Unit Width = Unit.Pixel (250);
    protected static readonly Unit Height = Unit.Point (12);
    public IClientScriptManager ClientScriptManagerMock { get; set; }
    public IBocReferenceValue Control { get; set; }
    public TypeWithReference BusinessObject { get; set; }
    public StubDropDownMenu OptionsMenu { get; set; }
    public StubDropDownList DropDownList { get; set; }

    [SetUp]
    public void SetUp ()
    {
      Initialize ();

      OptionsMenu = new StubDropDownMenu ();
      DropDownList = new StubDropDownList ();

      Control = MockRepository.GenerateStub<IBocReferenceValue> ();
      Control.Stub (stub => stub.ClientID).Return ("MyReferenceValue");
      Control.Stub (stub => stub.Command).Return (new BocCommand ());
      Control.Command.Type = CommandType.Event;
      Control.Command.Show = CommandShow.Always;

      Control.Stub (stub => stub.OptionsMenu).Return (OptionsMenu);

      IPage pageStub = MockRepository.GenerateStub<IPage> ();
      pageStub.Stub (stub => stub.WrappedInstance).Return (new PageMock ());
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
      Control.Stub (mock => mock.LabelStyle).Return (new Style (stateBag));
      Control.Stub (mock => mock.DropDownListStyle).Return (new DropDownListStyle ());
      Control.Stub (mock => mock.ControlStyle).Return (new Style (stateBag));

      Control.Stub (stub => stub.LabelClientID).Return (Control.ClientID + "_Boc_Label");
      Control.Stub (stub => stub.DropDownListClientID).Return (Control.ClientID + "_Boc_DropDownList");
      Control.Stub (stub => stub.IconClientID).Return (Control.ClientID + "_Boc_Icon");
      Control.Stub (stub => stub.PopulateDropDownList (Arg<DropDownList>.Is.NotNull))
          .WhenCalled (
          invocation =>
          {
            foreach (var item in BusinessObject.ReferenceList)
              ((DropDownList) invocation.Arguments[0]).Items.Add (new ListItem (item.DisplayName, item.UniqueIdentifier));
          });

      Control.Stub (stub => stub.GetLabelText ()).Return ("MyText");
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

      XmlNode containerDiv = GetAssertedContainerSpan(false);
      AssertControl(containerDiv, false);
    }

    [Test]
    public void RenderNullReferenceValueWithOptionsMenu ()
    {
      Control.Stub (stub => stub.Enabled).Return (true);
      SetUpClientScriptExpectations();
      Control.Stub (stub => stub.HasOptionsMenu).Return (true);
      
      XmlNode containerDiv = GetAssertedContainerSpan (false);
      AssertControl (containerDiv, false);
    }

    [Test]
    public void RenderNullReferenceValueWithEmbeddedOptionsMenu ()
    {
      Control.Stub (stub => stub.Enabled).Return (true);
      Control.Stub (stub => stub.HasValueEmbeddedInsideOptionsMenu).Return (true);
      Control.Stub (stub => stub.HasOptionsMenu).Return (true);

      XmlNode span = GetAssertedContainerSpan (false);
      AssertControl (span, false);
    }

    [Test]
    public void RenderNullReferenceValueWithEmbeddedOptionsMenuAndStyle ()
    {
      Control.Stub (stub => stub.Enabled).Return (true);
      Control.Stub (stub => stub.HasValueEmbeddedInsideOptionsMenu).Return (true);
      Control.Stub (stub => stub.HasOptionsMenu).Return (true);
      AddStyle ();

      XmlNode span = GetAssertedContainerSpan (true);
      AssertControl (span, false);
    }

    [Test]
    public void RenderNullReferenceValueReadOnly ()
    {
      Control.Stub (stub => stub.Enabled).Return (true);
      Control.Stub (stub => stub.EnableIcon).Return (true);
      Control.Stub (stub => stub.IsReadOnly).Return (true);

      XmlNode span = GetAssertedContainerSpan (false);
      AssertReadOnlyContent (span);
    }

    [Test]
    public void RenderNullReferenceValueReadOnlyWithStyle ()
    {
      Control.Stub (stub => stub.Enabled).Return (true);
      Control.Stub (stub => stub.EnableIcon).Return (true);
      Control.Stub (stub => stub.IsReadOnly).Return (true);
      AddStyle ();

      XmlNode span = GetAssertedContainerSpan (true);
      AssertReadOnlyContent (span);
    }

    [Test]
    public void RenderNullReferenceValueReadOnlyWithOptionsMenu ()
    {
      Control.Stub (stub => stub.Enabled).Return (true);
      Control.Stub (stub => stub.EnableIcon).Return (true);
      Control.Stub (stub => stub.HasOptionsMenu).Return (true);
      Control.Stub (stub => stub.IsReadOnly).Return (true);

      XmlNode span = GetAssertedContainerSpan (false);
      AssertReadOnlyContent (span);

      Assert.That (OptionsMenu.Style["width"], Is.Null);
      Assert.That (OptionsMenu.Style["height"], Is.Null);
    }

    [Test]
    public void RenderNullReferenceValueWithStyle ()
    {
      Control.Stub (stub => stub.Enabled).Return (true);
      AddStyle ();

      XmlNode span = GetAssertedContainerSpan (true);
      AssertControl (span, false);
    }

    [Test]
    public void RenderNullReferenceValueWithOptionsAndStyle ()
    {
      Control.Stub (stub => stub.Enabled).Return (true);
      Control.Stub (stub => stub.HasOptionsMenu).Return (true);
      AddStyle ();

      XmlNode span = GetAssertedContainerSpan (true);
      AssertControl (span, false);

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
      AssertControl (span, true);
    }

    [Test]
    public void RenderReferenceValue ()
    {
      Control.Stub (stub => stub.Enabled).Return (true);
      SetUpClientScriptExpectations ();
      SetValue ();
      XmlNode span = GetAssertedContainerSpan (false);
      AssertControl (span, false);
    }

    [Test]
    public void RenderReferenceValueAutoPostback ()
    {
      DropDownList.AutoPostBack = false;
      Control.Stub (stub => stub.Enabled).Return (true);
      SetUpClientScriptExpectations ();
      SetValue ();

      Control.Stub (stub => stub.DropDownListStyle).Return (new DropDownListStyle());
      Control.DropDownListStyle.AutoPostBack = true;

      XmlNode span = GetAssertedContainerSpan (false);
      AssertControl (span, false);
      Assert.IsTrue (DropDownList.AutoPostBack);
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
      AssertControl (span, false);

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
      AssertControl (span, false);
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
      AssertControl (span, false);
    }

    [Test]
    public void RenderReferenceValueReadOnly ()
    {
      Control.Stub (stub => stub.Enabled).Return (true);
      SetValue ();
      Control.Stub (stub => stub.EnableIcon).Return (true);
      Control.Stub (stub => stub.IsReadOnly).Return (true);

      XmlNode span = GetAssertedContainerSpan (false);
      AssertReadOnlyContent (span);
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
      AssertReadOnlyContent (span);
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
      AssertReadOnlyContent (span);

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
      AssertControl (span, false);
    }

    [Test]
    public void RenderReferenceValueWithOptionsAndStyle ()
    {
      Control.Stub (stub => stub.Enabled).Return (true);
      SetValue ();
      Control.Stub (stub => stub.HasOptionsMenu).Return (true);
      AddStyle ();

      XmlNode span = GetAssertedContainerSpan (true);
      AssertControl (span, false);

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
      AssertControl (span, true);
    }

    [Test]
    public void RenderOptions ()
    {
      var renderer = new BocReferenceValueRenderer (HttpContext, Control, () => new StubDropDownList ());

      Html.Writer.AddAttribute (HtmlTextWriterAttribute.Class, "bocReferenceValueContent");
      Html.Writer.RenderBeginTag (HtmlTextWriterTag.Span);
      renderer.RenderOptionsMenuTitle (Html.Writer);
      Html.Writer.RenderEndTag ();

      var document = Html.GetResultDocument ();
      AssertControl (document, false);
    }

    [Test]
    public void RenderOptionsReadOnly ()
    {
      Control.Stub (stub => stub.EnableIcon).Return (true);
      Control.Stub (stub => stub.IsReadOnly).Return (true);

      var renderer = new BocReferenceValueRenderer (HttpContext, Control, () => new StubDropDownList ());
      Html.Writer.AddAttribute (HtmlTextWriterAttribute.Class, "bocReferenceValueContent");
      Html.Writer.RenderBeginTag (HtmlTextWriterTag.Span);
      renderer.RenderOptionsMenuTitle (Html.Writer);
      Html.Writer.RenderEndTag ();


      var document = Html.GetResultDocument ();
      AssertReadOnlyContent (document);
    }

    private void AssertReadOnlyContent (XmlNode parent)
    {
      var span = parent.GetAssertedChildElement ("span", 0);
      span.AssertAttributeValueEquals ("class", "bocReferenceValueContent");
      span.AssertChildElementCount (Control.HasOptionsMenu ? 2 : 1);

      var commandSpan = span.GetAssertedChildElement ("span", 0);
      commandSpan.AssertAttributeValueEquals ("class", "bocReferenceValueCommand");
      commandSpan.AssertChildElementCount (1);

      var innerSpan = commandSpan.GetAssertedChildElement ("span", 0);
      innerSpan.AssertAttributeValueEquals ("id", Control.LabelClientID);
      innerSpan.AssertChildElementCount (0);
      innerSpan.AssertTextNode ("MyText", 0);

      if (Control.HasOptionsMenu)
      {
        var wrapperSpan = span.GetAssertedChildElement ("span", 1);
        wrapperSpan.AssertAttributeValueEquals ("class", "bocReferenceValueOptionsMenu");
        wrapperSpan.AssertChildElementCount (0);
        wrapperSpan.AssertTextNode ("DropDownMenu", 0);
      }
    }

    private void AssertControl (XmlNode containerDiv, bool withIcon)
    {
      var contentDiv = containerDiv.GetAssertedChildElement ("span", 0);
      contentDiv.AssertAttributeValueEquals ("class", "bocReferenceValueContent");

      if (withIcon)
        AssertIcon (contentDiv, true);

      var contentSpan = contentDiv.GetAssertedChildElement ("span", withIcon ? 1 : 0);
      contentSpan.AssertAttributeValueEquals ("class", "content");
      contentSpan.AssertTextNode ("DropDownList", 0);

      if (Control.HasOptionsMenu)
      {
        var optionsMenuDiv = contentDiv.GetAssertedChildElement ("span", 1);
        optionsMenuDiv.AssertAttributeValueEquals ("class", "bocReferenceValueOptionsMenu");
        optionsMenuDiv.AssertTextNode ("DropDownMenu", 0);
      }
    }

    private XmlNode GetAssertedContainerSpan (bool withStyle)
    {
      var renderer = new BocReferenceValueRenderer (HttpContext, Control, () => DropDownList);
      renderer.Render (Html.Writer);

      var document = Html.GetResultDocument();
      var containerDiv = document.GetAssertedChildElement ("span", 0);

      containerDiv.AssertAttributeValueEquals ("id", "MyReferenceValue");
      containerDiv.AssertAttributeValueContains ("class", "bocReferenceValue");
      if( Control.IsReadOnly )
        containerDiv.AssertAttributeValueContains ("class", "readOnly");
      if (!Control.Enabled)
        containerDiv.AssertAttributeValueContains ("class", "disabled");

      if (withStyle)
      {
        containerDiv.AssertStyleAttribute ("width", Width.ToString());
        containerDiv.AssertStyleAttribute ("height", Height.ToString ());
      }

      return containerDiv;
    }

    protected void AddStyle ()
    {
      Control.Height = Height;
      Control.Width = Width;
      Control.Style["height"] = Control.Height.ToString ();
      Control.Style["width"] = Control.Width.ToString ();
    }

    protected void SetUpGetIconExpectations ()
    {
      Control.Expect (mock => mock.GetIcon (null, null)).IgnoreArguments ().Return (new IconInfo ("~/Images/NullIcon.gif"));
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
          span.AssertAttributeValueEquals ("class", "bocReferenceValueCommand");

          iconParent = span;
        }

        var icon = iconParent.GetAssertedChildElement ("img", 0);
        icon.AssertAttributeValueEquals ("src", "~/Images/NullIcon.gif");
        icon.AssertStyleAttribute ("border-width", "0px");
      }
    }
  }
}
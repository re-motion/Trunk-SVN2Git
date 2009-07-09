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
using NUnit.Framework;
using Remotion.ObjectBinding.UnitTests.Web.Domain;
using Remotion.ObjectBinding.UnitTests.Web.UI.Controls.Rendering.BocReferenceValue;
using Remotion.ObjectBinding.Web;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.Rendering.BocAutoCompleteReferenceValue.StandardMode;
using Remotion.Web.Infrastructure;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls;
using Remotion.Web.UI.Controls.Rendering.DropDownMenu;
using Rhino.Mocks;

namespace Remotion.ObjectBinding.UnitTests.Web.UI.Controls.Rendering.BocAutoCompleteRefenceValue.StandardMode
{
  [TestFixture]
  public class BocAutoCompleteReferenceValueRendererTest : RendererTestBase
  {
    private BusinessObjectReferenceDataSource _dataSource;
    private IBusinessObjectProvider _provider;
    private IBocAutoCompleteReferenceValue Control { get; set; }
    private IDropDownMenu OptionsMenu { get; set; }
    private IClientScriptManager ClientScriptManagerMock { get; set; }
    private TypeWithReference BusinessObject { get; set; }

    [SetUp]
    public void SetUp ()
    {
      Initialize();

      OptionsMenu = new StubDropDownMenu ();

      Control = MockRepository.GenerateStub<IBocAutoCompleteReferenceValue> ();
      Control.Stub (stub => stub.ClientID).Return ("MyReferenceValue");
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
      Control.Stub (mock => mock.LabelStyle).Return (new Style (stateBag));
      Control.Stub (mock => mock.ControlStyle).Return (new Style (stateBag));
      Control.Stub (stub => stub.GetLabelText ()).Return ("MyText");
    }

    [Test]
    [Ignore]
    public void RenderNullReferenceValue ()
    {
      Control.Stub (stub => stub.Enabled).Return (true);

      var renderer = new BocAutoCompleteReferenceValueRenderer (HttpContext, Html.Writer, Control);
      renderer.Render();

      var document = Html.GetResultDocument();
      var containerDiv = document.GetAssertedChildElement ("div", 0);
      containerDiv.AssertAttributeValueEquals ("id", Control.ClientID);
      containerDiv.AssertAttributeValueContains ("class", renderer.CssClassBase);
      containerDiv.AssertChildElementCount (3);

      var inputSpan = containerDiv.GetAssertedChildElement ("span", 0);
      inputSpan.AssertChildElementCount (1);
      var input = inputSpan.GetAssertedChildElement ("input", 0);
      input.AssertAttributeValueEquals ("id", Control.TextBoxClientID);
      input.AssertAttributeValueContains ("class", renderer.CssClassInput);
      input.AssertAttributeValueEquals ("type", "text");

      var dropDownButton = containerDiv.GetAssertedChildElement ("img", 1);

      var hiddenField = containerDiv.GetAssertedChildElement ("input", 2);
      hiddenField.AssertAttributeValueEquals ("id", Control.HiddenFieldClientID);
      hiddenField.AssertAttributeValueEquals ("type", "hidden");
    }
  }
}
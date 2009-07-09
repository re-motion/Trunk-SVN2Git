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
using Remotion.ObjectBinding.UnitTests.Web.Domain;
using Remotion.ObjectBinding.Web;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.Web.Infrastructure;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls;
using Rhino.Mocks;

namespace Remotion.ObjectBinding.UnitTests.Web.UI.Controls.Rendering.BocReferenceValue
{
  public class BocReferenceValueRendererTestBase : RendererTestBase
  {
    private IBusinessObjectProvider _provider;
    private BusinessObjectReferenceDataSource _dataSource;
    protected static readonly Unit Width = Unit.Pixel (250);
    protected static readonly Unit Height = Unit.Point (12);
    public IClientScriptManager ClientScriptManagerMock { get; set; }
    public IBocReferenceValue Control { get; set; }
    public TypeWithReference BusinessObject { get; set; }
    public StubDropDownMenu OptionsMenu { get; set; }

    [SetUp]
    public void SetUp ()
    {
      Initialize();

      OptionsMenu = new StubDropDownMenu();

      Control = MockRepository.GenerateStub<IBocReferenceValue>();
      Control.Stub (stub => stub.ClientID).Return ("MyReferenceValue");
      Control.Stub (stub => stub.Command).Return (new BocCommand());
      Control.Command.Type = CommandType.Event;
      Control.Command.Show = CommandShow.Always;

      Control.Stub (stub => stub.OptionsMenu).Return (OptionsMenu);

      IPage pageStub = MockRepository.GenerateStub<IPage>();
      Control.Stub (stub => stub.Page).Return (pageStub);

      ClientScriptManagerMock = MockRepository.GenerateMock<IClientScriptManager>();
      pageStub.Stub (stub => stub.ClientScript).Return (ClientScriptManagerMock);

      BusinessObject = TypeWithReference.Create ("MyBusinessObject");
      BusinessObject.ReferenceList = new[]
                                      {
                                          TypeWithReference.Create ("ReferencedObject 0"),
                                          TypeWithReference.Create ("ReferencedObject 1"),
                                          TypeWithReference.Create ("ReferencedObject 2")
                                      };
      _dataSource = new BusinessObjectReferenceDataSource();
      _dataSource.BusinessObject = (IBusinessObject) BusinessObject;

      _provider = ((IBusinessObject) BusinessObject).BusinessObjectClass.BusinessObjectProvider;
      _provider.AddService<IBusinessObjectWebUIService> (new ReflectionBusinessObjectWebUIService());

      SetUpAddAndRemoveControlExpectation<DropDownMenu>(Control);

      StateBag stateBag = new StateBag();
      Control.Stub (mock => mock.Attributes).Return (new AttributeCollection (stateBag));
      Control.Stub (mock => mock.Style).Return (Control.Attributes.CssStyle);
      Control.Stub (mock => mock.LabelStyle).Return (new Style (stateBag));
      Control.Stub (mock => mock.DropDownListStyle).Return (new DropDownListStyle());
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

      Control.Stub (stub => stub.GetLabelText()).Return ("MyText");
    }

    [TestFixtureSetUp]
    public void TestFixtureSetUp ()
    {
      ServiceLocator.SetLocatorProvider (() => new StubServiceLocator());
    }

    [TearDown]
    public void TearDown ()
    {
      ClientScriptManagerMock.VerifyAllExpectations();
      ControlCollectionMock.VerifyAllExpectations();
    }

    protected void AddStyle ()
    {
      Control.Height = Height;
      Control.Width = Width;
      Control.Style["height"] = Control.Height.ToString();
      Control.Style["width"] = Control.Width.ToString();
    }

    protected void SetUpGetIconExpectations ()
    {
      Control.Expect (mock => mock.GetIcon (null, null)).IgnoreArguments().Return (new IconInfo ("~/Images/NullIcon.gif"));
    }

    protected void SetUpClientScriptExpectations ()
    {
      ClientScriptManagerMock.Expect (mock => mock.GetPostBackEventReference (Control, string.Empty)).Return ("PostBackEventReference");
    }

    protected void AssertIcon (XmlNode parent)
    {
      if (Control.IsCommandEnabled (Control.IsReadOnly))
      {
        var link = parent.GetAssertedChildElement ("a", 0);
        link.AssertAttributeValueEquals ("href", "#");
        link.AssertAttributeValueEquals ("onclick", "");
        link.AssertChildElementCount (1);

        var icon = link.GetAssertedChildElement ("img", 0);
        icon.AssertAttributeValueEquals ("src", "~/Images/Remotion.ObjectBinding.UnitTests.Web.Domain.TypeWithReference.gif");
        icon.AssertStyleAttribute ("border-width", "0px");
      }
      else
      {
        var icon = parent.GetAssertedChildElement ("img", 0);
        icon.AssertAttributeValueEquals ("src", "~/Images/NullIcon.gif");
        icon.AssertStyleAttribute ("border-width", "0px");
      }
    }
  }
}
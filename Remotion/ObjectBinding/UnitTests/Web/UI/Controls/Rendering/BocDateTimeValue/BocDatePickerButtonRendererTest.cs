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
using System.Xml;
using NUnit.Framework;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.Rendering.BocDateTimeValue.QuirksMode;
using Remotion.Web.Infrastructure;
using Rhino.Mocks;

namespace Remotion.ObjectBinding.UnitTests.Web.UI.Controls.Rendering.BocDateTimeValue
{
  [TestFixture]
  public class BocDatePickerButtonRendererTest : RendererTestBase
  {
    private IBocDatePickerButton _datePickerButton;

    [SetUp]
    public void SetUp ()
    {
      Initialize();
      _datePickerButton = MockRepository.GenerateStub<IBocDatePickerButton>();
      _datePickerButton.ID = "_Boc_DatePickerButton";
      _datePickerButton.Stub (mock => mock.ContainerControlId).Return ("Container");
      _datePickerButton.Stub (mock => mock.TargetControlId).Return ("Target");
      _datePickerButton.Stub (mock => mock.GetHyperLinkId()).Return (_datePickerButton.ID);
      _datePickerButton.Stub (mock => mock.GetResolvedImageUrl()).Return ("FullImagePath");
      _datePickerButton.Stub (mock => mock.GetDatePickerUrl()).Return ("DatePickerUrl");
    }

    [Test]
    public void RenderButton ()
    {
      _datePickerButton.Stub (mock => mock.Enabled).Return (true);
      _datePickerButton.Stub (mock => mock.HasClientScript).Return (true);

      var renderer = new BocDatePickerButtonRenderer (MockRepository.GenerateMock<IHttpContext>(), Html.Writer, _datePickerButton);
      renderer.Render();

      AssertDateTimePickerButton (Html.GetResultDocument(), false, true);
    }

    [Test]
    public void RenderButtonEmptyPopupSize ()
    {
      _datePickerButton.Stub (mock => mock.Enabled).Return (true);
      _datePickerButton.Stub (mock => mock.HasClientScript).Return (true);
      _datePickerButton.Stub (mock => mock.DatePickerPopupWidth).Return (Unit.Empty);
      _datePickerButton.Stub (mock => mock.DatePickerPopupHeight).Return (Unit.Empty);

      var renderer = new BocDatePickerButtonRenderer (MockRepository.GenerateMock<IHttpContext>(), Html.Writer, _datePickerButton);
      renderer.Render();

      AssertDateTimePickerButton (Html.GetResultDocument(), false, true);
    }

    [Test]
    public void RenderButtonNoClientScript ()
    {
      _datePickerButton.Stub (mock => mock.Enabled).Return (true);

      var renderer = new BocDatePickerButtonRenderer (MockRepository.GenerateMock<IHttpContext>(), Html.Writer, _datePickerButton);
      renderer.Render();

      AssertDateTimePickerButton (Html.GetResultDocument(), false, false);
    }

    [Test]
    public void RenderButtonDisabled ()
    {
      _datePickerButton.Stub (mock => mock.HasClientScript).Return (true);

      var renderer = new BocDatePickerButtonRenderer (MockRepository.GenerateMock<IHttpContext>(), Html.Writer, _datePickerButton);
      renderer.Render();

      AssertDateTimePickerButton (Html.GetResultDocument(), true, true);
    }

    [Test]
    public void RenderButtonDisabledNoClientScript ()
    {
      var renderer = new BocDatePickerButtonRenderer (MockRepository.GenerateMock<IHttpContext>(), Html.Writer, _datePickerButton);
      renderer.Render();

      AssertDateTimePickerButton (Html.GetResultDocument(), true, false);
    }

    private void AssertDateTimePickerButton (XmlNode buttonDocument, bool isDisabled, bool hasClientScript)
    {
      var button = Html.GetAssertedChildElement (buttonDocument, "a", 0);
      Html.AssertAttribute (button, "id", "_Boc_DatePickerButton");
      string script = string.Format (
          "DatePicker_ShowDatePicker(this, document.getElementById ('{0}'), " +
          "document.getElementById ('{1}'), '{2}', '{3}', '{4}');return false;",
          _datePickerButton.ContainerControlId,
          _datePickerButton.TargetControlId,
          _datePickerButton.GetDatePickerUrl(),
          _datePickerButton.DatePickerPopupWidth.IsEmpty ? "150pt" : _datePickerButton.DatePickerPopupWidth.ToString(),
          _datePickerButton.DatePickerPopupHeight.IsEmpty ? "150pt" : _datePickerButton.DatePickerPopupHeight.ToString()
          );

      if (isDisabled)
      {
        script = "return false;";
        Html.AssertAttribute (button, "disabled", "disabled");
      }
      if (hasClientScript)
      {
        Html.AssertAttribute (button, "onclick", script);
        Html.AssertAttribute (button, "href", "#");
      }
      Html.AssertStyleAttribute (button, "padding", "0px");
      Html.AssertStyleAttribute (button, "border", "none");
      Html.AssertStyleAttribute (button, "background-color", "transparent");

      if (hasClientScript)
      {
        var image = Html.GetAssertedChildElement (button, "img", 0);
        Html.AssertAttribute (image, "alt", _datePickerButton.AlternateText);
        Html.AssertAttribute (image, "src", _datePickerButton.GetResolvedImageUrl());
        Html.AssertStyleAttribute (image, "border-width", "0px");
      }
    }
  }
}
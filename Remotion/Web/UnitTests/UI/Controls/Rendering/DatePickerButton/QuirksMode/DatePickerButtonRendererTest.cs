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
using NUnit.Framework;
using Remotion.Web.UI.Controls;
using Remotion.Web.UI.Controls.Rendering.DatePickerButton.QuirksMode;
using Rhino.Mocks;

namespace Remotion.Web.UnitTests.UI.Controls.Rendering.DatePickerButton.QuirksMode
{
  [TestFixture]
  public class DatePickerButtonRendererTest : RendererTestBase
  {
    private IDatePickerButton _datePickerButton;

    [SetUp]
    public void SetUp ()
    {
      Initialize();
      _datePickerButton = MockRepository.GenerateStub<IDatePickerButton>();
      _datePickerButton.ID = "_Boc_DatePickerButton";
      _datePickerButton.Stub (mock => mock.ContainerControlID).Return ("Container");
      _datePickerButton.Stub (mock => mock.TargetControlID).Return ("Target");
      _datePickerButton.Stub (mock => mock.ClientID).Return (_datePickerButton.ID);
    }

    [Test]
    public void RenderButton ()
    {
      _datePickerButton.Stub (mock => mock.Enabled).Return (true);
      _datePickerButton.Stub (mock => mock.EnableClientScript).Return (true);

      AssertDateTimePickerButton (false, true);
    }

    [Test]
    public void RenderButtonNoClientScript ()
    {
      _datePickerButton.Stub (mock => mock.Enabled).Return (true);

      AssertDateTimePickerButton (false, false);
    }

    [Test]
    public void RenderButtonDisabled ()
    {
      _datePickerButton.Stub (mock => mock.EnableClientScript).Return (true);

      AssertDateTimePickerButton (true, true);
    }

    [Test]
    public void RenderButtonDisabledNoClientScript ()
    {
      AssertDateTimePickerButton (true, false);
    }

    private void AssertDateTimePickerButton (bool isDisabled, bool hasClientScript)
    {
      var preRenderer = new DatePickerButtonPreRenderer (HttpContext, _datePickerButton);
      preRenderer.PreRender();

      var renderer = new DatePickerButtonRenderer (HttpContext, Html.Writer, _datePickerButton);
      renderer.Render ();
      var buttonDocument = Html.GetResultDocument();

      var button = Html.GetAssertedChildElement (buttonDocument, "a", 0);
      Html.AssertAttribute (button, "id", "_Boc_DatePickerButton");
      string script = string.Format (
          "DatePicker_ShowDatePicker(this, document.getElementById ('{0}'), " +
          "document.getElementById ('{1}'), '{2}', '{3}', '{4}');return false;",
          _datePickerButton.ContainerControlID,
          _datePickerButton.TargetControlID,
          renderer.GetDatePickerUrl(),
          "150pt",
          "150pt"
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
        Html.AssertAttribute (image, "src", renderer.GetResolvedImageUrl());
        Html.AssertStyleAttribute (image, "border-width", "0px");
      }
    }
  }
}
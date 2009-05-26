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
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.Rendering.BocTextValueBase;
using Remotion.ObjectBinding.Web.UI.Controls.Rendering.BocTextValueBase.QuirksMode;
using Remotion.Web.Infrastructure;
using Rhino.Mocks;

namespace Remotion.ObjectBinding.UnitTests.Web.UI.Controls.Rendering.BocTextValue.QuirksMode
{
  [TestFixture]
  public class BocMultilineTextValueRendererTest : RendererTestBase
  {
    private const string c_firstLineText = "This is my test text.";
    private const string c_secondLineText = "with two lines now.";
    private const string c_cssClass = "SomeClass";

    private MockMultilineTextValue TextValue { get; set; }
    private IBocTextValueBaseRenderer Renderer { get; set; }

    [SetUp]
    public void SetUp ()
    {
      Initialize ();

      TextValue = new MockMultilineTextValue();
      TextValue.Text = c_firstLineText + Environment.NewLine + c_secondLineText;

      //TextValue = MockRepository.GenerateMock<IBocMultilineTextValue> ();
      //TextValue.Stub (mock => mock.CssClass).PropertyBehavior ();
      //TextValue.Stub (mock => mock.CssClassBase).Return ("cssClassBase");
      //TextValue.Stub (mock => mock.CssClassDisabled).Return ("cssClassDisabled");
      //TextValue.Stub (mock => mock.CssClassReadOnly).Return ("cssClassReadonly");

      Renderer = new BocMultilineTextValueRenderer (MockRepository.GenerateMock<IHttpContext> (), Html.Writer, TextValue);
    }

    [Test]
    public void RenderMultiLineEditable ()
    {
      TextValue.RenderControl (Html.Writer);
      var document = Html.GetResultDocument();
    }
  }
}
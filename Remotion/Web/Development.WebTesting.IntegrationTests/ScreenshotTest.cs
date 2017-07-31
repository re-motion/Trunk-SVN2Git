﻿// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using System.Drawing;
using System.Threading;
using Coypu;
using NUnit.Framework;
using Remotion.Web.Development.WebTesting.FluentControlSelection;
using Remotion.Web.Development.WebTesting.IntegrationTests.ScreenshotInfrastructure;
using Remotion.Web.Development.WebTesting.PageObjects;
using Remotion.Web.Development.WebTesting.ScreenshotCreation;
using Remotion.Web.Development.WebTesting.ScreenshotCreation.Annotations;
using Remotion.Web.Development.WebTesting.ScreenshotCreation.Resolvers;

namespace Remotion.Web.Development.WebTesting.IntegrationTests
{
  public class ScreenshotTest : IntegrationTest
  {
    private static readonly WebPadding s_rainbowPaddingBig = new WebPadding (10, 40, 30, 20);
    private static readonly WebPadding s_rainbowPaddingSmall = new WebPadding (1, 4, 3, 2);
    private static readonly WebPadding s_uniformPaddingSmall = new WebPadding (5);
    private static readonly WebPadding s_uniformPaddingMedium = new WebPadding (10);
    private static readonly WebPadding s_uniformPaddingBig = new WebPadding (100);

    private static readonly IScreenshotElementResolver<ControlObject> s_controlObjectResolver = ControlObjectResolver.Instance;
    private static readonly IScreenshotElementResolver<ElementScope> s_elementScopeResolver = ElementScopeResolver.Instance;

    private static readonly Color s_colorA = Color.FromArgb (0x00, 0x00, 0xFF);
    private static readonly Color s_colorB = Color.FromArgb (0x00, 0xFF, 0x33);
    private static readonly Color s_colorC = Color.FromArgb (0x66, 0x33, 0x00);
    private static readonly Color s_colorD = Color.FromArgb (0x99, 0x00, 0x99);
    private static readonly Color s_colorE = Color.FromArgb (0x99, 0xFF, 0x33);

    private static readonly Font s_font = new Font ("Consolas", 8.25f);
    private static readonly Brush s_foregroundBrush = new SolidBrush (Color.FromArgb (0x00, 0x00, 0x00));
    private static readonly Brush s_backgroundBrush = new SolidBrush (Color.FromArgb (0xCC, 0xCC, 0xFF));

    private static readonly StringFormat s_stringFormat = new StringFormat
                                                          {
                                                              Alignment = StringAlignment.Center,
                                                              LineAlignment = StringAlignment.Center,
                                                              Trimming = StringTrimming.Word
                                                          };

    /// <summary>
    /// Tests cropping without a border.
    /// </summary>
    [Category ("Screenshot")]
    [Test]
    public void ElementCroppingTest ()
    {
      ScreenshotTestingDelegate<ControlObject> test =
          (builder, target) => builder.Crop (target, s_controlObjectResolver, new ScreenshotCropping (WebPadding.None));

      Helper.RunScreenshotTestExact<ScreenshotTest> (PrepareTest(), ScreenshotTestingType.Both, test);
    }

    /// <summary>
    /// Tests cropping with a different padding on each side.
    /// </summary>
    [Category ("Screenshot")]
    [Test]
    public void ElementCroppingWithPaddingTest ()
    {
      ScreenshotTestingDelegate<ControlObject> test =
          (builder, target) => builder.Crop (target, s_controlObjectResolver, new ScreenshotCropping (s_rainbowPaddingBig));

      Helper.RunScreenshotTestExact<ScreenshotTest> (PrepareTest(), ScreenshotTestingType.Both, test);
    }

    /// <summary>
    /// Tests that a the box annotation is drawing a box around the target.
    /// </summary>
    [Category ("Screenshot")]
    [Test]
    public void ElementBoxAnnotationTest ()
    {
      ScreenshotTestingDelegate<ControlObject> test = (builder, target) =>
      {
        builder.Annotate (target, s_controlObjectResolver, new ScreenshotBoxAnnotation (new Pen (s_colorA), s_rainbowPaddingSmall, null));
        builder.Crop (target, s_controlObjectResolver, new ScreenshotCropping (s_uniformPaddingSmall));
      };

      ScreenshotTestingDelegate<ControlObject> testInverse = (builder, target) =>
      {
        builder.Crop (target, s_controlObjectResolver, new ScreenshotCropping (s_uniformPaddingSmall));
        builder.Annotate (target, s_controlObjectResolver, new ScreenshotBoxAnnotation (new Pen (s_colorA), s_rainbowPaddingSmall, null));
      };

      Helper.RunScreenshotTestExact<ScreenshotTest> (PrepareTest(), ScreenshotTestingType.Both, test);
      Helper.RunScreenshotTestExact<ScreenshotTest> (PrepareTest(), ScreenshotTestingType.Both, testInverse);
    }

    /// <summary>
    /// Tests that box annotations with different widths do not intersect with the element bounds.
    /// </summary>
    [Category ("Screenshot")]
    [Test]
    public void ElementBoxAnnotationWithWidthTest ()
    {
      ScreenshotTestingDelegate<ControlObject> test = (builder, target) =>
      {
        builder.Annotate (target, s_controlObjectResolver, new ScreenshotBoxAnnotation (new Pen (s_colorA, 5f), s_rainbowPaddingSmall, null));
        builder.Annotate (target, s_controlObjectResolver, new ScreenshotBoxAnnotation (new Pen (s_colorB, 4f), s_rainbowPaddingSmall, null));
        builder.Annotate (
            target,
            s_controlObjectResolver,
            new ScreenshotBoxAnnotation (new Pen (s_colorC, 3f), s_rainbowPaddingSmall, null));
        builder.Annotate (
            target,
            s_controlObjectResolver,
            new ScreenshotBoxAnnotation (new Pen (s_colorD, 2f), s_rainbowPaddingSmall, null));
        builder.Annotate (
            target,
            s_controlObjectResolver,
            new ScreenshotBoxAnnotation (new Pen (s_colorE, 1f), s_rainbowPaddingSmall, null));
        builder.Crop (target, s_controlObjectResolver, new ScreenshotCropping (s_uniformPaddingMedium));
      };

      Helper.RunScreenshotTestExact<ScreenshotTest> (PrepareTest(), ScreenshotTestingType.Both, test);
    }

    /// <summary>
    /// Tests that the text annotation is drawing text correctly.
    /// </summary>
    [Category ("Screenshot")]
    [Test]
    public void ElementTextAnnotations ()
    {
      ScreenshotTestingDelegate<ControlObject> test = (builder, target) =>
      {
        foreach (var value in Enum.GetValues (typeof (ContentAlignment)))
        {
          var name = Enum.GetName (typeof (ContentAlignment), value);
          builder.Annotate (
              target,
              s_controlObjectResolver,
              new ScreenshotTextAnnotation (
                  name,
                  s_font,
                  s_foregroundBrush,
                  s_backgroundBrush,
                  s_stringFormat,
                  (ContentAlignment) value,
                  s_rainbowPaddingSmall,
                  -1f,
                  null));
        }
        builder.Crop (target, s_controlObjectResolver, new ScreenshotCropping (s_uniformPaddingBig));
      };

      Helper.RunScreenshotTestExact<ScreenshotTest> (PrepareTest(), ScreenshotTestingType.Both, test);
    }

    /// <summary>
    /// Tests that the <see cref="WebElementResolver"/> (indirect also <see cref="ElementScopeResolver"/> and
    /// <see cref="ControlObjectResolver"/>) is capable of resolving an elements position if the element is
    /// contained in one or more frames.
    /// </summary>
    [Category ("Screenshot")]
    [Test]
    public void ResolveElementInFrame ()
    {
      var home = Start();

      ScreenshotTestingDelegate<ElementScope> test =
          (builder, target) => { builder.Crop (target, s_elementScopeResolver, new ScreenshotCropping (new WebPadding (1))); };

      // Give the browser some time to load the iframe
      Thread.Sleep (1000);

      var element = home.Scope.FindFrame ("frame").FindId ("target");

      Helper.RunScreenshotTestExact<ElementScope, ScreenshotTest> (element, ScreenshotTestingType.Both, test);
    }

    private ControlObject PrepareTest ()
    {
      var home = Start();

      var target = home.Scopes().GetByIDOrNull ("screenshotTarget");
      if (target == null)
        throw new InvalidOperationException ("Can not find the screenshot test target.");

      return target;
    }

    private HtmlPageObject Start ()
    {
      return Start<HtmlPageObject> ("ScreenshotTest.wxe");
    }
  }
}
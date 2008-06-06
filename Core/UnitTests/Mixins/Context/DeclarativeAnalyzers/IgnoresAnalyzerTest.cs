/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using NUnit.Framework;
using Remotion.Mixins;
using Remotion.Mixins.Context.DeclarativeAnalyzers;
using Remotion.Mixins.Context.FluentBuilders;
using Rhino.Mocks;
using Rhino.Mocks.Interfaces;

namespace Remotion.UnitTests.Mixins.Context.DeclarativeAnalyzers
{
  [TestFixture]
  public class IgnoresAnalyzerTest
  {
    private static readonly Type s_targetClassType = typeof (string);
    private static readonly Type s_mixinType = typeof (int);

    private MockRepository _mockRepository;
    private MixinConfigurationBuilder _configurationBuilderMock;
    private ClassContextBuilder _classBuilderMock;
    private IgnoresAnalyzer _analyzer;

    [SetUp]
    public void SetUp ()
    {
      _mockRepository = new MockRepository ();
      _configurationBuilderMock = _mockRepository.CreateMock<MixinConfigurationBuilder> ((MixinConfiguration) null);
      _classBuilderMock = _mockRepository.CreateMock<ClassContextBuilder> (_configurationBuilderMock, s_targetClassType, null);
      _analyzer = new IgnoresAnalyzer (_configurationBuilderMock);
    }

    [IgnoreForMixinConfiguration]
    [IgnoresClass (typeof (int))]
    [IgnoresClass (typeof (string))]
    [IgnoresMixin (typeof (double))]
    [IgnoresMixin (typeof (float))]
    public class ClassWithMultipleIgnoresAttributes { }

    [Test]
    public void AnalyzeIgnoresClassAttribute ()
    {
      IgnoresClassAttribute attribute = new IgnoresClassAttribute (s_targetClassType);

      Expect.Call (_configurationBuilderMock.ForClass (s_targetClassType)).Return (_classBuilderMock);
      Expect.Call (_classBuilderMock.SuppressMixin (s_mixinType)).Return (_classBuilderMock);

      _mockRepository.ReplayAll ();
      _analyzer.AnalyzeIgnoresClassAttribute (s_mixinType, attribute);
      _mockRepository.VerifyAll();
    }

    [Test]
    public void AnalyzeIgnoresMixinAttribute ()
    {
      IgnoresMixinAttribute attribute = new IgnoresMixinAttribute (s_mixinType);

      Expect.Call (_configurationBuilderMock.ForClass (s_targetClassType)).Return (_classBuilderMock);
      Expect.Call (_classBuilderMock.SuppressMixin (s_mixinType)).Return (_classBuilderMock);

      _mockRepository.ReplayAll ();
      _analyzer.AnalyzeIgnoresMixinAttribute (s_targetClassType, attribute);
      _mockRepository.VerifyAll ();
    }

    [Test]
    public void Analyze ()
    {
      IgnoresAnalyzer analyzer = _mockRepository.CreateMock<IgnoresAnalyzer> (_configurationBuilderMock);

      analyzer.Analyze (typeof (ClassWithMultipleIgnoresAttributes));
      LastCall.CallOriginalMethod (OriginalCallOptions.CreateExpectation);

      IgnoresClassAttribute[] ignoresClassAttributes =
          (IgnoresClassAttribute[]) typeof (ClassWithMultipleIgnoresAttributes).GetCustomAttributes (typeof (IgnoresClassAttribute), false);
      IgnoresMixinAttribute[] ignoresMixinAttributes =
          (IgnoresMixinAttribute[]) typeof (ClassWithMultipleIgnoresAttributes).GetCustomAttributes (typeof (IgnoresMixinAttribute), false);

      analyzer.AnalyzeIgnoresClassAttribute (typeof (ClassWithMultipleIgnoresAttributes), ignoresClassAttributes[0]); // expectation
      analyzer.AnalyzeIgnoresClassAttribute (typeof (ClassWithMultipleIgnoresAttributes), ignoresClassAttributes[1]); // expectation
      analyzer.AnalyzeIgnoresMixinAttribute (typeof (ClassWithMultipleIgnoresAttributes), ignoresMixinAttributes[0]); // expectation
      analyzer.AnalyzeIgnoresMixinAttribute (typeof (ClassWithMultipleIgnoresAttributes), ignoresMixinAttributes[1]); // expectation

      _mockRepository.ReplayAll ();
      analyzer.Analyze (typeof (ClassWithMultipleIgnoresAttributes));
      _mockRepository.VerifyAll ();
    }
  }
}

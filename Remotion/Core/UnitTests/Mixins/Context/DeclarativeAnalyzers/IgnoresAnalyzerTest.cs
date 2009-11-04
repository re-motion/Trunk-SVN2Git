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
      _configurationBuilderMock = _mockRepository.StrictMock<MixinConfigurationBuilder> ((MixinConfiguration) null);
      _classBuilderMock = _mockRepository.StrictMock<ClassContextBuilder> (_configurationBuilderMock, s_targetClassType);
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
      IgnoresAnalyzer analyzer = _mockRepository.StrictMock<IgnoresAnalyzer> (_configurationBuilderMock);

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

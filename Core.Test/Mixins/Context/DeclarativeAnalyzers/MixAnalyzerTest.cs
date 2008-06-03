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
  public class MixAnalyzerTest
  {
    private MockRepository _mockRepository;
    private MixinConfigurationBuilder _configurationBuilderMock;
    private MixAnalyzer _analyzer;

   [SetUp]
    public void SetUp ()
    {
      _mockRepository = new MockRepository();
      _configurationBuilderMock = _mockRepository.CreateMock<MixinConfigurationBuilder>((MixinConfiguration) null);
      _analyzer = new MixAnalyzer(_configurationBuilderMock);
    }

    [Test]
    public void AnalyzeMixAttribute ()
    {
      MixAttribute attribute = new MixAttribute (typeof (object), typeof (float));
      attribute.SuppressedMixins = new Type[] { typeof (int) };
      attribute.AdditionalDependencies = new Type[] { typeof (string) };

      Expect
          .Call (_configurationBuilderMock.AddMixinToClass (MixinKind.Extending, typeof (object), typeof (float), attribute.AdditionalDependencies,
          attribute.SuppressedMixins))
          .Return (null);

      _mockRepository.ReplayAll ();
      _analyzer.AnalyzeMixAttribute (attribute);
      _mockRepository.VerifyAll ();
    }

    [Test]
    public void AnalyzeMixAttribute_WithUsedKind ()
    {
      MixAttribute attribute = new MixAttribute (typeof (object), typeof (float));
      attribute.MixinKind = MixinKind.Used;

      Expect
          .Call (_configurationBuilderMock.AddMixinToClass (MixinKind.Used, typeof (object), typeof (float), attribute.AdditionalDependencies,
          attribute.SuppressedMixins))
          .Return (null);

      _mockRepository.ReplayAll ();
      _analyzer.AnalyzeMixAttribute (attribute);
      _mockRepository.VerifyAll ();
    }

    [Test]
    [ExpectedException (typeof (ConfigurationException), ExpectedMessage = "Supper?")]
    public void AnalyzeMixAttribute_InvalidOperation ()
    {
      MixAttribute attribute = new MixAttribute (typeof (object), typeof (float));
      attribute.SuppressedMixins = new Type[] { typeof (int) };
      attribute.AdditionalDependencies = new Type[] { typeof (string) };

      Expect
          .Call (
          _configurationBuilderMock.AddMixinToClass (MixinKind.Extending, typeof (object),
              typeof (float),
              attribute.AdditionalDependencies,
              attribute.SuppressedMixins))
          .Throw (new InvalidOperationException("Supper?"));

      _mockRepository.ReplayAll ();
      _analyzer.AnalyzeMixAttribute (attribute);
    }

    [Test]
    public void Analyze ()
    {
      MixAnalyzer analyzer = _mockRepository.CreateMock<MixAnalyzer> (_configurationBuilderMock);

      analyzer.Analyze (typeof (MixAnalyzerTest).Assembly);
      LastCall.CallOriginalMethod (OriginalCallOptions.CreateExpectation);

      MixAttribute[] attributes =
          (MixAttribute[]) typeof (MixAnalyzerTest).Assembly.GetCustomAttributes (typeof (MixAttribute), false);
      analyzer.AnalyzeMixAttribute (attributes[0]); // expectation
      analyzer.AnalyzeMixAttribute (attributes[1]); // expectation
      analyzer.AnalyzeMixAttribute (attributes[2]); // expectation

      _mockRepository.ReplayAll ();
      analyzer.Analyze (typeof (MixAnalyzerTest).Assembly);
      _mockRepository.VerifyAll ();
    }
  }
}

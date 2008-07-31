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
using System.Reflection;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
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
   public void MixAttribute_Defaults ()
   {
     MixAttribute attribute = new MixAttribute (typeof (string), typeof (int));
     Assert.That (attribute.AdditionalDependencies, Is.Empty);
     Assert.That (attribute.SuppressedMixins, Is.Empty);
     Assert.That (attribute.IntroducedMemberVisibility, Is.EqualTo (MemberVisibility.Private));
     Assert.That (attribute.MixinKind, Is.EqualTo (MixinKind.Extending));
     Assert.That (attribute.TargetType, Is.EqualTo (typeof (string)));
     Assert.That (attribute.MixinType, Is.EqualTo (typeof (int)));
   }

   [Test]
    public void AnalyzeMixAttribute ()
    {
      MixAttribute attribute = new MixAttribute (typeof (object), typeof (float));

      Expect
          .Call (_configurationBuilderMock.AddMixinToClass (MixinKind.Extending, typeof (object), typeof (float), MemberVisibility.Private, attribute.AdditionalDependencies,
          attribute.SuppressedMixins))
          .Return (null);

      _mockRepository.ReplayAll ();
      _analyzer.AnalyzeMixAttribute (attribute);
      _mockRepository.VerifyAll ();
    }

    [Test]
    public void AnalyzeMixAttribute_SuppressedMixins ()
    {
      MixAttribute attribute = new MixAttribute (typeof (object), typeof (float));
      attribute.SuppressedMixins = new Type[] { typeof (int) };

      Expect
          .Call (_configurationBuilderMock.AddMixinToClass (MixinKind.Extending, typeof (object), typeof (float), MemberVisibility.Private, attribute.AdditionalDependencies,
          attribute.SuppressedMixins))
          .Return (null);

      _mockRepository.ReplayAll ();
      _analyzer.AnalyzeMixAttribute (attribute);
      _mockRepository.VerifyAll ();
    }

    [Test]
    public void AnalyzeMixAttribute_AdditionalDependencies ()
    {
      MixAttribute attribute = new MixAttribute (typeof (object), typeof (float));
      attribute.AdditionalDependencies = new Type[] { typeof (string) };

      Expect
          .Call (_configurationBuilderMock.AddMixinToClass (MixinKind.Extending, typeof (object), typeof (float), MemberVisibility.Private, attribute.AdditionalDependencies,
          attribute.SuppressedMixins))
          .Return (null);

      _mockRepository.ReplayAll ();
      _analyzer.AnalyzeMixAttribute (attribute);
      _mockRepository.VerifyAll ();
    }

    [Test]
    public void AnalyzeMixAttribute_Extending ()
    {
      MixAttribute attribute = new MixAttribute (typeof (object), typeof (float));
      attribute.MixinKind = MixinKind.Extending;

      Expect
          .Call (_configurationBuilderMock.AddMixinToClass (MixinKind.Extending, typeof (object), typeof (float), MemberVisibility.Private, attribute.AdditionalDependencies,
          attribute.SuppressedMixins))
          .Return (null);

      _mockRepository.ReplayAll ();
      _analyzer.AnalyzeMixAttribute (attribute);
      _mockRepository.VerifyAll ();
    }

    [Test]
    public void AnalyzeMixAttribute_Used ()
    {
      MixAttribute attribute = new MixAttribute (typeof (object), typeof (float));
      attribute.MixinKind = MixinKind.Used;

      Expect
          .Call (_configurationBuilderMock.AddMixinToClass (MixinKind.Used, typeof (object), typeof (float), MemberVisibility.Private, attribute.AdditionalDependencies,
          attribute.SuppressedMixins))
          .Return (null);

      _mockRepository.ReplayAll ();
      _analyzer.AnalyzeMixAttribute (attribute);
      _mockRepository.VerifyAll ();
    }

    [Test]
    public void AnalyzeMixAttribute_PrivateVisibility ()
    {
      MixAttribute attribute = new MixAttribute (typeof (object), typeof (float));
      attribute.IntroducedMemberVisibility = MemberVisibility.Private;

      Expect
          .Call (_configurationBuilderMock.AddMixinToClass (MixinKind.Extending, typeof (object), typeof (float), MemberVisibility.Private, attribute.AdditionalDependencies,
          attribute.SuppressedMixins))
          .Return (null);

      _mockRepository.ReplayAll ();
      _analyzer.AnalyzeMixAttribute (attribute);
      _mockRepository.VerifyAll ();
    }

    [Test]
    public void AnalyzeMixAttribute_PublicVisibility ()
    {
      MixAttribute attribute = new MixAttribute (typeof (object), typeof (float));
      attribute.IntroducedMemberVisibility = MemberVisibility.Public;

      Expect
          .Call (_configurationBuilderMock.AddMixinToClass (MixinKind.Extending, typeof (object), typeof (float), MemberVisibility.Public, attribute.AdditionalDependencies,
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
              typeof (float), MemberVisibility.Private,
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

    [Test]
    public void Analyze_IgnoresDuplicates ()
    {
      var duplicateAttributes = new [] {new MixAttribute(typeof (object), typeof (string)), new MixAttribute (typeof (object), typeof (string))};
      
      var analyzer = _mockRepository.CreateMock<MixAnalyzer> (_configurationBuilderMock);
      var assemblyStub = _mockRepository.Stub<ICustomAttributeProvider> ();

      SetupResult.For (assemblyStub.GetCustomAttributes (typeof (MixAttribute), false)).Return (duplicateAttributes);

      analyzer.Analyze (assemblyStub);
      LastCall.CallOriginalMethod (OriginalCallOptions.CreateExpectation);

      analyzer.AnalyzeMixAttribute (duplicateAttributes[0]); // expectation, exactly once

      _mockRepository.ReplayAll ();
      analyzer.Analyze (assemblyStub);
      _mockRepository.VerifyAll ();
    }

    [Test]
    public void Analyze_DuplicatesMeansFullEquality ()
    {
      var duplicateAttributes = new[] { 
        new MixAttribute (typeof (object), typeof (string)) { MixinKind = MixinKind.Extending }, 
        new MixAttribute (typeof (object), typeof (string)) { MixinKind = MixinKind.Used } 
      };

      var analyzer = _mockRepository.CreateMock<MixAnalyzer> (_configurationBuilderMock);
      var assemblyStub = _mockRepository.Stub<ICustomAttributeProvider> ();

      SetupResult.For (assemblyStub.GetCustomAttributes (typeof (MixAttribute), false)).Return (duplicateAttributes);

      analyzer.Analyze (assemblyStub);
      LastCall.CallOriginalMethod (OriginalCallOptions.CreateExpectation);

      analyzer.AnalyzeMixAttribute (duplicateAttributes[0]); // expectation
      analyzer.AnalyzeMixAttribute (duplicateAttributes[1]); // expectation

      _mockRepository.ReplayAll ();
      analyzer.Analyze (assemblyStub);
      _mockRepository.VerifyAll ();
    }
  }
}

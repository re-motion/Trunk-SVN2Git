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
using NUnit.Framework.SyntaxHelpers;
using Remotion.Mixins;
using Remotion.Mixins.Context.DeclarativeAnalyzers;
using Remotion.Mixins.Context.FluentBuilders;
using Rhino.Mocks;
using Rhino.Mocks.Interfaces;

namespace Remotion.UnitTests.Mixins.Context.DeclarativeAnalyzers
{
  [TestFixture]
  public class UsesAnalyzerTest
  {
    private readonly Type _userType = typeof (object);

    private MockRepository _mockRepository;
    private MixinConfigurationBuilder _configurationBuilderMock;
    private UsesAnalyzer _analyzer;

    [SetUp]
    public void SetUp ()
    {
      _mockRepository = new MockRepository();
      _configurationBuilderMock = _mockRepository.StrictMock<MixinConfigurationBuilder>((MixinConfiguration) null);
      _analyzer = new UsesAnalyzer(_configurationBuilderMock);
    }

    [Test]
    public void UsesAttribute_Defaults ()
    {
      UsesAttribute attribute = new UsesAttribute (typeof (string));
      Assert.That (attribute.AdditionalDependencies, Is.Empty);
      Assert.That (attribute.SuppressedMixins, Is.Empty);
      Assert.That (attribute.MixinType, Is.EqualTo (typeof (string)));
      Assert.That (attribute.IntroducedMemberVisibility, Is.EqualTo (MemberVisibility.Private));
    }

    [Test]
    public void AnalyzeUsesAttribute ()
    {
      UsesAttribute attribute = new UsesAttribute (typeof (string));

      Expect
          .Call (_configurationBuilderMock.AddMixinToClass (MixinKind.Used, _userType, typeof (string), MemberVisibility.Private, attribute.AdditionalDependencies, attribute.SuppressedMixins))
          .Return (null);

      _mockRepository.ReplayAll();
      _analyzer.AnalyzeUsesAttribute (_userType, attribute);
      _mockRepository.VerifyAll();
    }

    [Test]
    public void AnalyzeUsesAttribute_AdditionalDependencies ()
    {
      UsesAttribute attribute = new UsesAttribute (typeof (string));
      attribute.AdditionalDependencies = new Type[] { typeof (int) };

      Expect
          .Call (_configurationBuilderMock.AddMixinToClass (MixinKind.Used, _userType, typeof (string), MemberVisibility.Private, attribute.AdditionalDependencies, attribute.SuppressedMixins))
          .Return (null);

      _mockRepository.ReplayAll ();
      _analyzer.AnalyzeUsesAttribute (_userType, attribute);
      _mockRepository.VerifyAll ();
    }

    [Test]
    public void AnalyzeUsesAttribute_SuppressedMixins ()
    {
      UsesAttribute attribute = new UsesAttribute (typeof (string));
      attribute.SuppressedMixins = new Type[] { typeof (double) };

      Expect
          .Call (_configurationBuilderMock.AddMixinToClass (MixinKind.Used, _userType, typeof (string), MemberVisibility.Private, attribute.AdditionalDependencies, attribute.SuppressedMixins))
          .Return (null);

      _mockRepository.ReplayAll ();
      _analyzer.AnalyzeUsesAttribute (_userType, attribute);
      _mockRepository.VerifyAll ();
    }

    [Test]
    public void AnalyzeUsesAttribute_PrivateVisibility ()
    {
      UsesAttribute attribute = new UsesAttribute (typeof (string));
      attribute.SuppressedMixins = new Type[] { typeof (double) };
      attribute.IntroducedMemberVisibility = MemberVisibility.Private;

      Expect
          .Call (_configurationBuilderMock.AddMixinToClass (MixinKind.Used, _userType, typeof (string), MemberVisibility.Private, attribute.AdditionalDependencies, attribute.SuppressedMixins))
          .Return (null);

      _mockRepository.ReplayAll ();
      _analyzer.AnalyzeUsesAttribute (_userType, attribute);
      _mockRepository.VerifyAll ();
    }

    [Test]
    public void AnalyzeUsesAttribute_PublicVisibility ()
    {
      UsesAttribute attribute = new UsesAttribute (typeof (string));
      attribute.SuppressedMixins = new Type[] { typeof (double) };
      attribute.IntroducedMemberVisibility = MemberVisibility.Public;

      Expect
          .Call (_configurationBuilderMock.AddMixinToClass (MixinKind.Used, _userType, typeof (string), MemberVisibility.Public, attribute.AdditionalDependencies, attribute.SuppressedMixins))
          .Return (null);

      _mockRepository.ReplayAll ();
      _analyzer.AnalyzeUsesAttribute (_userType, attribute);
      _mockRepository.VerifyAll ();
    }
    
    [Test]
    [ExpectedException (typeof (ConfigurationException), ExpectedMessage = "Text")]
    public void AnalyzeUsesAttribute_InvalidOperation ()
    {
      UsesAttribute attribute = new UsesAttribute (typeof (string));
      Expect
          .Call (_configurationBuilderMock.AddMixinToClass (MixinKind.Used, null, null, MemberVisibility.Private, null, null))
          .IgnoreArguments()
          .Throw (new InvalidOperationException ("Text"));

      _mockRepository.ReplayAll ();
      _analyzer.AnalyzeUsesAttribute (_userType, attribute);
    }

    [Uses (typeof (int))]
    [Uses (typeof (string))]
    [Uses (typeof (double), IntroducedMemberVisibility = MemberVisibility.Public)]
    [IgnoreForMixinConfiguration]
    public class ClassWithMultipleUsesAttributes { }

    [Test]
    public void Analyze ()
    {
      UsesAnalyzer analyzer = _mockRepository.StrictMock<UsesAnalyzer> (_configurationBuilderMock);

      analyzer.Analyze (typeof (ClassWithMultipleUsesAttributes));
      LastCall.CallOriginalMethod (OriginalCallOptions.CreateExpectation);

      UsesAttribute[] attributes = (UsesAttribute[]) typeof (ClassWithMultipleUsesAttributes).GetCustomAttributes (typeof (UsesAttribute), false);

      analyzer.AnalyzeUsesAttribute (typeof (ClassWithMultipleUsesAttributes), attributes[0]); // expect
      analyzer.AnalyzeUsesAttribute (typeof (ClassWithMultipleUsesAttributes), attributes[1]); // expect
      analyzer.AnalyzeUsesAttribute (typeof (ClassWithMultipleUsesAttributes), attributes[2]); // expect

      _mockRepository.ReplayAll ();
      analyzer.Analyze (typeof (ClassWithMultipleUsesAttributes));
      _mockRepository.VerifyAll ();
    }
  }
}

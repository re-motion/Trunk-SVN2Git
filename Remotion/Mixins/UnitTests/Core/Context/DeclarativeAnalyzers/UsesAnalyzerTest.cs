// This file is part of the re-motion Core Framework (www.re-motion.org)
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
using NUnit.Framework;
using Remotion.Mixins.Context;
using Remotion.Mixins.Context.DeclarativeAnalyzers;
using Remotion.Mixins.Context.FluentBuilders;
using Rhino.Mocks;
using Rhino.Mocks.Interfaces;

namespace Remotion.Mixins.UnitTests.Core.Context.DeclarativeAnalyzers
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
      _configurationBuilderMock = _mockRepository.StrictMock<MixinConfigurationBuilder> ((MixinConfiguration) null);
      _analyzer = new UsesAnalyzer (_configurationBuilderMock);
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
          .Call (
              _configurationBuilderMock.AddMixinToClass (
                  MixinKind.Used,
                  _userType,
                  typeof (string),
                  MemberVisibility.Private,
                  attribute.AdditionalDependencies,
                  attribute.SuppressedMixins,
                  CreateExpectedOrigin (attribute)))
          .Return (null);

      _mockRepository.ReplayAll();
      _analyzer.AnalyzeUsesAttribute (_userType, attribute);
      _mockRepository.VerifyAll();
    }

    [Test]
    public void AnalyzeUsesAttribute_AdditionalDependencies ()
    {
      UsesAttribute attribute = new UsesAttribute (typeof (string));
      attribute.AdditionalDependencies = new[] { typeof (int) };

      Expect
          .Call (
              _configurationBuilderMock.AddMixinToClass (
                  MixinKind.Used,
                  _userType,
                  typeof (string),
                  MemberVisibility.Private,
                  attribute.AdditionalDependencies,
                  attribute.SuppressedMixins,
                  CreateExpectedOrigin (attribute)))
          .Return (null);

      _mockRepository.ReplayAll();
      _analyzer.AnalyzeUsesAttribute (_userType, attribute);
      _mockRepository.VerifyAll();
    }

    [Test]
    public void AnalyzeUsesAttribute_SuppressedMixins ()
    {
      UsesAttribute attribute = new UsesAttribute (typeof (string));
      attribute.SuppressedMixins = new[] { typeof (double) };

      Expect
          .Call (
              _configurationBuilderMock.AddMixinToClass (
                  MixinKind.Used,
                  _userType,
                  typeof (string),
                  MemberVisibility.Private,
                  attribute.AdditionalDependencies,
                  attribute.SuppressedMixins,
                  CreateExpectedOrigin (attribute)))
          .Return (null);

      _mockRepository.ReplayAll();
      _analyzer.AnalyzeUsesAttribute (_userType, attribute);
      _mockRepository.VerifyAll();
    }

    [Test]
    public void AnalyzeUsesAttribute_PrivateVisibility ()
    {
      UsesAttribute attribute = new UsesAttribute (typeof (string));
      attribute.SuppressedMixins = new[] { typeof (double) };
      attribute.IntroducedMemberVisibility = MemberVisibility.Private;

      Expect
          .Call (
              _configurationBuilderMock.AddMixinToClass (
                  MixinKind.Used,
                  _userType,
                  typeof (string),
                  MemberVisibility.Private,
                  attribute.AdditionalDependencies,
                  attribute.SuppressedMixins,
                  CreateExpectedOrigin (attribute)))
          .Return (null);

      _mockRepository.ReplayAll();
      _analyzer.AnalyzeUsesAttribute (_userType, attribute);
      _mockRepository.VerifyAll();
    }

    [Test]
    public void AnalyzeUsesAttribute_PublicVisibility ()
    {
      UsesAttribute attribute = new UsesAttribute (typeof (string));
      attribute.SuppressedMixins = new[] { typeof (double) };
      attribute.IntroducedMemberVisibility = MemberVisibility.Public;

      Expect
          .Call (
              _configurationBuilderMock.AddMixinToClass (
                  MixinKind.Used,
                  _userType,
                  typeof (string),
                  MemberVisibility.Public,
                  attribute.AdditionalDependencies,
                  attribute.SuppressedMixins,
                  CreateExpectedOrigin (attribute)))
          .Return (null);

      _mockRepository.ReplayAll();
      _analyzer.AnalyzeUsesAttribute (_userType, attribute);
      _mockRepository.VerifyAll();
    }

    [Test]
    [ExpectedException (typeof (ConfigurationException), ExpectedMessage = "Text")]
    public void AnalyzeUsesAttribute_InvalidOperation ()
    {
      UsesAttribute attribute = new UsesAttribute (typeof (string));
      Expect
          .Call (_configurationBuilderMock.AddMixinToClass (MixinKind.Used, null, null, MemberVisibility.Private, null, null, null))
          .IgnoreArguments()
          .Throw (new InvalidOperationException ("Text"));

      _mockRepository.ReplayAll();
      _analyzer.AnalyzeUsesAttribute (_userType, attribute);
    }

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

      _mockRepository.ReplayAll();
      analyzer.Analyze (typeof (ClassWithMultipleUsesAttributes));
      _mockRepository.VerifyAll();
    }

    private MixinContextOrigin CreateExpectedOrigin (UsesAttribute attribute, Type userType = null)
    {
      return MixinContextOrigin.CreateForCustomAttribute (attribute, userType ?? _userType);
    }

    [Uses (typeof (int))]
    [Uses (typeof (string))]
    [Uses (typeof (double), IntroducedMemberVisibility = MemberVisibility.Public)]
    [IgnoreForMixinConfiguration]
    public class ClassWithMultipleUsesAttributes
    {
    }
  }
}
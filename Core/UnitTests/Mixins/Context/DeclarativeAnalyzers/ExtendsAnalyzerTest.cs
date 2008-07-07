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
using System.Collections.Generic;
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
  public class ExtendsAnalyzerTest
  {
    private readonly Type _extenderType = typeof (List<>);

    private MockRepository _mockRepository;
    private MixinConfigurationBuilder _configurationBuilderMock;
    private ExtendsAnalyzer _analyzer;

    [SetUp]
    public void SetUp ()
    {
      _mockRepository = new MockRepository();
      _configurationBuilderMock = _mockRepository.CreateMock<MixinConfigurationBuilder>((MixinConfiguration) null);
      _analyzer = new ExtendsAnalyzer(_configurationBuilderMock);
    }

    [Test]
    public void ExtendsAttribute_Defaults ()
    {
      ExtendsAttribute attribute = new ExtendsAttribute (typeof (string));
      Assert.That (attribute.AdditionalDependencies, Is.Empty);
      Assert.That (attribute.SuppressedMixins, Is.Empty);
      Assert.That (attribute.TargetType, Is.EqualTo (typeof (string)));
      Assert.That (attribute.IntroducedMemberVisibility, Is.EqualTo (MemberVisibility.Private));
    }

    [Test]
    public void AnalyzeExtendsAttribute ()
    {
      ExtendsAttribute attribute = new ExtendsAttribute (typeof (object));

      Expect
          .Call (_configurationBuilderMock.AddMixinToClass (MixinKind.Extending, typeof (object), _extenderType, MemberVisibility.Private, attribute.AdditionalDependencies,
          attribute.SuppressedMixins))
          .Return (null);

      _mockRepository.ReplayAll ();
      _analyzer.AnalyzeExtendsAttribute (_extenderType, attribute);
      _mockRepository.VerifyAll ();
    }

    [Test]
    public void AnalyzeExtendsAttribute_SuppressedMixins ()
    {
      ExtendsAttribute attribute = new ExtendsAttribute (typeof (object));
      attribute.SuppressedMixins = new Type[] { typeof (int) };

      Expect
          .Call (_configurationBuilderMock.AddMixinToClass (MixinKind.Extending, typeof (object), _extenderType, MemberVisibility.Private, attribute.AdditionalDependencies, 
          attribute.SuppressedMixins))
          .Return (null);

      _mockRepository.ReplayAll();
      _analyzer.AnalyzeExtendsAttribute (_extenderType, attribute);
      _mockRepository.VerifyAll();
    }

    [Test]
    public void AnalyzeExtendsAttribute_AdditionalDependencies ()
    {
      ExtendsAttribute attribute = new ExtendsAttribute (typeof (object));
      attribute.AdditionalDependencies = new Type[] { typeof (string) };

      Expect
          .Call (_configurationBuilderMock.AddMixinToClass (MixinKind.Extending, typeof (object), _extenderType, MemberVisibility.Private, attribute.AdditionalDependencies,
          attribute.SuppressedMixins))
          .Return (null);

      _mockRepository.ReplayAll ();
      _analyzer.AnalyzeExtendsAttribute (_extenderType, attribute);
      _mockRepository.VerifyAll ();
    }

    [Test]
    public void AnalyzeExtendsAttribute_PrivateVisibility ()
    {
      ExtendsAttribute attribute = new ExtendsAttribute (typeof (object));
      attribute.IntroducedMemberVisibility = MemberVisibility.Private;

      Expect
          .Call (_configurationBuilderMock.AddMixinToClass (MixinKind.Extending, typeof (object), _extenderType, MemberVisibility.Private, attribute.AdditionalDependencies,
          attribute.SuppressedMixins))
          .Return (null);

      _mockRepository.ReplayAll ();
      _analyzer.AnalyzeExtendsAttribute (_extenderType, attribute);
      _mockRepository.VerifyAll ();
    }

    [Test]
    public void AnalyzeExtendsAttribute_PublicVisibility ()
    {
      ExtendsAttribute attribute = new ExtendsAttribute (typeof (object));
      attribute.IntroducedMemberVisibility = MemberVisibility.Public;

      Expect
          .Call (_configurationBuilderMock.AddMixinToClass (MixinKind.Extending, typeof (object), _extenderType, MemberVisibility.Public, attribute.AdditionalDependencies,
          attribute.SuppressedMixins))
          .Return (null);

      _mockRepository.ReplayAll ();
      _analyzer.AnalyzeExtendsAttribute (_extenderType, attribute);
      _mockRepository.VerifyAll ();
    }

    [Test]
    public void AnalyzeExtendsAttribute_Generic ()
    {
      ExtendsAttribute attribute = new ExtendsAttribute (typeof (object));
      attribute.SuppressedMixins = new Type[] { typeof (int) };
      attribute.AdditionalDependencies = new Type[] { typeof (string) };
      attribute.MixinTypeArguments = new Type[] { typeof (double) };

      Expect
          .Call (_configurationBuilderMock.AddMixinToClass (MixinKind.Extending, typeof (object), typeof (List<double>), MemberVisibility.Private, attribute.AdditionalDependencies,
          attribute.SuppressedMixins))
          .Return (null);

      _mockRepository.ReplayAll ();
      _analyzer.AnalyzeExtendsAttribute (_extenderType, attribute);
      _mockRepository.VerifyAll ();
    }

    [Test]
    [ExpectedException (typeof (ConfigurationException), ExpectedMessage = "The ExtendsAttribute for target class System.Object applied to mixin type"
        + " System.Collections.Generic.List`1 specified invalid generic type arguments.")]
    public void AnalyzeExtendsAttribute_InvalidGenericArguments ()
    {
      ExtendsAttribute attribute = new ExtendsAttribute (typeof (object));
      attribute.MixinTypeArguments = new Type[] { typeof (double), typeof (string) };

      _analyzer.AnalyzeExtendsAttribute (_extenderType, attribute);
    }

    [Test]
    [ExpectedException (typeof (ConfigurationException), ExpectedMessage = "Foofa.")]
    public void AnalyzeExtendsAttribute_InvalidOperation ()
    {
      ExtendsAttribute attribute = new ExtendsAttribute (typeof (object));

      Expect
          .Call (_configurationBuilderMock.AddMixinToClass (MixinKind.Extending, null, null, MemberVisibility.Private, null, null))
          .IgnoreArguments()
          .Throw (new InvalidOperationException ("Foofa."));

      _mockRepository.ReplayAll ();
      _analyzer.AnalyzeExtendsAttribute (_extenderType, attribute);
    }

    [Extends (typeof (int))]
    [Extends (typeof (string))]
    [IgnoreForMixinConfiguration]
    public class ClassWithMultipleExtendsAttributes { }

    [Test]
    public void Analyze ()
    {
      ExtendsAnalyzer analyzer = _mockRepository.CreateMock<ExtendsAnalyzer> (_configurationBuilderMock);

      analyzer.Analyze (typeof (ClassWithMultipleExtendsAttributes));
      LastCall.CallOriginalMethod (OriginalCallOptions.CreateExpectation);

      ExtendsAttribute[] attributes =
          (ExtendsAttribute[]) typeof (ClassWithMultipleExtendsAttributes).GetCustomAttributes (typeof (ExtendsAttribute), false);
      analyzer.AnalyzeExtendsAttribute (typeof (ClassWithMultipleExtendsAttributes), attributes[0]); // expectation
      analyzer.AnalyzeExtendsAttribute (typeof (ClassWithMultipleExtendsAttributes), attributes[1]); // expectation

      _mockRepository.ReplayAll();
      analyzer.Analyze (typeof (ClassWithMultipleExtendsAttributes));
      _mockRepository.VerifyAll();
    }
  }
}

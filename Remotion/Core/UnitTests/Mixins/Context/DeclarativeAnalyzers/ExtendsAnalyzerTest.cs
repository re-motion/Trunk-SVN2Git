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
      _configurationBuilderMock = _mockRepository.StrictMock<MixinConfigurationBuilder>((MixinConfiguration) null);
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
        + " System.Collections.Generic.List`1 specified 2 generic type argument(s) when 1 argument(s) were expected.")]
    public void AnalyzeExtendsAttribute_WrongNumberOfGenericArguments ()
    {
      var attribute = new ExtendsAttribute (typeof (object));
      attribute.MixinTypeArguments = new[] { typeof (double), typeof (string) };

      _analyzer.AnalyzeExtendsAttribute (typeof (List<>), attribute);
    }

    [Test]
    [ExpectedException (typeof (ConfigurationException), ExpectedMessage = "The ExtendsAttribute for target class System.Object applied to mixin type"
        + " System.Object specified 2 generic type argument(s) when 0 argument(s) were expected.")]
    public void AnalyzeExtendsAttribute_WrongNumberOfGenericArguments_NoneExpected ()
    {
      var attribute = new ExtendsAttribute (typeof (object));
      attribute.MixinTypeArguments = new[] { typeof (double), typeof (string) };

      _analyzer.AnalyzeExtendsAttribute (typeof (object), attribute);
    }

    [Test]
    public void AnalyzeExtendsAttribute_GenericArgumentsPossible_NoneGiven ()
    {
      var attribute = new ExtendsAttribute (typeof (object));
      
      Expect
          .Call (_configurationBuilderMock.AddMixinToClass (MixinKind.Extending, typeof (object), typeof(List<>), MemberVisibility.Private, attribute.AdditionalDependencies,
                attribute.SuppressedMixins))
          .Return (null);

      _mockRepository.ReplayAll ();
      _analyzer.AnalyzeExtendsAttribute (typeof (List<>), attribute);
      _mockRepository.VerifyAll ();
    }

    [Test]
    [ExpectedException (typeof (ConfigurationException), ExpectedMessage = "The ExtendsAttribute for target class System.Object applied to mixin type"
        + " System.Nullable`1 specified invalid generic type arguments: GenericArguments[0], 'System.String', on 'System.Nullable`1[T]' violates the constraint of type 'T'.")]
    public void AnalyzeExtendsAttribute_WrongKindOfGenericArguments ()
    {
      var attribute = new ExtendsAttribute (typeof (object));
      attribute.MixinTypeArguments = new[] { typeof (string) };

      _analyzer.AnalyzeExtendsAttribute (typeof (Nullable<>), attribute);
    }

    [Test]
    [ExpectedException (typeof (ConfigurationException), ExpectedMessage = "The ExtendsAttribute for target class System.Object applied to mixin type "
        + "System.Collections.Generic.List`1[[System.String, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]] specified "
        + "generic type arguments, but the mixin type already has type arguments specified.")]
    public void AnalyzeExtendsAttribute_GenericArgumentsAlreadyGiven ()
    {
      var attribute = new ExtendsAttribute (typeof (object));
      attribute.MixinTypeArguments = new[] { typeof (string) };

      _analyzer.AnalyzeExtendsAttribute (typeof (List<string>), attribute);
    }

    [Test]
    [ExpectedException (typeof (ConfigurationException), ExpectedMessage = "Foofa.")]
    public void AnalyzeExtendsAttribute_InvalidOperation ()
    {
      var attribute = new ExtendsAttribute (typeof (object));

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
      ExtendsAnalyzer analyzer = _mockRepository.StrictMock<ExtendsAnalyzer> (_configurationBuilderMock);

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

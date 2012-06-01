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
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using NUnit.Framework;
using Remotion.Mixins.Context;
using Remotion.Mixins.Context.DeclarativeAnalyzers;
using Remotion.Mixins.Context.FluentBuilders;
using Rhino.Mocks;
using Rhino.Mocks.Interfaces;

namespace Remotion.Mixins.UnitTests.Core.Context.DeclarativeAnalyzers
{
  [TestFixture]
  public class MixAnalyzerTest
  {
    private MockRepository _mockRepository;
    private MixinConfigurationBuilder _configurationBuilderMock;
    private MixAnalyzer _analyzer;

    private Assembly _assembly;

    [SetUp]
    public void SetUp ()
    {
      _mockRepository = new MockRepository();
      _configurationBuilderMock = _mockRepository.StrictMock<MixinConfigurationBuilder> ((MixinConfiguration) null);
      _analyzer = new MixAnalyzer (_configurationBuilderMock);

      _assembly = GetType().Assembly;
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
          .Call (
              _configurationBuilderMock.AddMixinToClass (
                  MixinKind.Extending,
                  typeof (object),
                  typeof (float),
                  MemberVisibility.Private,
                  attribute.AdditionalDependencies,
                  attribute.SuppressedMixins,
                  CreateExpectedOrigin (attribute)))
          .Return (null);

      _mockRepository.ReplayAll();
      _analyzer.AnalyzeMixAttribute (attribute, _assembly);
      _mockRepository.VerifyAll();
    }

    [Test]
    public void AnalyzeMixAttribute_SuppressedMixins ()
    {
      MixAttribute attribute = new MixAttribute (typeof (object), typeof (float));
      attribute.SuppressedMixins = new[] { typeof (int) };

      Expect
          .Call (
              _configurationBuilderMock.AddMixinToClass (
                  MixinKind.Extending,
                  typeof (object),
                  typeof (float),
                  MemberVisibility.Private,
                  attribute.AdditionalDependencies,
                  attribute.SuppressedMixins,
                  CreateExpectedOrigin (attribute)))
          .Return (null);

      _mockRepository.ReplayAll();
      _analyzer.AnalyzeMixAttribute (attribute, _assembly);
      _mockRepository.VerifyAll();
    }

    [Test]
    public void AnalyzeMixAttribute_AdditionalDependencies ()
    {
      MixAttribute attribute = new MixAttribute (typeof (object), typeof (float));
      attribute.AdditionalDependencies = new[] { typeof (string) };

      Expect
          .Call (
              _configurationBuilderMock.AddMixinToClass (
                  MixinKind.Extending,
                  typeof (object),
                  typeof (float),
                  MemberVisibility.Private,
                  attribute.AdditionalDependencies,
                  attribute.SuppressedMixins,
                  CreateExpectedOrigin (attribute)))
          .Return (null);

      _mockRepository.ReplayAll();
      _analyzer.AnalyzeMixAttribute (attribute, _assembly);
      _mockRepository.VerifyAll();
    }

    [Test]
    public void AnalyzeMixAttribute_Extending ()
    {
      MixAttribute attribute = new MixAttribute (typeof (object), typeof (float));
      attribute.MixinKind = MixinKind.Extending;

      Expect
          .Call (
              _configurationBuilderMock.AddMixinToClass (
                  MixinKind.Extending,
                  typeof (object),
                  typeof (float),
                  MemberVisibility.Private,
                  attribute.AdditionalDependencies,
                  attribute.SuppressedMixins,
                  CreateExpectedOrigin (attribute)))
          .Return (null);

      _mockRepository.ReplayAll();
      _analyzer.AnalyzeMixAttribute (attribute, _assembly);
      _mockRepository.VerifyAll();
    }

    [Test]
    public void AnalyzeMixAttribute_Used ()
    {
      MixAttribute attribute = new MixAttribute (typeof (object), typeof (float));
      attribute.MixinKind = MixinKind.Used;

      Expect
          .Call (
              _configurationBuilderMock.AddMixinToClass (
                  MixinKind.Used,
                  typeof (object),
                  typeof (float),
                  MemberVisibility.Private,
                  attribute.AdditionalDependencies,
                  attribute.SuppressedMixins,
                 CreateExpectedOrigin (attribute)))
          .Return (null);

      _mockRepository.ReplayAll();
      _analyzer.AnalyzeMixAttribute (attribute, _assembly);
      _mockRepository.VerifyAll();
    }

    [Test]
    public void AnalyzeMixAttribute_PrivateVisibility ()
    {
      MixAttribute attribute = new MixAttribute (typeof (object), typeof (float));
      attribute.IntroducedMemberVisibility = MemberVisibility.Private;

      Expect
          .Call (
              _configurationBuilderMock.AddMixinToClass (
                  MixinKind.Extending,
                  typeof (object),
                  typeof (float),
                  MemberVisibility.Private,
                  attribute.AdditionalDependencies,
                  attribute.SuppressedMixins,
                 CreateExpectedOrigin (attribute)))
          .Return (null);

      _mockRepository.ReplayAll();
      _analyzer.AnalyzeMixAttribute (attribute, _assembly);
      _mockRepository.VerifyAll();
    }

    [Test]
    public void AnalyzeMixAttribute_PublicVisibility ()
    {
      MixAttribute attribute = new MixAttribute (typeof (object), typeof (float));
      attribute.IntroducedMemberVisibility = MemberVisibility.Public;

      Expect
          .Call (
              _configurationBuilderMock.AddMixinToClass (
                  MixinKind.Extending,
                  typeof (object),
                  typeof (float),
                  MemberVisibility.Public,
                  attribute.AdditionalDependencies,
                  attribute.SuppressedMixins,
                  CreateExpectedOrigin (attribute)))
          .Return (null);

      _mockRepository.ReplayAll();
      _analyzer.AnalyzeMixAttribute (attribute, _assembly);
      _mockRepository.VerifyAll();
    }

    [Test]
    [ExpectedException (typeof (ConfigurationException), ExpectedMessage = "Supper?")]
    public void AnalyzeMixAttribute_InvalidOperation ()
    {
      MixAttribute attribute = new MixAttribute (typeof (object), typeof (float));
      attribute.SuppressedMixins = new[] { typeof (int) };
      attribute.AdditionalDependencies = new[] { typeof (string) };

      Expect
          .Call (
              _configurationBuilderMock.AddMixinToClass (
                  MixinKind.Extending,
                  typeof (object),
                  typeof (float),
                  MemberVisibility.Private,
                  attribute.AdditionalDependencies,
                  attribute.SuppressedMixins,
                 CreateExpectedOrigin (attribute)))
          .Throw (new InvalidOperationException ("Supper?"));

      _mockRepository.ReplayAll();
      _analyzer.AnalyzeMixAttribute (attribute, _assembly);
    }

    [Test]
    public void Analyze ()
    {
      var attributes = new[] { new MixAttribute (typeof (object), typeof (object)), new MixAttribute (typeof (string), typeof (string)) };
      var assembly = CreateAssemblyWithAttributes (attributes);

      var analyzerMock = _mockRepository.StrictMock<MixAnalyzer> (_configurationBuilderMock);
      analyzerMock.Analyze (assembly);
      LastCall.CallOriginalMethod (OriginalCallOptions.CreateExpectation);

      analyzerMock.AnalyzeMixAttribute (attributes[0], assembly); // expectation
      analyzerMock.AnalyzeMixAttribute (attributes[1], assembly); // expectation
      _mockRepository.ReplayAll();

      analyzerMock.Analyze (assembly);
      
      _mockRepository.VerifyAll();
    }

    [Test]
    public void Analyze_IgnoresDuplicates ()
    {
      var duplicateAttributes = new[] { new MixAttribute (typeof (object), typeof (string)), new MixAttribute (typeof (object), typeof (string)) };
      var assembly = CreateAssemblyWithAttributes (duplicateAttributes);
      
      var analyzer = _mockRepository.StrictMock<MixAnalyzer> (_configurationBuilderMock);
      analyzer.Analyze (assembly);
      LastCall.CallOriginalMethod (OriginalCallOptions.CreateExpectation);

      // Duplicate is ignored
      analyzer.Expect (mock => mock.AnalyzeMixAttribute (duplicateAttributes[0], assembly)).Repeat.Once ();
      _mockRepository.ReplayAll ();

      analyzer.Analyze (assembly);

      _mockRepository.VerifyAll();
    }

    [Test]
    public void Analyze_DuplicatesMeansFullEquality ()
    {
      var duplicateAttributes = new[]
                                {
                                    new MixAttribute (typeof (object), typeof (string)) { MixinKind = MixinKind.Extending },
                                    new MixAttribute (typeof (object), typeof (string)) { MixinKind = MixinKind.Used }
                                };
      var assembly = CreateAssemblyWithAttributes (duplicateAttributes);

      var analyzer = _mockRepository.StrictMock<MixAnalyzer> (_configurationBuilderMock);
      analyzer.Analyze (assembly);
      LastCall.CallOriginalMethod (OriginalCallOptions.CreateExpectation);

      analyzer.Expect (mock => mock.AnalyzeMixAttribute (duplicateAttributes[0], assembly));
      analyzer.Expect (mock => mock.AnalyzeMixAttribute (duplicateAttributes[1], assembly));
      _mockRepository.ReplayAll();

      analyzer.Analyze (assembly);
      
      _mockRepository.VerifyAll();
    }

    private Assembly CreateAssemblyWithAttributes (MixAttribute[] attributes)
    {
      var assemblyName = new AssemblyName (GetType().Name + "_" + Guid.NewGuid());
      var assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly (assemblyName, AssemblyBuilderAccess.Run);

      var constructorInfo = typeof (MixAttribute).GetConstructors ().Single ();
      var settableProperties = typeof (MixAttribute).GetProperties().Where (pi => pi.CanWrite).ToArray();
      
      foreach (var mixAttribute in attributes)
      {
        var constructorArgs = new[] { mixAttribute.TargetType, mixAttribute.MixinType };
// ReSharper disable AccessToModifiedClosure
        var propertyValues = settableProperties.Select (pi => pi.GetValue (mixAttribute, null)).ToArray();
// ReSharper restore AccessToModifiedClosure
        var customAttributeBuilder = new CustomAttributeBuilder (constructorInfo, constructorArgs, settableProperties, propertyValues);

        assemblyBuilder.SetCustomAttribute (customAttributeBuilder);
      }

      return assemblyBuilder;
    }

    private MixinContextOrigin CreateExpectedOrigin (MixAttribute attribute)
    {
      return MixinContextOrigin.CreateForCustomAttribute (attribute, _assembly);
    }
  }
}
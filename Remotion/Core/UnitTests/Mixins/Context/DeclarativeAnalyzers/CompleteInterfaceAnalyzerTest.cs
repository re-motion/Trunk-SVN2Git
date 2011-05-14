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
using Remotion.UnitTests.Mixins.TestDomain;
using Rhino.Mocks;

namespace Remotion.UnitTests.Mixins.Context.DeclarativeAnalyzers
{
  [TestFixture]
  public class CompleteInterfaceAnalyzerTest
  {
    private MockRepository _mockRepository;
    private MixinConfigurationBuilder _configurationBuilderMock;
    private CompleteInterfaceAnalyzer _analyzer;

    [SetUp]
    public void SetUp ()
    {
      _mockRepository = new MockRepository();
      _configurationBuilderMock = _mockRepository.StrictMock<MixinConfigurationBuilder>((MixinConfiguration) null);
      _analyzer = new CompleteInterfaceAnalyzer(_configurationBuilderMock);
    }

    [Test]
    public void AnalyzeCompleteInterfaceAttribute ()
    {
      CompleteInterfaceAttribute attribute = new CompleteInterfaceAttribute (typeof (string));
      ClassContextBuilder classBuilderMock = _mockRepository.StrictMock<ClassContextBuilder> (_configurationBuilderMock, typeof (string));

      using (_mockRepository.Ordered ())
      {
        Expect.Call (_configurationBuilderMock.ForClass (typeof (string))).Return (classBuilderMock);
        Expect.Call (classBuilderMock.AddCompleteInterface (typeof (IServiceProvider))).Return (classBuilderMock);
      }

      _mockRepository.ReplayAll();
      _analyzer.AnalyzeCompleteInterfaceAttribute (typeof (IServiceProvider), attribute);
      _mockRepository.VerifyAll ();
    }

    [CompleteInterface (typeof (int))]
    [CompleteInterface (typeof (string))]
    [IgnoreForMixinConfiguration]
    public interface IInterfaceWithMultipleCompleteInterfaceAttributes { }

    [Test]
    public void Analyze ()
    {
      var intBuilderMock = MockRepository.GenerateStrictMock<ClassContextBuilder> (typeof (int));
      var stringBuilderMock = MockRepository.GenerateStrictMock<ClassContextBuilder> (typeof (string));

      _configurationBuilderMock.Expect (mock => mock.ForClass (typeof (int))).Return (intBuilderMock);
      _configurationBuilderMock.Expect (mock => mock.ForClass (typeof (string))).Return (stringBuilderMock);
      _configurationBuilderMock.Replay();

      intBuilderMock.Expect (mock => mock.AddCompleteInterface (typeof (IInterfaceWithMultipleCompleteInterfaceAttributes))).Return (null);
      intBuilderMock.Replay ();

      stringBuilderMock.Expect (mock => mock.AddCompleteInterface (typeof (IInterfaceWithMultipleCompleteInterfaceAttributes))).Return (null);
      stringBuilderMock.Replay ();

      var analyzer = new CompleteInterfaceAnalyzer (_configurationBuilderMock);
      analyzer.Analyze (typeof (IInterfaceWithMultipleCompleteInterfaceAttributes));

      _configurationBuilderMock.VerifyAllExpectations();
      intBuilderMock.VerifyAllExpectations ();
      stringBuilderMock.VerifyAllExpectations ();
    }

    [Test]
    public void Analyze_IncludesClasses_ImplementingIHasCompleteInterface ()
    {
      var classBuilderMock = MockRepository.GenerateStrictMock<ClassContextBuilder> (typeof (int));

      _configurationBuilderMock.Expect (mock => mock.ForClass (typeof (ClassWithHasCompleteInterfaces))).Return (classBuilderMock);
      _configurationBuilderMock.Replay ();

      classBuilderMock
          .Expect (mock => mock.AddCompleteInterfaces (
              typeof (ClassWithHasCompleteInterfaces.ICompleteInterface1), 
              typeof (ClassWithHasCompleteInterfaces.ICompleteInterface2)))
          .Return (null);
      classBuilderMock.Replay ();

      var analyzer = new CompleteInterfaceAnalyzer (_configurationBuilderMock);
      analyzer.Analyze (typeof (ClassWithHasCompleteInterfaces));

      _configurationBuilderMock.VerifyAllExpectations ();
      classBuilderMock.VerifyAllExpectations ();
    }

    [Test]
    public void Analyze_IgnoresClasses_ImplementingIHasCompleteInterfaceWithGenericParameters ()
    {
      _configurationBuilderMock.Replay ();

      var analyzer = new CompleteInterfaceAnalyzer (_configurationBuilderMock);
      analyzer.Analyze (typeof (BaseClassWithHasCompleteInterface<>));

      _configurationBuilderMock.AssertWasNotCalled (mock => mock.ForClass (Arg<Type>.Is.Anything));
    }
  }
}

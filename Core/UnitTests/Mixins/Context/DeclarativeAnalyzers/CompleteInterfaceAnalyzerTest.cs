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
  public class CompleteInterfaceAnalyzerTest
  {
    private MockRepository _mockRepository;
    private MixinConfigurationBuilder _configurationBuilderMock;
    private CompleteInterfaceAnalyzer _analyzer;

    [SetUp]
    public void SetUp ()
    {
      _mockRepository = new MockRepository();
      _configurationBuilderMock = _mockRepository.CreateMock<MixinConfigurationBuilder>((MixinConfiguration) null);
      _analyzer = new CompleteInterfaceAnalyzer(_configurationBuilderMock);
    }

    [Test]
    public void AnalyzeCompleteInterfaceAttribute ()
    {
      CompleteInterfaceAttribute attribute = new CompleteInterfaceAttribute (typeof (string));
      ClassContextBuilder classBuilderMock = _mockRepository.CreateMock<ClassContextBuilder> (_configurationBuilderMock, typeof (string), null);

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
      CompleteInterfaceAnalyzer analyzer = _mockRepository.CreateMock<CompleteInterfaceAnalyzer> (_configurationBuilderMock);

      analyzer.Analyze (typeof (IInterfaceWithMultipleCompleteInterfaceAttributes));
      LastCall.CallOriginalMethod (OriginalCallOptions.CreateExpectation);

      CompleteInterfaceAttribute[] attributes = (CompleteInterfaceAttribute[]) typeof (IInterfaceWithMultipleCompleteInterfaceAttributes).GetCustomAttributes (typeof (CompleteInterfaceAttribute), false);

      analyzer.AnalyzeCompleteInterfaceAttribute (typeof (IInterfaceWithMultipleCompleteInterfaceAttributes), attributes[0]); // expect
      analyzer.AnalyzeCompleteInterfaceAttribute (typeof (IInterfaceWithMultipleCompleteInterfaceAttributes), attributes[1]); // expect

      _mockRepository.ReplayAll ();
      analyzer.Analyze (typeof (IInterfaceWithMultipleCompleteInterfaceAttributes));
      _mockRepository.VerifyAll ();
    }
  }
}

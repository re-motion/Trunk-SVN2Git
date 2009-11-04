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
      CompleteInterfaceAnalyzer analyzer = _mockRepository.StrictMock<CompleteInterfaceAnalyzer> (_configurationBuilderMock);

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

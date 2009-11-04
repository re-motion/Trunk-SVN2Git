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

namespace Remotion.UnitTests.Mixins.Context.DeclarativeAnalyzers
{
  [TestFixture]
  public class DeclarativeConfigurationAnalyzerTest
  {
    private MockRepository _mockRepository;
    private MixinConfigurationBuilder _configurationBuilderMock;
    private ExtendsAnalyzer _extendsAnalyzerMock;
    private UsesAnalyzer _usesAnalyzerMock;
    private CompleteInterfaceAnalyzer _completeInterfaceAnalyzerMock;
    private MixAnalyzer _mixAnalyzerMock;
    private IgnoresAnalyzer _ignoresAnalyzerMock;

    [SetUp]
    public void SetUp ()
    {
      _mockRepository = new MockRepository();
      _configurationBuilderMock = _mockRepository.StrictMock<MixinConfigurationBuilder> ((MixinConfiguration) null);

      _extendsAnalyzerMock = _mockRepository.StrictMock<ExtendsAnalyzer> (_configurationBuilderMock);
      _usesAnalyzerMock = _mockRepository.StrictMock<UsesAnalyzer> (_configurationBuilderMock);
      _completeInterfaceAnalyzerMock = _mockRepository.StrictMock<CompleteInterfaceAnalyzer> (_configurationBuilderMock);
      _mixAnalyzerMock = _mockRepository.StrictMock<MixAnalyzer> (_configurationBuilderMock);
      _ignoresAnalyzerMock = _mockRepository.StrictMock<IgnoresAnalyzer> (_configurationBuilderMock);
    }

    [Test]
    public void Analyze ()
    {
      Type[] types = new Type[] { typeof (object), typeof (string), typeof (DeclarativeConfigurationAnalyzerTest) };

      using (_mockRepository.Unordered ())
      {
        _extendsAnalyzerMock.Analyze (typeof (object)); // expectation
        _extendsAnalyzerMock.Analyze (typeof (string)); // expectation
        _extendsAnalyzerMock.Analyze (typeof (DeclarativeConfigurationAnalyzerTest)); // expectation
        _usesAnalyzerMock.Analyze (typeof (object)); // expectation
        _usesAnalyzerMock.Analyze (typeof (string)); // expectation
        _usesAnalyzerMock.Analyze (typeof (DeclarativeConfigurationAnalyzerTest)); // expectation
        _completeInterfaceAnalyzerMock.Analyze (typeof (object)); // expectation
        _completeInterfaceAnalyzerMock.Analyze (typeof (string)); // expectation
        _completeInterfaceAnalyzerMock.Analyze (typeof (DeclarativeConfigurationAnalyzerTest)); // expectation
        _mixAnalyzerMock.Analyze (typeof (object).Assembly);
        _mixAnalyzerMock.Analyze (typeof (DeclarativeConfigurationAnalyzerTest).Assembly);
        _ignoresAnalyzerMock.Analyze (typeof (object)); // expectation
        _ignoresAnalyzerMock.Analyze (typeof (string)); // expectation
        _ignoresAnalyzerMock.Analyze (typeof (DeclarativeConfigurationAnalyzerTest)); // expectation
      }

      _mockRepository.ReplayAll();

      DeclarativeConfigurationAnalyzer analyzer = new DeclarativeConfigurationAnalyzer (_extendsAnalyzerMock, _usesAnalyzerMock, 
          _completeInterfaceAnalyzerMock, _mixAnalyzerMock, _ignoresAnalyzerMock);
      analyzer.Analyze (types);

      _mockRepository.VerifyAll();
    }
  }
}

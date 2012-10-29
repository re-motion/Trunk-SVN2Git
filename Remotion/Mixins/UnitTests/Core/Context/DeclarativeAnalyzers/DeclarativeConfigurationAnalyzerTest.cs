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
using Remotion.Mixins.Context.DeclarativeAnalyzers;
using Remotion.Mixins.Context.FluentBuilders;
using Rhino.Mocks;

namespace Remotion.Mixins.UnitTests.Core.Context.DeclarativeAnalyzers
{
  [TestFixture]
  public class DeclarativeConfigurationAnalyzerTest
  {
    private MockRepository _mockRepository;
    private MixinConfigurationBuilder _fakeConfigurationBuilder;
    private ExtendsAnalyzer _extendsAnalyzerMock;
    private UsesAnalyzer _usesAnalyzerMock;
    private HasCompleteInterfaceMarkerAnalyzer _hasCompleteInterfaceMarkerAnalyzerMock;
    private MixAnalyzer _mixAnalyzerMock;
    private IgnoresAnalyzer _ignoresAnalyzerMock;

    [SetUp]
    public void SetUp ()
    {
      _mockRepository = new MockRepository();
      _fakeConfigurationBuilder = _mockRepository.Stub<MixinConfigurationBuilder> ((MixinConfiguration) null);

      _extendsAnalyzerMock = _mockRepository.StrictMock<ExtendsAnalyzer> ();
      _usesAnalyzerMock = _mockRepository.StrictMock<UsesAnalyzer> ();
      _hasCompleteInterfaceMarkerAnalyzerMock = _mockRepository.StrictMock<HasCompleteInterfaceMarkerAnalyzer> ();
      _mixAnalyzerMock = _mockRepository.StrictMock<MixAnalyzer> ();
      _ignoresAnalyzerMock = _mockRepository.StrictMock<IgnoresAnalyzer> ();
    }

    [Test]
    public void Analyze ()
    {
      Type[] types = new[] { typeof (object), typeof (string), typeof (DeclarativeConfigurationAnalyzerTest) };

      using (_mockRepository.Unordered ())
      {
        _extendsAnalyzerMock.Analyze (typeof (object), _fakeConfigurationBuilder); // expectation
        _extendsAnalyzerMock.Analyze (typeof (string), _fakeConfigurationBuilder); // expectation
        _extendsAnalyzerMock.Analyze (typeof (DeclarativeConfigurationAnalyzerTest), _fakeConfigurationBuilder); // expectation
        _usesAnalyzerMock.Analyze (typeof (object), _fakeConfigurationBuilder); // expectation
        _usesAnalyzerMock.Analyze (typeof (string), _fakeConfigurationBuilder); // expectation
        _usesAnalyzerMock.Analyze (typeof (DeclarativeConfigurationAnalyzerTest), _fakeConfigurationBuilder); // expectation
        _hasCompleteInterfaceMarkerAnalyzerMock.Analyze (typeof (object), _fakeConfigurationBuilder); // expectation
        _hasCompleteInterfaceMarkerAnalyzerMock.Analyze (typeof (string), _fakeConfigurationBuilder); // expectation
        _hasCompleteInterfaceMarkerAnalyzerMock.Analyze (typeof (DeclarativeConfigurationAnalyzerTest), _fakeConfigurationBuilder); // expectation
        _mixAnalyzerMock.Analyze (typeof (object).Assembly, _fakeConfigurationBuilder);
        _mixAnalyzerMock.Analyze (typeof (DeclarativeConfigurationAnalyzerTest).Assembly, _fakeConfigurationBuilder);
        _ignoresAnalyzerMock.Analyze (typeof (object), _fakeConfigurationBuilder); // expectation
        _ignoresAnalyzerMock.Analyze (typeof (string), _fakeConfigurationBuilder); // expectation
        _ignoresAnalyzerMock.Analyze (typeof (DeclarativeConfigurationAnalyzerTest), _fakeConfigurationBuilder); // expectation
      }

      _mockRepository.ReplayAll();

      DeclarativeConfigurationAnalyzer analyzer = new DeclarativeConfigurationAnalyzer (_extendsAnalyzerMock, _usesAnalyzerMock, 
          _hasCompleteInterfaceMarkerAnalyzerMock, _mixAnalyzerMock, _ignoresAnalyzerMock);
      analyzer.Analyze (types, _fakeConfigurationBuilder);

      _mockRepository.VerifyAll();
    }
  }
}

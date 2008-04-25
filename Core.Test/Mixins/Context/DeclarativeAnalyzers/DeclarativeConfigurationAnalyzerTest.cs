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
      _configurationBuilderMock = _mockRepository.CreateMock<MixinConfigurationBuilder> ((MixinConfiguration) null);

      _extendsAnalyzerMock = _mockRepository.CreateMock<ExtendsAnalyzer> (_configurationBuilderMock);
      _usesAnalyzerMock = _mockRepository.CreateMock<UsesAnalyzer> (_configurationBuilderMock);
      _completeInterfaceAnalyzerMock = _mockRepository.CreateMock<CompleteInterfaceAnalyzer> (_configurationBuilderMock);
      _mixAnalyzerMock = _mockRepository.CreateMock<MixAnalyzer> (_configurationBuilderMock);
      _ignoresAnalyzerMock = _mockRepository.CreateMock<IgnoresAnalyzer> (_configurationBuilderMock);
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
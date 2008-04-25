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
      _configurationBuilderMock = _mockRepository.CreateMock<MixinConfigurationBuilder>((MixinConfiguration) null);
      _analyzer = new UsesAnalyzer(_configurationBuilderMock);
    }

    [Test]
    public void AnalyzeUsesAttribute ()
    {
      UsesAttribute attribute = new UsesAttribute (typeof (string));
      attribute.AdditionalDependencies = new Type[] { typeof (int) };
      attribute.SuppressedMixins = new Type[] { typeof (double) };

      Expect
          .Call (_configurationBuilderMock.AddMixinToClass (_userType, typeof (string), attribute.AdditionalDependencies, attribute.SuppressedMixins))
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
          .Call (_configurationBuilderMock.AddMixinToClass (null, null, null, null))
          .IgnoreArguments()
          .Throw (new InvalidOperationException ("Text"));

      _mockRepository.ReplayAll ();
      _analyzer.AnalyzeUsesAttribute (_userType, attribute);
    }

    [Uses (typeof (int))]
    [Uses (typeof (string))]
    [IgnoreForMixinConfiguration]
    public class ClassWithMultipleUsesAttributes { }

    [Test]
    public void Analyze ()
    {
      UsesAnalyzer analyzer = _mockRepository.CreateMock<UsesAnalyzer> (_configurationBuilderMock);

      analyzer.Analyze (typeof (ClassWithMultipleUsesAttributes));
      LastCall.CallOriginalMethod (OriginalCallOptions.CreateExpectation);

      UsesAttribute[] attributes = (UsesAttribute[]) typeof (ClassWithMultipleUsesAttributes).GetCustomAttributes (typeof (UsesAttribute), false);

      analyzer.AnalyzeUsesAttribute (typeof (ClassWithMultipleUsesAttributes), attributes[0]); // expect
      analyzer.AnalyzeUsesAttribute (typeof (ClassWithMultipleUsesAttributes), attributes[1]); // expect

      _mockRepository.ReplayAll ();
      analyzer.Analyze (typeof (ClassWithMultipleUsesAttributes));
      _mockRepository.VerifyAll ();
    }
  }
}
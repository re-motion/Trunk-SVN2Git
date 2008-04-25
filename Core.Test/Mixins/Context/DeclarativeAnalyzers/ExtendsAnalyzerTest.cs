using System;
using System.Collections.Generic;
using NUnit.Framework;
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
    public void AnalyzeExtendsAttribute_NonGeneric ()
    {
      ExtendsAttribute attribute = new ExtendsAttribute (typeof (object));
      attribute.SuppressedMixins = new Type[] { typeof (int) };
      attribute.AdditionalDependencies = new Type[] { typeof (string) };

      Expect
          .Call (_configurationBuilderMock.AddMixinToClass (typeof (object), _extenderType, attribute.AdditionalDependencies, 
          attribute.SuppressedMixins))
          .Return (null);

      _mockRepository.ReplayAll();
      _analyzer.AnalyzeExtendsAttribute (_extenderType, attribute);
      _mockRepository.VerifyAll();
    }

    [Test]
    public void AnalyzeExtendsAttribute_Generic ()
    {
      ExtendsAttribute attribute = new ExtendsAttribute (typeof (object));
      attribute.SuppressedMixins = new Type[] { typeof (int) };
      attribute.AdditionalDependencies = new Type[] { typeof (string) };
      attribute.MixinTypeArguments = new Type[] { typeof (double) };

      Expect
          .Call (_configurationBuilderMock.AddMixinToClass (typeof (object), typeof (List<double>), attribute.AdditionalDependencies,
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
          .Call (_configurationBuilderMock.AddMixinToClass (null, null, null, null))
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
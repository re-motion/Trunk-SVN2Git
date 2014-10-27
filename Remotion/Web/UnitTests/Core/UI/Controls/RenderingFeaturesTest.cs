using System;
using NUnit.Framework;
using Remotion.Web.UI.Controls;

namespace Remotion.Web.UnitTests.Core.UI.Controls
{
  [TestFixture]
  public class RenderingFeaturesTest
  {
    [Test]
    public void TestDefaultRenderingFeatures ()
    {
      Assert.That (RenderingFeatures.Default.EnableDiagnosticMetadata, Is.False);
    }

    [Test]
    public void TestWithDiagnosticMetadataRenderingFeatures ()
    {
      Assert.That (RenderingFeatures.WithDiagnosticMetadata.EnableDiagnosticMetadata, Is.True);
    }
  }
}
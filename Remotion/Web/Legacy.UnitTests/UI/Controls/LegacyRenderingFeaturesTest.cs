using System;
using NUnit.Framework;
using Remotion.Web.Legacy.UI.Controls;

namespace Remotion.Web.Legacy.UnitTests.UI.Controls
{
  [TestFixture]
  public class LegacyRenderingFeaturesTest
  {
    [Test]
    public void TestLegacyRenderingFeatures ()
    {
      Assert.That (LegacyRenderingFeatures.ForLegacy.EnableDiagnosticMetadata, Is.False);
    }
  }
}
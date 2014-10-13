using System;
using System.IO;
using System.Web.UI;
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.Development.Web.UnitTesting.UI.Controls;
using Remotion.ServiceLocation;
using Remotion.Web.Contract.DiagnosticMetadata;
using Remotion.Web.UI.Controls;

namespace Remotion.Web.UnitTests.Core.UI.Controls.WebButtonTests
{
  [TestFixture]
  public class WebButtonDiagnosticMetadataTest : BaseTest
  {
    private ServiceLocatorScope _serviceLocatorScope;

    public override void SetUp ()
    {
      base.SetUp();

      var serviceLocator = DefaultServiceLocator.Create();
      serviceLocator.RegisterSingle<IRenderingFeatures> (() => new WithDiagnosticMetadataRenderingFeatures());
      _serviceLocatorScope = new ServiceLocatorScope (serviceLocator);
    }

    public override void TearDown ()
    {
      base.TearDown();
      _serviceLocatorScope.Dispose();
    }

    [Test]
    public void RenderDiagnosticMetadataAttributes ()
    {
      var webButton = new TestWebButton { ID = "WebButton", Text = "My Button" };

      var renderedText = RenderControl (webButton);

      Assert.That (renderedText, Is.StringContaining (DiagnosticMetadataAttributes.Text + "=\"" + webButton.Text + "\""));
      Assert.That (renderedText, Is.Not.StringContaining (DiagnosticMetadataAttributes.CommandName));
      Assert.That (renderedText, Is.StringContaining (DiagnosticMetadataAttributes.TriggersPostBack + "=\"true\""));
    }

    [Test]
    public void RenderDiagnosticMetadataAttributesWithCommand ()
    {
      var webButton = new TestWebButton { ID = "WebButton", CommandName = "MyCommand" };

      var renderedText = RenderControl (webButton);

      Assert.That (renderedText, Is.StringContaining (DiagnosticMetadataAttributes.CommandName + "=\"MyCommand\""));
      Assert.That (renderedText, Is.StringContaining (DiagnosticMetadataAttributes.TriggersPostBack + "=\"true\""));
    }

    private string RenderControl (Control control)
    {
      var page = new Page();
      page.Controls.Add (control);

      var ci = new ControlInvoker (page);
      ci.InitRecursive();
      ci.LoadRecursive();
      ci.PreRenderRecursive();
      var stringWriter = new StringWriter();
      control.RenderControl (new HtmlTextWriter (stringWriter));

      return stringWriter.ToString();
    }
  }
}
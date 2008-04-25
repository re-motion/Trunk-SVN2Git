using System;
using System.Web.UI;
using NUnit.Framework;
using Remotion.Security;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls;
using Remotion.Web.UnitTests.AspNetFramework;

namespace Remotion.Web.UnitTests.UI.Controls.CommandTests
{
  [TestFixture]
  public class WxeFunctionCommandTest : BaseTest
  {
    private CommandTestHelper _testHelper;

    [SetUp]
    public virtual void SetUp ()
    {
      _testHelper = new CommandTestHelper ();
      HttpContextHelper.SetCurrent (_testHelper.HttpContext);
    }

    [Test]
    public void HasAccess_WithoutSeucrityProvider ()
    {
      Command command = _testHelper.CreateWxeFunctionCommand ();
      _testHelper.ReplayAll ();

      bool hasAccess = command.HasAccess (null);

      _testHelper.VerifyAll ();
      Assert.IsTrue (hasAccess);
    }

    [Test]
    public void HasAccess_WithAccessGranted ()
    {
      SecurityAdapterRegistry.Instance.SetAdapter (typeof (IWebSecurityAdapter), _testHelper.WebSecurityAdapter);
      SecurityAdapterRegistry.Instance.SetAdapter (typeof (IWxeSecurityAdapter), _testHelper.WxeSecurityAdapter);
      Command command = _testHelper.CreateWxeFunctionCommand ();
      _testHelper.ExpectWxeSecurityProviderHasStatelessAccess (typeof (ExecutionEngine.TestFunction), true);
      _testHelper.ReplayAll ();

      bool hasAccess = command.HasAccess (null);

      _testHelper.VerifyAll ();
      Assert.IsTrue (hasAccess);
    }

    [Test]
    public void HasAccess_WithAccessDenied ()
    {
      SecurityAdapterRegistry.Instance.SetAdapter (typeof (IWebSecurityAdapter), _testHelper.WebSecurityAdapter);
      SecurityAdapterRegistry.Instance.SetAdapter (typeof (IWxeSecurityAdapter), _testHelper.WxeSecurityAdapter);
      Command command = _testHelper.CreateWxeFunctionCommand ();
      _testHelper.ExpectWxeSecurityProviderHasStatelessAccess (typeof (ExecutionEngine.TestFunction), false);
      _testHelper.ReplayAll ();

      bool hasAccess = command.HasAccess (null);

      _testHelper.VerifyAll ();
      Assert.IsFalse (hasAccess);
    }

    [Test]
    public void Render_WithAccessGranted ()
    {
      Command command = _testHelper.CreateWxeFunctionCommandAsPartialMock ();
      string expectedOnClick = _testHelper.PostBackEvent + _testHelper.OnClick;
      _testHelper.ExpectOnceOnHasAccess (command, true);
      _testHelper.ReplayAll ();

      command.RenderBegin (_testHelper.HtmlWriter, _testHelper.PostBackEvent, new string[0], _testHelper.OnClick, _testHelper.SecurableObject);

      _testHelper.VerifyAll ();
      Assert.IsNotNull (_testHelper.HtmlWriter.Tag, "Missing Tag");
      Assert.AreEqual (HtmlTextWriterTag.A, _testHelper.HtmlWriter.Tag, "Wrong Tag");

      Assert.IsNotNull (_testHelper.HtmlWriter.Attributes[HtmlTextWriterAttribute.Href], "Missing Href");
      Assert.AreEqual ("#", _testHelper.HtmlWriter.Attributes[HtmlTextWriterAttribute.Href], "Wrong Href");

      Assert.IsNotNull (_testHelper.HtmlWriter.Attributes[HtmlTextWriterAttribute.Onclick], "Missing OnClick");
      Assert.AreEqual (expectedOnClick, _testHelper.HtmlWriter.Attributes[HtmlTextWriterAttribute.Onclick], "Wrong OnClick");

      Assert.IsNotNull (_testHelper.HtmlWriter.Attributes[HtmlTextWriterAttribute.Title], "Missing Title");
      Assert.AreEqual (_testHelper.ToolTip, _testHelper.HtmlWriter.Attributes[HtmlTextWriterAttribute.Title], "Wrong Title");

      Assert.IsNull (_testHelper.HtmlWriter.Attributes[HtmlTextWriterAttribute.Target], "Has Target");

    }

    [Test]
    public void Render_WithAccessDenied ()
    {
      Command command = _testHelper.CreateWxeFunctionCommandAsPartialMock ();
      _testHelper.ExpectOnceOnHasAccess (command, false);
      _testHelper.ReplayAll ();

      command.RenderBegin (_testHelper.HtmlWriter, _testHelper.PostBackEvent, new string[0], _testHelper.OnClick, _testHelper.SecurableObject);

      _testHelper.VerifyAll ();
      Assert.IsNotNull (_testHelper.HtmlWriter.Tag, "Missing Tag");
      Assert.AreEqual (HtmlTextWriterTag.A, _testHelper.HtmlWriter.Tag, "Wrong Tag");
      Assert.AreEqual (0, _testHelper.HtmlWriter.Attributes.Count, "Has Attributes");
    }
  }
}
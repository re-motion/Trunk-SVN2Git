using System;
using NUnit.Framework;
using Rhino.Mocks;
using Remotion.Security;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls;

namespace Remotion.Web.UnitTests.UI.Controls.WebButtonTests
{
  [TestFixture]
  public class SecurityTestWithEnabled : BaseTest
  {
    private MockRepository _mocks;
    private IWebSecurityAdapter _mockWebSecurityAdapter;
    private ISecurableObject _mockSecurableObject;

    [SetUp]
    public void Setup ()
    {
      _mocks = new MockRepository ();
      _mockWebSecurityAdapter = _mocks.CreateMock<IWebSecurityAdapter> ();
      _mockSecurableObject = _mocks.CreateMock<ISecurableObject> ();

      AdapterRegistry.Instance.SetAdapter (typeof (IWebSecurityAdapter), _mockWebSecurityAdapter);
    }

    [Test]
    public void EvaluateTrue_FromTrueAndWithMissingPermissionBehaviorSetToInvisible ()
    {
      WebButton button = CreateButtonWithClickEventHandler ();
      button.MissingPermissionBehavior = MissingPermissionBehavior.Invisible;
      button.Enabled = true;
      _mocks.ReplayAll ();

      bool enabled = button.Enabled;

      _mocks.VerifyAll ();
      Assert.IsTrue (enabled);
    }

    [Test]
    public void EvaluateFalse_FromFalseAndWithMissingPermissionBehaviorSetToInvisible ()
    {
      WebButton button = CreateButtonWithClickEventHandler ();
      button.MissingPermissionBehavior = MissingPermissionBehavior.Invisible;
      button.Enabled = false;
      _mocks.ReplayAll ();

      bool enabled = button.Enabled;

      _mocks.VerifyAll ();
      Assert.IsFalse (enabled);
    }

    [Test]
    public void EvaluateTrue_FromTrueAndWithoutWebSeucrityProvider ()
    {
      AdapterRegistry.Instance.SetAdapter (typeof (IWebSecurityAdapter), null);
      WebButton button = CreateButtonWithClickEventHandler ();
      button.Enabled = true;
      _mocks.ReplayAll ();

      bool enabled = button.Enabled;

      _mocks.VerifyAll ();
      Assert.IsTrue (enabled);
    }

    [Test]
    public void EvaluateFalse_FromFalseAndWithoutWebSeucrityProvider ()
    {
      AdapterRegistry.Instance.SetAdapter (typeof (IWebSecurityAdapter), null);
      WebButton button = CreateButtonWithClickEventHandler ();
      button.Enabled = false;
      _mocks.ReplayAll ();

      bool enabled = button.Enabled;

      _mocks.VerifyAll ();
      Assert.IsFalse (enabled);
    }

    [Test]
    public void EvaluateTrue_FromTrueAndWithoutClickEventHandler ()
    {
      WebButton button = CreateButtonWithoutClickEventHandler ();
      button.Enabled = true;
      _mocks.ReplayAll ();

      bool enabled = button.Enabled;

      _mocks.VerifyAll ();
      Assert.IsTrue (enabled);
    }

    [Test]
    public void EvaluateFalse_FromFalseAndWithoutClickEventHandler ()
    {
      WebButton button = CreateButtonWithoutClickEventHandler ();
      button.Enabled = false;
      _mocks.ReplayAll ();

      bool enabled = button.Enabled;

      _mocks.VerifyAll ();
      Assert.IsFalse (enabled);
    }

    [Test]
    public void EvaluateTrue_FromTrueAndAccessGranted ()
    {
      Expect.Call (_mockWebSecurityAdapter.HasAccess (_mockSecurableObject, new EventHandler (TestHandler))).Return (true);
      WebButton button = CreateButtonWithClickEventHandler ();
      button.Enabled = true;
      _mocks.ReplayAll ();

      bool enabled = button.Enabled;

      _mocks.VerifyAll ();
      Assert.IsTrue (enabled);
    }

    [Test]
    public void EvaluateFalse_FromTrueAndAccessDenied ()
    {
      Expect.Call (_mockWebSecurityAdapter.HasAccess (_mockSecurableObject, new EventHandler (TestHandler))).Return (false);
      WebButton button = CreateButtonWithClickEventHandler ();
      button.Enabled = true;
      _mocks.ReplayAll ();

      bool enabled = button.Enabled;

      _mocks.VerifyAll ();
      Assert.IsFalse (enabled);
    }

    [Test]
    public void EvaluateFalse_FromFalse ()
    {
      WebButton button = CreateButtonWithClickEventHandler ();
      button.Enabled = false;
      _mocks.ReplayAll ();

      bool enabled = button.Enabled;

      _mocks.VerifyAll ();
      Assert.IsFalse (enabled);
    }

    private void TestHandler (object sender, EventArgs e)
    {
    }

    private WebButton CreateButtonWithClickEventHandler ()
    {
      WebButton button = new WebButton ();
      button.MissingPermissionBehavior = MissingPermissionBehavior.Disabled;
      button.SecurableObject = _mockSecurableObject;
      button.Click += TestHandler;

      return button;
    }

    private WebButton CreateButtonWithoutClickEventHandler ()
    {
      WebButton button = new WebButton ();
      button.MissingPermissionBehavior = MissingPermissionBehavior.Disabled;
      button.SecurableObject = _mockSecurableObject;

      return button;
    }
  }
}

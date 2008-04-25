using System;
using NUnit.Framework;
using Remotion.Web.UI;
using Remotion.Web.UnitTests.AspNetFramework;

namespace Remotion.Web.UnitTests.UI.Controls
{

  public class WebControlTest
  {
    private WcagHelperMock _wcagHelperMock;
    private PageMock _page;
    private NamingContainerMock _namingContainer;
    private ControlInvoker _namingContainerInvoker;

    public WebControlTest ()
    {
    }

    [SetUp]
    public virtual void SetUp ()
    {
      _wcagHelperMock = new WcagHelperMock();
      WcagHelper.SetInstance (_wcagHelperMock);
      SetUpContext ();
      SetUpPage ();
    }

    protected virtual void SetUpContext ()
    {
    }

    protected virtual void SetUpPage ()
    {
      _page = new PageMock ();

      _namingContainer = new NamingContainerMock();
      _namingContainer.ID = "NamingContainer";
      _page.Controls.Add (_namingContainer);

      _namingContainerInvoker = new ControlInvoker (_namingContainer);
    }

    [TearDown]
    public virtual void TearDown ()
    {
      TearDownPage ();
      TearDownContext ();
      WcagHelper.SetInstance (new WcagHelperMock ());
    }

    protected virtual void TearDownContext ()
    {
      HttpContextHelper.SetCurrent (null);
    }

    protected virtual void TearDownPage ()
    {
    }

    protected WcagHelperMock WcagHelperMock
    {
      get { return _wcagHelperMock; }
    }

    public PageMock Page
    {
      get { return _page; }
    }

    public NamingContainerMock NamingContainer
    {
      get { return _namingContainer; }
    }

    public ControlInvoker NamingContainerInvoker
    {
      get { return _namingContainerInvoker; }
    }
  }

}

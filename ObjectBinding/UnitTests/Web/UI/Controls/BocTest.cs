using System;
using System.Web.UI;
using NUnit.Framework;
using Remotion.Web.UI;
using Remotion.Web.UnitTests.AspNetFramework;
using Remotion.Web.UnitTests.UI;
using Remotion.Web.UnitTests.UI.Controls;

namespace Remotion.ObjectBinding.UnitTests.Web.UI.Controls
{

public class BocTest
{
  private WcagHelperMock _wcagHelperMock;
  private Page _page;
  private NamingContainerMock _namingContainer;
  private ControlInvoker _invoker;

  public BocTest()
  {
  }
  
  [SetUp]
  public virtual void SetUp()
  {
    _wcagHelperMock = new WcagHelperMock();
    WcagHelper.SetInstance (_wcagHelperMock);

    _page = new Page();

    _namingContainer = new NamingContainerMock();
    _namingContainer.ID = "NamingContainer";
    _page.Controls.Add (_namingContainer);

    _invoker = new ControlInvoker (_namingContainer);
  }

  [TearDown]
  public virtual void TearDown()
  {
    WcagHelper.SetInstance (new WcagHelperMock ());
    HttpContextHelper.SetCurrent (null);
  }

  protected WcagHelperMock WcagHelperMock
  {
    get { return _wcagHelperMock; }
  }

  public Page Page
  {
    get { return _page; }
  }

  public NamingContainerMock NamingContainer
  {
    get { return _namingContainer; }
  }

  public ControlInvoker Invoker
  {
    get { return _invoker; }
  }
}

}

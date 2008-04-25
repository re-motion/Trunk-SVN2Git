using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NUnit.Framework;
using Remotion.Utilities;
using Remotion.Web.UnitTests.AspNetFramework;

namespace Remotion.Web.UnitTests.UI.Controls
{

[TestFixture]
public class ControlInvokerTest
{
  // types

  // static members and constants

  // member fields

  private HttpContext _currentHttpContext;
  private PlaceHolder _parent;
  private Literal _child;

  private PlaceHolder _parentAfterPostBack;
  private Literal _childAfterPostBack;
  
  private ControlInvoker _invoker;
  private ControlInvoker _invokerAfterPostBack;

  private string _events;

  // construction and disposing

  public ControlInvokerTest ()
  {
  }

  // methods and properties

  [SetUp]
  public void SetUp()
  {
    _currentHttpContext = HttpContextHelper.CreateHttpContext ("GET", "default.html", null);
    HttpContextHelper.SetCurrent (_currentHttpContext);

    _parent = new PlaceHolder();
    _parent.ID = "Parent";
    _parent.Init += new EventHandler (Control_Init);
    _parent.Load += new EventHandler (Control_Load);
    _parent.PreRender += new EventHandler (Control_PreRender);

    _child = new Literal();
    _child.ID = "Child";
    _child.Init += new EventHandler (Control_Init);
    _child.Load += new EventHandler (Control_Load);
    _child.PreRender += new EventHandler (Control_PreRender);

    _parent.Controls.Add (_child);

    _invoker = new ControlInvoker (_parent);

    _parentAfterPostBack = new PlaceHolder();
    _parentAfterPostBack.ID = "Parent";
    _parentAfterPostBack.Init += new EventHandler (Control_Init);
    _parentAfterPostBack.Load += new EventHandler (Control_Load);
    _parentAfterPostBack.PreRender += new EventHandler (Control_PreRender);

    _childAfterPostBack = new Literal();
    _childAfterPostBack.ID = "Child";
    _childAfterPostBack.Init += new EventHandler (Control_Init);
    _childAfterPostBack.Load += new EventHandler (Control_Load);
    _childAfterPostBack.PreRender += new EventHandler (Control_PreRender);

    _parentAfterPostBack.Controls.Add (_childAfterPostBack);

    _invokerAfterPostBack = new ControlInvoker (_parentAfterPostBack);

    _events = string.Empty;
  }

  [TearDown]
  public void TearDown()
  {
    _parent.Init -= new EventHandler (Control_Init);
    _parent.Load -= new EventHandler (Control_Load);
    _parent.PreRender -= new EventHandler (Control_PreRender);
    
    _child.Init -= new EventHandler (Control_Init);
    _child.Load -= new EventHandler (Control_Load);
    _child.PreRender -= new EventHandler (Control_PreRender);

    HttpContextHelper.SetCurrent (null);
  }

  [Test]
  public void Initialize ()
  {
    Assert.AreSame (_invoker.Control, _parent);
  }

  [Test]
  public void InitRecursive ()
  {
    _invoker.InitRecursive ();

    Assert.AreEqual ("Child Init, Parent Init", _events);
  }

  [Test]
  [Ignore ("LoadViewStateRecursive requires a posted back Page.")]
  public void TestViewState ()
  {
    _invoker.InitRecursive();
    _invoker.LoadRecursive();
    _child.Text = "Foo Bar";
    _invoker.PreRenderRecursive();

    object viewState = _invoker.SaveViewStateRecursive ();

    _invokerAfterPostBack.InitRecursive();
    Assert.AreEqual (string.Empty, _childAfterPostBack.Text);
    _invokerAfterPostBack.LoadViewStateRecursive (viewState);
    Assert.AreEqual ("Foo Bar", _childAfterPostBack.Text);
  }

  [Test]
  public void LoadRecursive ()
  {
    _invoker.LoadRecursive ();

    Assert.AreEqual ("Parent Load, Child Load", _events);
  }

  [Test]
  public void PreRenderRecursive ()
  {
    _invoker.PreRenderRecursive ();

    Assert.AreEqual ("Parent PreRender, Child PreRender", _events);
  }

  private void Control_Init (object sender, EventArgs e)
  {
    _events = AppendEvents ((Control) sender, _events, "Init");
  }

  private void Control_Load (object sender, EventArgs e)
  {
    _events = AppendEvents ((Control) sender, _events, "Load");
  }

  private void Control_PreRender (object sender, EventArgs e)
  {
    _events = AppendEvents ((Control) sender, _events, "PreRender");
  }

  private string AppendEvents (Control control, string events, string eventName)
  {
    events = StringUtility.NullToEmpty (events);
    if (events.Length > 0)
      events += ", ";

    return events + control.ID + " " + eventName;
  }
}

}

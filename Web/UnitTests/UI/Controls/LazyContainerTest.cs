using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web;
using System.Web.UI;
using NUnit.Framework;
using Remotion.Utilities;
using Remotion.Web.UI.Controls;
using Remotion.Web.UnitTests.AspNetFramework;

namespace Remotion.Web.UnitTests.UI.Controls
{

  [TestFixture]
  public class LazyContainerTest : WebControlTest
  {
    // types

    // static members and constants

    // member fields

    private HttpContext _currentHttpContext;

    private StringCollection _actualEvents;

    private LazyContainer _lazyContainer;
    private ControlInvoker _lazyContainerInvoker;

    private ControlMock _parent;
    private ControlInvoker _parentInvoker;
    private ControlMock _child;
    private ControlMock _childSecond;
    private ControlInvoker _childInvoker;
    private ControlInvoker _childSecondInvoker;

    // construction and disposing

    public LazyContainerTest ()
    {
    }

    // methods and properties

    protected override void SetUpContext ()
    {
      base.SetUpContext ();

      _currentHttpContext = HttpContextHelper.CreateHttpContext ("GET", "default.html", null);
      HttpContextHelper.SetCurrent (_currentHttpContext);
    }

    protected override void SetUpPage ()
    {
      base.SetUpPage ();

      _actualEvents = new StringCollection ();

      _lazyContainer = new LazyContainer ();
      _lazyContainer.ID = "LazyContainer";
      _lazyContainer.Init += new EventHandler (LazyContainer_Init);
      _lazyContainer.Load += new EventHandler (LazyContainer_Load);
      NamingContainer.Controls.Add (_lazyContainer);

      _lazyContainerInvoker = new ControlInvoker (_lazyContainer);

      _parent = new ControlMock ();
      _parent.ID = "Parent";
      _parent.Init += new EventHandler (Parent_Init);
      _parent.Load += new EventHandler (Parent_Load);
      _parentInvoker = new ControlInvoker (_parent);

      _child = new ControlMock ();
      _child.ID = "Child";
      _child.Init += new EventHandler (Child_Init);
      _child.Load += new EventHandler (Child_Load);
      _childInvoker = new ControlInvoker (_child);
      _parent.Controls.Add (_child);

      _childSecond = new ControlMock ();
      _childSecond.ID = "ChildSecond";
      _childSecond.Init += new EventHandler (ChildSecond_Init);
      _childSecond.Load += new EventHandler (ChildSecond_Load);
      _childSecondInvoker = new ControlInvoker (_childSecond);
      _parent.Controls.Add (_childSecond);
    }

    [Test]
    public void Initialize ()
    {
      Assert.IsTrue (_lazyContainer.Controls is EmptyControlCollection);

      Assert.IsNotNull (_lazyContainer.RealControls);
      Assert.IsFalse (_lazyContainer.RealControls is EmptyControlCollection);
    }

    [Test]
    public void Ensure ()
    {
      Assert.IsTrue (_lazyContainer.Controls is EmptyControlCollection);

      _lazyContainer.Ensure ();

      Assert.IsNotNull (_lazyContainer.Controls);
      Assert.IsFalse (_lazyContainer.Controls is EmptyControlCollection);
    }


    [Test]
    public void Control_Add_Init_Ensure ()
    {
      StringCollection expectedEvents = new StringCollection ();
      expectedEvents.Add (FormatInitEvent (_lazyContainer));

      _lazyContainer.RealControls.Add (_parent);
      NamingContainerInvoker.InitRecursive ();

      CollectionAssert.AreEqual (expectedEvents, _actualEvents);

      expectedEvents.Add (FormatInitEvent (_child));
      expectedEvents.Add (FormatInitEvent (_childSecond));
      expectedEvents.Add (FormatInitEvent (_parent));

      _lazyContainer.Ensure ();

      CollectionAssert.AreEqual (expectedEvents, _actualEvents);
    }

    [Test]
    public void Control_Init_Ensure_Add ()
    {
      StringCollection expectedEvents = new StringCollection ();
      expectedEvents.Add (FormatInitEvent (_lazyContainer));

      NamingContainerInvoker.InitRecursive ();
      _lazyContainer.Ensure ();

      CollectionAssert.AreEqual (expectedEvents, _actualEvents);

      expectedEvents.Add (FormatInitEvent (_child));
      expectedEvents.Add (FormatInitEvent (_childSecond));
      expectedEvents.Add (FormatInitEvent (_parent));

      _lazyContainer.RealControls.Add (_parent);

      CollectionAssert.AreEqual (expectedEvents, _actualEvents);
    }


    [Test]
    public void Control_Add_Init_Load_Ensure ()
    {
      StringCollection expectedEvents = new StringCollection ();
      expectedEvents.Add (FormatInitEvent (_lazyContainer));
      expectedEvents.Add (FormatLoadEvent (_lazyContainer));

      _lazyContainer.RealControls.Add (_parent);
      NamingContainerInvoker.InitRecursive ();
      NamingContainerInvoker.LoadRecursive ();

      CollectionAssert.AreEqual (expectedEvents, _actualEvents);

      expectedEvents.Add (FormatInitEvent (_child));
      expectedEvents.Add (FormatInitEvent (_childSecond));
      expectedEvents.Add (FormatInitEvent (_parent));
      expectedEvents.Add (FormatLoadEvent (_parent));
      expectedEvents.Add (FormatLoadEvent (_child));
      expectedEvents.Add (FormatLoadEvent (_childSecond));

      _lazyContainer.Ensure ();

      CollectionAssert.AreEqual (expectedEvents, _actualEvents);
    }

    [Test]
    public void Control_Init_Add_Load_Ensure ()
    {
      StringCollection expectedEvents = new StringCollection ();
      expectedEvents.Add (FormatInitEvent (_lazyContainer));
      expectedEvents.Add (FormatLoadEvent (_lazyContainer));

      NamingContainerInvoker.InitRecursive ();
      _lazyContainer.RealControls.Add (_parent);
      NamingContainerInvoker.LoadRecursive ();

      CollectionAssert.AreEqual (expectedEvents, _actualEvents);

      expectedEvents.Add (FormatInitEvent (_child));
      expectedEvents.Add (FormatInitEvent (_childSecond));
      expectedEvents.Add (FormatInitEvent (_parent));
      expectedEvents.Add (FormatLoadEvent (_parent));
      expectedEvents.Add (FormatLoadEvent (_child));
      expectedEvents.Add (FormatLoadEvent (_childSecond));

      _lazyContainer.Ensure ();

      CollectionAssert.AreEqual (expectedEvents, _actualEvents);
    }

    [Test]
    public void Control_Init_Load_Add_Ensure ()
    {
      StringCollection expectedEvents = new StringCollection ();
      expectedEvents.Add (FormatInitEvent (_lazyContainer));
      expectedEvents.Add (FormatLoadEvent (_lazyContainer));

      NamingContainerInvoker.InitRecursive ();
      NamingContainerInvoker.LoadRecursive ();
      _lazyContainer.RealControls.Add (_parent);

      CollectionAssert.AreEqual (expectedEvents, _actualEvents);

      expectedEvents.Add (FormatInitEvent (_child));
      expectedEvents.Add (FormatInitEvent (_childSecond));
      expectedEvents.Add (FormatInitEvent (_parent));
      expectedEvents.Add (FormatLoadEvent (_parent));
      expectedEvents.Add (FormatLoadEvent (_child));
      expectedEvents.Add (FormatLoadEvent (_childSecond));

      _lazyContainer.Ensure ();

      CollectionAssert.AreEqual (expectedEvents, _actualEvents);
    }

    [Test]
    public void Control_Init_Load_Ensure_Add ()
    {
      StringCollection expectedEvents = new StringCollection ();
      expectedEvents.Add (FormatInitEvent (_lazyContainer));
      expectedEvents.Add (FormatLoadEvent (_lazyContainer));

      NamingContainerInvoker.InitRecursive ();
      NamingContainerInvoker.LoadRecursive ();
      _lazyContainer.RealControls.Add (_parent);

      CollectionAssert.AreEqual (expectedEvents, _actualEvents);

      expectedEvents.Add (FormatInitEvent (_child));
      expectedEvents.Add (FormatInitEvent (_childSecond));
      expectedEvents.Add (FormatInitEvent (_parent));
      expectedEvents.Add (FormatLoadEvent (_parent));
      expectedEvents.Add (FormatLoadEvent (_child));
      expectedEvents.Add (FormatLoadEvent (_childSecond));

      _lazyContainer.Ensure ();

      CollectionAssert.AreEqual (expectedEvents, _actualEvents);
    }

    [Test]
    public void Control_Init_Load_Add_SaveViewState ()
    {
      NamingContainerInvoker.InitRecursive ();
      NamingContainerInvoker.LoadRecursive ();
      _lazyContainer.RealControls.Add (_parent);

      _parent.ValueInViewState = "Parent Value";
      _child.ValueInViewState = "Child Value";

      object viewState = _lazyContainerInvoker.SaveViewState ();

      Assert.IsNotNull (viewState);
      Assert.IsTrue (viewState is Pair);
      Pair values = (Pair) viewState;
      Assert.IsNull (values.Second);
    }

    [Test]
    public void Control_Init_LoadViewState ()
    {
      NamingContainerInvoker.InitRecursive ();

      _lazyContainerInvoker.LoadViewState (new Pair (null, null));
    }

    [Test]
    public void Control_Init_LoadViewStateWithNull ()
    {
      NamingContainerInvoker.InitRecursive ();

      _lazyContainerInvoker.LoadViewState (null);
    }

    [Test]
    public void Control_Init_Load_Add_Ensure_SaveViewStateRecursive ()
    {
      NamingContainerInvoker.InitRecursive ();
      NamingContainerInvoker.LoadRecursive ();
      _lazyContainer.RealControls.Add (_parent);
      _lazyContainer.Ensure ();

      _parent.ValueInViewState = "Parent Value";
      _child.ValueInViewState = "Child Value";

      object viewState = _lazyContainerInvoker.SaveViewState ();

      Assert.IsNotNull (viewState);
      Assert.IsTrue (viewState is Pair);
      Pair values = (Pair) viewState;
      Assert.IsNotNull (values.Second);
    }


    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "Cannot ensure LazyContainer 'LazyContainer' before its state has been loaded.")]
    public void Control_PostBack_Init_Add_Ensure ()
    {
      Page.SetRequestValueCollection (new NameValueCollection ());
      NamingContainerInvoker.InitRecursive ();
      _lazyContainer.RealControls.Add (_parent);
      _lazyContainer.Ensure ();
    }

    [Test]
    public void Control_Init_Add_LoadAllState_Ensure ()
    {
      Page_Init_Load_Add_Ensure_SaveAllState ("Parent Value", "Child Value", null);
      PageStatePersister pageStatePersisterBackup = Page.GetPageStatePersister ();

      TearDownPage ();
      SetUpPage ();

      Page.SetRequestValueCollection (new NameValueCollection ());
      NamingContainerInvoker.InitRecursive ();
      Page.SetPageStatePersister (pageStatePersisterBackup);
      Page.LoadAllState ();
      _lazyContainer.RealControls.Add (_parent);

      Assert.IsNull (_parent.ValueInControlState);
      Assert.IsNull (_child.ValueInControlState);
      Assert.IsNull (_childSecond.ValueInControlState);

      _lazyContainer.Ensure ();

      Assert.AreEqual ("Parent Value", _parent.ValueInControlState);
      Assert.AreEqual ("Child Value", _child.ValueInControlState);
      Assert.IsNull (_childSecond.ValueInControlState);
    }

    [Test]
    public void Control_BackUpChildControlState ()
    {
      Page_Init_Load_Add_Ensure_SaveAllState ("Parent Value", "Child Value", null);
      PageStatePersister pageStatePersisterBackup = Page.GetPageStatePersister ();

      Dictionary<string, object> expectedControlStates = new Dictionary<string, object> ();
      expectedControlStates[_parent.UniqueID] = _parentInvoker.SaveControlStateInternal ();
      expectedControlStates[_child.UniqueID] = _childInvoker.SaveControlStateInternal ();

      TearDownPage ();
      SetUpPage ();

      Page.SetRequestValueCollection (new NameValueCollection ());
      NamingContainerInvoker.InitRecursive ();
      Page.SetPageStatePersister (pageStatePersisterBackup);
      Page.LoadAllState ();
      NamingContainerInvoker.LoadRecursive ();
      _lazyContainer.RealControls.Add (_parent);

      object controlState = _lazyContainerInvoker.SaveControlState ();

      Assert.IsTrue (controlState is Triplet);
      Triplet values = (Triplet) controlState;
      Assert.IsTrue (values.Third is Dictionary<string, object>);
      Dictionary<string, object> actualControlStates = (Dictionary<string, object>) values.Third;
      Assert.AreEqual (2, actualControlStates.Count);

      foreach (string expectedKey in expectedControlStates.Keys)
      {
        Pair expectedValues = (Pair) expectedControlStates[expectedKey];

        object actualControlState = actualControlStates[expectedKey];
        Assert.IsTrue (actualControlState is Pair);
        Pair actualValues = (Pair) actualControlState;
        Assert.AreEqual (expectedValues.First, actualValues.First, expectedKey);
        Assert.AreEqual (expectedValues.Second, actualValues.Second, expectedKey);
      }
    }

    [Test]
    public void Control_RestoreChildControlState_EnsureAfterLoadAllState ()
    {
      Page_Init_Load_Add_Ensure_SaveAllState ("Parent Value", "Child Value", null);
      PageStatePersister pageStatePersisterBackup = Page.GetPageStatePersister ();

      TearDownPage ();
      SetUpPage ();

      Page.SetRequestValueCollection (new NameValueCollection ());
      Page.RegisterViewStateHandler ();
      NamingContainerInvoker.InitRecursive ();
      Page.SetPageStatePersister (pageStatePersisterBackup);
      Page.LoadAllState ();
      NamingContainerInvoker.LoadRecursive ();
      _lazyContainer.RealControls.Add (_parent);
      Page.SaveAllState ();

      pageStatePersisterBackup = Page.GetPageStatePersister ();

      Assert.IsTrue (pageStatePersisterBackup.ControlState is IDictionary);
      IDictionary controlStates = (IDictionary) pageStatePersisterBackup.ControlState;
      Assert.AreEqual (1, controlStates.Count);
      Assert.IsTrue (controlStates.Contains (_lazyContainer.UniqueID));

      TearDownPage ();
      SetUpPage ();

      Page.SetRequestValueCollection (new NameValueCollection ());
      Page.RegisterViewStateHandler ();
      NamingContainerInvoker.InitRecursive ();
      Page.SetPageStatePersister (pageStatePersisterBackup);
      Page.LoadAllState ();
      NamingContainerInvoker.LoadRecursive ();
      _lazyContainer.RealControls.Add (_parent);

      Assert.IsNull (_parent.ValueInControlState);
      Assert.IsNull (_child.ValueInControlState);
      Assert.IsNull (_childSecond.ValueInControlState);

      _lazyContainer.Ensure ();

      Assert.AreEqual ("Parent Value", _parent.ValueInControlState);
      Assert.AreEqual ("Child Value", _child.ValueInControlState);
      Assert.IsNull (_childSecond.ValueInControlState);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "Cannot ensure LazyContainer 'LazyContainer' before its state has been loaded.")]
    public void Control_RestoreChildControlState_EnsureBeforeLoadAllState ()
    {
      Page_Init_Load_Add_Ensure_SaveAllState ("Parent Value", "Child Value", null);
      PageStatePersister pageStatePersisterBackup = Page.GetPageStatePersister ();

      TearDownPage ();
      SetUpPage ();

      Page.RegisterViewStateHandler ();
      Page.SetRequestValueCollection (new NameValueCollection ());
      NamingContainerInvoker.InitRecursive ();
      Page.SetPageStatePersister (pageStatePersisterBackup);
      Page.LoadAllState ();
      NamingContainerInvoker.LoadRecursive ();
      _lazyContainer.RealControls.Add (_parent);
      Page.SaveAllState ();

      pageStatePersisterBackup = Page.GetPageStatePersister ();

      Assert.IsTrue (pageStatePersisterBackup.ControlState is IDictionary);
      IDictionary controlStates = (IDictionary) pageStatePersisterBackup.ControlState;
      Assert.AreEqual (1, controlStates.Count);
      Assert.IsTrue (controlStates.Contains (_lazyContainer.UniqueID));

      TearDownPage ();
      SetUpPage ();

      Page.SetRequestValueCollection (new NameValueCollection ());
      _lazyContainer.Ensure ();
    }

    private void Page_Init_Load_Add_Ensure_SaveAllState (string parentControlState, string childControlState, string childSecondControlState)
    {
      Page.RegisterViewStateHandler ();
      NamingContainerInvoker.InitRecursive ();
      NamingContainerInvoker.LoadRecursive ();
      _lazyContainer.RealControls.Add (_parent);
      _lazyContainer.Ensure ();

      _parent.ValueInControlState = parentControlState;
      _child.ValueInControlState = childControlState;
      _childSecond.ValueInControlState = childSecondControlState;

      Page.SaveAllState ();
    }

    private void LazyContainer_Init (object sender, EventArgs e)
    {
      _actualEvents.Add (FormatInitEvent (_lazyContainer));
    }

    private void LazyContainer_Load (object sender, EventArgs e)
    {
      _actualEvents.Add (FormatLoadEvent (_lazyContainer));
    }

    private void Parent_Init (object sender, EventArgs e)
    {
      _actualEvents.Add (FormatInitEvent (_parent));
    }

    private void Parent_Load (object sender, EventArgs e)
    {
      _actualEvents.Add (FormatLoadEvent (_parent));
    }

    private void Child_Load (object sender, EventArgs e)
    {
      _actualEvents.Add (FormatLoadEvent (_child));
    }

    private void Child_Init (object sender, EventArgs e)
    {
      _actualEvents.Add (FormatInitEvent (_child));
    }

    private void ChildSecond_Load (object sender, EventArgs e)
    {
      _actualEvents.Add (FormatLoadEvent (_childSecond));
    }

    private void ChildSecond_Init (object sender, EventArgs e)
    {
      _actualEvents.Add (FormatInitEvent (_childSecond));
    }


    private string FormatInitEvent (Control sender)
    {
      return FormatEvent (sender, "Init");
    }

    private string FormatLoadEvent (Control sender)
    {
      return FormatEvent (sender, "Load");
    }

    private string FormatEvent (Control sender, string eventName)
    {
      ArgumentUtility.CheckNotNull ("sender", sender);
      ArgumentUtility.CheckNotNullOrEmpty ("eventName", eventName);

      return sender.ID + " " + eventName;
    }
  }

}

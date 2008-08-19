using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Reflection;
using System.Web;
using System.Web.UI;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Development.Web.UnitTesting.AspNetFramework;
using Remotion.Development.Web.UnitTesting.UI.Controls;
using Remotion.Reflection;
using Remotion.Web.Utilities;

namespace Remotion.Web.UnitTests.Utilities
{
  [TestFixture]
  public class ControlHelperTest
  {
    private HttpContext _httpContext;
    private PageMock _page;
    private NamingContainerMock _namingContainer;
    private ControlInvoker _pageInvoker;
    private ControlMock _parent;
    private ControlMock _child;
    private Control _child2;
    private ControlMock _otherControl;

    [SetUp]
    public void SetUp ()
    {
      _httpContext = HttpContextHelper.CreateHttpContext ("GET", "default.html", null);
      _httpContext.Response.ContentEncoding = System.Text.Encoding.UTF8;
      HttpContextHelper.SetCurrent (_httpContext);

      _page = new PageMock();
      _page.SetRequestValueCollection (new NameValueCollection());
      _httpContext.Handler = _page;

      _namingContainer = new NamingContainerMock();
      _namingContainer.ID = "NamingContainer";
      _page.Controls.Add (_namingContainer);

      _parent = new ControlMock();
      _parent.ID = "Parent";
      _namingContainer.ValueInViewState = "NamingContainerValue";
      _namingContainer.ValueInControlState = "NamingContainerValue";
      _namingContainer.Controls.Add (_parent);

      _child = new ControlMock();
      _child.ID = "Child";
      _parent.Controls.Add (_child);

      _child2 = new Control();
      _child2.ID = "Child2";
      _parent.Controls.Add (_child2);

      NamingContainerMock otherNamingContainer = new NamingContainerMock();
      otherNamingContainer.ID = "OtherNamingContainer";
      _page.Controls.Add (otherNamingContainer);

      _otherControl = new ControlMock();
      _otherControl.ID = "OtherControl";
      _otherControl.ValueInViewState = "OtherValue";
      _otherControl.ValueInControlState = "OtherValue";
      otherNamingContainer.Controls.Add (_otherControl);

      _pageInvoker = new ControlInvoker (_page);
    }

    [TearDown]
    public void TearDown ()
    {
      HttpContextHelper.SetCurrent (null);
    }

    [Test]
    public void LoadViewStateRecursive ()
    {
      object viewState = new Pair ("ParentValue", new ArrayList { 0, new Pair ("ChildValue", null) });

      ControlHelper.LoadViewStateRecursive (_parent, viewState);

      Assert.That (_parent.ValueInViewState, Is.EqualTo ("ParentValue"));
      Assert.That (_child.ValueInViewState, Is.EqualTo ("ChildValue"));
    }

    [Test]
    public void SaveViewStateRecursive ()
    {
      _parent.ValueInViewState = "ParentValue";
      _child.ValueInViewState = "ChildValue";

      object viewState = ControlHelper.SaveViewStateRecursive (_parent);

      Assert.That (viewState, Is.InstanceOfType (typeof (Pair)));
      var parentViewState = (Pair) viewState;
      Assert.That (parentViewState.First, Is.EqualTo ("ParentValue"));
      Assert.That (parentViewState.Second, Is.InstanceOfType (typeof (ArrayList)));
      var childViewStates = (IList) parentViewState.Second;
      Assert.That (childViewStates.Count, Is.EqualTo (2));
      Assert.That (childViewStates[0], Is.EqualTo (0));
      Assert.That (childViewStates[1], new PairConstraint (new Pair ("ChildValue", null)));
    }

    [Test]
    public void GetPageStatePersister ()
    {
      var pageStatePersister = new SessionPageStatePersister (_page);
      _page.SetPageStatePersister (pageStatePersister);
      Assert.That (ControlHelper.GetPageStatePersister (_page), Is.SameAs (pageStatePersister));
    }

    [Test]
    public void SaveControlStateInternal ()
    {
      _parent.ValueInControlState = "ParentValue";
      Assert.That (ControlHelper.SaveControlStateInternal (_parent), new PairConstraint (new Pair ("ParentValue", null)));
    }

    [Test]
    public void SaveChildControlState ()
    {
      _parent.ValueInControlState = "ParentValue";
      _child.ValueInControlState = "ChildValue";

      _pageInvoker.InitRecursive();

      var pageStatePersister = new SessionPageStatePersister (_page);
      _page.SetPageStatePersister (pageStatePersister);
      pageStatePersister.ControlState = null;

      Dictionary<string, object> childControlState = ControlHelper.SaveChildControlState (_namingContainer);

      Assert.That (childControlState, Is.Not.Null);
      Assert.That (childControlState.Count, Is.EqualTo (2));
      Assert.That (childControlState[_parent.UniqueID], new PairConstraint (new Pair ("ParentValue", null)));
      Assert.That (childControlState[_child.UniqueID], new PairConstraint (new Pair ("ChildValue", null)));
    }

    [Test]
    public void SaveChildControlState_NoControlsRegistered ()
    {
      var pageStatePersister = new SessionPageStatePersister (_page);
      _page.SetPageStatePersister (pageStatePersister);
      pageStatePersister.ControlState = null;

      Assert.That (ControlHelper.SaveChildControlState (_namingContainer), Is.Null);
    }

    [Test]
    public void SaveChildControlState_NoControlStateFromRegisteredControl ()
    {
      _parent.ValueInControlState = null;
      _child.ValueInControlState = "ChildValue";
      _pageInvoker.InitRecursive();

      var pageStatePersister = new SessionPageStatePersister (_page);
      _page.SetPageStatePersister (pageStatePersister);
      pageStatePersister.ControlState = null;

      Dictionary<string, object> childControlState = ControlHelper.SaveChildControlState (_namingContainer);

      Assert.That (childControlState, Is.Not.Null);
      Assert.That (childControlState.Count, Is.EqualTo (1));
      Assert.That (childControlState[_child.UniqueID], new PairConstraint (new Pair ("ChildValue", null)));
    }

    [Test]
    public void SaveChildControlState_ControlRegisteredTwice ()
    {
      _parent.ValueInControlState = null;
      _child.ValueInControlState = "ChildValue";

      _pageInvoker.InitRecursive();
      _page.RegisterRequiresControlState (_child);

      var pageStatePersister = new SessionPageStatePersister (_page);
      _page.SetPageStatePersister (pageStatePersister);
      pageStatePersister.ControlState = null;

      Dictionary<string, object> childControlState = ControlHelper.SaveChildControlState (_namingContainer);

      Assert.That (childControlState, Is.Not.Null);
      Assert.That (childControlState.Count, Is.EqualTo (1));
      Assert.That (childControlState[_child.UniqueID], new PairConstraint (new Pair ("ChildValue", null)));
    }

    [Test]
    public void GetChildControlState ()
    {
      var pageStatePersister = new SessionPageStatePersister (_page);
      _page.SetPageStatePersister (pageStatePersister);
      var controlState = new Dictionary<string, object>();
      pageStatePersister.ControlState = controlState;

      controlState[_parent.UniqueID] = "ParentValue";
      controlState[_child.UniqueID] = "ChildValue";
      controlState[_otherControl.UniqueID] = "OtherValue";

      Dictionary<string, object> childControlState = ControlHelper.GetChildControlState (_namingContainer);

      Assert.That (childControlState, Is.Not.Null);
      Assert.That (childControlState.Count, Is.EqualTo (2));
      Assert.That (childControlState[_parent.UniqueID], Is.EqualTo ("ParentValue"));
      Assert.That (childControlState[_child.UniqueID], Is.EqualTo ("ChildValue"));
    }

    [Test]
    public void SetChildControlState ()
    {
      _pageInvoker.InitRecursive();
      
      var pageStatePersister = new SessionPageStatePersister (_page);
      _page.SetPageStatePersister (pageStatePersister);
      var controlState = new Dictionary<string, object>();
      pageStatePersister.ControlState = controlState;

      IDictionary childControlState = new Dictionary<string, object>();
      childControlState[_parent.UniqueID] = new Pair ("ParentValue", null);
      childControlState[_child.UniqueID] = new Pair ("ChildValue", null);

      ControlHelper.SetChildControlState (_namingContainer, childControlState);

      Assert.That (controlState.Count, Is.EqualTo (2));
      Assert.That (controlState[_parent.UniqueID], new PairConstraint (new Pair ("ParentValue", null)));
      Assert.That (controlState[_child.UniqueID], new PairConstraint (new Pair ("ChildValue", null)));

      _page.LoadAllState();

      Assert.That (_parent.ValueInControlState, Is.EqualTo ("ParentValue"));
      Assert.That (_child.ValueInControlState, Is.EqualTo ("ChildValue"));
    }
  }
}
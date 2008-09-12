/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using System.Collections;
using System.Collections.Specialized;
using System.Web.UI;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Development.Web.UnitTesting.UI.Controls;
using Remotion.Web.UI.Controls.ControlReplacing;
using Remotion.Web.Utilities;
using Rhino.Mocks;

namespace Remotion.Web.UnitTests.UI.Controls.ControlReplacing
{
  [TestFixture]
  public class ControlReplacerTest : TestBase
  {
    [Test]
    public void SaveAllState_ViewState ()
    {
      var testPageHolder = new TestPageHolder (true);
      ControlReplacer replacer = SetupControlReplacerForIntegrationTest (testPageHolder.NamingContainer, new LoadingStateSelectionStrategy ());
      testPageHolder.PageInvoker.InitRecursive();

      var formatter = new LosFormatter();
      var state = (Pair) formatter.Deserialize (replacer.SaveAllState());

      Pair replacerViewState = (Pair) state.Second;
      Assert.That (replacerViewState.First, Is.EqualTo ("value"));
      var namingContainerViewState = (Pair) ((IList) (replacerViewState).Second)[1];
      Assert.That (namingContainerViewState.First, Is.EqualTo ("NamingContainerValue"));
      var parentViewState = (Pair) ((IList) (namingContainerViewState).Second)[1];
      Assert.That (parentViewState.First, Is.EqualTo ("ParentValue"));
    }

    [Test]
    public void SaveViewStateRecursive ()
    {
      var testPageHolder = new TestPageHolder (true);
      SetupControlReplacerForIntegrationTest (testPageHolder.NamingContainer, new LoadingStateSelectionStrategy ());

      testPageHolder.PageInvoker.InitRecursive();
      object viewState = testPageHolder.PageInvoker.SaveViewStateRecursive();

      Assert.That (viewState, Is.InstanceOfType (typeof (Pair)));
      var replacerViewState = (Pair) ((IList) ((Pair) viewState).Second)[3];
      Assert.That (replacerViewState.First, Is.EqualTo ("value"));
      var namingContainerViewState = (Pair) ((IList) (replacerViewState).Second)[1];
      Assert.That (namingContainerViewState.First, Is.EqualTo ("NamingContainerValue"));
      var parentViewState = (Pair) ((IList) (namingContainerViewState).Second)[1];
      Assert.That (parentViewState.First, Is.EqualTo ("ParentValue"));
    }

    [Test]
    public void LoadViewStateRecursive_RegularPostBack ()
    {
      object viewState = CreateViewState();
      var testPageHolderWithoutState = new TestPageHolder (false);
      SetupControlReplacerForIntegrationTest (testPageHolderWithoutState.NamingContainer, new LoadingStateSelectionStrategy ());

      testPageHolderWithoutState.PageInvoker.InitRecursive();
      testPageHolderWithoutState.PageInvoker.LoadViewStateRecursive (viewState);

      Assert.That (testPageHolderWithoutState.OtherControl.ValueInViewState, Is.EqualTo ("OtherValue"));
      Assert.That (testPageHolderWithoutState.NamingContainer.ValueInViewState, Is.EqualTo ("NamingContainerValue"));
      Assert.That (testPageHolderWithoutState.Parent.ValueInViewState, Is.EqualTo ("ParentValue"));
    }

    [Test]
    public void LoadViewStateRecursive_ReplaceViewState ()
    {
      object originalViewState = CreateViewState();

      var testPageHolderWithChangedState = new TestPageHolder (false);
      var replacerWithChangedState = SetupControlReplacerForIntegrationTest (testPageHolderWithChangedState.NamingContainer, new LoadingStateSelectionStrategy ());
      testPageHolderWithChangedState.PageInvoker.InitRecursive();
      testPageHolderWithChangedState.Parent.ValueInViewState = "NewParentValue";
      testPageHolderWithChangedState.NamingContainer.ValueInViewState = "NewNamingContainerValue";
      string backedUpState = replacerWithChangedState.SaveAllState();

      var testPageHolderWithoutState = new TestPageHolder (false);
      SetupControlReplacerForIntegrationTest (testPageHolderWithoutState.NamingContainer, new ReplacingStateSelectionStrategy (backedUpState));

      testPageHolderWithoutState.Page.SetRequestValueCollection (new NameValueCollection());
      testPageHolderWithoutState.PageInvoker.InitRecursive();
      testPageHolderWithoutState.PageInvoker.LoadViewStateRecursive (originalViewState);

      Assert.That (testPageHolderWithoutState.OtherControl.ValueInViewState, Is.EqualTo ("OtherValue"));
      Assert.That (testPageHolderWithoutState.NamingContainer.ValueInViewState, Is.EqualTo ("NewNamingContainerValue"));
      Assert.That (testPageHolderWithoutState.Parent.ValueInViewState, Is.EqualTo ("NewParentValue"));
    }

    [Test]
    public void LoadViewStateRecursive_ClearViewState ()
    {
      object originalViewState = CreateViewState();

      var testPageHolderWithoutState = new TestPageHolder (false);
      SetupControlReplacerForIntegrationTest (testPageHolderWithoutState.NamingContainer, new ClearingStateSelectionStrategy ());

      testPageHolderWithoutState.Page.SetRequestValueCollection (new NameValueCollection());
      testPageHolderWithoutState.PageInvoker.InitRecursive();
      testPageHolderWithoutState.PageInvoker.LoadViewStateRecursive (originalViewState);

      Assert.That (testPageHolderWithoutState.OtherControl.ValueInViewState, Is.EqualTo ("OtherValue"));
      Assert.That (testPageHolderWithoutState.NamingContainer.ValueInViewState, Is.Null);
      Assert.That (testPageHolderWithoutState.Parent.ValueInViewState, Is.Null);
    }


    [Test]
    [Ignore]
    public void LoadViewStateRecursive_RegularPostBack_InitializedAfterLoadViewState ()
    {
      object viewState = CreateViewState ();
      var testPageHolderWithoutState = new TestPageHolder (false);
      SetupControlReplacerForIntegrationTest (testPageHolderWithoutState.NamingContainer, new LoadingStateSelectionStrategy ());
      testPageHolderWithoutState.Page.Controls.Remove (testPageHolderWithoutState.NamingContainer);

      testPageHolderWithoutState.PageInvoker.InitRecursive ();
      testPageHolderWithoutState.PageInvoker.LoadViewStateRecursive (viewState);

      Assert.That (testPageHolderWithoutState.OtherControl.ValueInViewState, Is.EqualTo ("OtherValue"));
      Assert.That (testPageHolderWithoutState.NamingContainer.ValueInViewState, Is.Null);
      Assert.That (testPageHolderWithoutState.Parent.ValueInViewState, Is.Null);

      testPageHolderWithoutState.Page.Controls.Add (testPageHolderWithoutState.NamingContainer);

      Assert.That (testPageHolderWithoutState.NamingContainer.ValueInViewState, Is.EqualTo ("NamingContainerValue"));
      Assert.That (testPageHolderWithoutState.Parent.ValueInViewState, Is.EqualTo ("ParentValue"));
    }

    [Test]
    [Ignore]
    public void LoadViewStateRecursive_ReplaceViewState_InitializedAfterLoadViewState ()
    {
      object originalViewState = CreateViewState ();

      var testPageHolderWithChangedState = new TestPageHolder (false);
      var replacerWithChangedState = SetupControlReplacerForIntegrationTest (testPageHolderWithChangedState.NamingContainer, new LoadingStateSelectionStrategy ());
      testPageHolderWithChangedState.PageInvoker.InitRecursive ();
      testPageHolderWithChangedState.Parent.ValueInViewState = "NewParentValue";
      testPageHolderWithChangedState.NamingContainer.ValueInViewState = "NewNamingContainerValue";
      string backedUpState = replacerWithChangedState.SaveAllState ();

      var testPageHolderWithoutState = new TestPageHolder (false);
      SetupControlReplacerForIntegrationTest (testPageHolderWithoutState.NamingContainer, new ReplacingStateSelectionStrategy (backedUpState));
      testPageHolderWithoutState.Page.Controls.Remove (testPageHolderWithoutState.NamingContainer);

      testPageHolderWithoutState.Page.SetRequestValueCollection (new NameValueCollection ());
      testPageHolderWithoutState.PageInvoker.InitRecursive ();
      testPageHolderWithoutState.PageInvoker.LoadViewStateRecursive (originalViewState);

      Assert.That (testPageHolderWithoutState.OtherControl.ValueInViewState, Is.EqualTo ("OtherValue"));
      Assert.That (testPageHolderWithoutState.NamingContainer.ValueInViewState, Is.Null);
      Assert.That (testPageHolderWithoutState.Parent.ValueInViewState, Is.Null);

      testPageHolderWithoutState.Page.Controls.Add (testPageHolderWithoutState.NamingContainer);

      Assert.That (testPageHolderWithoutState.NamingContainer.ValueInViewState, Is.EqualTo ("NewNamingContainerValue"));
      Assert.That (testPageHolderWithoutState.Parent.ValueInViewState, Is.EqualTo ("NewParentValue"));
    }

    [Test]
    [Ignore]
    public void LoadViewStateRecursive_ClearViewState_InitializedAfterLoadViewState ()
    {
      object originalViewState = CreateViewState ();

      var testPageHolderWithoutState = new TestPageHolder (false);
      SetupControlReplacerForIntegrationTest (testPageHolderWithoutState.NamingContainer, new ClearingStateSelectionStrategy ());
      testPageHolderWithoutState.Page.Controls.Remove (testPageHolderWithoutState.NamingContainer);

      testPageHolderWithoutState.Page.SetRequestValueCollection (new NameValueCollection ());
      testPageHolderWithoutState.PageInvoker.InitRecursive ();
      testPageHolderWithoutState.PageInvoker.LoadViewStateRecursive (originalViewState);

      Assert.That (testPageHolderWithoutState.OtherControl.ValueInViewState, Is.EqualTo ("OtherValue"));
      Assert.That (testPageHolderWithoutState.NamingContainer.ValueInViewState, Is.Null);
      Assert.That (testPageHolderWithoutState.Parent.ValueInViewState, Is.Null);

      testPageHolderWithoutState.Page.Controls.Add (testPageHolderWithoutState.NamingContainer);

      Assert.That (testPageHolderWithoutState.NamingContainer.ValueInViewState, Is.Null);
      Assert.That (testPageHolderWithoutState.Parent.ValueInViewState, Is.Null);
    }


    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "Controls can only load state after OnInit phase.")]
    public void LoadViewStateRecursive_ThrowsIfNotAfterOnInit ()
    {
      var testPageHolder = new TestPageHolder (false);
      var replacer = SetupControlReplacerForIntegrationTest (testPageHolder.NamingContainer, new LoadingStateSelectionStrategy ());
      var controlInvoker = new ControlInvoker (replacer);
      controlInvoker.LoadViewState (null);
    }


    [Test]
    public void SaveAllState_ControlState ()
    {
      var testPageHolder = new TestPageHolder (true);
      ControlReplacer replacer = SetupControlReplacerForIntegrationTest (testPageHolder.NamingContainer, new LoadingStateSelectionStrategy ());
      testPageHolder.PageInvoker.InitRecursive();

      var formatter = new LosFormatter();
      var state = (Pair) formatter.Deserialize (replacer.SaveAllState());

      IDictionary controlState = (IDictionary) state.First;
      Assert.That (controlState[replacer.UniqueID], Is.Null);
      Assert.That (controlState[testPageHolder.NamingContainer.UniqueID], new PairConstraint (new Pair ("NamingContainerValue", null)));
      Assert.That (controlState[testPageHolder.Parent.UniqueID], new PairConstraint (new Pair ("ParentValue", null)));
    }

    [Test]
    public void SaveControlStateRecursive ()
    {
      var testPageHolder = new TestPageHolder (true);
      ControlReplacer replacer = SetupControlReplacerForIntegrationTest (testPageHolder.NamingContainer, new LoadingStateSelectionStrategy ());

      testPageHolder.PageInvoker.InitRecursive();
      testPageHolder.Page.SaveAllState();

      var controlStateObject = testPageHolder.Page.GetPageStatePersister().ControlState;
      Assert.That (controlStateObject, Is.InstanceOfType (typeof (IDictionary)));
      IDictionary controlState = (IDictionary) controlStateObject;
      Assert.That (controlState[replacer.UniqueID], new PairConstraint (new Pair ("value", null)));
      Assert.That (controlState[testPageHolder.NamingContainer.UniqueID], new PairConstraint (new Pair ("NamingContainerValue", null)));
      Assert.That (controlState[testPageHolder.Parent.UniqueID], new PairConstraint (new Pair ("ParentValue", null)));
    }

    [Test]
    public void LoadControlStateRecursive_RegularPostBack ()
    {
      object controlState = CreateControlState();
      var testPageHolderWithoutState = new TestPageHolder (false);
      SetupControlReplacerForIntegrationTest (testPageHolderWithoutState.NamingContainer, new LoadingStateSelectionStrategy ());

      testPageHolderWithoutState.PageInvoker.InitRecursive();
      testPageHolderWithoutState.Page.SetPageStatePersister (
          new HiddenFieldPageStatePersister (testPageHolderWithoutState.Page) { ControlState = controlState });
      testPageHolderWithoutState.Page.LoadAllState();

      Assert.That (testPageHolderWithoutState.OtherControl.ValueInControlState, Is.EqualTo ("OtherValue"));
      Assert.That (testPageHolderWithoutState.NamingContainer.ValueInControlState, Is.EqualTo ("NamingContainerValue"));
      Assert.That (testPageHolderWithoutState.Parent.ValueInControlState, Is.EqualTo ("ParentValue"));
    }

    [Test]
    public void LoadControlStateRecursive_ReplaceControlState ()
    {
      object originalControlState = CreateControlState ();

      var testPageHolderWithChangedState = new TestPageHolder (false);
      var replacerWithChangedState = SetupControlReplacerForIntegrationTest (testPageHolderWithChangedState.NamingContainer, new LoadingStateSelectionStrategy ());
      testPageHolderWithChangedState.PageInvoker.InitRecursive ();
      testPageHolderWithChangedState.Parent.ValueInControlState = "NewParentValue";
      testPageHolderWithChangedState.NamingContainer.ValueInControlState = "NewNamingContainerValue";
      string backedUpState = replacerWithChangedState.SaveAllState ();

      var testPageHolderWithoutState = new TestPageHolder (false);
      SetupControlReplacerForIntegrationTest (testPageHolderWithoutState.NamingContainer, new ReplacingStateSelectionStrategy (backedUpState));

      testPageHolderWithoutState.PageInvoker.InitRecursive ();
      testPageHolderWithoutState.Page.SetPageStatePersister (
          new HiddenFieldPageStatePersister (testPageHolderWithoutState.Page) { ControlState = originalControlState });
      testPageHolderWithoutState.Page.LoadAllState ();

      Assert.That (testPageHolderWithoutState.OtherControl.ValueInControlState, Is.EqualTo ("OtherValue"));
      Assert.That (testPageHolderWithoutState.NamingContainer.ValueInControlState, Is.EqualTo ("NewNamingContainerValue"));
      Assert.That (testPageHolderWithoutState.Parent.ValueInControlState, Is.EqualTo ("NewParentValue"));
    }

    [Test]
    [Ignore]
    public void LoadControlStateRecursive_RegularPostBack_InitializedAfterLoadControlState ()
    {
      object controlState = CreateControlState ();
      var testPageHolderWithoutState = new TestPageHolder (false);
      SetupControlReplacerForIntegrationTest (testPageHolderWithoutState.NamingContainer, new LoadingStateSelectionStrategy ());
      testPageHolderWithoutState.Page.Controls.Remove (testPageHolderWithoutState.NamingContainer);

      testPageHolderWithoutState.PageInvoker.InitRecursive ();
      testPageHolderWithoutState.Page.SetPageStatePersister (
          new HiddenFieldPageStatePersister (testPageHolderWithoutState.Page) { ControlState = controlState });
      testPageHolderWithoutState.Page.LoadAllState ();

      Assert.That (testPageHolderWithoutState.OtherControl.ValueInControlState, Is.EqualTo ("OtherValue"));
      Assert.That (testPageHolderWithoutState.NamingContainer.ValueInControlState, Is.Null);
      Assert.That (testPageHolderWithoutState.Parent.ValueInControlState, Is.Null);

      testPageHolderWithoutState.Page.Controls.Add (testPageHolderWithoutState.NamingContainer);

      Assert.That (testPageHolderWithoutState.NamingContainer.ValueInControlState, Is.EqualTo ("NamingContainerValue"));
      Assert.That (testPageHolderWithoutState.Parent.ValueInControlState, Is.EqualTo ("ParentValue"));
    }

    [Test]
    [Ignore]
    public void LoadControlStateRecursive_ReplaceControlState_InitializedAfterLoadControlState ()
    {
      object originalControlState = CreateControlState ();

      var testPageHolderWithChangedState = new TestPageHolder (false);
      var replacerWithChangedState = SetupControlReplacerForIntegrationTest (testPageHolderWithChangedState.NamingContainer, new LoadingStateSelectionStrategy ());
      testPageHolderWithChangedState.PageInvoker.InitRecursive ();
      testPageHolderWithChangedState.Parent.ValueInControlState = "NewParentValue";
      testPageHolderWithChangedState.NamingContainer.ValueInControlState = "NewNamingContainerValue";
      string backedUpState = replacerWithChangedState.SaveAllState ();

      var testPageHolderWithoutState = new TestPageHolder (false);
      SetupControlReplacerForIntegrationTest (testPageHolderWithoutState.NamingContainer, new ReplacingStateSelectionStrategy (backedUpState));
      testPageHolderWithoutState.Page.Controls.Remove (testPageHolderWithoutState.NamingContainer);

      testPageHolderWithoutState.PageInvoker.InitRecursive ();
      testPageHolderWithoutState.Page.SetPageStatePersister (
          new HiddenFieldPageStatePersister (testPageHolderWithoutState.Page) { ControlState = originalControlState });
      testPageHolderWithoutState.Page.LoadAllState ();

      Assert.That (testPageHolderWithoutState.OtherControl.ValueInControlState, Is.EqualTo ("OtherValue"));
      Assert.That (testPageHolderWithoutState.NamingContainer.ValueInControlState, Is.Null);
      Assert.That (testPageHolderWithoutState.Parent.ValueInControlState, Is.Null);

      testPageHolderWithoutState.Page.Controls.Add (testPageHolderWithoutState.NamingContainer);

      Assert.That (testPageHolderWithoutState.NamingContainer.ValueInControlState, Is.EqualTo ("NewNamingContainerValue"));
      Assert.That (testPageHolderWithoutState.Parent.ValueInControlState, Is.EqualTo ("NewParentValue"));
    }

    [Test]
    [Ignore]
    public void LoadControlStateRecursive_ClearControlState_InitializedAfterLoadControlState ()
    {
      object originalControlState = CreateControlState ();

      var testPageHolderWithoutState = new TestPageHolder (false);
      SetupControlReplacerForIntegrationTest (testPageHolderWithoutState.NamingContainer, new ClearingStateSelectionStrategy ());
      testPageHolderWithoutState.Page.Controls.Remove (testPageHolderWithoutState.NamingContainer);

      testPageHolderWithoutState.PageInvoker.InitRecursive ();
      testPageHolderWithoutState.Page.SetPageStatePersister (
          new HiddenFieldPageStatePersister (testPageHolderWithoutState.Page) { ControlState = originalControlState });
      testPageHolderWithoutState.Page.LoadAllState ();

      Assert.That (testPageHolderWithoutState.OtherControl.ValueInControlState, Is.EqualTo ("OtherValue"));
      Assert.That (testPageHolderWithoutState.NamingContainer.ValueInControlState, Is.Null);
      Assert.That (testPageHolderWithoutState.Parent.ValueInControlState, Is.Null);

      testPageHolderWithoutState.Page.Controls.Add (testPageHolderWithoutState.NamingContainer);
      
      Assert.That (testPageHolderWithoutState.NamingContainer.ValueInControlState, Is.Null);
      Assert.That (testPageHolderWithoutState.Parent.ValueInControlState, Is.Null);
    }

    [Test]
    public void LoadControlStateRecursive_ClearControlState ()
    {
      object originalControlState = CreateControlState ();

      var testPageHolderWithoutState = new TestPageHolder (false);
      SetupControlReplacerForIntegrationTest (testPageHolderWithoutState.NamingContainer, new ClearingStateSelectionStrategy());

      testPageHolderWithoutState.PageInvoker.InitRecursive ();
      testPageHolderWithoutState.Page.SetPageStatePersister (
          new HiddenFieldPageStatePersister (testPageHolderWithoutState.Page) { ControlState = originalControlState });
      testPageHolderWithoutState.Page.LoadAllState ();

      Assert.That (testPageHolderWithoutState.OtherControl.ValueInControlState, Is.EqualTo ("OtherValue"));
      Assert.That (testPageHolderWithoutState.NamingContainer.ValueInControlState, Is.Null);
      Assert.That (testPageHolderWithoutState.Parent.ValueInControlState, Is.Null);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "Controls can only load state after OnInit phase.")]
    public void LoadControlStateRecursive_ThrowsIfNotAfterOnInit ()
    {
      var testPageHolder = new TestPageHolder (false);
      var replacer = SetupControlReplacerForIntegrationTest (testPageHolder.NamingContainer, new LoadingStateSelectionStrategy ());
      var controlInvoker = new ControlInvoker (replacer);
      controlInvoker.LoadControlState (null);
    }


    [Test]
    public void WrapControlWithParentContainer_ReplacesControl ()
    {
      var testPageHolder = new TestPageHolder (true);
      ControlReplacer replacer = new ControlReplacer (MemberCallerMock) { ID = "TheReplacer" };
      var controlToReplace = new ReplaceableControlMock();
      var controlToWrap = new ReplaceableControlMock ();
      MemberCallerMock.Stub (stub => stub.GetControlState (controlToReplace)).Return (ControlState.ChildrenInitialized);

      using (MemberCallerMock.GetMockRepository ().Ordered ())
      {
        MemberCallerMock.Expect (mock => mock.SetCollectionReadOnly (testPageHolder.Page.Controls, null)).Return ("error");
        MemberCallerMock.Expect (mock => mock.InitRecursive (replacer, testPageHolder.Page));
        MemberCallerMock.Expect (mock => mock.SetCollectionReadOnly (testPageHolder.Page.Controls, "error")).Return (null).Do (
            invocation => Assert.That (
                              testPageHolder.Page.Controls,
                              Is.EqualTo (new Control[] { testPageHolder.OtherNamingContainer, testPageHolder.NamingContainer, replacer })));
        Assert.That (replacer.Controls, Is.Empty);
      }

      testPageHolder.Page.Controls.Add (controlToReplace);
      replacer.ReplaceAndWrap (controlToReplace, controlToWrap, new LoadingStateSelectionStrategy ());

      Assert.That (
          testPageHolder.Page.Controls, 
          Is.EqualTo (new Control[] { testPageHolder.OtherNamingContainer, testPageHolder.NamingContainer, replacer }));
      Assert.That (replacer.Controls, Is.EqualTo (new[] { controlToWrap }));
      Assert.That (controlToReplace.Replacer, Is.Null);
      Assert.That (controlToWrap.Replacer, Is.SameAs (replacer));
      Assert.That (replacer.WrappedControl, Is.SameAs (controlToWrap));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "Controls can only be wrapped during OnInit phase.")]
    public void WrapControlWithParentContainer_ThrowsIfNotInOnInit ()
    {
      ControlReplacer replacer = new ControlReplacer (MemberCallerMock) { ID = "TheReplacer" };
      var control = new ReplaceableControlMock();
      MemberCallerMock.Stub (stub => stub.GetControlState (control)).Return (ControlState.Initialized);

      replacer.ReplaceAndWrap (control, control, new LoadingStateSelectionStrategy ());
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "Controls can only be wrapped before they are initialized.")]
    public void WrapControlWithParentContainer_ThrowsIfAlreadyInitialized ()
    {
      ControlReplacer replacer = new ControlReplacer (MemberCallerMock) { ID = "TheReplacer" };
      var control = new ReplaceableControlMock ();
      MemberCallerMock.Stub (stub => stub.GetControlState (control)).Return (ControlState.ChildrenInitialized);
      control.EnsureLazyInitializationContainer ();

      replacer.ReplaceAndWrap (control, control, new LoadingStateSelectionStrategy ());
    }

    [Test]
    [Ignore]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "The WrappedControl property can only be accessed after ReplaceAndWrap was invoked.")]
    public void GetWrappedControl_BeforeReplaceAndWrap ()
    {
      
    }
  }
}

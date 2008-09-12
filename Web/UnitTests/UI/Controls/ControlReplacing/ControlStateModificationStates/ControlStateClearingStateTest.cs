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
using Remotion.Web.UI.Controls.ControlReplacing;
using Remotion.Web.UI.Controls.ControlReplacing.ControlStateModificationStates;

namespace Remotion.Web.UnitTests.UI.Controls.ControlReplacing.ControlStateModificationStates
{
  [TestFixture]
  public class ControlStateClearingStateTest : TestBase
  {
    [Test]
    public void LoadControlState ()
    {
      object originalControlState = CreateControlState();
      TestPageHolder testPageHolder = new TestPageHolder (false);
      ControlReplacer replacer = SetupControlReplacerForIntegrationTest (testPageHolder.NamingContainer, null, false);
      ControlStateClearingState state = new ControlStateClearingState (replacer);
      testPageHolder.Page.SetRequestValueCollection (new NameValueCollection());
      testPageHolder.PageInvoker.InitRecursive();
      testPageHolder.Page.SetPageStatePersister (new HiddenFieldPageStatePersister (testPageHolder.Page) { ControlState = originalControlState });

      state.LoadControlState (null);

      Assert.That (replacer.ControlStateModificationState, Is.InstanceOfType (typeof (ControlStateCompletedState)));
      Assert.That (((ControlStateModificationStateBase) replacer.ControlStateModificationState).Replacer, Is.SameAs (replacer));
      IDictionary controlState = (IDictionary) testPageHolder.Page.GetPageStatePersister().ControlState;
      Assert.That (controlState, List.Not.Contains (testPageHolder.OtherNamingContainer.UniqueID));
      Assert.That (controlState, List.Not.Contains (testPageHolder.NamingContainer.UniqueID));
      Assert.That (controlState, List.Not.Contains (testPageHolder.Parent.UniqueID));
    }
  }
}
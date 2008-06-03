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
using System.Web.UI.WebControls;
using NUnit.Framework;
using Remotion.Globalization;

namespace Remotion.ObjectBinding.UnitTests.Web.UI.Controls.Infrastructure.BocList
{

[TestFixture]
public class EditModeControllerWithoutEditModeTest : EditModeControllerTestBase
{
  [Test]
  public void Initialize ()
  {
    Assert.AreSame (BocList, Controller.OwnerControl);
    Assert.IsFalse (Controller.IsRowEditModeActive);
    Assert.IsFalse (Controller.IsListEditModeActive);
  }

  [Test]
  public void InitRecursive()
  {
    Invoker.InitRecursive();

    Assert.AreEqual (0, Controller.Controls.Count);
  }

  [Test]
  public void CreateValidators ()
  {
    Invoker.InitRecursive();

    BaseValidator[] validators = Controller.CreateValidators (NullResourceManager.Instance);
    
    Assert.IsNotNull (validators);
    Assert.AreEqual (0, validators.Length);
  }

  [Test]
  public void Validate ()
  {
    Invoker.InitRecursive();
    Invoker.LoadRecursive();

    Assert.IsTrue (Controller.Validate());
  }

  [Test]
  public void IsRequired ()
  {
    Invoker.InitRecursive();
    Assert.IsFalse (Controller.IsRequired (0));
    Assert.IsFalse (Controller.IsRequired (1));
  }

  [Test]
  public void IsDirty ()
  {
    Invoker.InitRecursive();
    Assert.IsFalse (Controller.IsDirty());
  }

  [Test]
  public void GetTrackedIDs ()
  {
    Invoker.InitRecursive();

    Assert.AreEqual (new string[0], Controller.GetTrackedClientIDs());
  }

  [Test]
  public void SaveAndLoadControlState ()
  {
    Invoker.InitRecursive();

    object viewState = ControllerInvoker.SaveControlState();
    Assert.IsNotNull (viewState);
    ControllerInvoker.LoadControlState (viewState);
  }

  [Test]
  public void LoadControlStateWithNull ()
  {
    Invoker.InitRecursive();

    ControllerInvoker.LoadControlState (null);

    Assert.IsFalse (Controller.IsRowEditModeActive);
    Assert.IsFalse (Controller.IsListEditModeActive);
  }

  [Test]
  public void EnsureEditModeRestored ()
  {
    Assert.IsFalse (Controller.IsRowEditModeActive);

    Controller.EnsureEditModeRestored (Columns);
    
    Assert.IsFalse (Controller.IsRowEditModeActive);
  }

  [Test]
  public void EnsureEditModeRestoredWithValueNull ()
  {
    Controller.OwnerControl.LoadUnboundValue (null, false);    
   
    Assert.IsFalse (Controller.IsRowEditModeActive);

    Controller.EnsureEditModeRestored (Columns);
    
    Assert.IsFalse (Controller.IsRowEditModeActive);
  }
}

}

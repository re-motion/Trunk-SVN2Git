// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;
using System.Web.UI.WebControls;
using NUnit.Framework;
using Remotion.Globalization;

namespace Remotion.ObjectBinding.UnitTests.Web.UI.Controls.BocListImplementation.EditableRowSupport
{
  [TestFixture]
  public class EditModeControllerWithoutEditModeTest : EditModeControllerTestBase
  {
    [Test]
    public void Initialize ()
    {
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
      EditModeHost.Value = null;
   
      Assert.IsFalse (Controller.IsRowEditModeActive);

      Controller.EnsureEditModeRestored (Columns);
    
      Assert.IsFalse (Controller.IsRowEditModeActive);
    }
  }
}
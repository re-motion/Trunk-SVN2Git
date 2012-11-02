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
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using NUnit.Framework;
using Remotion.Globalization;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.EditableRowSupport;

namespace Remotion.ObjectBinding.UnitTests.Web.UI.Controls.BocListImplementation.EditableRowSupport
{
  [TestFixture]
  public class EditModeControllerInRowEditModeTest : EditModeControllerTestBase
  {
    [Test]
    public void GetFactoriesFromOwnerControl ()
    {
      EditModeHost.EditModeDataSourceFactory = new EditableRowDataSourceFactory();
      EditModeHost.EditModeControlFactory = EditableRowControlFactory.CreateEditableRowControlFactory();

      Invoker.InitRecursive();
      Controller.SwitchRowIntoEditMode (0, Columns, Columns);
      EditableRow row = (EditableRow) Controller.Controls[0];

      Assert.AreSame (EditModeHost.EditModeDataSourceFactory, row.DataSourceFactory);
      Assert.AreSame (EditModeHost.EditModeControlFactory, row.ControlFactory);
    }

    [Test]
    public void SwitchRowIntoEditMode ()
    {
      Invoker.InitRecursive();
      Controller.SwitchRowIntoEditMode (2, Columns, Columns);
     
      Assert.IsTrue (Controller.IsRowEditModeActive);
      Assert.AreEqual (2, Controller.GetEditedRow().Index);
    
      Assert.AreEqual (1, Controller.Controls.Count);
      Assert.IsTrue (Controller.Controls[0] is EditableRow);

      EditableRow row = (EditableRow) Controller.Controls[0];
      Assert.AreEqual ("Controller_Row_2", row.ID);

      Assert.AreEqual (0, ActualEvents.Count);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), 
        ExpectedMessage = "Cannot initialize row edit mode: The BocList 'BocList' does not have a Value.")]
    public void SwitchRowIntoEditModeWithValueNull ()
    {
      Invoker.InitRecursive();
      EditModeHost.Value = null;
      Controller.SwitchRowIntoEditMode (0, Columns, Columns);
    }

    [Test]
    [ExpectedException (typeof (ArgumentOutOfRangeException))]
    public void SwitchRowIntoEditModeWithIndexToHigh ()
    {
      Invoker.InitRecursive();
      Controller.SwitchRowIntoEditMode (5, Columns, Columns);
    }

    [Test]
    [ExpectedException (typeof (ArgumentOutOfRangeException))]
    public void SwitchRowIntoEditModeWithIndexToLow ()
    {
      Invoker.InitRecursive();
      Controller.SwitchRowIntoEditMode (-1, Columns, Columns);
    }

    [Test]
    public void SwitchRowIntoEditModeWhileRowEditModeIsActiveOnOtherRowWithValidValues ()
    {
      StringCollection expectedEvents = new StringCollection();
      expectedEvents.Add (FormatChangesSavingEventMessage (1, Values[1]));
      expectedEvents.Add (FormatValidateEditableRows());
      expectedEvents.Add (FormatChangesSavedEventMessage (1, Values[1]));
      expectedEvents.Add (FormatEndRowEditModeCleanUp(1));

      Invoker.InitRecursive();
      Controller.SwitchRowIntoEditMode (1, Columns, Columns);
     
      Assert.IsTrue (Controller.IsRowEditModeActive);
      Assert.AreEqual (1, Controller.GetEditedRow().Index);
    
      SetValues ((EditableRow) Controller.Controls[0], "New Value B", "200");

      Controller.SwitchRowIntoEditMode (2, Columns, Columns);
     
      CheckEvents (expectedEvents, ActualEvents);

      Assert.IsTrue (Controller.IsRowEditModeActive);
      Assert.AreEqual (2, Controller.GetEditedRow().Index);
    
      CheckValues (Values[1], "New Value B", 200);
    }

    [Test]
    public void SwitchRowIntoEditModeWhileRowEditModeIsActiveOnOtherRowWithInvalidValues ()
    {
      StringCollection expectedEvents = new StringCollection();
      expectedEvents.Add (FormatChangesSavingEventMessage (1, Values[1]));
      expectedEvents.Add (FormatValidateEditableRows());

      Invoker.InitRecursive();
      Controller.SwitchRowIntoEditMode (1, Columns, Columns);
     
      Assert.IsTrue (Controller.IsRowEditModeActive);
      Assert.AreEqual (1, Controller.GetEditedRow().Index);
    
      SetValues ((EditableRow) Controller.Controls[0], "New Value B", "");

      Controller.SwitchRowIntoEditMode (2, Columns, Columns);
     
      CheckEvents (expectedEvents, ActualEvents);

      Assert.IsTrue (Controller.IsRowEditModeActive);
      Assert.AreEqual (1, Controller.GetEditedRow().Index);
    
      CheckValues (Values[1], "B", 2);
    }
  
    [Test]
    public void SwitchRowIntoEditModeWhileRowEditModeIsActiveOnThisRowWithValidValues ()
    {
      StringCollection expectedEvents = new StringCollection();
      expectedEvents.Add (FormatChangesSavingEventMessage (2, Values[2]));
      expectedEvents.Add (FormatValidateEditableRows());
      expectedEvents.Add (FormatChangesSavedEventMessage (2, Values[2]));
      expectedEvents.Add (FormatEndRowEditModeCleanUp (2));

      Invoker.InitRecursive();
      Controller.SwitchRowIntoEditMode (2, Columns, Columns);
     
      Assert.IsTrue (Controller.IsRowEditModeActive);
      Assert.AreEqual (2, Controller.GetEditedRow().Index);
    
      SetValues ((EditableRow) Controller.Controls[0], "New Value C", "300");

      Controller.SwitchRowIntoEditMode (2, Columns, Columns);
     
      CheckEvents (expectedEvents, ActualEvents);

      Assert.IsTrue (Controller.IsRowEditModeActive);
      Assert.AreEqual (2, Controller.GetEditedRow().Index);
    
      CheckValues (Values[2], "New Value C", 300);
    }

    [Test]
    public void SwitchRowIntoEditModeWhileRowEditModeIsActiveOnThisRowWithInvalidValues ()
    {
      StringCollection expectedEvents = new StringCollection();
      expectedEvents.Add (FormatChangesSavingEventMessage (2, Values[2]));
      expectedEvents.Add (FormatValidateEditableRows());

      Invoker.InitRecursive();
      Controller.SwitchRowIntoEditMode (2, Columns, Columns);
     
      Assert.IsTrue (Controller.IsRowEditModeActive);
      Assert.AreEqual (2, Controller.GetEditedRow().Index);
    
      SetValues ((EditableRow) Controller.Controls[0], "New Value C", "");

      Controller.SwitchRowIntoEditMode (2, Columns, Columns);
     
      CheckEvents (expectedEvents, ActualEvents);

      Assert.IsTrue (Controller.IsRowEditModeActive);
      Assert.AreEqual (2, Controller.GetEditedRow().Index);
    
      CheckValues (Values[2], "C", 3);
    }
    
    [Test]
    public void SwitchRowIntoEditModeWhileListEditModeIsActiveWithValidValues ()
    {
      StringCollection expectedEvents = new StringCollection();
      expectedEvents.Add (FormatChangesSavingEventMessage (0, Values[0]));
      expectedEvents.Add (FormatChangesSavingEventMessage (1, Values[1]));
      expectedEvents.Add (FormatChangesSavingEventMessage (2, Values[2]));
      expectedEvents.Add (FormatChangesSavingEventMessage (3, Values[3]));
      expectedEvents.Add (FormatChangesSavingEventMessage (4, Values[4]));
      expectedEvents.Add (FormatValidateEditableRows());
      expectedEvents.Add (FormatChangesSavedEventMessage (0, Values[0]));
      expectedEvents.Add (FormatChangesSavedEventMessage (1, Values[1]));
      expectedEvents.Add (FormatChangesSavedEventMessage (2, Values[2]));
      expectedEvents.Add (FormatChangesSavedEventMessage (3, Values[3]));
      expectedEvents.Add (FormatChangesSavedEventMessage (4, Values[4]));
      expectedEvents.Add (FormatEndListEditModeCleanUp());

      Invoker.InitRecursive();
      Controller.SwitchListIntoEditMode (Columns, Columns);
     
      Assert.IsTrue (Controller.IsListEditModeActive);
    
      SetValues ((EditableRow) Controller.Controls[0], "New Value A", "100");
      SetValues ((EditableRow) Controller.Controls[1], "New Value B", "200");
      SetValues ((EditableRow) Controller.Controls[2], "New Value C", "300");
      SetValues ((EditableRow) Controller.Controls[3], "New Value D", "400");
      SetValues ((EditableRow) Controller.Controls[4], "New Value E", "500");

      Controller.SwitchRowIntoEditMode (2, Columns, Columns);
     
      CheckEvents (expectedEvents, ActualEvents);

      Assert.IsTrue (Controller.IsRowEditModeActive);
      Assert.AreEqual (2, Controller.GetEditedRow().Index);
    
      CheckValues (Values[0], "New Value A", 100);
      CheckValues (Values[1], "New Value B", 200);
      CheckValues (Values[2], "New Value C", 300);
      CheckValues (Values[3], "New Value D", 400);
      CheckValues (Values[4], "New Value E", 500);
    }

    [Test]
    public void SwitchRowIntoEditModeWhileListEditModeIsActiveWithInvalidValues ()
    {
      StringCollection expectedEvents = new StringCollection();
      expectedEvents.Add (FormatChangesSavingEventMessage (0, Values[0]));
      expectedEvents.Add (FormatChangesSavingEventMessage (1, Values[1]));
      expectedEvents.Add (FormatChangesSavingEventMessage (2, Values[2]));
      expectedEvents.Add (FormatChangesSavingEventMessage (3, Values[3]));
      expectedEvents.Add (FormatChangesSavingEventMessage (4, Values[4]));
      expectedEvents.Add (FormatValidateEditableRows());

      Invoker.InitRecursive();
      Controller.SwitchListIntoEditMode (Columns, Columns);
     
      Assert.IsTrue (Controller.IsListEditModeActive);
    
      SetValues ((EditableRow) Controller.Controls[0], "New Value A", "");
      SetValues ((EditableRow) Controller.Controls[1], "New Value B", "");
      SetValues ((EditableRow) Controller.Controls[2], "New Value C", "");
      SetValues ((EditableRow) Controller.Controls[3], "New Value D", "");
      SetValues ((EditableRow) Controller.Controls[4], "New Value E", "");

      Controller.SwitchRowIntoEditMode (2, Columns, Columns);
     
      CheckEvents (expectedEvents, ActualEvents);

      Assert.IsTrue (Controller.IsListEditModeActive);
      Assert.AreEqual (5, EditModeHost.Value.Count);
    
      CheckValues (Values[0], "A", 1);
      CheckValues (Values[1], "B", 2);
      CheckValues (Values[2], "C", 3);
      CheckValues (Values[3], "D", 4);
      CheckValues (Values[4], "E", 5);
    }

  
    [Test]
    public void AddAndEditRow ()
    {
      Invoker.InitRecursive();

      Assert.IsTrue (Controller.AddAndEditRow (NewValues[0], Columns, Columns));
     
      Assert.IsTrue (Controller.IsRowEditModeActive);
      Assert.AreEqual (5, Controller.GetEditedRow().Index);
      Assert.AreEqual (6, EditModeHost.Value.Count);
      Assert.AreSame (NewValues[0], EditModeHost.Value[5]);
    
      Assert.AreEqual (1, Controller.Controls.Count);
      Assert.IsTrue (Controller.Controls[0] is EditableRow);

      EditableRow row = (EditableRow) Controller.Controls[0];
      Assert.AreEqual ("Controller_Row_5", row.ID);

      Assert.AreEqual (0, ActualEvents.Count);
    }

    [Test]
    public void AddAndEditRowWhileRowEditModeIsActiveWithValidValues ()
    {
      StringCollection expectedEvents = new StringCollection();
      expectedEvents.Add (FormatChangesSavingEventMessage (2, Values[2]));
      expectedEvents.Add (FormatValidateEditableRows());
      expectedEvents.Add (FormatChangesSavedEventMessage (2, Values[2]));
      expectedEvents.Add (FormatEndRowEditModeCleanUp (2));

      Invoker.InitRecursive();
      Controller.SwitchRowIntoEditMode (2, Columns, Columns);
     
      Assert.IsTrue (Controller.IsRowEditModeActive);
      Assert.AreEqual (2, Controller.GetEditedRow().Index);
    
      SetValues ((EditableRow) Controller.Controls[0], "New Value C", "300");

      Assert.IsTrue (Controller.AddAndEditRow (NewValues[0], Columns, Columns));
     
      CheckEvents (expectedEvents, ActualEvents);

      Assert.IsTrue (Controller.IsRowEditModeActive);
      Assert.AreEqual (5, Controller.GetEditedRow().Index);
      Assert.AreEqual (6, EditModeHost.Value.Count);
      Assert.AreSame (NewValues[0], EditModeHost.Value[5]);
    
      CheckValues (Values[2], "New Value C", 300);
    }
  
    [Test]
    public void AddAndEditRowWhileRowEditModeIsActiveWithInvalidValues ()
    {
      StringCollection expectedEvents = new StringCollection();
      expectedEvents.Add (FormatChangesSavingEventMessage (2, Values[2]));
      expectedEvents.Add (FormatValidateEditableRows());

      Invoker.InitRecursive();
      Controller.SwitchRowIntoEditMode (2, Columns, Columns);
     
      Assert.IsTrue (Controller.IsRowEditModeActive);
      Assert.AreEqual (2, Controller.GetEditedRow().Index);
    
      SetValues ((EditableRow) Controller.Controls[0], "New Value C", "");

      Assert.IsFalse (Controller.AddAndEditRow (NewValues[0], Columns, Columns));
     
      CheckEvents (expectedEvents, ActualEvents);

      Assert.IsTrue (Controller.IsRowEditModeActive);
      Assert.AreEqual (2, Controller.GetEditedRow().Index);
      Assert.AreEqual (5, EditModeHost.Value.Count);
    
      CheckValues (Values[2], "C", 3);
    }
  
    [Test]
    public void AddAndEditRowWhileListEditModeIsActiveWithValidValues ()
    {
      StringCollection expectedEvents = new StringCollection();
      expectedEvents.Add (FormatChangesSavingEventMessage (0, Values[0]));
      expectedEvents.Add (FormatChangesSavingEventMessage (1, Values[1]));
      expectedEvents.Add (FormatChangesSavingEventMessage (2, Values[2]));
      expectedEvents.Add (FormatChangesSavingEventMessage (3, Values[3]));
      expectedEvents.Add (FormatChangesSavingEventMessage (4, Values[4]));
      expectedEvents.Add (FormatValidateEditableRows());
      expectedEvents.Add (FormatChangesSavedEventMessage (0, Values[0]));
      expectedEvents.Add (FormatChangesSavedEventMessage (1, Values[1]));
      expectedEvents.Add (FormatChangesSavedEventMessage (2, Values[2]));
      expectedEvents.Add (FormatChangesSavedEventMessage (3, Values[3]));
      expectedEvents.Add (FormatChangesSavedEventMessage (4, Values[4]));
      expectedEvents.Add (FormatEndListEditModeCleanUp());

      Invoker.InitRecursive();
      Controller.SwitchListIntoEditMode (Columns, Columns);
     
      Assert.IsTrue (Controller.IsListEditModeActive);
    
      SetValues ((EditableRow) Controller.Controls[0], "New Value A", "100");
      SetValues ((EditableRow) Controller.Controls[1], "New Value B", "200");
      SetValues ((EditableRow) Controller.Controls[2], "New Value C", "300");
      SetValues ((EditableRow) Controller.Controls[3], "New Value D", "400");
      SetValues ((EditableRow) Controller.Controls[4], "New Value E", "500");

      Assert.IsTrue (Controller.AddAndEditRow (NewValues[0], Columns, Columns));
     
      CheckEvents (expectedEvents, ActualEvents);

      Assert.IsTrue (Controller.IsRowEditModeActive);
      Assert.AreEqual (5, Controller.GetEditedRow().Index);
      Assert.AreEqual (6, EditModeHost.Value.Count);
      Assert.AreSame (NewValues[0], EditModeHost.Value[5]);
    
      CheckValues (Values[0], "New Value A", 100);
      CheckValues (Values[1], "New Value B", 200);
      CheckValues (Values[2], "New Value C", 300);
      CheckValues (Values[3], "New Value D", 400);
      CheckValues (Values[4], "New Value E", 500);
    }
  
    [Test]
    public void AddAndEditRowWhileListEditModeIsActiveWithInvalidValues ()
    {
      StringCollection expectedEvents = new StringCollection();
      expectedEvents.Add (FormatChangesSavingEventMessage (0, Values[0]));
      expectedEvents.Add (FormatChangesSavingEventMessage (1, Values[1]));
      expectedEvents.Add (FormatChangesSavingEventMessage (2, Values[2]));
      expectedEvents.Add (FormatChangesSavingEventMessage (3, Values[3]));
      expectedEvents.Add (FormatChangesSavingEventMessage (4, Values[4]));
      expectedEvents.Add (FormatValidateEditableRows());

      Invoker.InitRecursive();
      Controller.SwitchListIntoEditMode (Columns, Columns);
     
      Assert.IsTrue (Controller.IsListEditModeActive);
    
      SetValues ((EditableRow) Controller.Controls[0], "New Value A", "");
      SetValues ((EditableRow) Controller.Controls[1], "New Value B", "");
      SetValues ((EditableRow) Controller.Controls[2], "New Value C", "");
      SetValues ((EditableRow) Controller.Controls[3], "New Value D", "");
      SetValues ((EditableRow) Controller.Controls[4], "New Value E", "");

      Assert.IsFalse (Controller.AddAndEditRow (NewValues[0], Columns, Columns));
     
      CheckEvents (expectedEvents, ActualEvents);

      Assert.IsTrue (Controller.IsListEditModeActive);
      Assert.AreEqual (5, EditModeHost.Value.Count);
    
      CheckValues (Values[0], "A", 1);
      CheckValues (Values[1], "B", 2);
      CheckValues (Values[2], "C", 3);
      CheckValues (Values[3], "D", 4);
      CheckValues (Values[4], "E", 5);
    }

  
    [Test]
    public void EndRowEditModeAndSaveChangesWithValidValues ()
    {
      StringCollection expectedEvents = new StringCollection();
      expectedEvents.Add (FormatChangesSavingEventMessage (2, Values[2]));
      expectedEvents.Add (FormatValidateEditableRows());
      expectedEvents.Add (FormatChangesSavedEventMessage (2, Values[2]));
      expectedEvents.Add (FormatEndRowEditModeCleanUp (2));

      Invoker.InitRecursive();
      Controller.SwitchRowIntoEditMode (2, Columns, Columns);
     
      Assert.IsTrue (Controller.IsRowEditModeActive);
      Assert.AreEqual (2, Controller.GetEditedRow().Index);
    
      SetValues ((EditableRow) Controller.Controls[0], "New Value C", "300");
      Controller.EndRowEditMode (true, Columns);

      CheckEvents (expectedEvents, ActualEvents);
    
      Assert.IsFalse (Controller.IsRowEditModeActive);

      CheckValues (Values[2], "New Value C", 300);
    }

    [Test]
    public void EndRowEditModeAndDiscardChangesWithValidValues ()
    {
      StringCollection expectedEvents = new StringCollection();
      expectedEvents.Add (FormatChangesCancelingEventMessage (2, Values[2]));
      expectedEvents.Add (FormatChangesCanceledEventMessage (2, Values[2]));
      expectedEvents.Add (FormatEndRowEditModeCleanUp (2));

      Invoker.InitRecursive();
      Controller.SwitchRowIntoEditMode (2, Columns, Columns);
     
      Assert.IsTrue (Controller.IsRowEditModeActive);
      Assert.AreEqual (2, Controller.GetEditedRow().Index);
    
      SetValues ((EditableRow) Controller.Controls[0], "New Value C", "300");
      Controller.EndRowEditMode (false, Columns);

      CheckEvents (expectedEvents, ActualEvents);
    
      Assert.IsFalse (Controller.IsRowEditModeActive);

      CheckValues (Values[2], "C", 3);
    }

    [Test]
    public void EndRowEditModeWithNewRowAndSaveChangesWithValidValues ()
    {
      StringCollection expectedEvents = new StringCollection();
      expectedEvents.Add (FormatChangesSavingEventMessage (5, NewValues[0]));
      expectedEvents.Add (FormatValidateEditableRows());
      expectedEvents.Add (FormatChangesSavedEventMessage (5, NewValues[0]));
      expectedEvents.Add (FormatEndRowEditModeCleanUp (5));

      Invoker.InitRecursive();    
      Controller.AddAndEditRow (NewValues[0], Columns, Columns);
     
      Assert.IsTrue (Controller.IsRowEditModeActive);
      Assert.AreEqual (5, Controller.GetEditedRow().Index);

      SetValues ((EditableRow) Controller.Controls[0], "New Value F", "600");
      Controller.EndRowEditMode (true, Columns);

      CheckEvents (expectedEvents, ActualEvents);
    
      Assert.IsFalse (Controller.IsRowEditModeActive);

      Assert.AreEqual (6, EditModeHost.Value.Count);
      CheckValues (NewValues[0], "New Value F", 600);
    }

    [Test]
    public void EndRowEditModeWithNewRowAndDiscardChangesWithValidValues ()
    {
      StringCollection expectedEvents = new StringCollection();
      expectedEvents.Add (FormatChangesCancelingEventMessage (5, NewValues[0]));
      expectedEvents.Add (FormatChangesCanceledEventMessage (-1, NewValues[0]));
      expectedEvents.Add (FormatEndRowEditModeCleanUp (5));

      Invoker.InitRecursive();
      Controller.AddAndEditRow (NewValues[0], Columns, Columns);
     
      Assert.IsTrue (Controller.IsRowEditModeActive);
      Assert.AreEqual (5, Controller.GetEditedRow().Index);

      SetValues ((EditableRow) Controller.Controls[0], "New Value F", "600");
      Controller.EndRowEditMode (false, Columns);

      CheckEvents (expectedEvents, ActualEvents);
    
      Assert.IsFalse (Controller.IsRowEditModeActive);

      Assert.AreEqual (5, EditModeHost.Value.Count);
      CheckValues (NewValues[0], "F", 6);
    }

    [Test]
    public void EndRowEditModeAndSaveChangesWithInvalidValues ()
    {
      StringCollection expectedEvents = new StringCollection();
      expectedEvents.Add (FormatChangesSavingEventMessage (2, Values[2]));
      expectedEvents.Add (FormatValidateEditableRows());

      Invoker.InitRecursive();
      Controller.SwitchRowIntoEditMode (2, Columns, Columns);
     
      Assert.IsTrue (Controller.IsRowEditModeActive);
      Assert.AreEqual (2, Controller.GetEditedRow().Index);
    
      SetValues ((EditableRow) Controller.Controls[0], "New Value C", "");
      Controller.EndRowEditMode (true, Columns);

      CheckEvents (expectedEvents, ActualEvents);
    
      Assert.IsTrue(Controller.IsRowEditModeActive);
      Assert.AreEqual (2, Controller.GetEditedRow().Index);

      CheckValues (Values[2], "C", 3);
    }

    [Test]
    public void EndRowEditModeAndDiscardChangesWithInvalidValues ()
    {
      StringCollection expectedEvents = new StringCollection();
      expectedEvents.Add (FormatChangesCancelingEventMessage (2, Values[2]));
      expectedEvents.Add (FormatChangesCanceledEventMessage (2, Values[2]));
      expectedEvents.Add (FormatEndRowEditModeCleanUp (2));

      Invoker.InitRecursive();
      Controller.SwitchRowIntoEditMode (2, Columns, Columns);
     
      Assert.IsTrue (Controller.IsRowEditModeActive);
      Assert.AreEqual (2, Controller.GetEditedRow().Index);
    
      SetValues ((EditableRow) Controller.Controls[0], "New Value C", "");
      Controller.EndRowEditMode (false, Columns);

      CheckEvents (expectedEvents, ActualEvents);
    
      Assert.IsFalse (Controller.IsRowEditModeActive);

      CheckValues (Values[2], "C", 3);
    }

    [Test]
    public void EndRowEditModeWithoutBeingActive ()
    {
      Invoker.InitRecursive();
     
      Assert.IsFalse (Controller.IsRowEditModeActive);
    
      Controller.EndRowEditMode (true, Columns);

      Assert.IsFalse (Controller.IsRowEditModeActive);
      Assert.AreEqual (0, ActualEvents.Count);
    }

    [Test]
    public void EndRowEditModeWithoutBeingActiveAndValueNull ()
    {
      Invoker.InitRecursive();
      EditModeHost.Value = null;
     
      Assert.IsFalse (Controller.IsRowEditModeActive);
    
      Controller.EndRowEditMode (true, Columns);

      Assert.IsFalse (Controller.IsRowEditModeActive);
      Assert.AreEqual (0, ActualEvents.Count);
    }


    [Test]
    public void EnsureEditModeRestored ()
    {
      Assert.IsFalse (Controller.IsRowEditModeActive);
      ControllerInvoker.LoadControlState (CreateControlState (null, false, 2, false));
      Assert.IsTrue (Controller.IsRowEditModeActive);
    
      Controller.EnsureEditModeRestored (Columns);
      Assert.IsTrue (Controller.IsRowEditModeActive);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), 
        ExpectedMessage = "Cannot restore row edit mode: The Value collection of the BocList 'BocList' no longer contains the previously edited row.")]
    public void EnsureEditModeRestoredWithInvalidRowIndex ()
    {
      Assert.IsFalse (Controller.IsRowEditModeActive);
      ControllerInvoker.LoadControlState (CreateControlState (null, false, 6, false));
      Assert.IsTrue (Controller.IsRowEditModeActive);
 
      Controller.EnsureEditModeRestored (Columns);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), 
        ExpectedMessage = "Cannot restore edit mode: The BocList 'BocList' does not have a Value.")]
    public void EnsureEditModeRestoredWithValueNull ()
    {
      Assert.IsFalse (Controller.IsRowEditModeActive);
      ControllerInvoker.LoadControlState (CreateControlState (null, false, 6, false));
      Assert.IsTrue (Controller.IsRowEditModeActive);
      EditModeHost.Value = null;

      Controller.EnsureEditModeRestored (Columns);
    }


    [Test]
    public void AddRows ()
    {
      Invoker.InitRecursive();
      Controller.SwitchRowIntoEditMode (2, Columns, Columns);
     
      Assert.IsTrue (Controller.IsRowEditModeActive);
      Assert.AreEqual (2, Controller.GetEditedRow().Index);
      Assert.AreEqual (5, EditModeHost.Value.Count);

      Controller.AddRows (NewValues, Columns, Columns);
    
      Assert.AreEqual (7, EditModeHost.Value.Count);
      Assert.AreSame (NewValues[0], EditModeHost.Value[5]);
      Assert.AreSame (NewValues[1], EditModeHost.Value[6]);
      Assert.IsTrue (Controller.IsRowEditModeActive);
      Assert.AreEqual (2, Controller.GetEditedRow().Index);

      Assert.AreEqual (0, ActualEvents.Count);
    }

    [Test]
    public void AddRow ()
    {
      Invoker.InitRecursive();
      Controller.SwitchRowIntoEditMode (2, Columns, Columns);
     
      Assert.IsTrue (Controller.IsRowEditModeActive);
      Assert.AreEqual (2, Controller.GetEditedRow().Index);
      Assert.AreEqual (5, EditModeHost.Value.Count);

      Assert.AreEqual (5, Controller.AddRow (NewValues[0], Columns, Columns));
    
      Assert.AreEqual (6, EditModeHost.Value.Count);
      Assert.AreSame (NewValues[0], EditModeHost.Value[5]);
      Assert.IsTrue (Controller.IsRowEditModeActive);
      Assert.AreEqual (2, Controller.GetEditedRow().Index);

      Assert.AreEqual (0, ActualEvents.Count);
    }


    [Test]
    [ExpectedException (typeof (InvalidOperationException),
        ExpectedMessage = "Cannot remove rows while the BocList 'BocList' is in row edit mode. Call EndEditMode() before removing the rows.")]
    public void RemoveRows ()
    {
      Invoker.InitRecursive();
      Controller.SwitchRowIntoEditMode (2, Columns, Columns);
     
      Assert.IsTrue (Controller.IsRowEditModeActive);
      Assert.AreEqual (2, Controller.GetEditedRow().Index);
      Assert.AreEqual (5, EditModeHost.Value.Count);

      Controller.RemoveRows (new IBusinessObject[] {Values[2]});
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException),
        ExpectedMessage = "Cannot remove rows while the BocList 'BocList' is in row edit mode. Call EndEditMode() before removing the rows.")]
    public void RemoveRow ()
    {
      Invoker.InitRecursive();
      Controller.SwitchRowIntoEditMode (2, Columns, Columns);
     
      Assert.IsTrue (Controller.IsRowEditModeActive);
      Assert.AreEqual (2, Controller.GetEditedRow().Index);
      Assert.AreEqual (5, EditModeHost.Value.Count);

      Controller.RemoveRow (Values[2]);
    }

  
    [Test]
    public void CreateValidators ()
    {
      IResourceManager resourceManager = (IResourceManager) NullResourceManager.Instance;

      Invoker.InitRecursive();
      Controller.SwitchRowIntoEditMode (2, Columns, Columns);

      Assert.IsTrue (Controller.IsRowEditModeActive);
      Assert.AreEqual (2, Controller.GetEditedRow().Index);

      BaseValidator[] validators = Controller.CreateValidators (resourceManager);
    
      Assert.IsNotNull (validators);
      Assert.AreEqual (1, validators.Length);
      Assert.IsInstanceOf (typeof (EditModeValidator), validators[0]);
      Assert.AreEqual (EditModeHost.ID, validators[0].ControlToValidate);
      Assert.AreEqual (resourceManager.GetString (Remotion.ObjectBinding.Web.UI.Controls.BocList.ResourceIdentifier.RowEditModeErrorMessage), validators[0].ErrorMessage);
    }

    [Test]
    public void CreateValidatorsWithErrorMessageFromOwnerControl ()
    {
      Invoker.InitRecursive();
      Controller.SwitchRowIntoEditMode (2, Columns, Columns);
      EditModeHost.ErrorMessage = "Foo Bar";

      Assert.IsTrue (Controller.IsRowEditModeActive);
      Assert.AreEqual (2, Controller.GetEditedRow().Index);

      BaseValidator[] validators = Controller.CreateValidators (NullResourceManager.Instance);
    
      Assert.IsNotNull (validators);
      Assert.AreEqual (1, validators.Length);
      Assert.IsInstanceOf (typeof (EditModeValidator), validators[0]);
      Assert.AreEqual (EditModeHost.ID, validators[0].ControlToValidate);
      Assert.AreEqual ("Foo Bar", validators[0].ErrorMessage);
    }


    [Test]
    public void ValidateWithValidValues ()
    {
      Invoker.InitRecursive();
      Controller.SwitchRowIntoEditMode (2, Columns, Columns);

      Assert.IsTrue (Controller.IsRowEditModeActive);
      Assert.AreEqual (2, Controller.GetEditedRow().Index);

      SetValues ((EditableRow) Controller.Controls[0], "New Value C", "300");

      Assert.IsTrue (Controller.Validate());
    }

    [Test]
    public void ValidateWithInvalidValues ()
    {
      Invoker.InitRecursive();
      Controller.SwitchRowIntoEditMode (2, Columns, Columns);

      SetValues ((EditableRow) Controller.Controls[0], "New Value C", "");

      Assert.IsFalse (Controller.Validate());
    }


    [Test]
    public void PrepareValidation ()
    {
      Invoker.InitRecursive();
      Controller.SwitchRowIntoEditMode (2, Columns, Columns);

      EditableRow editableRow = (EditableRow) Controller.Controls[0];

      BocTextValue stringValueField = (BocTextValue) editableRow.GetEditControl (0);
      BocTextValue int32ValueField = (BocTextValue) editableRow.GetEditControl (1);
    
      Controller.PrepareValidation();
    
      Assert.AreEqual (stringValueField.Text, stringValueField.Text);
      Assert.AreEqual (int32ValueField.Text, int32ValueField.Text);
    }
  

    [Test]
    public void IsRequired ()
    {
      Controller.SwitchRowIntoEditMode (2, Columns, Columns);
    
      Assert.IsTrue (Controller.IsRowEditModeActive);

      Assert.IsFalse (Controller.IsRequired (0));
      Assert.IsTrue (Controller.IsRequired (1));
    }

    [Test]
    public void IsDirty ()
    {
      Invoker.InitRecursive();
      Controller.SwitchRowIntoEditMode (2, Columns, Columns);
    
      EditableRow row = (EditableRow) Controller.Controls[0];
      Remotion.ObjectBinding.Web.UI.Controls.BocTextValue stringValueField = 
          (Remotion.ObjectBinding.Web.UI.Controls.BocTextValue) row.GetEditControl (0);
      stringValueField.Value = "New Value";

      Assert.IsTrue (Controller.IsDirty());
    }

    [Test]
    public void GetTrackedIDs ()
    {
      Invoker.InitRecursive();
      Controller.SwitchRowIntoEditMode (2, Columns, Columns);

      string id = "NamingContainer_Controller_Row_{0}_{1}_Boc_TextBox";
      string[] trackedIDs = new string[2];
      trackedIDs[0] = string.Format (id, 2, 0);
      trackedIDs[1] = string.Format (id, 2, 1);

      Assert.AreEqual (trackedIDs, Controller.GetTrackedClientIDs());
    }


    [Test]
    public void SaveAndLoadControlState ()
    {
      Invoker.InitRecursive();
      Controller.SwitchRowIntoEditMode (2, Columns, Columns);
      Assert.IsTrue (Controller.IsRowEditModeActive);
      Assert.AreEqual (2, Controller.GetEditedRow().Index);

      object viewState = ControllerInvoker.SaveControlState();
      Assert.IsNotNull (viewState);

      Controller.EndRowEditMode (false, Columns);
      Assert.IsFalse (Controller.IsRowEditModeActive);

      ControllerInvoker.LoadControlState (viewState);
      Assert.IsTrue (Controller.IsRowEditModeActive);
      Assert.AreEqual (2, Controller.GetEditedRow().Index);
    }
  }
}
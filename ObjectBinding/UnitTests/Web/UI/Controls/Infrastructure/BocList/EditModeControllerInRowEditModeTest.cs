// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
//
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
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
using Remotion.ObjectBinding.Web.UI.Controls.Infrastructure.BocList;

namespace Remotion.ObjectBinding.UnitTests.Web.UI.Controls.Infrastructure.BocList
{

[TestFixture]
public class EditModeControllerInRowEditModeTest : EditModeControllerTestBase
{
  [Test]
  public void GetFactoriesFromOwnerControl ()
  {
    Controller.OwnerControl.EditModeDataSourceFactory = new EditableRowDataSourceFactory();
    Controller.OwnerControl.EditModeControlFactory = new EditableRowControlFactory();

    Invoker.InitRecursive();
    Controller.SwitchRowIntoEditMode (0, Columns, Columns);
    EditableRow row = (EditableRow) Controller.Controls[0];

    Assert.AreSame (Controller.OwnerControl.EditModeDataSourceFactory, row.DataSourceFactory);
    Assert.AreSame (Controller.OwnerControl.EditModeControlFactory, row.ControlFactory);
  }

  [Test]
  public void SwitchRowIntoEditMode ()
  {
    Invoker.InitRecursive();
    Controller.SwitchRowIntoEditMode (2, Columns, Columns);
     
    Assert.IsTrue (Controller.IsRowEditModeActive);
    Assert.AreEqual (2, Controller.EditableRowIndex.Value);
    
    Assert.AreEqual (1, Controller.Controls.Count);
    Assert.IsTrue (Controller.Controls[0] is EditableRow);

    EditableRow row = (EditableRow) Controller.Controls[0];
    Assert.AreEqual ("Controller_Row0", row.ID);

    Assert.AreEqual (0, ActualEvents.Count);
  }

  [Test]
  [ExpectedException (typeof (InvalidOperationException), 
      ExpectedMessage = "Cannot initialize row edit mode: The BocList 'BocList' does not have a Value.")]
  public void SwitchRowIntoEditModeWithValueNull ()
  {
    Invoker.InitRecursive();
    BocList.LoadUnboundValue (null, false);
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
    expectedEvents.Add (FormatChangesSavedEventMessage (1, Values[1]));

    Invoker.InitRecursive();
    Controller.SwitchRowIntoEditMode (1, Columns, Columns);
     
    Assert.IsTrue (Controller.IsRowEditModeActive);
    Assert.AreEqual (1, Controller.EditableRowIndex.Value);
    
    SetValues ((EditableRow) Controller.Controls[0], "New Value B", "200");

    Controller.SwitchRowIntoEditMode (2, Columns, Columns);
     
    CheckEvents (expectedEvents, ActualEvents);

    Assert.IsTrue (Controller.IsRowEditModeActive);
    Assert.AreEqual (2, Controller.EditableRowIndex.Value);
    
    CheckValues (Values[1], "New Value B", 200);
  }

  [Test]
  public void SwitchRowIntoEditModeWhileRowEditModeIsActiveOnOtherRowWithInvalidValues ()
  {
    StringCollection expectedEvents = new StringCollection();
    expectedEvents.Add (FormatChangesSavingEventMessage (1, Values[1]));

    Invoker.InitRecursive();
    Controller.SwitchRowIntoEditMode (1, Columns, Columns);
     
    Assert.IsTrue (Controller.IsRowEditModeActive);
    Assert.AreEqual (1, Controller.EditableRowIndex.Value);
    
    SetValues ((EditableRow) Controller.Controls[0], "New Value B", "");

    Controller.SwitchRowIntoEditMode (2, Columns, Columns);
     
    CheckEvents (expectedEvents, ActualEvents);

    Assert.IsTrue (Controller.IsRowEditModeActive);
    Assert.AreEqual (1, Controller.EditableRowIndex.Value);
    
    CheckValues (Values[1], "B", 2);
  }
  
  [Test]
  public void SwitchRowIntoEditModeWhileRowEditModeIsActiveOnThisRowWithValidValues ()
  {
    StringCollection expectedEvents = new StringCollection();
    expectedEvents.Add (FormatChangesSavingEventMessage (2, Values[2]));
    expectedEvents.Add (FormatChangesSavedEventMessage (2, Values[2]));

    Invoker.InitRecursive();
    Controller.SwitchRowIntoEditMode (2, Columns, Columns);
     
    Assert.IsTrue (Controller.IsRowEditModeActive);
    Assert.AreEqual (2, Controller.EditableRowIndex.Value);
    
    SetValues ((EditableRow) Controller.Controls[0], "New Value C", "300");

    Controller.SwitchRowIntoEditMode (2, Columns, Columns);
     
    CheckEvents (expectedEvents, ActualEvents);

    Assert.IsTrue (Controller.IsRowEditModeActive);
    Assert.AreEqual (2, Controller.EditableRowIndex.Value);
    
    CheckValues (Values[2], "New Value C", 300);
  }

  [Test]
  public void SwitchRowIntoEditModeWhileRowEditModeIsActiveOnThisRowWithInvalidValues ()
  {
    StringCollection expectedEvents = new StringCollection();
    expectedEvents.Add (FormatChangesSavingEventMessage (2, Values[2]));

    Invoker.InitRecursive();
    Controller.SwitchRowIntoEditMode (2, Columns, Columns);
     
    Assert.IsTrue (Controller.IsRowEditModeActive);
    Assert.AreEqual (2, Controller.EditableRowIndex.Value);
    
    SetValues ((EditableRow) Controller.Controls[0], "New Value C", "");

    Controller.SwitchRowIntoEditMode (2, Columns, Columns);
     
    CheckEvents (expectedEvents, ActualEvents);

    Assert.IsTrue (Controller.IsRowEditModeActive);
    Assert.AreEqual (2, Controller.EditableRowIndex.Value);
    
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
    expectedEvents.Add (FormatChangesSavedEventMessage (0, Values[0]));
    expectedEvents.Add (FormatChangesSavedEventMessage (1, Values[1]));
    expectedEvents.Add (FormatChangesSavedEventMessage (2, Values[2]));
    expectedEvents.Add (FormatChangesSavedEventMessage (3, Values[3]));
    expectedEvents.Add (FormatChangesSavedEventMessage (4, Values[4]));

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
    Assert.AreEqual (2, Controller.EditableRowIndex.Value);
    
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
    Assert.AreEqual (5, Controller.OwnerControl.Value.Count);
    
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
    Assert.AreEqual (5, Controller.EditableRowIndex.Value);
    Assert.AreEqual (6, Controller.OwnerControl.Value.Count);
    Assert.AreSame (NewValues[0], Controller.OwnerControl.Value[5]);
    
    Assert.AreEqual (1, Controller.Controls.Count);
    Assert.IsTrue (Controller.Controls[0] is EditableRow);

    EditableRow row = (EditableRow) Controller.Controls[0];
    Assert.AreEqual ("Controller_Row0", row.ID);

    Assert.AreEqual (0, ActualEvents.Count);
  }

  [Test]
  public void AddAndEditRowWhileRowEditModeIsActiveWithValidValues ()
  {
    StringCollection expectedEvents = new StringCollection();
    expectedEvents.Add (FormatChangesSavingEventMessage (2, Values[2]));
    expectedEvents.Add (FormatChangesSavedEventMessage (2, Values[2]));

    Invoker.InitRecursive();
    Controller.SwitchRowIntoEditMode (2, Columns, Columns);
     
    Assert.IsTrue (Controller.IsRowEditModeActive);
    Assert.AreEqual (2, Controller.EditableRowIndex.Value);
    
    SetValues ((EditableRow) Controller.Controls[0], "New Value C", "300");

    Assert.IsTrue (Controller.AddAndEditRow (NewValues[0], Columns, Columns));
     
    CheckEvents (expectedEvents, ActualEvents);

    Assert.IsTrue (Controller.IsRowEditModeActive);
    Assert.AreEqual (5, Controller.EditableRowIndex.Value);
    Assert.AreEqual (6, Controller.OwnerControl.Value.Count);
    Assert.AreSame (NewValues[0], Controller.OwnerControl.Value[5]);
    
    CheckValues (Values[2], "New Value C", 300);
  }
  
  [Test]
  public void AddAndEditRowWhileRowEditModeIsActiveWithInvalidValues ()
  {
    StringCollection expectedEvents = new StringCollection();
    expectedEvents.Add (FormatChangesSavingEventMessage (2, Values[2]));

    Invoker.InitRecursive();
    Controller.SwitchRowIntoEditMode (2, Columns, Columns);
     
    Assert.IsTrue (Controller.IsRowEditModeActive);
    Assert.AreEqual (2, Controller.EditableRowIndex.Value);
    
    SetValues ((EditableRow) Controller.Controls[0], "New Value C", "");

    Assert.IsFalse (Controller.AddAndEditRow (NewValues[0], Columns, Columns));
     
    CheckEvents (expectedEvents, ActualEvents);

    Assert.IsTrue (Controller.IsRowEditModeActive);
    Assert.AreEqual (2, Controller.EditableRowIndex.Value);
    Assert.AreEqual (5, Controller.OwnerControl.Value.Count);
    
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
    expectedEvents.Add (FormatChangesSavedEventMessage (0, Values[0]));
    expectedEvents.Add (FormatChangesSavedEventMessage (1, Values[1]));
    expectedEvents.Add (FormatChangesSavedEventMessage (2, Values[2]));
    expectedEvents.Add (FormatChangesSavedEventMessage (3, Values[3]));
    expectedEvents.Add (FormatChangesSavedEventMessage (4, Values[4]));

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
    Assert.AreEqual (5, Controller.EditableRowIndex.Value);
    Assert.AreEqual (6, Controller.OwnerControl.Value.Count);
    Assert.AreSame (NewValues[0], Controller.OwnerControl.Value[5]);
    
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
    Assert.AreEqual (5, Controller.OwnerControl.Value.Count);
    
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
    expectedEvents.Add (FormatChangesSavedEventMessage (2, Values[2]));

    Invoker.InitRecursive();
    Controller.SwitchRowIntoEditMode (2, Columns, Columns);
     
    Assert.IsTrue (Controller.IsRowEditModeActive);
    Assert.AreEqual (2, Controller.EditableRowIndex.Value);
    
    SetValues ((EditableRow) Controller.Controls[0], "New Value C", "300");
    Controller.EndRowEditMode (true, Columns);

    CheckEvents (expectedEvents, ActualEvents);
    
    Assert.IsFalse (Controller.IsRowEditModeActive);
    Assert.IsNull (Controller.EditableRowIndex);

    CheckValues (Values[2], "New Value C", 300);
  }

  [Test]
  public void EndRowEditModeAndDiscardChangesWithValidValues ()
  {
    StringCollection expectedEvents = new StringCollection();
    expectedEvents.Add (FormatChangesCancelingEventMessage (2, Values[2]));
    expectedEvents.Add (FormatChangesCanceledEventMessage (2, Values[2]));

    Invoker.InitRecursive();
    Controller.SwitchRowIntoEditMode (2, Columns, Columns);
     
    Assert.IsTrue (Controller.IsRowEditModeActive);
    Assert.AreEqual (2, Controller.EditableRowIndex.Value);
    
    SetValues ((EditableRow) Controller.Controls[0], "New Value C", "300");
    Controller.EndRowEditMode (false, Columns);

    CheckEvents (expectedEvents, ActualEvents);
    
    Assert.IsFalse (Controller.IsRowEditModeActive);
    Assert.IsNull (Controller.EditableRowIndex);

    CheckValues (Values[2], "C", 3);
  }

  [Test]
  public void EndRowEditModeWithNewRowAndSaveChangesWithValidValues ()
  {
    StringCollection expectedEvents = new StringCollection();
    expectedEvents.Add (FormatChangesSavingEventMessage (5, NewValues[0]));
    expectedEvents.Add (FormatChangesSavedEventMessage (5, NewValues[0]));

    Invoker.InitRecursive();    
    Controller.AddAndEditRow (NewValues[0], Columns, Columns);
     
    Assert.IsTrue (Controller.IsRowEditModeActive);
    Assert.AreEqual (5, Controller.EditableRowIndex.Value);

    SetValues ((EditableRow) Controller.Controls[0], "New Value F", "600");
    Controller.EndRowEditMode (true, Columns);

    CheckEvents (expectedEvents, ActualEvents);
    
    Assert.IsFalse (Controller.IsRowEditModeActive);
    Assert.IsNull (Controller.EditableRowIndex);

    Assert.AreEqual (6, Controller.OwnerControl.Value.Count);
    CheckValues (NewValues[0], "New Value F", 600);
  }

  [Test]
  public void EndRowEditModeWithNewRowAndDiscardChangesWithValidValues ()
  {
    StringCollection expectedEvents = new StringCollection();
    expectedEvents.Add (FormatChangesCancelingEventMessage (5, NewValues[0]));
    expectedEvents.Add (FormatChangesCanceledEventMessage (-1, NewValues[0]));

    Invoker.InitRecursive();
    Controller.AddAndEditRow (NewValues[0], Columns, Columns);
     
    Assert.IsTrue (Controller.IsRowEditModeActive);
    Assert.AreEqual (5, Controller.EditableRowIndex.Value);

    SetValues ((EditableRow) Controller.Controls[0], "New Value F", "600");
    Controller.EndRowEditMode (false, Columns);

    CheckEvents (expectedEvents, ActualEvents);
    
    Assert.IsFalse (Controller.IsRowEditModeActive);
    Assert.IsNull (Controller.EditableRowIndex);

    Assert.AreEqual (5, Controller.OwnerControl.Value.Count);
    CheckValues (NewValues[0], "F", 6);
  }

  [Test]
  public void EndRowEditModeAndSaveChangesWithInvalidValues ()
  {
    StringCollection expectedEvents = new StringCollection();
    expectedEvents.Add (FormatChangesSavingEventMessage (2, Values[2]));

    Invoker.InitRecursive();
    Controller.SwitchRowIntoEditMode (2, Columns, Columns);
     
    Assert.IsTrue (Controller.IsRowEditModeActive);
    Assert.AreEqual (2, Controller.EditableRowIndex.Value);
    
    SetValues ((EditableRow) Controller.Controls[0], "New Value C", "");
    Controller.EndRowEditMode (true, Columns);

    CheckEvents (expectedEvents, ActualEvents);
    
    Assert.IsTrue(Controller.IsRowEditModeActive);
    Assert.AreEqual (2, Controller.EditableRowIndex.Value);

    CheckValues (Values[2], "C", 3);
  }

  [Test]
  public void EndRowEditModeAndDiscardChangesWithInvalidValues ()
  {
    StringCollection expectedEvents = new StringCollection();
    expectedEvents.Add (FormatChangesCancelingEventMessage (2, Values[2]));
    expectedEvents.Add (FormatChangesCanceledEventMessage (2, Values[2]));

    Invoker.InitRecursive();
    Controller.SwitchRowIntoEditMode (2, Columns, Columns);
     
    Assert.IsTrue (Controller.IsRowEditModeActive);
    Assert.AreEqual (2, Controller.EditableRowIndex.Value);
    
    SetValues ((EditableRow) Controller.Controls[0], "New Value C", "");
    Controller.EndRowEditMode (false, Columns);

    CheckEvents (expectedEvents, ActualEvents);
    
    Assert.IsFalse (Controller.IsRowEditModeActive);
    Assert.IsNull (Controller.EditableRowIndex);

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
    Controller.OwnerControl.LoadUnboundValue (null, false);
     
    Assert.IsFalse (Controller.IsRowEditModeActive);
    
    Controller.EndRowEditMode (true, Columns);

    Assert.IsFalse (Controller.IsRowEditModeActive);
    Assert.AreEqual (0, ActualEvents.Count);
  }


  [Test]
  public void EnsureEditModeRestored ()
  {
    Assert.IsFalse (Controller.IsRowEditModeActive);
    ControllerInvoker.LoadControlState (
        CreateControlState (null, false, 2, false, new EditableRowIDProvider (Controller.ID + "_Row{0}")));
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
    ControllerInvoker.LoadControlState (
        CreateControlState (null, false, 6, false, new EditableRowIDProvider (Controller.ID + "_Row{0}")));
    Assert.IsTrue (Controller.IsRowEditModeActive);
 
    Controller.EnsureEditModeRestored (Columns);
  }

  [Test]
  [ExpectedException (typeof (InvalidOperationException), 
      ExpectedMessage = "Cannot restore edit mode: The BocList 'BocList' does not have a Value.")]
  public void EnsureEditModeRestoredWithValueNull ()
  {
    Assert.IsFalse (Controller.IsRowEditModeActive);
    ControllerInvoker.LoadControlState (
        CreateControlState (null, false, 6, false, new EditableRowIDProvider (Controller.ID + "_Row{0}")));
    Assert.IsTrue (Controller.IsRowEditModeActive);
    Controller.OwnerControl.LoadUnboundValue (null, false);

    Controller.EnsureEditModeRestored (Columns);
  }


  [Test]
  public void AddRows ()
  {
    Invoker.InitRecursive();
    Controller.SwitchRowIntoEditMode (2, Columns, Columns);
     
    Assert.IsTrue (Controller.IsRowEditModeActive);
    Assert.AreEqual (2, Controller.EditableRowIndex.Value);
    Assert.AreEqual (5, Controller.OwnerControl.Value.Count);

    Controller.AddRows (NewValues, Columns, Columns);
    
    Assert.AreEqual (7, Controller.OwnerControl.Value.Count);
    Assert.AreSame (NewValues[0], Controller.OwnerControl.Value[5]);
    Assert.AreSame (NewValues[1], Controller.OwnerControl.Value[6]);
    Assert.IsTrue (Controller.IsRowEditModeActive);
    Assert.AreEqual (2, Controller.EditableRowIndex.Value);

    Assert.AreEqual (0, ActualEvents.Count);
  }

  [Test]
  public void AddRow ()
  {
    Invoker.InitRecursive();
    Controller.SwitchRowIntoEditMode (2, Columns, Columns);
     
    Assert.IsTrue (Controller.IsRowEditModeActive);
    Assert.AreEqual (2, Controller.EditableRowIndex.Value);
    Assert.AreEqual (5, Controller.OwnerControl.Value.Count);

    Assert.AreEqual (5, Controller.AddRow (NewValues[0], Columns, Columns));
    
    Assert.AreEqual (6, Controller.OwnerControl.Value.Count);
    Assert.AreSame (NewValues[0], Controller.OwnerControl.Value[5]);
    Assert.IsTrue (Controller.IsRowEditModeActive);
    Assert.AreEqual (2, Controller.EditableRowIndex.Value);

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
    Assert.AreEqual (2, Controller.EditableRowIndex.Value);
    Assert.AreEqual (5, Controller.OwnerControl.Value.Count);

    Controller.RemoveRows (new IBusinessObject[] {Values[2]});
  }

  [Test]
  [ExpectedException (typeof (InvalidOperationException),
      ExpectedMessage = "Cannot remove a row while the BocList 'BocList' is in row edit mode. Call EndEditMode() before removing the row.")]
  public void RemoveRow ()
  {
    Invoker.InitRecursive();
    Controller.SwitchRowIntoEditMode (2, Columns, Columns);
     
    Assert.IsTrue (Controller.IsRowEditModeActive);
    Assert.AreEqual (2, Controller.EditableRowIndex.Value);
    Assert.AreEqual (5, Controller.OwnerControl.Value.Count);

    Controller.RemoveRow (Values[2]);
  }

  
  [Test]
  public void CreateValidators ()
  {
    IResourceManager resourceManager = (IResourceManager) NullResourceManager.Instance;

    Invoker.InitRecursive();
    Controller.SwitchRowIntoEditMode (2, Columns, Columns);
    
    Assert.IsTrue (Controller.IsRowEditModeActive);
    Assert.AreEqual (2, Controller.EditableRowIndex.Value);

    BaseValidator[] validators = Controller.CreateValidators (resourceManager);
    
    Assert.IsNotNull (validators);
    Assert.AreEqual (1, validators.Length);
    Assert.IsInstanceOfType (typeof (EditModeValidator), validators[0]);
    Assert.AreEqual (BocList.ID, validators[0].ControlToValidate);
    Assert.AreEqual (resourceManager.GetString (Remotion.ObjectBinding.Web.UI.Controls.BocList.ResourceIdentifier.RowEditModeErrorMessage), validators[0].ErrorMessage);
  }

  [Test]
  public void CreateValidatorsWithErrorMessageFromOwnerControl ()
  {
    Invoker.InitRecursive();
    Controller.SwitchRowIntoEditMode (2, Columns, Columns);
    Controller.OwnerControl.ErrorMessage = "Foo Bar";
    
    Assert.IsTrue (Controller.IsRowEditModeActive);
    Assert.AreEqual (2, Controller.EditableRowIndex.Value);

    BaseValidator[] validators = Controller.CreateValidators (NullResourceManager.Instance);
    
    Assert.IsNotNull (validators);
    Assert.AreEqual (1, validators.Length);
    Assert.IsInstanceOfType (typeof (EditModeValidator), validators[0]);
    Assert.AreEqual (BocList.ID, validators[0].ControlToValidate);
    Assert.AreEqual ("Foo Bar", validators[0].ErrorMessage);
  }


  [Test]
  public void ValidateWithValidValues ()
  {
    Invoker.InitRecursive();
    Controller.SwitchRowIntoEditMode (2, Columns, Columns);
    
    Assert.IsTrue (Controller.IsRowEditModeActive);
    Assert.AreEqual (2, Controller.EditableRowIndex.Value);

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
    
    Assert.AreEqual (string.Empty, stringValueField.TextBox.Text);
    Assert.AreEqual (string.Empty, int32ValueField.TextBox.Text);

    Controller.PrepareValidation();
    
    Assert.AreEqual (stringValueField.Text, stringValueField.TextBox.Text);
    Assert.AreEqual (int32ValueField.Text, int32ValueField.TextBox.Text);
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

    string id = "NamingContainer_Controller_Row{0}_{1}_Boc_TextBox";
    string[] trackedIDs = new string[2];
    trackedIDs[0] = string.Format (id, 0, 0);
    trackedIDs[1] = string.Format (id, 0, 1);

    Assert.AreEqual (trackedIDs, Controller.GetTrackedClientIDs());
  }


  [Test]
  public void SaveAndLoadControlState ()
  {
    Invoker.InitRecursive();
    Controller.SwitchRowIntoEditMode (2, Columns, Columns);
    Assert.IsTrue (Controller.IsRowEditModeActive);
    Assert.AreEqual (2, Controller.EditableRowIndex.Value);

    object viewState = ControllerInvoker.SaveControlState();
    Assert.IsNotNull (viewState);

    Controller.EndRowEditMode (false, Columns);
    Assert.IsFalse (Controller.IsRowEditModeActive);
    Assert.IsNull (Controller.EditableRowIndex);

    ControllerInvoker.LoadControlState (viewState);
    Assert.IsTrue (Controller.IsRowEditModeActive);
    Assert.AreEqual (2, Controller.EditableRowIndex.Value);
  }
}

}

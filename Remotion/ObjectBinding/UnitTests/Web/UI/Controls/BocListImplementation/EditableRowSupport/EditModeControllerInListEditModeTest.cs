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
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web.UI.WebControls;
using NUnit.Framework;
using Remotion.Globalization;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.EditableRowSupport;

namespace Remotion.ObjectBinding.UnitTests.Web.UI.Controls.BocListImplementation.EditableRowSupport
{
  [TestFixture]
  public class EditModeControllerInListEditModeTest : EditModeControllerTestBase
  {
    [Test]
    public void SwitchListIntoEditMode ()
    {
      Invoker.InitRecursive();
      Controller.SwitchListIntoEditMode (Columns, Columns);
     
      Assert.IsTrue (Controller.IsListEditModeActive);

      Assert.AreEqual (5, Controller.Controls.Count);
      string idFormat = "Controller_Row_{0}";
      Assert.AreEqual (string.Format (idFormat, 0), Controller.Controls[0].ID);
      Assert.AreEqual (string.Format (idFormat, 1), Controller.Controls[1].ID);
      Assert.AreEqual (string.Format (idFormat, 2), Controller.Controls[2].ID);
      Assert.AreEqual (string.Format (idFormat, 3), Controller.Controls[3].ID);
      Assert.AreEqual (string.Format (idFormat, 4), Controller.Controls[4].ID);

      Assert.AreEqual (0, ActualEvents.Count);
    }

    [Test]
    public void SwitchListIntoEditModeWithValueEmpty ()
    {
      Invoker.InitRecursive();
      EditModeHost.Value = new IBusinessObject[0];
      Controller.SwitchListIntoEditMode (Columns, Columns);
     
      Assert.IsTrue (Controller.IsListEditModeActive);
      Assert.AreEqual (0, Controller.Controls.Count);

      Assert.AreEqual (0, ActualEvents.Count);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), 
        ExpectedMessage = "Cannot initialize list edit mode: The BocList 'BocList' does not have a Value.")]
    public void SwitchListIntoEditModeWithValueNull ()
    {
      Invoker.InitRecursive();
      EditModeHost.Value = null;
      Controller.SwitchListIntoEditMode (Columns, Columns);
    }

    [Test]
    public void SwitchListIntoEditModeWhileListEditModeIsActiveWithValidValues ()
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

      Controller.SwitchListIntoEditMode (Columns, Columns);
     
      CheckEvents (expectedEvents, ActualEvents);

      Assert.IsTrue (Controller.IsListEditModeActive);
    
      CheckValues (Values[0], "New Value A", 100);
      CheckValues (Values[1], "New Value B", 200);
      CheckValues (Values[2], "New Value C", 300);
      CheckValues (Values[3], "New Value D", 400);
      CheckValues (Values[4], "New Value E", 500);
    }

    [Test]
    public void SwitchListIntoEditModeWhileListEditModeIsActiveWithInvalidValues ()
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

      Controller.SwitchListIntoEditMode (Columns, Columns);
     
      CheckEvents (expectedEvents, ActualEvents);

      Assert.IsTrue (Controller.IsListEditModeActive);
    
      CheckValues (Values[0], "A", 1);
      CheckValues (Values[1], "B", 2);
      CheckValues (Values[2], "C", 3);
      CheckValues (Values[3], "D", 4);
      CheckValues (Values[4], "E", 5);
    }

    [Test]
    public void SwitchListIntoEditModeWhileRowEditModeIsActiveWithValidValues ()
    {
      StringCollection expectedEvents = new StringCollection();
      expectedEvents.Add (FormatChangesSavingEventMessage (2, Values[2]));
      expectedEvents.Add (FormatValidateEditableRows());
      expectedEvents.Add (FormatChangesSavedEventMessage (2, Values[2]));
      expectedEvents.Add (FormatEndRowEditModeCleanUp (2));

      Invoker.InitRecursive();
      Controller.SwitchRowIntoEditMode (2, Columns, Columns);
     
      Assert.IsTrue (Controller.IsRowEditModeActive);
      Assert.AreEqual (2, Controller.EditableRowIndex.Value);
    
      SetValues ((EditableRow) Controller.Controls[0], "New Value C", "300");

      Controller.SwitchListIntoEditMode (Columns, Columns);
     
      CheckEvents (expectedEvents, ActualEvents);

      Assert.IsTrue (Controller.IsListEditModeActive);
    
      CheckValues (Values[2], "New Value C", 300);
    }

    [Test]
    public void SwitchListIntoEditModeWhileRowEditModeIsActiveWithInvalidValues ()
    {
      StringCollection expectedEvents = new StringCollection();
      expectedEvents.Add (FormatChangesSavingEventMessage (2, Values[2]));
      expectedEvents.Add (FormatValidateEditableRows());

      Invoker.InitRecursive();
      Controller.SwitchRowIntoEditMode (2, Columns, Columns);
     
      Assert.IsTrue (Controller.IsRowEditModeActive);
      Assert.AreEqual (2, Controller.EditableRowIndex.Value);
    
      SetValues ((EditableRow) Controller.Controls[0], "New Value C", "");

      Controller.SwitchListIntoEditMode (Columns, Columns);
     
      CheckEvents (expectedEvents, ActualEvents);

      Assert.IsTrue (Controller.IsRowEditModeActive);
      Assert.AreEqual (2, Controller.EditableRowIndex.Value);
    
      CheckValues (Values[2], "C", 3);
    }
    

    [Test]
    public void EndListEditModeAndSaveChangesWithValidValues ()
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
      Controller.EndListEditMode (true, Columns);

      CheckEvents (expectedEvents, ActualEvents);
    
      Assert.IsFalse (Controller.IsListEditModeActive);

      CheckValues (Values[0], "New Value A", 100);
      CheckValues (Values[1], "New Value B", 200);
      CheckValues (Values[2], "New Value C", 300);
      CheckValues (Values[3], "New Value D", 400);
      CheckValues (Values[4], "New Value E", 500);
    }

    [Test]
    public void EndListEditModeAndDiscardChangesWithValidValues ()
    {
      StringCollection expectedEvents = new StringCollection();
      expectedEvents.Add (FormatChangesCancelingEventMessage (0, Values[0]));
      expectedEvents.Add (FormatChangesCancelingEventMessage (1, Values[1]));
      expectedEvents.Add (FormatChangesCancelingEventMessage (2, Values[2]));
      expectedEvents.Add (FormatChangesCancelingEventMessage (3, Values[3]));
      expectedEvents.Add (FormatChangesCancelingEventMessage (4, Values[4]));
      expectedEvents.Add (FormatChangesCanceledEventMessage (0, Values[0]));
      expectedEvents.Add (FormatChangesCanceledEventMessage (1, Values[1]));
      expectedEvents.Add (FormatChangesCanceledEventMessage (2, Values[2]));
      expectedEvents.Add (FormatChangesCanceledEventMessage (3, Values[3]));
      expectedEvents.Add (FormatChangesCanceledEventMessage (4, Values[4]));
      expectedEvents.Add (FormatEndListEditModeCleanUp());

      Invoker.InitRecursive();
      Controller.SwitchListIntoEditMode (Columns, Columns);
     
      Assert.IsTrue (Controller.IsListEditModeActive);
    
      SetValues ((EditableRow) Controller.Controls[0], "New Value A", "100");
      SetValues ((EditableRow) Controller.Controls[1], "New Value B", "200");
      SetValues ((EditableRow) Controller.Controls[2], "New Value C", "300");
      SetValues ((EditableRow) Controller.Controls[3], "New Value D", "400");
      SetValues ((EditableRow) Controller.Controls[4], "New Value E", "500");
      Controller.EndListEditMode (false, Columns);

      CheckEvents (expectedEvents, ActualEvents);
    
      Assert.IsFalse (Controller.IsListEditModeActive);

      CheckValues (Values[0], "A", 1);
      CheckValues (Values[1], "B", 2);
      CheckValues (Values[2], "C", 3);
      CheckValues (Values[3], "D", 4);
      CheckValues (Values[4], "E", 5);
    }

    [Test]
    public void EndListEditModeAndSaveChangesWithInvalidValues ()
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
      Controller.EndListEditMode (true, Columns);

      CheckEvents (expectedEvents, ActualEvents);
    
      Assert.IsTrue (Controller.IsListEditModeActive);

      CheckValues (Values[0], "A", 1);
      CheckValues (Values[1], "B", 2);
      CheckValues (Values[2], "C", 3);
      CheckValues (Values[3], "D", 4);
      CheckValues (Values[4], "E", 5);
    }

    [Test]
    public void EndListEditModeAndDiscardChangesWithInvalidValues ()
    {
      StringCollection expectedEvents = new StringCollection();
      expectedEvents.Add (FormatChangesCancelingEventMessage (0, Values[0]));
      expectedEvents.Add (FormatChangesCancelingEventMessage (1, Values[1]));
      expectedEvents.Add (FormatChangesCancelingEventMessage (2, Values[2]));
      expectedEvents.Add (FormatChangesCancelingEventMessage (3, Values[3]));
      expectedEvents.Add (FormatChangesCancelingEventMessage (4, Values[4]));
      expectedEvents.Add (FormatChangesCanceledEventMessage (0, Values[0]));
      expectedEvents.Add (FormatChangesCanceledEventMessage (1, Values[1]));
      expectedEvents.Add (FormatChangesCanceledEventMessage (2, Values[2]));
      expectedEvents.Add (FormatChangesCanceledEventMessage (3, Values[3]));
      expectedEvents.Add (FormatChangesCanceledEventMessage (4, Values[4]));
      expectedEvents.Add (FormatEndListEditModeCleanUp());

      Invoker.InitRecursive();
      Controller.SwitchListIntoEditMode (Columns, Columns);
     
      Assert.IsTrue (Controller.IsListEditModeActive);
    
      SetValues ((EditableRow) Controller.Controls[0], "New Value A", "");
      SetValues ((EditableRow) Controller.Controls[1], "New Value B", "");
      SetValues ((EditableRow) Controller.Controls[2], "New Value C", "");
      SetValues ((EditableRow) Controller.Controls[3], "New Value D", "");
      SetValues ((EditableRow) Controller.Controls[4], "New Value E", "");
      Controller.EndListEditMode (false, Columns);

      CheckEvents (expectedEvents, ActualEvents);
    
      Assert.IsFalse (Controller.IsListEditModeActive);

      CheckValues (Values[0], "A", 1);
      CheckValues (Values[1], "B", 2);
      CheckValues (Values[2], "C", 3);
      CheckValues (Values[3], "D", 4);
      CheckValues (Values[4], "E", 5);
    }

    [Test]
    public void EndListEditModeWithoutBeingActive ()
    {
      Invoker.InitRecursive();
     
      Assert.IsFalse (Controller.IsListEditModeActive);
    
      Controller.EndListEditMode (true, Columns);

      Assert.IsFalse (Controller.IsListEditModeActive);
      Assert.AreEqual (0, ActualEvents.Count);
    }

    [Test]
    public void EndListEditModeWithoutBeingActiveAndValueNull ()
    {
      Invoker.InitRecursive();
      EditModeHost.Value = null;
     
      Assert.IsFalse (Controller.IsListEditModeActive);
    
      Controller.EndListEditMode (true, Columns);

      Assert.IsFalse (Controller.IsListEditModeActive);
      Assert.AreEqual (0, ActualEvents.Count);
    }


    [Test]
    public void EnsureEditModeRestored ()
    {
      string idFormat = "Controller_Row_{0}";

      Assert.IsFalse (Controller.IsListEditModeActive);
      ControllerInvoker.LoadControlState (CreateControlState (null, true, null, false));
      Assert.IsTrue (Controller.IsListEditModeActive);
    
      Controller.EnsureEditModeRestored (Columns);
      Assert.IsTrue (Controller.IsListEditModeActive);
  
      Assert.AreEqual (string.Format (idFormat, 0), Controller.Controls[0].ID);
      Assert.AreEqual (string.Format (idFormat, 1), Controller.Controls[1].ID);
      Assert.AreEqual (string.Format (idFormat, 2), Controller.Controls[2].ID);
      Assert.AreEqual (string.Format (idFormat, 3), Controller.Controls[3].ID);
      Assert.AreEqual (string.Format (idFormat, 4), Controller.Controls[4].ID);
    }

    [Test]
    public void AddRow ()
    {
      Invoker.InitRecursive();
      Controller.SwitchListIntoEditMode (Columns, Columns);
     
      Assert.IsTrue (Controller.IsListEditModeActive);
      Assert.AreEqual (5, EditModeHost.Value.Count);

      Assert.AreEqual (5, Controller.AddRow (NewValues[0], Columns, Columns));
    
      Assert.AreEqual (6, EditModeHost.Value.Count);
      Assert.AreSame (NewValues[0], EditModeHost.Value[5]);

      Assert.IsTrue (Controller.IsListEditModeActive);
      Assert.AreEqual (6, Controller.Controls.Count);
      string idFormat = "Controller_Row_{0}";
      Assert.AreEqual (string.Format (idFormat, 0), Controller.Controls[0].ID);
      Assert.AreEqual (string.Format (idFormat, 1), Controller.Controls[1].ID);
      Assert.AreEqual (string.Format (idFormat, 2), Controller.Controls[2].ID);
      Assert.AreEqual (string.Format (idFormat, 3), Controller.Controls[3].ID);
      Assert.AreEqual (string.Format (idFormat, 4), Controller.Controls[4].ID);
      Assert.AreEqual (string.Format (idFormat, 5), Controller.Controls[5].ID);

      Assert.AreEqual (0, ActualEvents.Count);
    }

    [Test]
    public void AddRow_CallsEditModeHost ()
    {
      Invoker.InitRecursive();
      Controller.SwitchListIntoEditMode (Columns, Columns);

      IBusinessObject[] addedRows = null;
      EditModeHost.NotifyAddRows = objects =>
      {
        addedRows = objects;
        return new BocListRow[0];
      };
      Controller.AddRow (NewValues[0], Columns, Columns);

      CollectionAssert.AreEquivalent (new[] { NewValues[0] }, addedRows);
    }


    [Test]
    public void AddRows ()
    {
      Invoker.InitRecursive();
      Controller.SwitchListIntoEditMode (Columns, Columns);
     
      Assert.IsTrue (Controller.IsListEditModeActive);
      Assert.AreEqual (5, EditModeHost.Value.Count);

      Controller.AddRows (NewValues, Columns, Columns);
    
      Assert.AreEqual (7, EditModeHost.Value.Count);
      Assert.AreSame (NewValues[0], EditModeHost.Value[5]);
      Assert.AreSame (NewValues[1], EditModeHost.Value[6]);

      Assert.IsTrue (Controller.IsListEditModeActive);
      Assert.AreEqual (7, Controller.Controls.Count);
      string idFormat = "Controller_Row_{0}";
      Assert.AreEqual (string.Format (idFormat, 0), Controller.Controls[0].ID);
      Assert.AreEqual (string.Format (idFormat, 1), Controller.Controls[1].ID);
      Assert.AreEqual (string.Format (idFormat, 2), Controller.Controls[2].ID);
      Assert.AreEqual (string.Format (idFormat, 3), Controller.Controls[3].ID);
      Assert.AreEqual (string.Format (idFormat, 4), Controller.Controls[4].ID);
      Assert.AreEqual (string.Format (idFormat, 5), Controller.Controls[5].ID);
      Assert.AreEqual (string.Format (idFormat, 6), Controller.Controls[6].ID);

      Assert.AreEqual (0, ActualEvents.Count);
    }

    [Test]
    public void AddRows_CallsEditModeHost ()
    {
      Invoker.InitRecursive();
      Controller.SwitchListIntoEditMode (Columns, Columns);

      IBusinessObject[] addedRows = null;
      EditModeHost.NotifyAddRows = objects =>
      {
        addedRows = objects;
        return new BocListRow[0];
      };
      Controller.AddRows (NewValues, Columns, Columns);

      Assert.AreSame (NewValues, addedRows);
    }


    [Test]
    public void RemoveRow ()
    {
      Invoker.InitRecursive();
      Controller.SwitchListIntoEditMode (Columns, Columns);
     
      Assert.IsTrue (Controller.IsListEditModeActive);
      Assert.AreEqual (5, EditModeHost.Value.Count);

      Controller.RemoveRow (Values[2]);
  
      Assert.AreEqual (4, EditModeHost.Value.Count);
      Assert.AreSame (Values[0], EditModeHost.Value[0]);
      Assert.AreSame (Values[1], EditModeHost.Value[1]);
      Assert.AreSame (Values[3], EditModeHost.Value[2]);
      Assert.AreSame (Values[4], EditModeHost.Value[3]);

      Assert.IsTrue (Controller.IsListEditModeActive);
      Assert.AreEqual (4, Controller.Controls.Count);
      string idFormat = "Controller_Row_{0}";
      Assert.AreEqual (string.Format (idFormat, 0), Controller.Controls[0].ID);
      Assert.AreEqual (string.Format (idFormat, 1), Controller.Controls[1].ID);
      Assert.AreEqual (string.Format (idFormat, 3), Controller.Controls[2].ID);
      Assert.AreEqual (string.Format (idFormat, 4), Controller.Controls[3].ID);

      Assert.AreEqual (0, ActualEvents.Count);
    }

    [Test]
    public void RemoveRow_CallsEditModeHost ()
    {
      Invoker.InitRecursive();
      Controller.SwitchListIntoEditMode (Columns, Columns);

      var businessObject = Values[2];
      IBusinessObject[] removedRows = null;
      EditModeHost.NotifyRemoveRows = objects =>
      {
        removedRows = objects;
        return new BocListRow[0];
      };
      Controller.RemoveRow (businessObject);

      CollectionAssert.AreEqual (new[] { businessObject }, removedRows);
    }

    [Test]
    public void RemoveRows ()
    {
      Invoker.InitRecursive();
      Controller.SwitchListIntoEditMode (Columns, Columns);

      Assert.IsTrue (Controller.IsListEditModeActive);
      Assert.AreEqual (5, EditModeHost.Value.Count);

      Controller.RemoveRows (new[] { Values[2] });
  
      Assert.AreEqual (4, EditModeHost.Value.Count);
      Assert.AreSame (Values[0], EditModeHost.Value[0]);
      Assert.AreSame (Values[1], EditModeHost.Value[1]);
      Assert.AreSame (Values[3], EditModeHost.Value[2]);
      Assert.AreSame (Values[4], EditModeHost.Value[3]);

      Assert.IsTrue (Controller.IsListEditModeActive);
      Assert.AreEqual (4, Controller.Controls.Count);
      string idFormat = "Controller_Row_{0}";
      Assert.AreEqual (string.Format (idFormat, 0), Controller.Controls[0].ID);
      Assert.AreEqual (string.Format (idFormat, 1), Controller.Controls[1].ID);
      Assert.AreEqual (string.Format (idFormat, 3), Controller.Controls[2].ID);
      Assert.AreEqual (string.Format (idFormat, 4), Controller.Controls[3].ID);

      Assert.AreEqual (0, ActualEvents.Count);
    }

    [Test]
    public void RemoveRows_CallsEditModeHost ()
    {
      Invoker.InitRecursive();
      Controller.SwitchListIntoEditMode (Columns, Columns);

      var businessObjects = new[] { Values[2] };
      IBusinessObject[] removedRows = null;
      EditModeHost.NotifyRemoveRows = objects =>
      {
        removedRows = objects;
        return new BocListRow[0];
      };
      Controller.RemoveRows (businessObjects);

      Assert.AreSame (businessObjects, removedRows);
    }

    [Test]
    public void CreateValidators ()
    {
      IResourceManager resourceManager = NullResourceManager.Instance;

      Invoker.InitRecursive();
      Controller.SwitchListIntoEditMode (Columns, Columns);
    
      Assert.IsTrue (Controller.IsListEditModeActive);

      BaseValidator[] validators = Controller.CreateValidators (resourceManager);
    
      Assert.IsNotNull (validators);
      Assert.AreEqual (1, validators.Length);
      Assert.IsInstanceOf (typeof (EditModeValidator), validators[0]);
      Assert.AreEqual (EditModeHost.ID, validators[0].ControlToValidate);
      Assert.AreEqual (resourceManager.GetString (Remotion.ObjectBinding.Web.UI.Controls.BocList.ResourceIdentifier.ListEditModeErrorMessage), validators[0].ErrorMessage);
    }

    [Test]
    public void CreateValidatorsWithErrorMessageFromOwnerControl ()
    {
      Invoker.InitRecursive();
      Controller.SwitchListIntoEditMode (Columns, Columns);
      EditModeHost.ErrorMessage = "Foo Bar";

      Assert.IsTrue (Controller.IsListEditModeActive);

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
      Controller.SwitchListIntoEditMode (Columns, Columns);

      SetValues ((EditableRow) Controller.Controls[0], "New Value A", "100");
      SetValues ((EditableRow) Controller.Controls[1], "New Value B", "200");
      SetValues ((EditableRow) Controller.Controls[2], "New Value C", "300");
      SetValues ((EditableRow) Controller.Controls[3], "New Value D", "400");
      SetValues ((EditableRow) Controller.Controls[4], "New Value E", "500");

      Assert.IsTrue (Controller.Validate());
    }

    [Test]
    public void ValidateWithInvalidValues ()
    {
      Invoker.InitRecursive();
      Controller.SwitchListIntoEditMode (Columns, Columns);

      SetValues ((EditableRow) Controller.Controls[0], "New Value A", "");
      SetValues ((EditableRow) Controller.Controls[1], "New Value B", "");
      SetValues ((EditableRow) Controller.Controls[2], "New Value C", "");
      SetValues ((EditableRow) Controller.Controls[3], "New Value D", "");
      SetValues ((EditableRow) Controller.Controls[4], "New Value E", "");

      Assert.IsFalse (Controller.Validate());
    }

  
    [Test]
    public void PrepareValidation ()
    {
      Invoker.InitRecursive();
      Controller.SwitchRowIntoEditMode (2, Columns, Columns);

      for (int i = 0; i < Controller.Controls.Count; i++)
      {
        EditableRow editableRow = (EditableRow) Controller.Controls[i];

        BocTextValue stringValueField = (BocTextValue) editableRow.GetEditControl (0);
        BocTextValue int32ValueField = (BocTextValue) editableRow.GetEditControl (1);
      
        Controller.PrepareValidation();
      
        Assert.AreEqual (stringValueField.Text, stringValueField.Text, "Row {0}", i);
        Assert.AreEqual (int32ValueField.Text, int32ValueField.Text, "Row {0}", i);
      }
    }


    [Test]
    public void IsRequired ()
    {
      Invoker.InitRecursive();
      Controller.SwitchListIntoEditMode (Columns, Columns);
   
      Assert.IsTrue (Controller.IsListEditModeActive);

      Assert.IsFalse (Controller.IsRequired (0));
      Assert.IsTrue (Controller.IsRequired (1));
    }

    [Test]
    public void IsDirty ()
    {
      Invoker.InitRecursive();
      Controller.SwitchListIntoEditMode (Columns, Columns);

      EditableRow row = (EditableRow) Controller.Controls[2];
      Remotion.ObjectBinding.Web.UI.Controls.BocTextValue stringValueField = 
          (Remotion.ObjectBinding.Web.UI.Controls.BocTextValue) row.GetEditControl (0);
      stringValueField.Value = "New Value";

      Assert.IsTrue (Controller.IsDirty());
    }

    [Test]
    public void GetTrackedIDs ()
    {
      Invoker.InitRecursive();
      Controller.SwitchListIntoEditMode (Columns, Columns);

      string id = "NamingContainer_Controller_Row_{0}_{1}_Boc_TextBox";
      string[] trackedIDs = new string[10];
      for (int i = 0; i < 5; i++)
      {
        trackedIDs[2 * i] = string.Format (id, i, 0);
        trackedIDs[2 * i + 1] = string.Format (id, i, 1);
      }

      Assert.AreEqual (trackedIDs, Controller.GetTrackedClientIDs());
    }


    [Test]
    public void SaveAndLoadControlState ()
    {
      Invoker.InitRecursive();
      Controller.SwitchListIntoEditMode (Columns, Columns);
      Assert.IsTrue (Controller.IsListEditModeActive);

      object viewState = ControllerInvoker.SaveControlState();
      Assert.IsNotNull (viewState);

      Controller.EndListEditMode (false, Columns);
      Assert.IsFalse (Controller.IsListEditModeActive);

      ControllerInvoker.LoadControlState (viewState);
      Assert.IsTrue (Controller.IsListEditModeActive);
    }

    [Test]
    public void SaveAndLoadControlStateAfterRemovingSingleRow ()
    {
      Invoker.InitRecursive();
      Controller.SwitchListIntoEditMode (Columns, Columns);
      Controller.RemoveRow (Values[2]);
      Assert.IsTrue (Controller.IsListEditModeActive);

      object viewState = ControllerInvoker.SaveControlState();
      Assert.IsNotNull (viewState);
      Assert.IsTrue (viewState is Object[]);
      object[] values = (object[]) viewState;
      Assert.AreEqual (4, values.Length);

      Controller.EndListEditMode (false, Columns);
      Assert.IsFalse (Controller.IsListEditModeActive);

      ControllerInvoker.LoadControlState (viewState);
      Assert.IsTrue (Controller.IsListEditModeActive);
    }

    [Test]
    public void SaveAndLoadControlStateAfterRemovingMultipleRows ()
    {
      Invoker.InitRecursive();
      Controller.SwitchListIntoEditMode (Columns, Columns);
      Controller.RemoveRows (new [] {Values[2], Values[3]});
      Assert.IsTrue (Controller.IsListEditModeActive);

      object viewState = ControllerInvoker.SaveControlState();
      Assert.IsNotNull (viewState);
      Assert.IsTrue (viewState is Object[]);
      object[] values = (object[]) viewState;
      Assert.AreEqual (4, values.Length);

      Controller.EndListEditMode (false, Columns);
      Assert.IsFalse (Controller.IsListEditModeActive);

      ControllerInvoker.LoadControlState (viewState);
      Assert.IsTrue (Controller.IsListEditModeActive);
    }
  }
}
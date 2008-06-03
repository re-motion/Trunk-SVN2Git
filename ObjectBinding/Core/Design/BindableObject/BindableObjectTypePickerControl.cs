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
using System.Collections.Generic;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.Design.BindableObject
{
  public partial class BindableObjectTypePickerControl : EditorControlBase
  {
    private static bool s_isGacIncluded;

    private Type _value;
    private TypeTreeViewController _typeTreeViewController;
    private SearchFieldController _searchFieldController;
    private BindableObjectTypeFinder _typeFinder;

    public BindableObjectTypePickerControl (IServiceProvider provider, IWindowsFormsEditorService editorService, BindableObjectTypeFinder typeFinder)
        : base (provider, editorService)
    {
      ArgumentUtility.CheckNotNull ("typeFinder", typeFinder);

      Initialize (typeFinder);
    }

    public BindableObjectTypePickerControl ()
    {
      Initialize (null);
    }

    private void Initialize (BindableObjectTypeFinder typeFinder)
    {
      InitializeComponent ();
      _searchFieldController = new SearchFieldController (SearchField, SearchButton);
      _typeTreeViewController = new TypeTreeViewController (TypeTreeView);
      _typeFinder = typeFinder;
    }

    public override object Value
    {
      get { return _value; }
      set { _value = ArgumentUtility.CheckType<Type> ("value", value); }
    }

    protected override void OnLoad (EventArgs e)
    {
      base.OnLoad (e);
      IncludeGacCheckBox.Checked = s_isGacIncluded;
      PopulateTypeTreeView ();
    }

    private void IncludeGacCheckBox_CheckedChanged (object sender, System.EventArgs e)
    {
      s_isGacIncluded = IncludeGacCheckBox.Checked;
      PopulateTypeTreeView ();
    }

    private void TypeTreeView_AfterSelect (object sender, TreeViewEventArgs e)
    {
      HandleSelectionChanged();
    }

    private void PopulateTypeTreeView ()
    {
      Cursor cursorBackUp = Cursor;
      Cursor = Cursors.WaitCursor;
      
      List<Type> types;
      try
      {
        types = _typeFinder.GetTypes (IncludeGacCheckBox.Checked);
      }
      catch (Exception e)
      {
        MessageBox.Show (e.Message, "Error looking up the available types", MessageBoxButtons.OK, MessageBoxIcon.Error);
        Cursor = cursorBackUp;
        EditorService.CloseDropDown ();
        return;
      }
      
      _typeTreeViewController.PopulateTreeNodes (types, _value);
      Cursor = cursorBackUp;
    }

    private void HandleSelectionChanged ()
    {
      Type type = _typeTreeViewController.GetSelectedType();
      if (type != null)
        SelectButton.Enabled = true;
      else
        SelectButton.Enabled = false;
    }

    private void SelectButton_Click (object sender, EventArgs e)
    {
      _value = _typeTreeViewController.GetSelectedType ();

      EditorService.CloseDropDown();
    }

    private void FilterField_TextChanged (object sender, EventArgs e)
    {
    }
  }
}

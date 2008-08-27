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
using System.Text;
using System.Web.UI;
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.UI;
using Remotion.Web.UI.Globalization;

namespace OBWTest.IndividualControlTests
{
  [WebMultiLingualResources ("OBWTest.Globalization.SingleBocTestBasePage")]
  public partial class IndividualControlTestForm : TestBasePage
  {
    private IDataEditControl _dataEditControl;
    private bool _isCurrentObjectSaved = false;

    protected override void RegisterEventHandlers ()
    {
      base.RegisterEventHandlers();

      PostBackButton.Click += new EventHandler (PostBackButton_Click);
      SaveButton.Click += new EventHandler (SaveButton_Click);
      SaveAndRestartButton.Click += new EventHandler (SaveAndRestartButton_Click);
      CancelButton.Click += new EventHandler (CancelButton_Click);
    }

    protected override void OnInit (EventArgs e)
    {
      base.OnInit (e);

      //EnableAbort = false;
      EnableOutOfSequencePostBacks = true;
      ShowAbortConfirmation = ShowAbortConfirmation.OnlyIfDirty;
    }

    protected override void OnLoad (EventArgs e)
    {
      base.OnLoad (e);

      LoadUserControl();
      PopulateDataSources();
      LoadValues (IsPostBack);
      string test = GetPermanentUrl();

      if (ScriptManager.GetCurrent (this).IsInAsyncPostBack)
        StackUpdatePanel.Update();
    }

    protected override void OnPreRender (EventArgs e)
    {
      base.OnPreRender (e);

      StringBuilder sb = new StringBuilder();
      sb.Append ("<b>Stack:</b><br>");
      for (WxeStep step = CurrentPageStep; step != null; step = step.ParentStep)
        sb.AppendFormat ("{0}<br>", step.ToString());
      Stack.Text = sb.ToString();
    }

    protected override void OnUnload (EventArgs e)
    {
      base.OnUnload (e);

      if (! _isCurrentObjectSaved)
      {
        SaveValues (true);
      }
    }

    protected override void LoadViewState (object savedState)
    {
      base.LoadViewState (savedState);
    }

    private void PostBackButton_Click (object sender, EventArgs e)
    {
    }

    private void SaveButton_Click (object sender, EventArgs e)
    {
      bool isValid = ValidateDataSources();
      if (isValid)
      {
        SaveValues (false);
        _isCurrentObjectSaved = true;
      }
    }

    private void SaveAndRestartButton_Click (object sender, EventArgs e)
    {
      bool isValid = ValidateDataSources();
      if (isValid)
      {
        SaveValues (false);
        _isCurrentObjectSaved = true;
        ExecuteNextStep();
      }
    }

    private void CancelButton_Click (object sender, EventArgs e)
    {
      throw new WxeUserCancelException();
    }

    private void LoadUserControl ()
    {
      _dataEditControl = (IDataEditControl) LoadControl (CurrentFunction.UserControl);
      if (_dataEditControl == null)
        throw new InvalidOperationException (string.Format ("IDataEditControl '{0}' could not be loaded.", CurrentFunction.UserControl));
      _dataEditControl.ID = "DataEditControl";
      UserControlPlaceHolder.Controls.Add ((Control) _dataEditControl);
    }

    private void PopulateDataSources ()
    {
      CurrentObject.BusinessObject = (IBusinessObject) CurrentFunction.Person;
      if (_dataEditControl != null)
        _dataEditControl.BusinessObject = (IBusinessObject) CurrentFunction.Person;
    }

    private void LoadValues (bool interim)
    {
      CurrentObject.LoadValues (interim);
      if (_dataEditControl != null)
        _dataEditControl.LoadValues (interim);
    }

    private void SaveValues (bool interim)
    {
      if (_dataEditControl != null)
        _dataEditControl.SaveValues (interim);
      CurrentObject.SaveValues (interim);
    }

    private bool ValidateDataSources ()
    {
      PrepareValidation();

      bool isValid = _dataEditControl.Validate();
      isValid &= CurrentObject.Validate();

      return isValid;
    }
  }
}

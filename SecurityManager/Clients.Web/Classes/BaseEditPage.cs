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
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.SecurityManager.Clients.Web.WxeFunctions;

namespace Remotion.SecurityManager.Clients.Web.Classes
{
  public abstract class BaseEditPage : BasePage
  {
    // types

    // static members and constants

    // member fields
    private List<DataEditUserControl> _dataEditUserControls = new List<DataEditUserControl> ();

    // construction and disposing

    // methods and properties
    protected new FormFunction CurrentFunction
    {
      get { return (FormFunction) base.CurrentFunction; }
    }

    protected override void OnLoad (EventArgs e)
    {
      base.OnLoad (e);

      foreach (DataEditUserControl control in _dataEditUserControls)
      {
        control.DataSource.BusinessObject = CurrentFunction.CurrentObject;
        control.LoadValues (IsPostBack);
      }
      LoadValues (IsPostBack);
    }

    protected virtual void LoadValues (bool interim)
    {
    }

    protected void SaveButton_Click (object sender, EventArgs e)
    {
      bool isValid = true;

      PrepareValidation ();

      foreach (DataEditUserControl dataEditUserControl in _dataEditUserControls)
        isValid &= dataEditUserControl.Validate ();
      isValid &= ValidatePage ();

      if (isValid)
      {
        foreach (DataEditUserControl dataEditUserControl in _dataEditUserControls)
          dataEditUserControl.SaveValues (false);
        SaveValues (false);

        if (ValidatePagePostSaveValues ())
        {
          CurrentFunction.CurrentTransaction.Commit ();
          ExecuteNextStep ();
        }
        else
        {
          ShowErrors ();
        }
      }
      else
      {
        ShowErrors ();
      }
    }

    protected virtual void ShowErrors ()
    {
    }
   
    protected virtual bool ValidatePage ()
    {
      return true;
    }

    protected virtual void SaveValues (bool interim)
    {
    }

    protected virtual bool ValidatePagePostSaveValues ()
    {
      return true;
    }


    protected override void OnUnload (EventArgs e)
    {
      base.OnUnload (e);

      foreach (DataEditUserControl control in _dataEditUserControls)
        control.SaveValues (true);
    }

    protected void RegisterDataEditUserControl (DataEditUserControl dataEditUserControl)
    {
      _dataEditUserControls.Add (dataEditUserControl);
    }
  }
}

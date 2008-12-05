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
using System.Collections.Generic;
using Remotion.Data.DomainObjects;
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
          ClientTransaction.Current.Commit ();
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

using System;
using System.Web.UI;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.Web.ExecutionEngine;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.TestSite
{
  public partial class ControlTestForm : WxePage
  {
    private IDataEditControl _dataEditControl;

    private new ControlTestFunction CurrentFunction
    {
      get { return (ControlTestFunction) base.CurrentFunction; }
    }

    protected override void OnInit (EventArgs e)
    {
      base.OnInit (e);

      LoadUserControl();
    }

    protected override void OnLoad (EventArgs e)
    {
      base.OnLoad (e);

      PopulateDataSources();
      LoadValues (IsPostBack);
    }

    private void LoadUserControl ()
    {
      var loadControl = LoadControl (CurrentFunction.UserControl);
      _dataEditControl = (IDataEditControl) loadControl;
      if (_dataEditControl == null)
        throw new InvalidOperationException (string.Format ("IDataEditControl '{0}' could not be loaded.", CurrentFunction.UserControl));
      _dataEditControl.ID = "DataEditControl";
      ControlPlaceHolder.Controls.Add ((Control) _dataEditControl);
    }

    private void PopulateDataSources ()
    {
      if (_dataEditControl != null)
        _dataEditControl.BusinessObject = (IBusinessObject) CurrentFunction.Person;
    }

    private void LoadValues (bool interim)
    {
      if (_dataEditControl != null)
        _dataEditControl.LoadValues (interim);
    }

    private bool SaveValues (bool interim)
    {
      if (_dataEditControl == null)
        return true;

      return _dataEditControl.SaveValues (interim);
    }

    private bool ValidateDataSources ()
    {
      PrepareValidation();
      return _dataEditControl.Validate();
    }
  }
}
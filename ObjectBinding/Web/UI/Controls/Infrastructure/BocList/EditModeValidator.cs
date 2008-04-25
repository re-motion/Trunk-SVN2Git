using System;
using System.ComponentModel;
using System.Web.UI.WebControls;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.Web.UI.Controls.Infrastructure.BocList
{

[ToolboxItem (false)]
public class EditModeValidator : CustomValidator
{
  // types

  // static members and constants

  // member fields
  private Controls.BocList _owner;

  // construction and disposing

  public EditModeValidator (Controls.BocList owner)
  {
    _owner = owner;
  }

  // methods and properties

  protected override bool EvaluateIsValid()
  {
    return _owner.ValidateEditableRows();
  }

  protected override bool ControlPropertiesValid()
  {
    string controlToValidate = ControlToValidate;
    if (StringUtility.IsNullOrEmpty (controlToValidate))
      return base.ControlPropertiesValid();
    else
      return NamingContainer.FindControl (controlToValidate) == _owner;
  }
}

}

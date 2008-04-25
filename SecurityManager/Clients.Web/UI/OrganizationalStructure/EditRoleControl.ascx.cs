using System;
using System.Web.UI;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.SecurityManager.Clients.Web.Classes;
using Remotion.SecurityManager.Clients.Web.Globalization.UI.OrganizationalStructure;
using Remotion.SecurityManager.Clients.Web.WxeFunctions.OrganizationalStructure;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.Web.UI.Controls;
using Remotion.Web.UI.Globalization;

namespace Remotion.SecurityManager.Clients.Web.UI.OrganizationalStructure
{
  [WebMultiLingualResources (typeof (EditRoleControlResources))]
  public partial class EditRoleControl : BaseControl
  {
    private BusinessObjectBoundEditableWebControl _groupField;
    private BusinessObjectBoundEditableWebControl _userField;

    public override IBusinessObjectDataSourceControl DataSource
    {
      get { return CurrentObject; }
    }

    protected new EditRoleFormFunction CurrentFunction
    {
      get { return (EditRoleFormFunction) base.CurrentFunction; }
    }

    public override IFocusableControl InitialFocusControl
    {
      get { return (IFocusableControl) _userField; }
    }

    protected override void OnInit (EventArgs e)
    {
      base.OnInit (e);

      _groupField = GetBoundEditableWebControl ("GroupField", "Group");
      _userField = GetBoundEditableWebControl ("UserField", "User");
    }

    protected override void OnLoad (EventArgs e)
    {
      base.OnLoad (e);

      InitializeUserField();
      InitializeGroupField();
      InitializePositionField (IsPostBack);
    }

    public override bool Validate ()
    {
      bool isValid = base.Validate();

      isValid &= FormGridManager.Validate();

      return isValid;
    }

    private void InitializeUserField ()
    {
      if (CurrentFunction.User != null)
        _userField.ReadOnly = true;
    }

    private void InitializeGroupField ()
    {
      if (CurrentFunction.Group != null)
        _groupField.ReadOnly = true;
    }

    private void InitializePositionField (bool interim)
    {
      bool isGroupSelected = _groupField.Value != null;
      PositionField.Enabled = isGroupSelected;
      if (!interim)
        FillPositionField();
    }

    private void FillPositionField ()
    {
      if (_groupField.Value == null)
        PositionField.ClearBusinessObjectList();
      else
        PositionField.SetBusinessObjectList (CurrentFunction.Role.GetPossiblePositions ((Group) _groupField.Value));
    }

    protected void GroupField_SelectionChanged (object sender, EventArgs e)
    {
      InitializePositionField (false);
    }

    private BusinessObjectBoundEditableWebControl GetBoundEditableWebControl (string controlID, string propertyIdentifier)
    {
      Control control = FindControl (controlID);
      
      if (control == null)
        throw new InvalidOperationException (string.Format ("No control with the ID '{0}' found.", controlID));
      
      if (!(control is BusinessObjectBoundEditableWebControl))
      {
        throw new InvalidOperationException (
            string.Format ("Control '{0}' must be of type '{1}'.", controlID, typeof (BusinessObjectBoundEditableWebControl).FullName));
      }
      
      if (!(control is IFocusableControl))
      {
        throw new InvalidOperationException (
            string.Format ("Control '{0}' must implement the '{1}' interface.", controlID, typeof (IFocusableControl).FullName));
      }
      
      BusinessObjectBoundEditableWebControl boundEditableWebControl = (BusinessObjectBoundEditableWebControl) control;
      if (boundEditableWebControl.Property == null || boundEditableWebControl.Property.Identifier != propertyIdentifier)
        throw new InvalidOperationException (string.Format ("Control '{0}' is not bound to property '{1}'.", controlID, propertyIdentifier));

      return boundEditableWebControl;
    }
  }
}
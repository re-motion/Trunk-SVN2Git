using System;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.SecurityManager.Clients.Web.Classes;
using Remotion.SecurityManager.Clients.Web.Globalization.UI.OrganizationalStructure;
using Remotion.SecurityManager.Clients.Web.WxeFunctions.OrganizationalStructure;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.Web.UI.Controls;
using Remotion.Web.UI.Globalization;

namespace Remotion.SecurityManager.Clients.Web.UI.OrganizationalStructure
{
  [WebMultiLingualResources (typeof (EditGroupTypePositionControlResources))]
  public partial class EditGroupTypePositionControl : BaseControl
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    // methods and properties
    public override IBusinessObjectDataSourceControl DataSource
    {
      get { return CurrentObject; }
    }

    protected new EditGroupTypePositionFormFunction CurrentFunction
    {
      get { return (EditGroupTypePositionFormFunction) base.CurrentFunction; }
    }

    public override IFocusableControl InitialFocusControl
    {
      get 
      {
        if (!GroupTypeField.IsReadOnly)
          return GroupTypeField;
        else if (!PositionField.IsReadOnly)
          return PositionField;
        else
          return null;
      }
    }

    protected override void OnLoad (EventArgs e)
    {
      base.OnLoad (e);

      if (CurrentFunction.Position != null)
        PositionField.ReadOnly = true;
      else
        FillPositionField ();

      if (CurrentFunction.GroupType != null)
        GroupTypeField.ReadOnly = true;
      else
        FillGroupTypeField ();
    }

    private void FillGroupTypeField ()
    {
      GroupTypeField.SetBusinessObjectList (GroupType.FindAll ());
    }

    private void FillPositionField ()
    {
      PositionField.SetBusinessObjectList (Position.FindAll ());
    }

    public override bool Validate ()
    {
      bool isValid = base.Validate ();

      isValid &= FormGridManager.Validate ();

      return isValid;
    }
  }
}
using System;
using Remotion.Data.DomainObjects.Web.ExecutionEngine;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.SecurityManager.Clients.Web.Classes;
using Remotion.SecurityManager.Clients.Web.Globalization.UI.OrganizationalStructure;
using Remotion.SecurityManager.Clients.Web.WxeFunctions.OrganizationalStructure;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.Web.UI.Controls;
using Remotion.Web.UI.Globalization;

namespace Remotion.SecurityManager.Clients.Web.UI.OrganizationalStructure
{
  [WebMultiLingualResources (typeof (EditPositionControlResources))]
  public partial class EditPositionControl : BaseControl
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

    protected new EditPositionFormFunction CurrentFunction
    {
      get { return (EditPositionFormFunction) base.CurrentFunction; }
    }

    public override IFocusableControl InitialFocusControl
    {
      get { return NameField; }
    }

    protected override void OnLoad (EventArgs e)
    {
      base.OnLoad (e);

      if (!IsPostBack)
      {
        GroupTypesList.SetSortingOrder (new BocListSortingOrderEntry ((IBocSortableColumnDefinition) GroupTypesList.FixedColumns[0], SortingDirection.Ascending));
      }

      if (GroupTypesList.IsReadOnly)
        GroupTypesList.Selection = RowSelection.Disabled;
    }

    public override bool Validate ()
    {
      bool isValid = base.Validate ();

      isValid &= FormGridManager.Validate ();

      return isValid;
    }

    protected void GroupTypesList_MenuItemClick (object sender, WebMenuItemClickEventArgs e)
    {
      if (e.Item.ItemID == "NewItem")
      {
        if (!Page.IsReturningPostBack)
        {
          EditGroupTypePosition (null, CurrentFunction.Position, null);
        }
        else
        {
          EditGroupTypePositionFormFunction returningFunction = (EditGroupTypePositionFormFunction) Page.ReturningFunction;

          GroupTypesList.LoadValue (!returningFunction.HasUserCancelled);
          if (returningFunction.HasUserCancelled)
            returningFunction.GroupTypePosition.Delete ();
          else
            GroupTypesList.IsDirty = true;
        }
      }
      if (e.Item.ItemID == "EditItem")
      {
        if (!Page.IsReturningPostBack)
        {
          EditGroupTypePosition ((GroupTypePosition) GroupTypesList.GetSelectedBusinessObjects ()[0], CurrentFunction.Position, null);
        }
        else
        {
          EditGroupTypePositionFormFunction returningFunction = (EditGroupTypePositionFormFunction) Page.ReturningFunction;

          if (!returningFunction.HasUserCancelled)
            GroupTypesList.IsDirty = true;
        }
      }

      if (e.Item.ItemID == "DeleteItem")
      {
        foreach (GroupTypePosition groupTypePosition in GroupTypesList.GetSelectedBusinessObjects ())
        {
          GroupTypesList.RemoveRow (groupTypePosition);
          groupTypePosition.Delete ();
        }
      }

      GroupTypesList.ClearSelectedRows ();
    }

    private void EditGroupTypePosition (GroupTypePosition groupTypePosition, Position position, GroupType groupType)
    {
      EditGroupTypePositionFormFunction editGroupTypePositionFormFunction =
        new EditGroupTypePositionFormFunction ( (groupTypePosition != null) ? groupTypePosition.ID : null, position, groupType);

      editGroupTypePositionFormFunction.TransactionMode = WxeTransactionMode.None;
      Page.ExecuteFunction (editGroupTypePositionFormFunction);
    }
  }
}
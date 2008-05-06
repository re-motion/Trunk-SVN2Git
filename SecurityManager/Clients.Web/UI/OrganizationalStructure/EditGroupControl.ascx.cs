using System;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.SecurityManager.Clients.Web.Classes;
using Remotion.SecurityManager.Clients.Web.Globalization.UI.OrganizationalStructure;
using Remotion.SecurityManager.Clients.Web.WxeFunctions.OrganizationalStructure;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.UI.Controls;
using Remotion.Web.UI.Globalization;

namespace Remotion.SecurityManager.Clients.Web.UI.OrganizationalStructure
{
  [WebMultiLingualResources (typeof (EditGroupControlResources))]
  public partial class EditGroupControl : BaseControl
  {
    public override IBusinessObjectDataSourceControl DataSource
    {
      get { return CurrentObject; }
    }

    protected new EditGroupFormFunction CurrentFunction
    {
      get { return (EditGroupFormFunction) base.CurrentFunction; }
    }

    public override IFocusableControl InitialFocusControl
    {
      get { return NameField; }
    }

    protected override void OnLoad (EventArgs e)
    {
      base.OnLoad (e);

      FillGroupTypeField ();

      if (ChildrenList.IsReadOnly)
        ChildrenList.Selection = RowSelection.Disabled;

      if (RolesList.IsReadOnly)
        RolesList.Selection = RowSelection.Disabled;
    }

    private void FillGroupTypeField ()
    {
      GroupTypeField.SetBusinessObjectList (GroupType.FindAll ());
    }

    public override bool Validate ()
    {
      bool isValid = base.Validate ();

      isValid &= FormGridManager.Validate ();

      return isValid;
    }

    protected void RolesList_MenuItemClick (object sender, WebMenuItemClickEventArgs e)
    {
      if (e.Item.ItemID == "NewItem")
      {
        if (!Page.IsReturningPostBack)
        {
          EditRole (null, null, CurrentFunction.Group, null);
        }
        else
        {
          EditRoleFormFunction returningFunction = (EditRoleFormFunction) Page.ReturningFunction;

          RolesList.LoadValue (!returningFunction.HasUserCancelled);
          if (returningFunction.HasUserCancelled)
            returningFunction.Role.Delete ();
          else
            RolesList.IsDirty = true;
        }
      }

      if (e.Item.ItemID == "EditItem")
      {
        if (!Page.IsReturningPostBack)
        {
          EditRole ((Role) RolesList.GetSelectedBusinessObjects ()[0], null, CurrentFunction.Group, null);
        }
        else
        {
          EditRoleFormFunction returningFunction = (EditRoleFormFunction) Page.ReturningFunction;
          if (!returningFunction.HasUserCancelled)
            RolesList.IsDirty = true;
        }
      }

      if (e.Item.ItemID == "DeleteItem")
      {
        foreach (Role role in RolesList.GetSelectedBusinessObjects ())
        {
          RolesList.RemoveRow (role);
          role.Delete ();
        }
      }

      RolesList.ClearSelectedRows ();
    }

    private void EditRole (Role role, User user, Group group, Position position)
    {
      EditRoleFormFunction editRoleFormFunction = new EditRoleFormFunction ((role != null) ? role.ID : null, user, group);

      editRoleFormFunction.TransactionMode = WxeTransactionMode.None;
      Page.ExecuteFunction (editRoleFormFunction);
    }

    protected void ChildrenList_MenuItemClick (object sender, WebMenuItemClickEventArgs e)
    {
      if (e.Item.ItemID == "AddItem")
      {
        if (!Page.IsReturningPostBack)
        {
          SearchGroupFormFunction searchGroupFormFunction = new SearchGroupFormFunction ();
          searchGroupFormFunction.TransactionMode = WxeTransactionMode.None;

          Page.ExecuteFunction (searchGroupFormFunction);
        }
        else
        {
          SearchGroupFormFunction returningFunction = (SearchGroupFormFunction) Page.ReturningFunction;

          if (!returningFunction.HasUserCancelled)
          {
            if (!ChildrenList.Value.Contains (returningFunction.SelectedGroup))
              ChildrenList.AddRow (returningFunction.SelectedGroup);
          }
        }
      }

      if (e.Item.ItemID == "RemoveItem")
        ChildrenList.RemoveRows (ChildrenList.GetSelectedBusinessObjects ());

      ChildrenList.ClearSelectedRows ();
    }
  }
}
using System;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.Security;
using Remotion.Security.Configuration;
using Remotion.SecurityManager.Clients.Web.Classes;
using Remotion.SecurityManager.Clients.Web.Globalization.UI.OrganizationalStructure;
using Remotion.SecurityManager.Clients.Web.WxeFunctions.OrganizationalStructure;
using Remotion.SecurityManager.Configuration;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.UI.Globalization;

namespace Remotion.SecurityManager.Clients.Web.UI.OrganizationalStructure
{
  [WebMultiLingualResources (typeof (PositionListControlResources))]
  public partial class PositionListControl : BaseControl
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

    protected new PositionListFormFunction CurrentFunction
    {
      get { return (PositionListFormFunction) base.CurrentFunction; }
    }

    protected override void OnLoad (EventArgs e)
    {
      base.OnLoad (e);

      if (!IsPostBack)
        PositionList.SetSortingOrder (new BocListSortingOrderEntry ((IBocSortableColumnDefinition) PositionList.FixedColumns[0], SortingDirection.Ascending));
      PositionList.LoadUnboundValue (Position.FindAll (), false);

      if (!SecurityConfiguration.Current.SecurityProvider.IsNull)
      {
        SecurityClient securityClient = SecurityClient.CreateSecurityClientFromConfiguration ();
        Type positionType = SecurityManagerConfiguration.Current.OrganizationalStructureFactory.GetPositionType ();
        NewPositionButton.Visible = securityClient.HasConstructorAccess (positionType);
      }
    }

    protected void PositionList_ListItemCommandClick (object sender, BocListItemCommandClickEventArgs e)
    {
      if (!Page.IsReturningPostBack)
      {
        EditPositionFormFunction editPositionFormFunction = new EditPositionFormFunction (((Position) e.BusinessObject).ID);
        editPositionFormFunction.TransactionMode = WxeTransactionMode.None;
        Page.ExecuteFunction (editPositionFormFunction);
      }
      else
      {
        if (!((EditPositionFormFunction) Page.ReturningFunction).HasUserCancelled)
          PositionList.LoadUnboundValue (Position.FindAll (), false);
      }
    }

    protected void NewPositionButton_Click (object sender, EventArgs e)
    {
      if (!Page.IsReturningPostBack)
      {
        EditPositionFormFunction editPositionFormFunction = new EditPositionFormFunction (null);
        editPositionFormFunction.TransactionMode = WxeTransactionMode.None;
        Page.ExecuteFunction (editPositionFormFunction);
      }
      else
      {
        if (!((EditPositionFormFunction) Page.ReturningFunction).HasUserCancelled)
          PositionList.LoadUnboundValue (Position.FindAll (), false);
      }
    }
  }
}

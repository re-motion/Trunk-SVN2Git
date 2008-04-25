using System;
using Remotion.Web.Security.ExecutionEngine;
using Remotion.SecurityManager.Clients.Web.UI.OrganizationalStructure;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.Web.ExecutionEngine;

namespace Remotion.SecurityManager.Clients.Web.WxeFunctions.OrganizationalStructure
{
  [WxeDemandTargetStaticMethodPermission (Position.Methods.Search)]
  [Serializable]
  public class PositionListFormFunction : BaseTransactedFunction
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    public PositionListFormFunction ()
    {
    }

    // TODO: Make protected once a way is found to solve the "WxeDemandTargetStaticMethodPermission being typed on fixed class" problem
    public PositionListFormFunction (params object[] args)
      : base (args)
    {
    }

    // methods and properties

    WxeResourcePageStep Step1 = new WxeResourcePageStep (typeof (PositionListForm), "UI/OrganizationalStructure/PositionListForm.aspx");
  }
}

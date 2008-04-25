using System;
using Remotion.SecurityManager.Clients.Web.UI.OrganizationalStructure;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.Web.ExecutionEngine;

namespace Remotion.SecurityManager.Clients.Web.WxeFunctions.OrganizationalStructure
{
  [Serializable]
  public class SearchGroupFormFunction : BaseTransactedFunction
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    public SearchGroupFormFunction ()
    {
    }

    protected SearchGroupFormFunction (params object[] args)
      : base (args)
    {
    }

    // methods and properties

    public Group SelectedGroup
    {
      get { return (Group) Variables["Group"]; }
      set { Variables["Group"] = value; }
    }

    WxeResourcePageStep Step1 = new WxeResourcePageStep (typeof (SearchGroupForm), "UI/OrganizationalStructure/SearchGroupForm.aspx");
  }
}

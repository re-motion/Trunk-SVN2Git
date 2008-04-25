using System;
using Remotion.Data.DomainObjects;
using Remotion.SecurityManager.Clients.Web.UI.OrganizationalStructure;
using Remotion.SecurityManager.Configuration;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.Web.ExecutionEngine;

namespace Remotion.SecurityManager.Clients.Web.WxeFunctions.OrganizationalStructure
{
  [Serializable]
  public class EditUserFormFunction : FormFunction
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    public EditUserFormFunction ()
    {
    }

    protected EditUserFormFunction (params object[] args)
      : base (args)
    {
    }

    public EditUserFormFunction (ObjectID organizationalStructureObjectID)
      : base (organizationalStructureObjectID)
    {
    }

    // methods and properties
    public User User
    {
      get { return (User) CurrentObject; }
      set { CurrentObject = value; }
    }

    private void Step1 ()
    {
      // TODO check CurrentTransaction
      if (CurrentObject == null)
      {
        User = SecurityManagerConfiguration.Current.OrganizationalStructureFactory.CreateUser ();
        User.Tenant = Tenant.GetObject (TenantID);
      }
    }

    WxeResourcePageStep Step2 = new WxeResourcePageStep (typeof (EditUserForm), "UI/OrganizationalStructure/EditUserForm.aspx");
  }
}

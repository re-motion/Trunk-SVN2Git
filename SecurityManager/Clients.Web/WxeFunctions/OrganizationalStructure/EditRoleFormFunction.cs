using System;
using Remotion.Data.DomainObjects;
using Remotion.SecurityManager.Clients.Web.UI.OrganizationalStructure;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.Web.ExecutionEngine;

namespace Remotion.SecurityManager.Clients.Web.WxeFunctions.OrganizationalStructure
{
  [Serializable]
  public class EditRoleFormFunction : FormFunction
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    public EditRoleFormFunction ()
    {
    }

    protected EditRoleFormFunction (params object[] args)
      : base (args)
    {
    }

    public EditRoleFormFunction (ObjectID organizationalStructureObjectID, User user, Group group)
      : base (organizationalStructureObjectID)
    {
      User = user;
      Group = group;
    }

    // methods and properties
    public User User
    {
      get { return (User) Variables["User"]; }
      private set { Variables["User"] = value; }
    }

    public Group Group
    {
      get { return (Group) Variables["Group"]; }
      private set { Variables["Group"] = value; }
    }

    public Role Role
    {
      get { return (Role) CurrentObject; }
      private set { CurrentObject = value; }
    }

    private void Step1 ()
    {
      // TODO check CurrentTransaction
      if (CurrentObject == null)
      {
        Role = Role.NewObject ();
        Role.User = User;
        Role.Group = Group;
      }
    }

    WxeResourcePageStep Step2 = new WxeResourcePageStep (typeof (EditRoleForm), "UI/OrganizationalStructure/EditRoleForm.aspx");

  }
}

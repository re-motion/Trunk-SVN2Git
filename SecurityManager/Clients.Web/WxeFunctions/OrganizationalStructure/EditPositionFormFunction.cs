using System;
using Remotion.Data.DomainObjects;
using Remotion.SecurityManager.Clients.Web.UI.OrganizationalStructure;
using Remotion.SecurityManager.Configuration;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.Web.ExecutionEngine;

namespace Remotion.SecurityManager.Clients.Web.WxeFunctions.OrganizationalStructure
{
  [Serializable]
  public class EditPositionFormFunction : FormFunction
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    public EditPositionFormFunction ()
    {
    }

    protected EditPositionFormFunction (params object[] args)
      : base (args)
    {
    }

    public EditPositionFormFunction (ObjectID organizationalStructureObjectID)
      : base (organizationalStructureObjectID)
    {
    }

    // methods and properties
    public Position Position
    {
      get { return (Position) CurrentObject; }
      set { CurrentObject = value; }
    }

    private void Step1 ()
    {
      // TODO check CurrentTransaction
      if (CurrentObject == null)
      {
        Position = SecurityManagerConfiguration.Current.OrganizationalStructureFactory.CreatePosition ();
      }
    }

    WxeResourcePageStep Step2 = new WxeResourcePageStep (typeof (EditPositionForm), "UI/OrganizationalStructure/EditPositionForm.aspx");
  }
}

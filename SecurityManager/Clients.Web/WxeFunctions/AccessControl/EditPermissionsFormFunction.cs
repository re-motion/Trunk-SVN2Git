using System;
using Remotion.Data.DomainObjects;
using Remotion.SecurityManager.Clients.Web.UI.AccessControl;
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.Web.ExecutionEngine;

namespace Remotion.SecurityManager.Clients.Web.WxeFunctions.AccessControl
{
  [Serializable]
  public class EditPermissionsFormFunction : FormFunction
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    public EditPermissionsFormFunction ()
    {
    }

    protected EditPermissionsFormFunction (params object[] args)
      : base (args)
    {
    }

    public EditPermissionsFormFunction (ObjectID securableClassDefinitionObjectID)
      : base (securableClassDefinitionObjectID)
    {
    }

    // methods and properties
    public SecurableClassDefinition SecurableClassDefinition
    {
      get { return (SecurableClassDefinition) CurrentObject; }
      set { CurrentObject = value; }
    }

    WxeResourcePageStep Step1 = new WxeResourcePageStep (typeof (EditPermissionsForm), "UI/AccessControl/EditPermissionsForm.aspx");
  }
}

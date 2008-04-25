using System;
using Remotion.Data.DomainObjects;
using Remotion.SecurityManager.Clients.Web.UI.OrganizationalStructure;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.Web.ExecutionEngine;

namespace Remotion.SecurityManager.Clients.Web.WxeFunctions.OrganizationalStructure
{
  [Serializable]
  public class EditGroupTypePositionFormFunction : FormFunction
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    public EditGroupTypePositionFormFunction ()
    {
    }

    protected EditGroupTypePositionFormFunction (params object[] args)
      : base (args)
    {
    }

    public EditGroupTypePositionFormFunction (ObjectID organizationalStructureObjectID, Position position, GroupType groupType)
      : base (organizationalStructureObjectID)
    {
      GroupType = groupType;
      Position = position;
    }

    // methods and properties
    public GroupType GroupType
    {
      get { return (GroupType) Variables["GroupType"]; }
      set { Variables["GroupType"] = value; }
    }

    public Position Position
    {
      get { return (Position) Variables["Position"]; }
      set { Variables["Position"] = value; }
    }

    public GroupTypePosition GroupTypePosition
    {
      get { return (GroupTypePosition) CurrentObject; }
      set { CurrentObject = value; }
    }

    private void Step1 ()
    {
      // TODO check CurrentTransaction
      if (CurrentObject == null)
      {
        GroupTypePosition = GroupTypePosition.NewObject ();
        GroupTypePosition.GroupType = GroupType;
        GroupTypePosition.Position = Position;
      }
    }

    WxeResourcePageStep Step2 = new WxeResourcePageStep (typeof (EditGroupTypePositionForm), "UI/OrganizationalStructure/EditGroupTypePositionForm.aspx");
  }
}

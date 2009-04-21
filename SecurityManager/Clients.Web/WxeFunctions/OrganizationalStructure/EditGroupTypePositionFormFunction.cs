// This file is part of re-strict (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License version 3.0 
// as published by the Free Software Foundation.
// 
// This program is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program; if not, see http://www.gnu.org/licenses.
// 
// Additional permissions are listed in the file re-motion_exceptions.txt.
// 
using System;
using Remotion.Data.DomainObjects;
using Remotion.SecurityManager.Clients.Web.UI.OrganizationalStructure;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.ExecutionEngine.Infrastructure;

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

    protected EditGroupTypePositionFormFunction (ITransactionMode transactionMode, params object[] args)
      : base (transactionMode, args)
    {
    }

    public EditGroupTypePositionFormFunction (ITransactionMode transactionMode, ObjectID organizationalStructureObjectID, Position position, GroupType groupType)
      : base (transactionMode, organizationalStructureObjectID)
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

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
using System.Linq;
using System.Web.UI.WebControls;
using Remotion.FunctionalProgramming;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.SecurityManager.Clients.Web.Classes;
using Remotion.SecurityManager.Clients.Web.Classes.OrganizationalStructure;
using Remotion.SecurityManager.Clients.Web.Globalization.UI.OrganizationalStructure;
using Remotion.SecurityManager.Clients.Web.WxeFunctions.OrganizationalStructure;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.Web.UI.Controls;
using Remotion.Web.UI.Globalization;

namespace Remotion.SecurityManager.Clients.Web.UI.OrganizationalStructure
{
  [WebMultiLingualResources (typeof (EditGroupControlResources))]
  public partial class EditGroupControl : BaseControl
  {
    private BocAutoCompleteReferenceValue _parentField;
    private BocAutoCompleteReferenceValue _groupTypeField;

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

    protected override void OnInit (EventArgs e)
    {
      base.OnInit (e);

      _parentField = GetControl<BocAutoCompleteReferenceValue> ("ParentField", "Parent");
      _groupTypeField = GetControl<BocAutoCompleteReferenceValue> ("GroupTypeField", "GroupType");

      var bocListInlineEditingConfigurator = new BocListInlineEditingConfigurator (ResourceUrlFactory);
      bocListInlineEditingConfigurator.Configure (RolesList, Role.NewObject);

      if (string.IsNullOrEmpty (_parentField.SearchServicePath))
        SecurityManagerSearchWebService.BindServiceToControl (_parentField);

      if (string.IsNullOrEmpty (_groupTypeField.SearchServicePath))
        SecurityManagerSearchWebService.BindServiceToControl (_groupTypeField);
    }

    protected override void OnLoad (EventArgs e)
    {
      base.OnLoad (e);

      if (!IsPostBack)
      {
        RolesList.SetSortingOrder (
            new BocListSortingOrderEntry ((IBocSortableColumnDefinition) RolesList.FixedColumns.Find ("User"), SortingDirection.Ascending));
      }

      if (ChildrenList.IsReadOnly)
        ChildrenList.Selection = RowSelection.Disabled;

      if (RolesList.IsReadOnly)
        RolesList.Selection = RowSelection.Disabled;
    }

    protected override void OnPreRender (EventArgs e)
    {
      base.OnPreRender (e);

      if (CurrentFunction.TenantID == null)
        throw new InvalidOperationException ("No current tenant has been set. Possible reason: session timeout");

      _parentField.Args = CurrentFunction.TenantID.ToString();
    }

    public override bool Validate ()
    {
      bool isValid = base.Validate();

      isValid &= FormGridManager.Validate();

      return isValid;
    }

    protected void ParentValidator_ServerValidate (object source, ServerValidateEventArgs args)
    {
      args.IsValid = IsParentHierarchyValid ((Group) _parentField.Value);
    }

    private bool IsParentHierarchyValid (Group group)
    {
      var groups = group.CreateSequence (g => g.Parent, g => g != null && g != CurrentFunction.Group && g.Parent != group).ToArray();
      if (groups.Length == 0)
        return false;
      if (groups.Last().Parent != null)
        return false;
      return true;
    }
  }
}
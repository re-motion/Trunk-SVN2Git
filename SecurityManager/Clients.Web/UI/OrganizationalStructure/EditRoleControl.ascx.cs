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
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.SecurityManager.Clients.Web.Classes;
using Remotion.SecurityManager.Clients.Web.Globalization.UI.OrganizationalStructure;
using Remotion.SecurityManager.Clients.Web.WxeFunctions.OrganizationalStructure;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.Web.UI.Controls;
using Remotion.Web.UI.Globalization;

namespace Remotion.SecurityManager.Clients.Web.UI.OrganizationalStructure
{
  [WebMultiLingualResources (typeof (EditRoleControlResources))]
  public partial class EditRoleControl : BaseControl
  {
    private BocAutoCompleteReferenceValue _groupField;
    private BocAutoCompleteReferenceValue _userField;

    public override IBusinessObjectDataSourceControl DataSource
    {
      get { return CurrentObject; }
    }

    protected new EditRoleFormFunction CurrentFunction
    {
      get { return (EditRoleFormFunction) base.CurrentFunction; }
    }

    public override IFocusableControl InitialFocusControl
    {
      get { return _userField; }
    }

    protected override void OnInit (EventArgs e)
    {
      base.OnInit (e);

      _groupField = GetControl<BocAutoCompleteReferenceValue> ("GroupField", "Group");
      _userField = GetControl<BocAutoCompleteReferenceValue> ("UserField", "User");

      if (string.IsNullOrEmpty (_groupField.SearchServicePath))
        SecurityManagerSearchWebService.BindServiceToControl (_groupField);

      if (string.IsNullOrEmpty (_userField.SearchServicePath))
        SecurityManagerSearchWebService.BindServiceToControl (_userField);
    }

    protected override void OnLoad (EventArgs e)
    {
      base.OnLoad (e);

      InitializeUserField();
      InitializeGroupField();
      InitializePositionField (IsPostBack);
    }

    protected override void OnPreRender (EventArgs e)
    {
      base.OnPreRender (e);
      
      if (CurrentFunction.TenantID == null)
        throw new InvalidOperationException ("No current tenant has been set. Possible reason: session timeout");

      _groupField.Args = CurrentFunction.TenantID.ToString();
      _userField.Args = CurrentFunction.TenantID.ToString();
    }

    public override bool Validate ()
    {
      bool isValid = base.Validate();

      isValid &= FormGridManager.Validate();

      return isValid;
    }

    private void InitializeUserField ()
    {
      if (CurrentFunction.User != null)
        _userField.ReadOnly = true;
    }

    private void InitializeGroupField ()
    {
      if (CurrentFunction.Group != null)
        _groupField.ReadOnly = true;
    }

    private void InitializePositionField (bool interim)
    {
      bool isGroupSelected = _groupField.Value != null;
      PositionField.Enabled = isGroupSelected;
      if (!interim)
        FillPositionField();
    }

    private void FillPositionField ()
    {
      if (_groupField.Value == null)
        PositionField.ClearBusinessObjectList();
      else
        PositionField.SetBusinessObjectList (CurrentFunction.Role.GetPossiblePositions ((Group) _groupField.Value));
    }

    protected void GroupField_SelectionChanged (object sender, EventArgs e)
    {
      InitializePositionField (false);
    }
  }
}

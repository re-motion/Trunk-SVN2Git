// This file is part of re-strict (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// re-strict is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License version 3.0 as
// published by the Free Software Foundation.
// 
// re-strict is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with re-strict; if not, see http://www.gnu.org/licenses.
// 
// Additional permissions are listed in the file re-motion_exceptions.txt.
// 
using System;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.SecurityManager.Clients.Web.Classes;
using Remotion.SecurityManager.Clients.Web.Globalization.UI.AccessControl;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.Web.UI.Globalization;

namespace Remotion.SecurityManager.Clients.Web.UI.AccessControl
{
  [WebMultiLingualResources (typeof (AccessControlResources))]
  public partial class EditPermissionControl : BaseControl
  {
    public override IBusinessObjectDataSourceControl DataSource
    {
      get { return CurrentObject; }
    }

    public void SetPermissionValue (bool? allowed)
    {
      AllowedField.Value = allowed;
    }

    protected override void OnPreRender (EventArgs e)
    {
      base.OnPreRender (e);

      string accessTypeName = ((Permission) CurrentObject.BusinessObject).AccessType.DisplayName;
      AllowedField.TrueDescription = string.Format (AccessControlResources.PermissionGranted_Text, accessTypeName);
      AllowedField.FalseDescription = string.Format (AccessControlResources.PermissionDenied_Text, accessTypeName);
      AllowedField.NullDescription = string.Format (AccessControlResources.PermissionUndefined_Text, accessTypeName);
    }
  }
}

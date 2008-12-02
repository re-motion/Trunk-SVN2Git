/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

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

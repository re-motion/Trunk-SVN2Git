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
using System.Web.UI.HtmlControls;
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.SecurityManager.Clients.Web.Classes;
using Remotion.SecurityManager.Clients.Web.Globalization.UI.AccessControl;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.Web.UI.Globalization;

namespace Remotion.SecurityManager.Clients.Web.UI.AccessControl
{
  [WebMultiLingualResources (typeof (AccessControlResources))]
  public partial class EditAccessControlEntryHeaderControl : BaseControl
  {
    public override IBusinessObjectDataSourceControl DataSource
    {
      get { return CurrentObject; }
    }

    protected SecurableClassDefinition CurrentClassDefinition
    {
      get { return (SecurableClassDefinition) CurrentObject.BusinessObject; }
    }

    public override void LoadValues (bool interim)
    {
      base.LoadValues (interim);

      IBusinessObjectClass aceClass = BindableObjectProvider.GetBindableObjectClass (typeof (AccessControlEntry));

      var cssHorizontal = "titleCellHorizontal";
      var cssVertical = "titleCellVertical";
      HeaderCells.Controls.Add (CreateTableCell (aceClass.GetPropertyDefinition ("TenantCondition").DisplayName, cssHorizontal));
      HeaderCells.Controls.Add (CreateTableCell (aceClass.GetPropertyDefinition ("GroupCondition").DisplayName, cssHorizontal));
      HeaderCells.Controls.Add (CreateTableCell (aceClass.GetPropertyDefinition ("UserCondition").DisplayName, cssHorizontal));
      HeaderCells.Controls.Add (CreateTableCell (aceClass.GetPropertyDefinition ("SpecificAbstractRole").DisplayName, cssHorizontal));
      HeaderCells.Controls.Add (CreateTableCell (string.Empty, cssHorizontal)); //Toggle Permissions
      foreach (var accessType in CurrentClassDefinition.AccessTypes)
        HeaderCells.Controls.Add (CreateTableCell (accessType.DisplayName, cssVertical));
      HeaderCells.Controls.Add (CreateTableCell (string.Empty, cssHorizontal)); //ExpandButton
      HeaderCells.Controls.Add (CreateTableCell (string.Empty, cssHorizontal)); //DeleteButton
    }

    private HtmlGenericControl CreateTableCell (string title, string cssClass)
    {
      HtmlGenericControl th = new HtmlGenericControl ("th");
      th.Attributes.Add ("class", cssClass);

      HtmlGenericControl div = new HtmlGenericControl ("div");
      div.InnerText = title;
      th.Controls.Add (div);

      return th;
    }
  }
}
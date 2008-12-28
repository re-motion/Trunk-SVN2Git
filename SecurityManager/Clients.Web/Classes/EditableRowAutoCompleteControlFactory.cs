// This file is part of re-strict (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using System.Web;
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.Infrastructure.BocList;
using Remotion.SecurityManager.Clients.Web.UI;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.Utilities;

namespace Remotion.SecurityManager.Clients.Web.Classes
{
  public class EditableRowAutoCompleteControlFactory : EditableRowControlFactory
  {
    public override void RegisterHtmlHeadContents (HttpContext context)
    {
      base.RegisterHtmlHeadContents (context);
      new BocAutoCompleteReferenceValue().RegisterHtmlHeadContents (context);
    }

    protected override IBusinessObjectBoundEditableWebControl CreateFromPropertyPath (IBusinessObjectPropertyPath propertyPath)
    {
      ArgumentUtility.CheckNotNull ("propertyPath", propertyPath);

      if (IsAutoCompleteReferenceValueRequired (propertyPath))
      {
        var control = new BocAutoCompleteReferenceValue();
        control.PreRender += delegate
        {
          BasePage page = (BasePage) control.Page;
          control.Args = page.CurrentFunction.TenantID.ToString();
          SecurityManagerSearchWebService.BindServiceToControl (control);
        };

        return control;
      }

      return base.CreateFromPropertyPath (propertyPath);
    }

    private bool IsAutoCompleteReferenceValueRequired (IBusinessObjectPropertyPath propertyPath)
    {
      bool isScalarReferenceProperty = !propertyPath.LastProperty.IsList && propertyPath.LastProperty is IBusinessObjectReferenceProperty;
      if (!isScalarReferenceProperty)
        return false;

      Type propertyType = propertyPath.LastProperty.PropertyType;
      if (typeof (User).IsAssignableFrom (propertyType) || typeof (Group).IsAssignableFrom (propertyType))
        return true;

      return false;
    }
  }
}

// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.Web.UI.Globalization;
using $DOMAIN_ROOTNAMESPACE$;
using $PROJECT_ROOTNAMESPACE$.Classes;

namespace $PROJECT_ROOTNAMESPACE$.UI
{
  public partial class Edit$DOMAIN_CLASSNAME$Control : BaseControl
  {
    protected void Page_Load (object sender, EventArgs e)
    {
    }

    public override IBusinessObjectDataSourceControl DataSource
    {
      get { return CurrentObject; }
    }

    $REPEAT_FOREACHREFERENCEDPROPERTY_BEGIN$(isList=true)
    protected void $DOMAIN_PROPERTYNAME$Field_MenuItemClick (object sender, Remotion.Web.UI.Controls.WebMenuItemClickEventArgs e)
    {
      if (e.Item.ItemID == "AddMenuItem")
      {
        $DOMAIN_REFERENCEDCLASSNAME$ row = $DOMAIN_REFERENCEDCLASSNAME$.NewObject();
        $DOMAIN_PROPERTYNAME$Field.AddAndEditRow (row);
      }
    }

    $REPEAT_FOREACHREFERENCEDPROPERTY_END$
  }
}

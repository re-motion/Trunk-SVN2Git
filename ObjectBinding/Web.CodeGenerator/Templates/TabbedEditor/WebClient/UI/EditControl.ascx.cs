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

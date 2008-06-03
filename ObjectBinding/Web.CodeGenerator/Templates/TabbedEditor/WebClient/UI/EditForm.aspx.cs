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
using System.Collections.Generic;
using Remotion.Data.DomainObjects;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.UI.Globalization;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.Web.UI.Controls;
using $PROJECT_ROOTNAMESPACE$;
using $PROJECT_ROOTNAMESPACE$.Classes;
using $DOMAIN_ROOTNAMESPACE$;

namespace $PROJECT_ROOTNAMESPACE$.UI
{
  // <WxePageFunction>
  //   <Parameter name="obj" type="$DOMAIN_CLASSNAME$" />
  // </WxePageFunction>
  public partial class Edit$DOMAIN_CLASSNAME$Form : EditFormPage
  {
    protected void Page_Load (object sender, EventArgs e)
    {
      if (!IsPostBack)
      {
        if (obj == null)
          obj = $DOMAIN_CLASSNAME$.NewObject();
      }

      LoadObject (obj);
    }

    protected override IBusinessObjectDataSourceControl DataSource
    {
      get { return CurrentObject; }
    }

    protected override TabbedMultiView UserControlMultiView
    {
      get { return MultiView; }
    }

    protected void SaveButton_Click (object sender, EventArgs e)
    {
      if (SaveObject())
        Return();
    }

    protected void CancelButton_Click (object sender, EventArgs e)
    {
      throw new WxeUserCancelException ();
    }
  }
}

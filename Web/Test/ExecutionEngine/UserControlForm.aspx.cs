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
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Remotion.Web.ExecutionEngine;

namespace Remotion.Web.Test.ExecutionEngine
{
  public partial class UserControlForm : WxePage
  {
    protected override void OnLoad (EventArgs e)
    {
      base.OnLoad (e);
    }

    protected void PageButton_Click (object sender, EventArgs e)
    {
      PageLabel.Text = DateTime.Now.ToString ("HH:mm:ss");
    }
  }
}

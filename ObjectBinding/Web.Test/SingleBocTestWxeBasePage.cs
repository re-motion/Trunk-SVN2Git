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
using Remotion.Globalization;
using Remotion.Web.UI;

namespace OBWTest
{

[MultiLingualResources ("OBWTest.Globalization.SingleBocTestBasePage")]
public class SingleBocTestWxeBasePage: TestWxeBasePage
{
  protected override void OnInit(EventArgs e)
  {
    base.OnInit (e);
    this.EnableAbort = true;
    this.ShowAbortConfirmation = ShowAbortConfirmation.OnlyIfDirty;
  }

}

}

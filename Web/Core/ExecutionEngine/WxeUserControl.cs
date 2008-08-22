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
using System.Web.UI;
using Remotion.Collections;

namespace Remotion.Web.ExecutionEngine
{

public class WxeUserControl: UserControl, IWxeTemplateControl
{
  WxeTemplateControlInfo _wxeInfo;

  public WxeUserControl ()
  {
    _wxeInfo = new WxeTemplateControlInfo (this);
  }

  protected override void OnInit (EventArgs e)
  {
    _wxeInfo.Initialize (Context);
    base.OnInit (e);
  }

  public WxeUIStep CurrentStep
  {
    get { return _wxeInfo.CurrentStep; }
  }
  
  public WxeFunction CurrentFunction
  {
    get { return _wxeInfo.CurrentFunction; }
  }

  public NameObjectCollection Variables 
  {
    get { return _wxeInfo.Variables; }
  }

  public IWxePage WxePage
  {
    get { return (IWxePage) base.Page; }
  }
}

}

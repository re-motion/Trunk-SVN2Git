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
  readonly WxeTemplateControlInfo _wxeInfo;

  public WxeUserControl ()
  {
    _wxeInfo = new WxeTemplateControlInfo (this);
  }

  protected override void OnInit (EventArgs e)
  {
    _wxeInfo.Initialize (Context);
    base.OnInit (e);
  }

  public WxePageStep CurrentPageStep
  {
    get { return _wxeInfo.CurrentPageStep; }
  }
  
  public WxeFunction CurrentFunction
  {
    get { return _wxeInfo.CurrentPageFunction; }
  }

  public NameObjectCollection Variables 
  {
    get { return _wxeInfo.PageVariables; }
  }

  public IWxePage WxePage
  {
    get { return (IWxePage) base.Page; }
  }
}

}

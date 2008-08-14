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

namespace Remotion.Web.ExecutionEngine
{
  public interface IWxeExecutor
  {
    void ExecuteFunction (WxeFunction function, WxePermaUrlOptions permaUrlOptions);
    void ExecuteFunctionNoRepost (WxeFunction function, Control sender, WxeCallOptionsNoRepost options);
    void ExecuteFunctionExternalByRedirect (WxeFunction function, WxeCallOptionsExternalByRedirect options);
    void ExecuteFunctionExternal (WxeFunction function, Control sender, WxeCallOptionsExternal options);
  }
}
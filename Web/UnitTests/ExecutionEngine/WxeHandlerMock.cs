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
using System.Web;
using Remotion.Web.ExecutionEngine;

namespace Remotion.Web.UnitTests.ExecutionEngine
{

/// <summary> Exposes non-public members of the <see cref="WxeHandler"/> type. </summary>
public class WxeHandlerMock: WxeHandler
{
  public new void CheckTimeoutConfiguration (HttpContext context)
  {
    base.CheckTimeoutConfiguration (context);
  }

  public new Type GetTypeByPath (string path)
  {
    return base.GetTypeByPath (path);
  }

  public new Type GetType (HttpContext context)
  {
    return base.GetType (context);
  }

  public new Type GetTypeByTypeName (string typeName)
  {
    return base.GetTypeByTypeName (typeName);
  }

  public new WxeFunctionState CreateNewFunctionState (HttpContext context, Type type)
  {
    return base.CreateNewFunctionState (context, type);
  }

  public new WxeFunctionState ResumeExistingFunctionState (HttpContext context, string functionToken)
  {
    return base.ResumeExistingFunctionState (context, functionToken);
  }

  public new void ProcessFunctionState (HttpContext context, WxeFunctionState functionState, bool isNewFunction)
  {
    base.ProcessFunctionState (context, functionState, isNewFunction);
  }

  public new void ExecuteFunctionState (HttpContext context, WxeFunctionState functionState, bool isNewFunction)
  {
    base.ExecuteFunctionState (context, functionState, isNewFunction);
  }

  public new virtual void ExecuteFunction (WxeFunction function, WxeContext context, bool isNew)
  {
    base.ExecuteFunction (function, context, isNew);
  }

  public new void CleanUpFunctionState (WxeFunctionState functionState)
  {
    base.CleanUpFunctionState (functionState);
  }

  public new void ProcessReturnUrl (HttpContext context, string returnUrl)
  {
    base.ProcessReturnUrl (context, returnUrl);
  }
}

}

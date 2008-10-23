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
using System.Runtime.Serialization;

namespace Remotion.Web.ExecutionEngine.Infrastructure
{
  [Serializable]
  public class NullUserControlExecutor : IUserControlExecutor, IObjectReference
  {
    public static readonly IUserControlExecutor Null = new NullUserControlExecutor();

    private NullUserControlExecutor ()
    {
      
    }
    public bool IsNull
    {
      get { return true; }
    }

    public void Execute (WxeContext context)
    {
      //NOP
    }

    public WxeFunction Function
    {
      get { return null; }
    }

    public string BackedUpUserControlState
    {
      get { return null; }
    }

    public string BackedUpUserControl
    {
      get { return null; }
    }

    public string UserControlID
    {
      get { return null; }
    }

    public bool IsReturningInnerFunction
    {
      get { return false; }
    }

    object IObjectReference.GetRealObject (StreamingContext context)
    {
      return NullUserControlExecutor.Null;
    }
  }
}

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

namespace Remotion.Web.ExecutionEngine.Infrastructure
{
  /// <summary>
  /// <see cref="INullObject"/> implemention of the <see cref="IWxeFunctionExecutionListener"/> interface.
  /// </summary>
  [Serializable]
  public class NullExecutionListener : IWxeFunctionExecutionListener
  {
    public static readonly NullExecutionListener Null = new NullExecutionListener();
    
    private NullExecutionListener ()
    {
    }

    public void OnExecutionPlay (WxeContext context)
    {
      //NOP
    }

    public void OnExecutionStop (WxeContext context)
    {
      //NOP
    }

    public void OnExecutionPause (WxeContext context)
    {
      //NOP
    }

    public void OnExecutionFail (WxeContext context, Exception exception)
    {
      //NOP
    }

    public bool IsNull
    {
      get { return true; }
    }
  }
}
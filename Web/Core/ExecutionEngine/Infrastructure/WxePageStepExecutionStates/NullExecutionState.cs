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

namespace Remotion.Web.ExecutionEngine.Infrastructure.WxePageStepExecutionStates
{
  /// <summary>
  /// The <see cref="NullExecutionState"/> is the null-object implementation of the <see cref="IExecutionState"/> interface. This state always
  /// returns <see langword="false" /> for the <see cref="IsExecuting"/> property.
  /// </summary>
  [Serializable]
  public class NullExecutionState : IExecutionState, IObjectReference
  {
    public static readonly NullExecutionState Null = new NullExecutionState();

    private NullExecutionState ()
    {
    }

    public bool IsNull
    {
      get { return true; }
    }

    public IExecutionStateContext ExecutionStateContext
    {
      get { return null; }
    }

    public IExecutionStateParameters Parameters
    {
      get { return null; }
    }

    public bool IsExecuting
    {
      get { return false; }
    }

    public void ExecuteSubFunction (WxeContext context)
    {
    }

    public object GetRealObject (StreamingContext context)
    {
      return Null;
    }
  }
}
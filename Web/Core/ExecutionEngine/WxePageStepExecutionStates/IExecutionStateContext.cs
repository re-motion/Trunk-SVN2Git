/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

namespace Remotion.Web.ExecutionEngine.WxePageStepExecutionStates
{
  /// <summary>
  /// The <see cref="IExecutionStateContext"/> interface exposes the context in which the <see cref="IExecutionState"/> implementations operate.
  /// </summary>
  public interface IExecutionStateContext
  {
    /// <summary> Gets the current <see cref="IExecutionState"/> for the <see cref="WxeStep"/>. </summary>
    IExecutionState ExecutionState { get; }

    /// <summary> Transistions the <see cref="WxeStep"/> into the next <see cref="IExecutionState"/>. </summary>
    void SetExecutionState (IExecutionState executionState);

    /// <summary> Gets the function currently executing, i.e. usually the parent of the <see cref="CurrentStep"/>. </summary>
    WxeFunction CurrentFunction { get; }

    /// <summary> Getst the <see cref="WxeStep"/> that is executing the <see cref="WxeFunction"/>. </summary>
    WxeStep CurrentStep { get; }
  }
}
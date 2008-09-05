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

namespace Remotion.Web.ExecutionEngine.WxePageStepExecutionStates
{
  /// <summary>
  /// The <see cref="IExecutionState"/> interface is defines the state-pattern for executing a sub-function within a <see cref="WxeStep"/>.
  /// </summary>
  public interface IExecutionState : INullObject
  {
    /// <summary> Gets the context of the execution. Use this member to transistion the <see cref="WxeStep"/> into the next state. </summary>
    IExecutionStateContext ExecutionStateContext { get; }

    /// <summary> Gets a set of parameters common for all execution states, such as the executing <see cref="WxeFunction"/>. </summary>
    IExecutionStateParameters Parameters { get; }

    /// <summary> Gets a flag that informs the observer whether the state is executing. This value is typically constant for a state implementation. </summary>
    bool IsExecuting { get; }

    /// <summary>
    /// Executes the behavor of the current state and uses the <see cref="ExecutionStateContext"/> to transistion the <see cref="WxeStep"/>
    /// into the next state. 
    /// </summary>
    void ExecuteSubFunction (WxeContext context);
  }
}
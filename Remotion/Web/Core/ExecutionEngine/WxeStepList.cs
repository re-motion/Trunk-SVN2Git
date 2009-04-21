// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;
using System.Collections;
using System.Reflection;
using Remotion.Utilities;
using Remotion.Web.ExecutionEngine.Infrastructure;

namespace Remotion.Web.ExecutionEngine
{

  /// <summary>
  ///   Performs a sequence of steps in a web application.
  /// </summary>
  [Serializable]
  public class WxeStepList : WxeStep
  {
    /// <summary> ArrayList&lt;WxeStep&gt; </summary>
    private ArrayList _steps;

    private int _nextStep;
    private int _lastExecutedStep;

    public WxeStepList ()
    {
      _steps = new ArrayList ();
      _nextStep = 0;
      _lastExecutedStep = -1;
      InitializeSteps ();
    }

    /// <summary>
    ///   Moves all steps to <paramref name="innerList"/> and makes <paramref name="innerList"/> the only step of 
    ///   this step list.
    /// </summary>
    /// <param name="innerList"> 
    ///   This list will be the only step of the <see cref="WxeStepList"/> and contain all of the 
    ///   <see cref="WxeStepList"/>'s steps. Must be empty and not executing.
    /// </param>
    protected void Encapsulate (WxeStepList innerList)
    {
      if (this._nextStep != 0 || this._lastExecutedStep != -1)
        throw new InvalidOperationException ("Cannot encapsulate executing list.");
      if (innerList._steps.Count > 0)
        throw new ArgumentException ("List must be empty.", "innerList");
      if (innerList._nextStep != 0 || innerList._lastExecutedStep != -1)
        throw new ArgumentException ("Cannot encapsulate into executing list.", "innerList");

      innerList._steps = this._steps;
      foreach (WxeStep step in innerList._steps)
        step.SetParentStep (innerList);

      this._steps = new ArrayList (1);
      this._steps.Add (innerList);
      innerList.SetParentStep (this);
    }

    public override void Execute (WxeContext context)
    {
      for (int i = _nextStep; i < _steps.Count; ++i)
      {
        _lastExecutedStep = i;
        WxeStep currentStep = this[i];
        if (currentStep.IsAborted)
          throw new InvalidOperationException ("Step " + i + " of " + this.GetType ().FullName + " is aborted.");
        currentStep.Execute (context);
        _nextStep = i + 1;
      }

      if (_steps.Count == 0)
        _lastExecutedStep = 0;
    }

    public WxeStep this[int index]
    {
      get { return (WxeStep) _steps[index]; }
    }

    public int Count
    {
      get { return _steps.Count; }
    }

    public void Add (WxeStep step)
    {
      ArgumentUtility.CheckNotNull ("step", step);

      _steps.Add (step);
      step.SetParentStep (this);
    }

    public void Add (WxeStepList target, MethodInfo method)
    {
      ArgumentUtility.CheckNotNull ("target", target);
      ArgumentUtility.CheckNotNull ("method", method);

      Add (new WxeMethodStep (target, method));
    }

    public void AddStepList (WxeStepList steps)
    {
      ArgumentUtility.CheckNotNull ("steps", steps);

      for (int i = 0; i < steps.Count; i++)
        Add ((WxeStep) steps._steps[i]);
    }

    public void Insert (int index, WxeStep step)
    {
      if (_lastExecutedStep >= index)
        throw new ArgumentException ("Cannot insert step only after the last executed step.", "index");
      ArgumentUtility.CheckNotNull ("step", step);
      
      _steps.Insert (index, step);
      step.SetParentStep (this);
    }

    public override WxeStep ExecutingStep
    {
      get
      {
        if (_lastExecutedStep < _steps.Count)
          return ((WxeStep) _steps[_lastExecutedStep]).ExecutingStep;
        else
          return this;
      }
    }

    public WxeStep LastExecutedStep
    {
      get
      {
        if (_lastExecutedStep < _steps.Count && _lastExecutedStep >= 0)
          return (WxeStep) _steps[_lastExecutedStep];
        else
          return null;
      }
    }

    public bool IsExecutionStarted
    {
      get { return _lastExecutedStep >= 0; }
    }

    private void InitializeSteps ()
    {
      Type type = this.GetType ();
      MemberInfo[] members = NumberedMemberFinder.FindMembers (
          type,
          "Step",
          MemberTypes.Field | MemberTypes.Method | MemberTypes.NestedType,
          BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);

      foreach (MemberInfo member in members)
      {
        if (member is FieldInfo)
        {
          FieldInfo fieldInfo = (FieldInfo) member;
          Add ((WxeStep) fieldInfo.GetValue (this));
        }
        else if (member is MethodInfo)
        {
          MethodInfo methodInfo = (MethodInfo) member;
          Add (this, methodInfo);
        }
        else if (member is Type)
        {
          Type subtype = member as Type;
          if (typeof (WxeStep).IsAssignableFrom (subtype))
            Add ((WxeStep) Activator.CreateInstance (subtype));
        }
      }
    }

    protected override void AbortRecursive ()
    {
      base.AbortRecursive ();
      foreach (WxeStep step in _steps)
        step.Abort ();
    }
  }

}

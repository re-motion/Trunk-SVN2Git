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
using System.Reflection;
using System.Web.UI;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Web.Utilities
{
  public class InternalControlMemberCaller : IInternalControlMemberCaller
  {
    private const BindingFlags c_bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic;
    public static readonly Type InternalControlStateType = typeof (Control).Assembly.GetType ("System.Web.UI.ControlState", true, false);

    public void SetControlState (Control control, ControlState value)
    {
      object internalValue = Enum.ToObject (InternalControlStateType, value);
      MethodCaller.CallAction ("set_ControlState", c_bindingFlags)
          .Invoke (new[] { typeof (Control), InternalControlStateType }, new[] { control, internalValue });
    }

    public ControlState GetControlState (Control control)
    {
      int internalValue = MethodCaller.CallFunc<int> ("get_ControlState", c_bindingFlags).With (control);
      return (ControlState) internalValue;
    }

    public void InitRecursive (Control control, Control namingContainer)
    {
      ArgumentUtility.CheckNotNull ("control", control);
      ArgumentUtility.CheckNotNull ("namingContainer", namingContainer);
      MethodCaller.CallAction ("InitRecursive", c_bindingFlags).With (control, namingContainer);
    }
  }
}
// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Remotion.Utilities;

namespace Remotion.Reflection
{
  public class MemberLookupInfo
  {
    private readonly string _memberName;
    private readonly BindingFlags _bindingFlags;
    private readonly Binder _binder;
    private readonly CallingConventions _callingConvention;
    private readonly ParameterModifier[] _parameterModifiers;

    public MemberLookupInfo (string memberName, BindingFlags bindingFlags)
        : this (memberName, bindingFlags, null, CallingConventions.Any, null)
    {
    }

    public MemberLookupInfo (string memberName)
        : this (memberName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static)
    {
    }

    public MemberLookupInfo (
        string memberName, BindingFlags bindingFlags, Binder binder, CallingConventions callingConvention, ParameterModifier[] parameterModifiers)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("memberName", memberName);

      _memberName = memberName;
      _bindingFlags = bindingFlags;
      _binder = binder;
      _callingConvention = callingConvention;
      _parameterModifiers = parameterModifiers;
    }

    public string MemberName
    {
      get { return _memberName; }
    }

    public BindingFlags BindingFlags
    {
      get { return _bindingFlags; }
    }

    public Binder Binder
    {
      get { return _binder; }
    }

    public CallingConventions CallingConvention
    {
      get { return _callingConvention; }
    }

    public ParameterModifier[] ParameterModifiers
    {
      get { return _parameterModifiers; }
    }

    public Type[] GetParameterTypes (Type delegateType)
    {
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom("delegateType", delegateType, typeof (Delegate));

      MethodInfo invokeMethod = delegateType.GetMethod ("Invoke");
      Assertion.IsNotNull (invokeMethod, "Delegate has no Invoke() method."); // according to the CLI specs, each delegate type must define this method

      return invokeMethod.GetParameters ().Select (par => par.ParameterType).ToArray ();
    }
  }
}

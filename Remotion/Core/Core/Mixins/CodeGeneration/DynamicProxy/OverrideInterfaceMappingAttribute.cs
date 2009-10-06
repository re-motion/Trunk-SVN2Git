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
using System.Reflection;
using Remotion.Mixins.Utilities;
using Remotion.Utilities;

namespace Remotion.Mixins.CodeGeneration.DynamicProxy
{
  /// <summary>
  /// Defines a mapping between an overridden mixin member and the member in the mixin's override interface. The attribute is applied to the members
  /// of the interface so that the mixin member can be determined when needed.
  /// </summary>
  [AttributeUsage (AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
  public class OverrideInterfaceMappingAttribute : Attribute
  {
    private readonly Type _declaringType;
    private readonly string _methodName;
    private readonly string _methodSignature;

    public OverrideInterfaceMappingAttribute (Type declaringType, string methodName, string methodSignature)
    {
      ArgumentUtility.CheckNotNull ("declaringType", declaringType);
      ArgumentUtility.CheckNotNullOrEmpty ("methodName", methodName);
      ArgumentUtility.CheckNotNullOrEmpty ("methodSignature", methodSignature);

      _declaringType = declaringType;
      _methodName = methodName;
      _methodSignature = methodSignature;
    }

    public Type DeclaringType
    {
      get { return _declaringType; }
    }

    public string MethodName
    {
      get { return _methodName; }
    }

    public string MethodSignature
    {
      get { return _methodSignature; }
    }

    public MethodInfo ResolveMethod ()
    {
      return MethodResolver.ResolveMethod (DeclaringType, MethodName, MethodSignature);
    }
  }
}
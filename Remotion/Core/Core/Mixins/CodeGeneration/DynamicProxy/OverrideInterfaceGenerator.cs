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
using Remotion.Reflection.CodeGeneration;
using Remotion.Utilities;

namespace Remotion.Mixins.CodeGeneration.DynamicProxy
{
  public class OverrideInterfaceGenerator
  {
    private readonly IClassEmitter _emitter;

    public OverrideInterfaceGenerator (ICodeGenerationModule module, string typeName)
    {
      ArgumentUtility.CheckNotNull ("module", module);
      ArgumentUtility.CheckNotNullOrEmpty ("typeName", typeName);

      _emitter = module.CreateClassEmitter (typeName, typeof (object), Type.EmptyTypes, TypeAttributes.Public | TypeAttributes.Interface, false);
    }

    public MethodInfo AddOverriddenMethod (MethodInfo overriddenMethod)
    {
      ArgumentUtility.CheckNotNull ("overriddenMethod", overriddenMethod);

      var emitter = _emitter.CreateMethod (overriddenMethod.Name, MethodAttributes.Public | MethodAttributes.Abstract | MethodAttributes.Virtual);
      emitter.CopyParametersAndReturnType (overriddenMethod);
      return emitter.MethodBuilder;
    }

    public Type GetBuiltType ()
    {
      return _emitter.BuildType ();
    }
  }
}
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
using System.Reflection;
using System.Reflection.Emit;
using Remotion.Reflection.CodeGeneration;
using Remotion.Utilities;

namespace Remotion.Mixins.CodeGeneration.DynamicProxy
{
  public class OverrideInterfaceGenerator
  {
    private static readonly ConstructorInfo s_mappingAttributeCtor = 
        typeof (OverrideInterfaceMappingAttribute).GetConstructor (new[] { typeof (Type), typeof (string), typeof (string) });

    public static OverrideInterfaceGenerator CreateTopLevelGenerator (ICodeGenerationModule module, string typeName)
    {
      ArgumentUtility.CheckNotNull ("module", module);
      ArgumentUtility.CheckNotNullOrEmpty ("typeName", typeName);

      var emitter = module.CreateClassEmitter (
          typeName, 
          null, 
          Type.EmptyTypes, 
          TypeAttributes.Public | TypeAttributes.Interface | TypeAttributes.Abstract, 
          false);
      return new OverrideInterfaceGenerator (emitter);
    }

    public static OverrideInterfaceGenerator CreateNestedGenerator (IClassEmitter outerType, string typeName)
    {
      ArgumentUtility.CheckNotNull ("outerType", outerType);
      ArgumentUtility.CheckNotNullOrEmpty ("typeName", typeName);

      var emitter = outerType.CreateNestedClass (
          typeName, 
          null, 
          Type.EmptyTypes, 
          TypeAttributes.NestedPublic | TypeAttributes.Interface | TypeAttributes.Abstract);
      return new OverrideInterfaceGenerator (emitter);
    }

    private readonly IClassEmitter _emitter;
    private readonly Dictionary<MethodInfo, MethodBuilder> _interfaceMethods = new Dictionary<MethodInfo, MethodBuilder> ();

    public OverrideInterfaceGenerator (IClassEmitter emitter)
    {
      ArgumentUtility.CheckNotNull ("emitter", emitter);
      _emitter = emitter;
    }

    public MethodBuilder AddOverriddenMethod (MethodInfo overriddenMethod)
    {
      ArgumentUtility.CheckNotNull ("overriddenMethod", overriddenMethod);

      var emitter = _emitter.CreateMethod (overriddenMethod.Name, MethodAttributes.Public | MethodAttributes.Abstract | MethodAttributes.Virtual);
      emitter.CopyParametersAndReturnType (overriddenMethod);
      
      var attributeBuilder = new CustomAttributeBuilder (
          s_mappingAttributeCtor, 
          new object[] { overriddenMethod.DeclaringType, overriddenMethod.Name, overriddenMethod.ToString() });
      emitter.AddCustomAttribute (attributeBuilder);
      
      _interfaceMethods.Add (overriddenMethod, emitter.MethodBuilder);
      return emitter.MethodBuilder;
    }

    public Type GetBuiltType ()
    {
      return _emitter.BuildType ();
    }

    public Dictionary<MethodInfo, MethodBuilder> GetInterfaceMethodsForOverriddenMethods ()
    {
      return _interfaceMethods;
    }
  }
}

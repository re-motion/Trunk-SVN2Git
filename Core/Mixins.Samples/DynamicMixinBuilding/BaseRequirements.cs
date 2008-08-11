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
using System.Collections.Generic;
using System.Reflection;
using Castle.DynamicProxy;
using Remotion.Reflection.CodeGeneration;
using Remotion.Utilities;

namespace Remotion.Mixins.Samples.DynamicMixinBuilding
{
  internal class BaseRequirements
  {
    private readonly Type _requirementsType;

    private readonly IDictionary<MethodInfo, MethodInfo> _methodToInterfaceMap;

    private BaseRequirements (Type requirementsType, IDictionary<MethodInfo, MethodInfo> methodToInterfaceMap)
    {
      ArgumentUtility.CheckNotNull ("requirementsType", requirementsType);
      ArgumentUtility.CheckNotNull ("methodToInterfaceMap", methodToInterfaceMap);

      _requirementsType = requirementsType;
      _methodToInterfaceMap = methodToInterfaceMap;
    }

    public Type RequirementsType
    {
      get { return _requirementsType; }
    }

    public MethodInfo GetBaseCallMethod (MethodInfo targetMethod)
    {
      return _methodToInterfaceMap[targetMethod];
    }

    public static BaseRequirements BuildBaseRequirements (IEnumerable<MethodInfo> methodsToOverride, string typeName, ModuleScope scope)
    {
      ArgumentUtility.CheckNotNull ("methodsToOverride", methodsToOverride);
      ArgumentUtility.CheckNotNullOrEmpty ("typeName", typeName);
      ArgumentUtility.CheckNotNull ("scope", scope);

      CustomClassEmitter requirementsInterface = new CustomClassEmitter (new InterfaceEmitter (scope, typeName));
      
      Dictionary<MethodInfo, MethodInfo> methodToInterfaceMap = new Dictionary<MethodInfo, MethodInfo> ();
      foreach (MethodInfo method in methodsToOverride)
      {
        MethodInfo interfaceMethod = DefineEquivalentInterfaceMethod (requirementsInterface, method);
        methodToInterfaceMap.Add (method, interfaceMethod);
      }

      BaseRequirements result = new BaseRequirements (requirementsInterface.BuildType (), methodToInterfaceMap);
      return result;
    }

    private static MethodInfo DefineEquivalentInterfaceMethod (CustomClassEmitter emitter, MethodInfo method)
    {
      CustomMethodEmitter interfaceMethod = emitter.CreateMethod (method.Name, MethodAttributes.Public | MethodAttributes.Abstract | MethodAttributes.Virtual);
      interfaceMethod.CopyParametersAndReturnType (method);
      return interfaceMethod.MethodBuilder;
    }
  }
}

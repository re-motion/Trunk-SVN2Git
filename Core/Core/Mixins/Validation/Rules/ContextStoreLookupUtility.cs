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
using Remotion.Collections;
using Remotion.Mixins.Definitions;

namespace Remotion.Mixins.Validation.Rules
{
  public class ContextStoreLookupUtility
  {
    public IEnumerable<MethodDefinition> GetCachedMethodsByName (IDataStore<object, object> contextStore, TargetClassDefinition targetClass, string name)
    {
      Tuple<string, TargetClassDefinition> cacheKey =
          Tuple.NewTuple (typeof (DefaultMethodIntroductionRules).FullName + ".GetCachedMethodsByName", targetClass);

      MultiDictionary<string, MethodDefinition> methodDefinitions = (MultiDictionary<string, MethodDefinition>)
          contextStore.GetOrCreateValue (cacheKey, delegate { return GetUncachedMethodDefinitions (targetClass); });
      return methodDefinitions[name];
    }

    private MultiDictionary<string, MethodDefinition> GetUncachedMethodDefinitions (TargetClassDefinition targetClass)
    {
      MultiDictionary<string, MethodDefinition> methodDefinitions = new MultiDictionary<string, MethodDefinition> ();
      foreach (MethodDefinition methodDefinition in targetClass.GetAllMethods ())
        methodDefinitions.Add (methodDefinition.Name, methodDefinition);
      return methodDefinitions;
    }

    public IEnumerable<MethodIntroductionDefinition> GetCachedPublicIntroductionsByName (IDataStore<object, object> contextStore, TargetClassDefinition targetClass, string name)
    {
      Tuple<string, TargetClassDefinition> cacheKey =
          Tuple.NewTuple (typeof (DefaultMethodIntroductionRules).FullName + ".GetCachedPublicIntroductionsByName", targetClass);

      MultiDictionary<string, MethodIntroductionDefinition> methodIntroductionDefinitions = (MultiDictionary<string, MethodIntroductionDefinition>)
          contextStore.GetOrCreateValue (cacheKey, delegate { return GetUncachedIntroductions (targetClass); });
      return methodIntroductionDefinitions[name];
    }

    private MultiDictionary<string, MethodIntroductionDefinition> GetUncachedIntroductions (TargetClassDefinition targetClass)
    {
      MultiDictionary<string, MethodIntroductionDefinition> introductionDefinitions = new MultiDictionary<string, MethodIntroductionDefinition> ();
      foreach (InterfaceIntroductionDefinition interfaceIntroduction in targetClass.IntroducedInterfaces)
      {
        foreach (MethodIntroductionDefinition method in interfaceIntroduction.IntroducedMethods)
        {
          if (method.Visibility == MemberVisibility.Public)
            introductionDefinitions.Add (method.InterfaceMember.Name, method);
        }
      }
      return introductionDefinitions;
    }
  }
}
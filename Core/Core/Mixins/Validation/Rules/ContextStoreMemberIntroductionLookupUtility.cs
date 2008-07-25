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
  public class ContextStoreMemberIntroductionLookupUtility<TMemberIntroductionDefinition>
      where TMemberIntroductionDefinition : class, IMemberIntroductionDefinition
  {

    public IEnumerable<TMemberIntroductionDefinition> GetCachedPublicIntroductionsByName (IDataStore<object, object> contextStore, TargetClassDefinition targetClass, string name)
    {
      Tuple<string, TargetClassDefinition> cacheKey = Tuple.NewTuple (
          typeof (ContextStoreMemberIntroductionLookupUtility<TMemberIntroductionDefinition>).FullName + ".GetCachedPublicIntroductionsByName",
          targetClass);

      MultiDictionary<string, TMemberIntroductionDefinition> introductionDefinitions = (MultiDictionary<string, TMemberIntroductionDefinition>)
          contextStore.GetOrCreateValue (cacheKey, delegate { return GetUncachedIntroductions (targetClass); });
      return introductionDefinitions[name];
    }

    private MultiDictionary<string, TMemberIntroductionDefinition> GetUncachedIntroductions (TargetClassDefinition targetClass)
    {
      MultiDictionary<string, TMemberIntroductionDefinition> introductionDefinitions = new MultiDictionary<string, TMemberIntroductionDefinition> ();
      foreach (InterfaceIntroductionDefinition interfaceIntroduction in targetClass.ReceivedInterfaces)
      {
        foreach (IMemberIntroductionDefinition memberIntroduction in interfaceIntroduction.GetIntroducedMembers())
        {
          TMemberIntroductionDefinition castMemberIntroduction = memberIntroduction as TMemberIntroductionDefinition;
          if (castMemberIntroduction != null && castMemberIntroduction.Visibility == MemberVisibility.Public)
            introductionDefinitions.Add (castMemberIntroduction.Name, castMemberIntroduction);
        }
      }
      return introductionDefinitions;
    }
  }
}
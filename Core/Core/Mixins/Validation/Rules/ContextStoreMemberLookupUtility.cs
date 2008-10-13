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
  public class ContextStoreMemberLookupUtility<TMemberDefinition>
      where TMemberDefinition : MemberDefinitionBase
  {
    public IEnumerable<TMemberDefinition> GetCachedMembersByName (IDataStore<object, object> contextStore, TargetClassDefinition targetClass, string name)
    {
      Tuple<string, TargetClassDefinition> cacheKey =
          Tuple.NewTuple (typeof (ContextStoreMemberLookupUtility<TMemberDefinition>).FullName + ".GetCachedMembersByName", targetClass);

      var methodDefinitions = (MultiDictionary<string, TMemberDefinition>)
          contextStore.GetOrCreateValue (cacheKey, delegate { return GetUncachedMethodDefinitions (targetClass); });
      return methodDefinitions[name];
    }

    private MultiDictionary<string, TMemberDefinition> GetUncachedMethodDefinitions (TargetClassDefinition targetClass)
    {
      var memberDefinitions = new MultiDictionary<string, TMemberDefinition> ();
      foreach (MemberDefinitionBase memberDefinition in targetClass.GetAllMembers ())
      {
        var castMemberDefinition = memberDefinition as TMemberDefinition;
        if (castMemberDefinition != null)
          memberDefinitions.Add (memberDefinition.Name, castMemberDefinition);
      }
      return memberDefinitions;
    }
  }
}
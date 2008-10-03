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
using System.Runtime.InteropServices;
using Remotion.Collections;
using Remotion.Mixins.Definitions;
using Remotion.Reflection.CodeGeneration;
using Remotion.Utilities;

namespace Remotion.Mixins.CodeGeneration
{
  public class AttributeBasedMetadataImporter : IConcreteTypeMetadataImporter
  {
    // CLS-incompliant version for better testing
    [CLSCompliant (false)]
    public virtual IEnumerable<TargetClassDefinition> GetMetadataForMixedType (_Type concreteMixedType, ITargetClassDefinitionCache targetClassDefinitionCache)
    {
      ArgumentUtility.CheckNotNull ("concreteMixedType", concreteMixedType);
      ArgumentUtility.CheckNotNull ("targetClassDefinitionCache", targetClassDefinitionCache);

      foreach (ConcreteMixedTypeAttribute typeDescriptor in concreteMixedType.GetCustomAttributes (typeof (ConcreteMixedTypeAttribute), false))
      {
        TargetClassDefinition targetClassDefinition = typeDescriptor.GetTargetClassDefinition (targetClassDefinitionCache);
        yield return targetClassDefinition;
      }
    }

    // CLS-incompliant version for better testing
    [CLSCompliant (false)]
    public virtual IEnumerable<MixinDefinition> GetMetadataForMixinType (_Type concreteMixinType, ITargetClassDefinitionCache targetClassDefinitionCache)
    {
      ArgumentUtility.CheckNotNull ("concreteMixinType", concreteMixinType);
      ArgumentUtility.CheckNotNull ("targetClassDefinitionCache", targetClassDefinitionCache);

      foreach (ConcreteMixinTypeAttribute typeDescriptor in concreteMixinType.GetCustomAttributes (typeof (ConcreteMixinTypeAttribute), false))
      {
        MixinDefinition mixinDefinition = typeDescriptor.GetMixinDefinition (targetClassDefinitionCache);
        yield return mixinDefinition;
      }
    }

    // CLS-incompliant version for better testing
    [CLSCompliant (false)]
    public virtual IEnumerable<Tuple<MethodInfo, MethodInfo>> GetMethodWrappersForMixinType (_Type concreteMixinType)
    {
      ArgumentUtility.CheckNotNull ("concreteMixinType", concreteMixinType);
      foreach (MethodInfo potentialWrapper in concreteMixinType.GetMethods (BindingFlags.Instance | BindingFlags.Public))
      {
        MethodInfo wrappedMethod = GetWrappedMethod (potentialWrapper);
        if (wrappedMethod != null)
          yield return Tuple.NewTuple (wrappedMethod, potentialWrapper);
      }
    }

    private MethodInfo GetWrappedMethod (MethodInfo potentialWrapper)
    {
      var attribute = GetWrapperAttribute(potentialWrapper);
      if (attribute != null)
        return (MethodInfo) potentialWrapper.Module.ResolveMethod (attribute.WrappedMethodRefToken);
      else
        return null;
    }

    protected virtual GeneratedMethodWrapperAttribute GetWrapperAttribute(MethodInfo potentialWrapper)
    {
      return AttributeUtility.GetCustomAttribute<GeneratedMethodWrapperAttribute> (potentialWrapper, false);
    }

    public IEnumerable<TargetClassDefinition> GetMetadataForMixedType (Type concreteMixedType, ITargetClassDefinitionCache targetClassDefinitionCache)
    {
      ArgumentUtility.CheckNotNull ("concreteMixedType", concreteMixedType);
      ArgumentUtility.CheckNotNull ("targetClassDefinitionCache", targetClassDefinitionCache);

      return GetMetadataForMixedType ((_Type) concreteMixedType, targetClassDefinitionCache);
    }

    public IEnumerable<MixinDefinition> GetMetadataForMixinType (Type concreteMixinType, ITargetClassDefinitionCache targetClassDefinitionCache)
    {
      ArgumentUtility.CheckNotNull ("concreteMixinType", concreteMixinType);
      ArgumentUtility.CheckNotNull ("targetClassDefinitionCache", targetClassDefinitionCache);

      return (GetMetadataForMixinType ((_Type) concreteMixinType, targetClassDefinitionCache));
    }

    public IEnumerable<Tuple<MethodInfo, MethodInfo>> GetMethodWrappersForMixinType(Type concreteMixinType)
    {
      ArgumentUtility.CheckNotNull ("concreteMixinType", concreteMixinType);
      return GetMethodWrappersForMixinType ((_Type) concreteMixinType);
    }
  }
}
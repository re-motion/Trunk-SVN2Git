// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
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
        return attribute.ResolveWrappedMethod(potentialWrapper.Module);
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

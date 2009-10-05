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
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using Remotion.Collections;
using Remotion.Reflection.CodeGeneration;
using Remotion.Utilities;
using System.Linq;
using Remotion.Mixins.Context;

namespace Remotion.Mixins.CodeGeneration
{
  public class AttributeBasedMetadataImporter : IConcreteTypeMetadataImporter
  {
    // CLS-incompliant version for better testing
    [CLSCompliant (false)]
    public virtual ClassContext GetMetadataForMixedType (_Type concreteMixedType)
    {
      ArgumentUtility.CheckNotNull ("concreteMixedType", concreteMixedType);

      var attribute = 
          (ConcreteMixedTypeAttribute) concreteMixedType.GetCustomAttributes (typeof (ConcreteMixedTypeAttribute), false).SingleOrDefault ();
      if (attribute != null)
        return attribute.GetClassContext ();
      else
        return null;
    }

    // CLS-incompliant version for better testing
    [CLSCompliant (false)]
    public virtual ConcreteMixinTypeIdentifier GetIdentifierForMixinType (_Type concreteMixinType)
    {
      ArgumentUtility.CheckNotNull ("concreteMixinType", concreteMixinType);

      var attribute = 
          (ConcreteMixinTypeAttribute) concreteMixinType.GetCustomAttributes (typeof (ConcreteMixinTypeAttribute), false).SingleOrDefault ();
      if (attribute != null)
        return attribute.GetIdentifier ();
      else
        return null;
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
        return attribute.ResolveWrappedMethod (potentialWrapper.Module);
      else
        return null;
    }

    // This is a separate method in order to be able to test it with Rhino.Mocks.
    protected virtual GeneratedMethodWrapperAttribute GetWrapperAttribute (MethodInfo potentialWrapper)
    {
      return AttributeUtility.GetCustomAttribute<GeneratedMethodWrapperAttribute> (potentialWrapper, false);
    }

    public ClassContext GetMetadataForMixedType (Type concreteMixedType)
    {
      ArgumentUtility.CheckNotNull ("concreteMixedType", concreteMixedType);

      return GetMetadataForMixedType ((_Type) concreteMixedType);
    }

    public ConcreteMixinType GetMetadataForMixinType (Type concreteMixinType)
    {
      ArgumentUtility.CheckNotNull ("concreteMixinType", concreteMixinType);

      var identifier = GetIdentifierForMixinType (concreteMixinType);
      if (identifier == null)
        return null;

      var generatedOverrideInterface = concreteMixinType.GetNestedType ("IOverriddenMethods");
      if (generatedOverrideInterface == null)
      {
        var message = string.Format (
            "The given type '{0}' has a concrete mixin type identifier, but no IOverriddenMethods interface.", 
            concreteMixinType);
        throw new TypeImportException (message);
      }

      var result = new ConcreteMixinType (identifier, concreteMixinType, generatedOverrideInterface);
      var methodWrappers = GetMethodWrappersForMixinType (concreteMixinType);
      foreach (Tuple<MethodInfo, MethodInfo> wrapper in methodWrappers)
        result.AddMethodWrapper (wrapper.A, wrapper.B);

      return result;
    }
  }
}

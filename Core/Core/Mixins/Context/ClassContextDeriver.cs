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
using Remotion.Collections;
using Remotion.Utilities;

namespace Remotion.Mixins.Context
{
  public class ClassContextDeriver
  {
    public static readonly ClassContextDeriver Instance = new ClassContextDeriver();
    
    public ClassContext DeriveContext (ClassContext contextToBeDerived, IEnumerable<ClassContext> baseContexts)
    {
      ArgumentUtility.CheckNotNull ("contextToBeDerived", contextToBeDerived);
      ArgumentUtility.CheckNotNull ("baseContexts", baseContexts);

      List<MixinContext> mixins;
      List<Type> interfaces;
      mixins = new List<MixinContext> (contextToBeDerived.Mixins);
      interfaces = new List<Type> (contextToBeDerived.CompleteInterfaces);

      foreach (ClassContext baseContext in baseContexts)
        ApplyInheritance (contextToBeDerived.Type, contextToBeDerived.Mixins, baseContext, mixins, interfaces);

      return new ClassContext (contextToBeDerived.Type, mixins, interfaces);
    }

    public void ApplyInheritance (Type targetClass, IEnumerable<MixinContext> ownMixins, ClassContext baseContext, ICollection<MixinContext> mixins, ICollection<Type> interfaces)
    {
      Tuple<MixinContext, MixinContext> overridden_override = GetFirstOverrideThatIsNotOverriddenByBase (mixins, baseContext.Mixins);
      if (overridden_override != null)
      {
        string message = string.Format (
            "The class {0} inherits the mixin {1} from class {2}, but it is explicitly "
                + "configured for the less specific mixin {3}.",
            targetClass.FullName,
            overridden_override.B.MixinType.FullName,
            baseContext.Type.FullName,
            overridden_override.A.MixinType);
        throw new ConfigurationException (message);
      }

      ApplyInheritanceForMixins (ownMixins, baseContext, mixins);
      ApplyInheritanceForInterfaces (baseContext, interfaces);
    }

    public void ApplyInheritanceForMixins (IEnumerable<MixinContext> ownMixins, ClassContext baseContext, ICollection<MixinContext> mixins)
    {
      foreach (MixinContext inheritedMixin in baseContext.Mixins)
      {
        if (!MixinContextCollection.ContainsOverrideForMixin (ownMixins, inheritedMixin.MixinType))
          mixins.Add (inheritedMixin);
      }
    }

    public void ApplyInheritanceForInterfaces (ClassContext baseContext, ICollection<Type> interfaces)
    {
      foreach (Type inheritedInterface in baseContext.CompleteInterfaces)
      {
        if (!interfaces.Contains (inheritedInterface))
          interfaces.Add (inheritedInterface);
      }
    }

    // A = overridden, B = override
    public Tuple<MixinContext, MixinContext> GetFirstOverrideThatIsNotOverriddenByBase (IEnumerable<MixinContext> baseMixins,
        IEnumerable<MixinContext> potentialOverrides)
    {
      foreach (MixinContext mixin in baseMixins)
      {
        MixinContext overrideForMixin;
        if ((overrideForMixin = MixinContextCollection.GetOverrideForMixin (potentialOverrides, mixin.MixinType)) != null
            && !MixinContextCollection.ContainsOverrideForMixin (baseMixins, overrideForMixin.MixinType))
          return Tuple.NewTuple (mixin, overrideForMixin);
      }
      return null;
    }
  }
}

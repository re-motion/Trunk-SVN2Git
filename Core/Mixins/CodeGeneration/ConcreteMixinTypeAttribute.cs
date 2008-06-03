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
using Remotion.Mixins.Context;
using Remotion.Mixins.Definitions;

namespace Remotion.Mixins.CodeGeneration
{
  [CLSCompliant (false)]
  [AttributeUsage (AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
  public class ConcreteMixinTypeAttribute : ConcreteMixedTypeAttribute
  {
    public static ConcreteMixinTypeAttribute FromClassContext (int mixinIndex, ClassContext targetClassContext)
    {
      ConcreteMixedTypeAttribute baseAttribute = ConcreteMixedTypeAttribute.FromClassContext (targetClassContext);
      return new ConcreteMixinTypeAttribute (mixinIndex, baseAttribute.TargetType, baseAttribute.MixinKinds, baseAttribute.MixinTypes, 
          baseAttribute.CompleteInterfaces, baseAttribute.ExplicitDependenciesPerMixin);
    }

    private readonly int _mixinIndex;

    public ConcreteMixinTypeAttribute (int mixinIndex, Type baseType, MixinKind[] mixinKinds, Type[] mixinTypes, Type[] completeInterfaces, Type[] explicitDependenciesPerMixin)
        : base (baseType, mixinKinds, mixinTypes, completeInterfaces, explicitDependenciesPerMixin)
    {
      _mixinIndex = mixinIndex;
    }

    public int MixinIndex
    {
      get { return _mixinIndex; }
    }

    public MixinDefinition GetMixinDefinition ()
    {
      return GetTargetClassDefinition ().Mixins[MixinIndex];
    }
  }
}

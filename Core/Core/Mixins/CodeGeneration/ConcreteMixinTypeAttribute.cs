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
      ConcreteMixedTypeAttribute baseAttribute = FromClassContext (targetClassContext);
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

    public MixinDefinition GetMixinDefinition (ITargetClassDefinitionCache targetClassDefinitionCache)
    {
      return GetTargetClassDefinition (targetClassDefinitionCache).Mixins[MixinIndex];
    }
  }
}

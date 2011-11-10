// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) rubicon IT GmbH, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
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
using Remotion.Mixins.BridgeInterfaces;
using Remotion.Mixins.CodeGeneration;
using Remotion.Utilities;

namespace Remotion.Mixins.BridgeImplementations
{
  public class TypeFactoryImplementation : ITypeFactoryImplementation
  {
    public Type GetConcreteType (Type targetOrConcreteType, GenerationPolicy generationPolicy)
    {
      ArgumentUtility.CheckNotNull ("targetOrConcreteType", targetOrConcreteType);
      var classContext = 
          generationPolicy == GenerationPolicy.ForceGeneration 
              ? MixinConfiguration.ActiveConfiguration.GetContextForce (targetOrConcreteType)
              : MixinConfiguration.ActiveConfiguration.GetContext (targetOrConcreteType);

      if (classContext == null)
        return targetOrConcreteType;
      else
        return ConcreteTypeBuilder.Current.GetConcreteType (classContext);
    }

    public void InitializeUnconstructedInstance (object mixinTarget)
    {
      ArgumentUtility.CheckNotNull ("mixinTarget", mixinTarget);
      ArgumentUtility.CheckType<IMixinTarget> ("mixinTarget", mixinTarget);
      ConcreteTypeBuilder.Current.InitializeUnconstructedInstance ((IMixinTarget) mixinTarget);
    }
  }
}

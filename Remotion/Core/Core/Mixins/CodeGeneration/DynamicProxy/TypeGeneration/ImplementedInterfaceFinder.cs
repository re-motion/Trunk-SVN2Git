// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Remotion.Mixins.Definitions;
using Remotion.Utilities;

namespace Remotion.Mixins.CodeGeneration.DynamicProxy.TypeGeneration
{
  /// <summary>
  /// Finds the interfaces to be implemented by the generated type.
  /// </summary>
  public class ImplementedInterfaceFinder
  {
    private readonly IEnumerable<ConcreteMixinType> _concreteMixinTypes;
    private readonly IEnumerable<InterfaceIntroductionDefinition> _receivedInterfaces;
    private readonly IEnumerable<Type> _alreadyImplementedInterfaces;
    private readonly IEnumerable<RequiredFaceTypeDefinition> _requiredFaceTypes;

    public ImplementedInterfaceFinder (
        IEnumerable<Type> alreadyImplementedInterfaces, 
        IEnumerable<InterfaceIntroductionDefinition> receivedInterfaces, 
        IEnumerable<RequiredFaceTypeDefinition> requiredFaceTypes, 
        IEnumerable<ConcreteMixinType> concreteMixinTypes)
    {
      ArgumentUtility.CheckNotNull ("alreadyImplementedInterfaces", alreadyImplementedInterfaces);
      ArgumentUtility.CheckNotNull ("receivedInterfaces", receivedInterfaces);
      ArgumentUtility.CheckNotNull ("requiredFaceTypes", requiredFaceTypes);
      ArgumentUtility.CheckNotNull ("concreteMixinTypes", concreteMixinTypes);

      _alreadyImplementedInterfaces = alreadyImplementedInterfaces;
      _receivedInterfaces = receivedInterfaces;
      _requiredFaceTypes = requiredFaceTypes;
      _concreteMixinTypes = concreteMixinTypes;
    }

    public Type[] GetInterfacesToImplement ()
    {
      var interfaces = new HashSet<Type> ();
      interfaces.UnionWith (_requiredFaceTypes
                                .Select (faceTypeDefinition => faceTypeDefinition.Type)
                                .Where (t => t.IsInterface));
      interfaces.ExceptWith (_alreadyImplementedInterfaces); // remove required interfaces the type already implements

      interfaces.UnionWith (_receivedInterfaces.Select (introduction => introduction.InterfaceType));
      interfaces.UnionWith (_concreteMixinTypes.Select (concreteMixinType => concreteMixinType.GeneratedOverrideInterface));
      interfaces.Add (typeof (IMixinTarget));
      interfaces.Add (typeof (IInitializableMixinTarget));
      interfaces.Add (typeof (ISerializable));

      return interfaces.ToArray ();
    }
  }
}

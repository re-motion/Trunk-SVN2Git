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
using Remotion.Implementation;
using Remotion.Mixins.BridgeInterfaces;

namespace Remotion.Mixins
{
  /// <summary>
  /// Provides a central point for reflectively working with mixin targets and generated concrete types.
  /// </summary>
  public static class MixinTypeUtility
  {
    /// <summary>
    /// Determines whether the given <paramref name="type"/> is a concrete, mixed type generated by the mixin infrastructure.
    /// </summary>
    /// <param name="type">The type to be checked.</param>
    /// <returns>
    /// True if <paramref name="type"/> or one of its base types was generated by the mixin infrastructure as a concrete, mixed type; otherwise, false.
    /// </returns>
    public static bool IsGeneratedConcreteMixedType (Type type)
    {
      return VersionDependentImplementationBridge<IMixinTypeUtilityImplementation>.Implementation.IsGeneratedConcreteMixedType (type);
    }

    /// <summary>
    /// Determines whether the given <paramref name="type"/> was generated by the mixin infrastructure.
    /// </summary>
    /// <param name="type">The type to be checked.</param>
    /// <returns>
    /// True if <paramref name="type"/> or one of its base types was generated by the mixin infrastructure. It might be a concrete, mixed type,
    /// a derived mixin type, or any other type needed by the mixin infrastructure.
    /// </returns>
    public static bool IsGeneratedByMixinEngine (Type type)
    {
      return VersionDependentImplementationBridge<IMixinTypeUtilityImplementation>.Implementation.IsGeneratedByMixinEngine (type);
    }

    /// <summary>
    /// Gets the concrete type for a given <paramref name="baseType"/> which contains all mixins currently configured for the type.
    /// </summary>
    /// <param name="baseType">The base type for which to retrieve a concrete type.</param>
    /// <returns>The <paramref name="baseType"/> itself if there are no mixins configured for the type or if the type itself is a generated type;
    /// otherwise, a generated type containing all the mixins currently configured for <paramref name="baseType"/>.</returns>
    public static Type GetConcreteMixedType (Type baseType)
    {
      return VersionDependentImplementationBridge<IMixinTypeUtilityImplementation>.Implementation.GetConcreteMixedType (baseType);
    }

    /// <summary>
    /// Gets the underlying target type for a given <paramref name="type"/>.
    /// </summary>
    /// <param name="type">The type to get the underlying target type for.</param>
    /// <returns>The <paramref name="type"/> itself if it is not a generated type; otherwise, the type that was used as a target type when the
    /// given <paramref name="type"/> was generated.</returns>
    public static Type GetUnderlyingTargetType (Type type)
    {
      return VersionDependentImplementationBridge<IMixinTypeUtilityImplementation>.Implementation.GetUnderlyingTargetType (type);
    }

    /// <summary>
    /// Determines whether the given <paramref name="typeToAssign"/> would be assignable to <paramref name="baseOrInterface"/> after all mixins
    /// currently configured for the type have been taken into account.
    /// </summary>
    /// <param name="baseOrInterface">The base or interface to assign to.</param>
    /// <param name="typeToAssign">The type to check for assignment compatibility to <paramref name="baseOrInterface"/>.</param>
    /// <returns>
    /// True if the type returned by <see cref="GetConcreteMixedType"/> for <paramref name="typeToAssign"/> is the same as, derived from, or an
    /// implementation of <paramref name="baseOrInterface"/>; otherwise, false.
    /// </returns>
    public static bool IsAssignableFrom (Type baseOrInterface, Type typeToAssign)
    {
      return VersionDependentImplementationBridge<IMixinTypeUtilityImplementation>.Implementation.IsAssignableFrom (baseOrInterface, typeToAssign);
    }

    /// <summary>
    /// Determines whether the specified <paramref name="type"/> is associated with any mixins.
    /// </summary>
    /// <param name="type">The type to check for mixins.</param>
    /// <returns>
    /// True if the specified type is a generated type containing any mixins or a base type for which there are mixins currently configured;
    /// otherwise, false.
    /// </returns>
    public static bool HasMixins (Type type)
    {
      return VersionDependentImplementationBridge<IMixinTypeUtilityImplementation>.Implementation.HasMixins (type);
    }

    /// <summary>
    /// Determines whether the specified <paramref name="typeToCheck"/> is associated with a mixin of the given <paramref name="mixinType"/>.
    /// </summary>
    /// <param name="typeToCheck">The type to check.</param>
    /// <param name="mixinType">The mixin type to check for.</param>
    /// <returns>
    /// True if the specified type is a generated type containing a mixin of the given <paramref name="mixinType"/> or a base type currently
    /// configured with such a mixin; otherwise, false.
    /// </returns>
    /// <remarks>This method checks for the exact mixin type, it does not take assignability or generic type instantiations into account. If the
    /// check should be broadened to include these properties, <see cref="GetAscribableMixinType"/> should be used.</remarks>
    public static bool HasMixin (Type typeToCheck, Type mixinType)
    {
      return VersionDependentImplementationBridge<IMixinTypeUtilityImplementation>.Implementation.HasMixin (typeToCheck, mixinType);
    }

    /// <summary>
    /// Determines whether the specified <paramref name="typeToCheck"/> is associated with a mixin that can be ascribed to the given
    /// <paramref name="mixinType"/> and returns the respective mixin type.
    /// </summary>
    /// <param name="typeToCheck">The type to check.</param>
    /// <param name="mixinType">The mixin type to check for.</param>
    /// <returns>
    /// The exact mixin type if the specified type is a generated type containing a mixin that can be ascribed to <paramref name="mixinType"/> or a
    /// base type currently configured with such a mixin; otherwise <see langword="null"/>.
    /// </returns>
    public static Type GetAscribableMixinType (Type typeToCheck, Type mixinType)
    {
      return VersionDependentImplementationBridge<IMixinTypeUtilityImplementation>.Implementation.GetAscribableMixinType (typeToCheck, mixinType);
    }

    /// <summary>
    /// Determines whether the specified <paramref name="typeToCheck"/> is associated with a mixin that can be ascribed to the given
    /// <paramref name="mixinType"/>.
    /// </summary>
    /// <param name="typeToCheck">The type to check.</param>
    /// <param name="mixinType">The mixin type to check for.</param>
    /// <returns>
    /// True, if the specified type is a generated type containing a mixin that can be ascribed to <paramref name="mixinType"/> or a
    /// base type currently configured with such a mixin; otherwise false.
    /// </returns>
    public static bool HasAscribableMixin (Type typeToCheck, Type mixinType)
    {
      return VersionDependentImplementationBridge<IMixinTypeUtilityImplementation>.Implementation.HasAscribableMixin (typeToCheck, mixinType);
    }

    /// <summary>
    /// Gets the mixin types associated with the given <paramref name="type"/>.
    /// </summary>
    /// <param name="type">The type to check for mixin types.</param>
    /// <returns>The mixins included in <paramref name="type"/> if it is a generated type; otherwise the mixins currently configured for
    /// <paramref name="type"/>.</returns>
    public static IEnumerable<Type> GetMixinTypes (Type type)
    {
      return VersionDependentImplementationBridge<IMixinTypeUtilityImplementation>.Implementation.GetMixinTypes (type);
    }

    /// <summary>
    /// Creates an instance of the type returned by <see cref="GetConcreteMixedType"/> for the given <paramref name="type"/>.
    /// </summary>
    /// <param name="type">The type for whose concrete type to create an instance.</param>
    /// <param name="args">The arguments to be passed to the constructor.</param>
    /// <returns>An instance of the type returned by <see cref="GetConcreteMixedType"/> for <paramref name="type"/> created via a constructor taking the
    /// specified <paramref name="args"/>.</returns>
    public static object CreateInstance (Type type, params object[] args)
    {
      return VersionDependentImplementationBridge<IMixinTypeUtilityImplementation>.Implementation.CreateInstance (type, args);
    }
  }
}

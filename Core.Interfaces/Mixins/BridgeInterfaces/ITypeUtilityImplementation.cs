using System;
using System.Collections.Generic;
using Remotion.Implementation;

namespace Remotion.Mixins.BridgeInterfaces
{
  [ConcreteImplementation ("Remotion.Mixins.BridgeImplementations.TypeUtilityImplementation, Remotion, Version = <version>")]
  public interface ITypeUtilityImplementation
  {
    bool IsGeneratedConcreteMixedType (Type type);
    bool IsGeneratedByMixinEngine (Type type);
    Type GetConcreteMixedType (Type baseType);
    Type GetUnderlyingTargetType (Type type);
    bool IsAssignableFrom (Type baseOrInterface, Type typeToAssign);
    bool HasMixins (Type type);
    bool HasMixin (Type typeToCheck, Type mixinType);
    Type GetAscribableMixinType (Type typeToCheck, Type mixinType);
    bool HasAscribableMixin (Type typeToCheck, Type mixinType);
    IEnumerable<Type> GetMixinTypes (Type type);
    object CreateInstance (Type type, params object[] args);
  }
}
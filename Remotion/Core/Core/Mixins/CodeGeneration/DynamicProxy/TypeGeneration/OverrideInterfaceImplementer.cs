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
using System.Linq;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using Remotion.Mixins.Definitions;
using Remotion.Reflection.CodeGeneration;
using Remotion.Reflection.CodeGeneration.DPExtensions;
using Remotion.Utilities;

namespace Remotion.Mixins.CodeGeneration.DynamicProxy.TypeGeneration
{
  /// <summary>
  /// When a concrete mixed type is generated, this class implements the override interfaces of the mixins applied to the target class.
  /// </summary>
  public class OverrideInterfaceImplementer
  {
    private readonly TargetClassDefinition _targetClassDefinition;
    private readonly ConcreteMixinType[] _overriddenMixins;

    public OverrideInterfaceImplementer (TargetClassDefinition targetClassDefinition, ConcreteMixinType[] overriddenMixins)
    {
      ArgumentUtility.CheckNotNull ("targetClassDefinition", targetClassDefinition);
      ArgumentUtility.CheckNotNull ("overriddenMixins", overriddenMixins);

      _targetClassDefinition = targetClassDefinition;
      _overriddenMixins = overriddenMixins;
    }

    // It's necessary to explicitly implement some members defined by the concrete mixins' override interfaces: implicit implementation doesn't
    // work if the overrider is non-public or generic. Because it's simpler, we just implement all the members explicitly.
    public void ImplementOverridingMethods (IClassEmitter emitter)
    {
      ArgumentUtility.CheckNotNull ("emitter", emitter);

      var selfReference = new TypeReferenceWrapper (SelfReference.Self, _targetClassDefinition.Type);

      var overriders = _targetClassDefinition.GetAllMethods ().Where (methodDefinition => methodDefinition.Base != null);
      foreach (var overrider in overriders)
      {
        var mixin = overrider.Base.DeclaringClass as MixinDefinition;
        Assertion.IsNotNull (mixin, "We only support mixins as overriders of target class members.");
        // ReSharper disable PossibleNullReferenceException
        var mixinIndex = mixin.MixinIndex;
        // ReSharper restore PossibleNullReferenceException

        var concreteMixinType = _overriddenMixins[mixinIndex];
        Assertion.IsNotNull (concreteMixinType, "If a mixin method is overridden, a concrete type must have been created for it.");
        var methodInOverrideInterface = concreteMixinType.GetOverrideInterfaceMethod (overrider.Base.MethodInfo);

        emitter.CreateInterfaceMethodImplementation (methodInOverrideInterface).ImplementByDelegating (selfReference, overrider.MethodInfo);
      }
    }
  }
}
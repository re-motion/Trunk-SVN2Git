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
using Remotion.Mixins.CodeGeneration;
using Remotion.Mixins.Context;
using Remotion.Mixins.Definitions;
using Remotion.Utilities;

namespace Remotion.Mixins
{
  /// <summary>
  /// Provides methods that support isolated testing of mixins by initializing them with mock versions of their TThis and TBase parameters.
  /// </summary>
  public static class MixinTargetMockUtility
  {
    /// <summary>
    /// Mocks the target of the given mixin instance by setting or replacing its <see cref="Mixin{TThis}.This"/> and
    /// <see cref="Mixin{TThis,TBase}.Base"/> properties to/with the given mocks.
    /// </summary>
    /// <typeparam name="TThis">The type of the mixin's TThis parameter.</typeparam>
    /// <typeparam name="TBase">The type of the mixin's TBase parameter.</typeparam>
    /// <param name="mixin">The mixin whose target is to be mocked.</param>
    /// <param name="thisMock">The mock object to use for the mixin's <see cref="Mixin{TThis}.This"/> property.</param>
    /// <param name="baseMock">The mock object to use for the mixin's <see cref="Mixin{TThis,TBase}.Base"/> property.</param>
    /// <remarks>
    /// Use this method if you already have a mixin instance. To mock the target of an abstract mixin which can't be instantiated in isolation,
    /// use the <see cref="CreateMixinWithMockedTarget{TMixin,TThis,TBase}"/> method.
    /// </remarks>
    public static void MockMixinTarget<TThis, TBase> (Mixin<TThis, TBase> mixin, TThis thisMock, TBase baseMock)
        where TThis : class
        where TBase: class
    {
      ArgumentUtility.CheckNotNull ("mixin", mixin);
      ArgumentUtility.CheckNotNull ("thisMock", thisMock);
      ArgumentUtility.CheckNotNull ("baseMock", baseMock);

      ((IInitializableMixin)mixin).Initialize (thisMock, baseMock, false);
    }

    /// <summary>
    /// Mocks the target of the given mixin instance by setting or replacing its <see cref="Mixin{TThis}.This"/> property to/with the given mocks.
    /// </summary>
    /// <typeparam name="TThis">The type of the mixin's TThis parameter.</typeparam>
    /// <param name="mixin">The mixin whose target is to be mocked.</param>
    /// <param name="thisMock">The mock object to use for the mixin's <see cref="Mixin{TThis}.This"/> property.</param>
    /// <remarks>
    /// Use this method if you already have a mixin instance. To mock the target of an abstract mixin which can't be instantiated in isolation,
    /// use the <see cref="CreateMixinWithMockedTarget{TMixin,TThis}"/> method.
    /// </remarks>
    public static void MockMixinTarget<TThis> (Mixin<TThis> mixin, TThis thisMock)
        where TThis : class
    {
      ArgumentUtility.CheckNotNull ("mixin", mixin);
      ArgumentUtility.CheckNotNull ("thisMock", thisMock);

      ((IInitializableMixin) mixin).Initialize (thisMock, null, false);
    }

    /// <summary>
    /// Creates a mixin with a mocked target object.
    /// </summary>
    /// <typeparam name="TMixin">The type of mixin to create.</typeparam>
    /// <typeparam name="TThis">The TThis parameter of the mixin.</typeparam>
    /// <typeparam name="TBase">The TBase parameter of the mixin.</typeparam>
    /// <param name="thisMock">The mock object to use for the mixin's <see cref="Mixin{TThis}.This"/> property.</param>
    /// <param name="baseMock">The mock object to use for the mixin's <see cref="Mixin{TThis,TBase}.Base"/> property.</param>
    /// <param name="args">The constructor arguments to be used when instantiating the mixin.</param>
    /// <returns>A mixin instance with the given mock objects as its <see cref="Mixin{TThis}.This"/> and <see cref="Mixin{TThis,TBase}.Base"/>
    /// parameters.</returns>
    /// <remarks>
    /// <para>
    /// This method is useful if a mixin's method should be overridden by the <paramref name="thisMock"/>, especially if the mixin type
    /// is abstract. If you already have a mixin instance to be mocked, use the <see cref="MockMixinTarget{TThis,TBase}"/> method instead.
    /// </para>
    /// <para>
    /// For use with Rhino Mocks, be sure to configure the current <see cref="ConcreteTypeBuilder"/>'s <see cref="IModuleManager"/> to
    /// generate transient modules instead of persistent ones (which is the default). The following example code shows how to do this:
    /// <code>
    /// ((Remotion.Mixins.CodeGeneration.DynamicProxy.ModuleManager) ConcreteTypeBuilder.Current.Scope).Scope = new ModuleScope (false);
    /// </code>
    /// </para>
    /// </remarks>
    public static TMixin CreateMixinWithMockedTarget<TMixin, TThis, TBase> (TThis thisMock, TBase baseMock, params object[] args)
        where TThis : class
        where TBase : class
        where TMixin : Mixin<TThis, TBase>
    {
      ArgumentUtility.CheckNotNull ("thisMock", thisMock);
      ArgumentUtility.CheckNotNull ("baseMock", baseMock);
      ArgumentUtility.CheckNotNull ("args", args);

      MixinDefinition mixinDefinition = GetMixinDefinitionForThisMock<TThis, TMixin> (thisMock);
      TMixin mixin = CreateMixinForMocking<TMixin> (mixinDefinition, args);
      MockMixinTarget (mixin, thisMock, baseMock);
      return mixin;
    }

    /// <summary>
    /// Creates a mixin with a mocked target object.
    /// </summary>
    /// <typeparam name="TMixin">The type of mixin to create.</typeparam>
    /// <typeparam name="TThis">The TThis parameter of the mixin.</typeparam>
    /// <param name="thisMock">The mock object to use for the mixin's <see cref="Mixin{TThis}.This"/> property.</param>
    /// <param name="args">The constructor arguments to be used when instantiating the mixin.</param>
    /// <returns>A mixin instance with the given mock objects as its <see cref="Mixin{TThis}.This"/> and <see cref="Mixin{TThis,TBase}.Base"/>
    /// parameters.</returns>
    /// <remarks>
    /// <para>
    /// This method is useful if a mixin's method should be overridden by the <paramref name="thisMock"/>, especially if the mixin type
    /// is abstract. If you already have a mixin instance to be mocked, use the <see cref="MockMixinTarget{TThis}"/> method instead.
    /// </para>
    /// <para>
    /// For use with Rhino Mocks, be sure to configure the current <see cref="ConcreteTypeBuilder"/>'s <see cref="IModuleManager"/> to
    /// generate transient modules instead of persistent ones (which is the default). The following example code shows how to do this:
    /// <code>
    /// ((Remotion.Mixins.CodeGeneration.DynamicProxy.ModuleManager) ConcreteTypeBuilder.Current.Scope).Scope = new ModuleScope (false);
    /// </code>
    /// </para>
    /// </remarks>
    public static TMixin CreateMixinWithMockedTarget<TMixin, TThis> (TThis thisMock, params object[] args)
      where TThis : class
      where TMixin : Mixin<TThis>
    {
      ArgumentUtility.CheckNotNull ("thisMock", thisMock);
      ArgumentUtility.CheckNotNull ("args", args);

      MixinDefinition mixinDefinition = GetMixinDefinitionForThisMock<TThis, TMixin> (thisMock);
      TMixin mixin = CreateMixinForMocking<TMixin> (mixinDefinition, args);
      MockMixinTarget (mixin, thisMock);
      return mixin;
    }

    // Creates a dedicated MixinDefinition of TMixin in the context of thisMock's type.
    private static MixinDefinition GetMixinDefinitionForThisMock<TThis, TMixin> (TThis thisMock)
    {
      ClassContext classContext = new ClassContext (thisMock.GetType (), typeof (TMixin));
      TargetClassDefinition classDefinition = TargetClassDefinitionCache.Current.GetTargetClassDefinition (classContext);
      return classDefinition.Mixins[0];
    }

    private static TMixin CreateMixinForMocking<TMixin> (MixinDefinition mixinDefinition, object[] args)
    {
      Type mixinType;
      if (mixinDefinition.NeedsDerivedMixinType ())
        mixinType = ConcreteTypeBuilder.Current.GetConcreteMixinType (mixinDefinition).GeneratedType;
      else
        mixinType = typeof (TMixin);

      return (TMixin) ObjectFactory.Create (true, mixinType).Invoke (args);
    }

    public static void SignalOnDeserialization<TThis> (Mixin<TThis> mixin, TThis thisMock) 
        where TThis : class
    {
      ArgumentUtility.CheckNotNull ("mixin", mixin);
      ((IInitializableMixin) mixin).Initialize (thisMock, null, true);
    }

    public static void SignalOnDeserialization<TThis, TBase> (Mixin<TThis, TBase> mixin, TThis thisMock, TBase baseMock)
        where TThis : class
        where TBase : class
    {
      ArgumentUtility.CheckNotNull ("mixin", mixin);
      ((IInitializableMixin) mixin).Initialize (thisMock, baseMock, true);
    }
  }
}

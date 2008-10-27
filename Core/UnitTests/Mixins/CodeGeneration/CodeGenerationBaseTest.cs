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
using NUnit.Framework;
using Remotion.Mixins;
using Remotion.Mixins.CodeGeneration;
using Remotion.Reflection;

namespace Remotion.UnitTests.Mixins.CodeGeneration
{
  public abstract class CodeGenerationBaseTest
  {
    private IModuleManager _savedScope;
    private IModuleManager _alternativeScope;

    [SetUp]
    public virtual void SetUp()
    {
      _savedScope = SavedTypeBuilder.Scope;
      _alternativeScope = AlternativeTypeBuilder.Scope;
      ConcreteTypeBuilder.SetCurrent (SavedTypeBuilder);
    }

    [TearDown]
    public virtual void TearDown()
    {
      ConcreteTypeBuilder.SetCurrent (null);
      SavedTypeBuilder.Scope = _savedScope;
      AlternativeTypeBuilder.Scope = _alternativeScope;
    }

    public ConcreteTypeBuilder SavedTypeBuilder
    {
      get { return SetUpFixture.SavedTypeBuilder; }
    }

    public ConcreteTypeBuilder AlternativeTypeBuilder
    {
      get { return SetUpFixture.AlternativeTypeBuilder; }
    }

    public Type CreateMixedType (Type targetType, params Type[] mixinTypes)
    {
      using (MixinConfiguration.BuildFromActive().ForClass (targetType).Clear().AddMixins (mixinTypes).EnterScope())
        return TypeFactory.GetConcreteType (targetType, GenerationPolicy.ForceGeneration);
    }

    public FuncInvokerWrapper<T> CreateMixedObject<T> (params Type[] mixinTypes)
    {
      using (MixinConfiguration.BuildNew().ForClass<T> ().AddMixins (mixinTypes).EnterScope())
        return ObjectFactory.Create<T>(GenerationPolicy.ForceGeneration);
    }
  }
}
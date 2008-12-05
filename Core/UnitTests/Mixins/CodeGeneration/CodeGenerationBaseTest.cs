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

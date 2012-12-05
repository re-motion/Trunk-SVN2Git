// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using NUnit.Framework;
using Remotion.Mixins.CodeGeneration;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Mixins.UnitTests.Core.CodeGeneration
{
  public abstract class CodeGenerationBaseTest
  {
    [SetUp]
    public virtual void SetUp()
    {
      ConcreteTypeBuilder.SetCurrent (SavedTypeBuilder);
    }

    [TearDown]
    public virtual void TearDown()
    {
      ConcreteTypeBuilder.SetCurrent (null);
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
      ArgumentUtility.CheckNotNullOrEmpty ("mixinTypes", mixinTypes);
      
      // TODO 3213: Inconsistent - cleared here, but not in CreateMixedObject?
      using (MixinConfiguration.BuildFromActive().ForClass (targetType).Clear().AddMixins (mixinTypes).EnterScope())
        return TypeFactory.GetConcreteType (targetType);
    }

    public T CreateMixedObject<T> (params Type[] mixinTypes)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("mixinTypes", mixinTypes);

      using (MixinConfiguration.BuildNew().ForClass<T> ().AddMixins (mixinTypes).EnterScope())
        return ObjectFactory.Create<T> (ParamList.Empty);
    }

    public Type CreateGeneratedTypeWithoutMixins (Type targetType)
    {
      using (MixinConfiguration.BuildNew ().ForClass (targetType).Clear ().EnterScope ())
        return TypeGenerationHelper.ForceTypeGeneration (targetType);
    }

    public T CreateGeneratedTypeInstanceWithoutMixins<T> ()
    {
      using (MixinConfiguration.BuildNew ().ForClass<T> ().Clear ().EnterScope ())
        return TypeGenerationHelper.ForceTypeGenerationAndCreateInstance<T> ();
    }

    /// <summary>
    /// Signals that the <see cref="SetUpFixture"/> should not delete the files it generates. Call this ad-hoc in a test to keep the files and inspect
    /// them with Reflector or ildasm.
    /// </summary>
    public void SkipDeletion ()
    {
      SetUpFixture.SkipDeletion ();
    }
  }
}

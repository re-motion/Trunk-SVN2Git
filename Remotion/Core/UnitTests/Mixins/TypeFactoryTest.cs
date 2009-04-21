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
using System;
using System.Runtime.Serialization;
using NUnit.Framework;
using Remotion.Mixins;
using Remotion.Mixins.Context;
using Remotion.Mixins.Definitions;
using Remotion.UnitTests.Mixins.SampleTypes;

namespace Remotion.UnitTests.Mixins
{
  [TestFixture]
  public class TypeFactoryTest
  {
    [Test]
    public void GetActiveConfiguration()
    {
      using (MixinConfiguration.BuildNew().EnterScope())
      {
        Assert.IsFalse (MixinConfiguration.ActiveConfiguration.ClassContexts.ContainsWithInheritance (typeof (BaseType1)));
        Assert.IsFalse (MixinConfiguration.ActiveConfiguration.ClassContexts.ContainsWithInheritance (typeof (BaseType2)));
        Assert.IsNull (TargetClassDefinitionUtility.GetActiveConfiguration (typeof (BaseType1)));
        Assert.IsNull (TargetClassDefinitionUtility.GetActiveConfiguration (typeof (BaseType2)));
        Assert.IsFalse (MixinConfiguration.ActiveConfiguration.ClassContexts.ContainsWithInheritance (typeof (BaseType1)));
        Assert.IsFalse (MixinConfiguration.ActiveConfiguration.ClassContexts.ContainsWithInheritance (typeof (BaseType2)));

        using (MixinConfiguration.BuildFromActive().ForClass (typeof (BaseType1)).Clear().EnterScope())
        {
          Assert.IsTrue (MixinConfiguration.ActiveConfiguration.ClassContexts.ContainsWithInheritance (typeof (BaseType1)));
          Assert.IsFalse (MixinConfiguration.ActiveConfiguration.ClassContexts.ContainsWithInheritance (typeof (BaseType2)));
          Assert.AreSame (
              TargetClassDefinitionCache.Current.GetTargetClassDefinition (new ClassContext (typeof (BaseType1))),
              TargetClassDefinitionUtility.GetActiveConfiguration (typeof (BaseType1)));
          Assert.IsNull (TargetClassDefinitionUtility.GetActiveConfiguration (typeof (BaseType2)));
          Assert.IsTrue (MixinConfiguration.ActiveConfiguration.ClassContexts.ContainsWithInheritance (typeof (BaseType1)));
          Assert.IsFalse (MixinConfiguration.ActiveConfiguration.ClassContexts.ContainsWithInheritance (typeof (BaseType2)));

          using (MixinConfiguration.BuildFromActive().ForClass (typeof (BaseType2)).Clear().EnterScope())
          {
            Assert.IsTrue (MixinConfiguration.ActiveConfiguration.ClassContexts.ContainsWithInheritance (typeof (BaseType1)));
            Assert.IsTrue (MixinConfiguration.ActiveConfiguration.ClassContexts.ContainsWithInheritance (typeof (BaseType2)));

            Assert.IsNotNull (TargetClassDefinitionUtility.GetActiveConfiguration (typeof (BaseType1)));
            Assert.AreSame (
                TargetClassDefinitionCache.Current.GetTargetClassDefinition (new ClassContext (typeof (BaseType1))),
                TargetClassDefinitionUtility.GetActiveConfiguration (typeof (BaseType1)));
            Assert.IsNotNull (TargetClassDefinitionUtility.GetActiveConfiguration (typeof (BaseType2)));
            Assert.AreSame (
                TargetClassDefinitionCache.Current.GetTargetClassDefinition (new ClassContext (typeof (BaseType2))),
                TargetClassDefinitionUtility.GetActiveConfiguration (typeof (BaseType2)));
          }
        }
      }
    }

    [Uses (typeof (NullMixin))]
    public class GenericTypeWithMixin<T>
    { }

    [Test]
    public void GetActiveConfigurationWithGenericTypes ()
    {
      TargetClassDefinition def = TargetClassDefinitionUtility.GetActiveConfiguration (typeof (GenericTypeWithMixin<int>));
      Assert.AreEqual (typeof (GenericTypeWithMixin<int>), def.Type);
      Assert.IsTrue (def.Mixins.ContainsKey (typeof (NullMixin)));
    }

    [Test]
    public void MixinExtendingSpecificGenericType ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass (typeof (GenericClassExtendedByMixin<int>)).Clear().AddMixins (typeof (MixinExtendingSpecificGenericClass)).EnterScope())
      {
        TargetClassDefinition targetClassDefinition =
            TargetClassDefinitionUtility.GetActiveConfiguration (typeof (GenericClassExtendedByMixin<int>), GenerationPolicy.GenerateOnlyIfConfigured);
        Assert.IsNotNull (targetClassDefinition);
        Assert.IsTrue (targetClassDefinition.Mixins.ContainsKey (typeof (MixinExtendingSpecificGenericClass)));

        Assert.IsNull (TargetClassDefinitionUtility.GetActiveConfiguration (typeof (GenericClassExtendedByMixin<string>), GenerationPolicy.GenerateOnlyIfConfigured));
        Assert.IsNull (TargetClassDefinitionUtility.GetActiveConfiguration (typeof (GenericClassExtendedByMixin<>), GenerationPolicy.GenerateOnlyIfConfigured));
      }
    }

    [Test]
    public void NoDefinitionGeneratedIfNoConfigByDefault()
    {
      Assert.IsFalse (MixinConfiguration.ActiveConfiguration.ClassContexts.ContainsWithInheritance (typeof (object)));
      Assert.IsNull (TargetClassDefinitionUtility.GetActiveConfiguration (typeof (object)));
    }

    [Test]
    public void NoNewDefinitionGeneratedForGeneratedTypeByDefault ()
    {
      Type generatedType = TypeFactory.GetConcreteType (typeof (BaseType1));
      TargetClassDefinition definition = TargetClassDefinitionUtility.GetActiveConfiguration (generatedType);
      Assert.AreSame (definition, TargetClassDefinitionUtility.GetActiveConfiguration (typeof (BaseType1)));
    }

    [Test]
    public void DefinitionGeneratedIfNoConfigViaPolicy ()
    {
      Assert.IsFalse (MixinConfiguration.ActiveConfiguration.ClassContexts.ContainsWithInheritance (typeof (object)));
      TargetClassDefinition configuration = TargetClassDefinitionUtility.GetActiveConfiguration (typeof (object), GenerationPolicy.ForceGeneration);
      Assert.IsNotNull (configuration);
      Assert.AreEqual (typeof (object), configuration.Type);
    }

    [Test]
    public void NewDefinitionGeneratedForGeneratedTypeViaPolicy ()
    {
      Type generatedType = TypeFactory.GetConcreteType (typeof (BaseType1));
      TargetClassDefinition newDefinition = TargetClassDefinitionUtility.GetActiveConfiguration (generatedType, GenerationPolicy.ForceGeneration);
      TargetClassDefinition baseDefinition = TargetClassDefinitionUtility.GetActiveConfiguration (typeof (BaseType1));
      Assert.AreNotSame (baseDefinition, newDefinition);
      Assert.AreSame (baseDefinition.Type, newDefinition.Type.BaseType);
    }

    [Test]
    public void ForcedDefinitionsAreCached ()
    {
      Assert.IsFalse (MixinConfiguration.ActiveConfiguration.ClassContexts.ContainsWithInheritance (typeof (object)));
      TargetClassDefinition d1 = TargetClassDefinitionUtility.GetActiveConfiguration (typeof (object), GenerationPolicy.ForceGeneration);
      TargetClassDefinition d2 = TargetClassDefinitionUtility.GetActiveConfiguration (typeof (object), GenerationPolicy.ForceGeneration);
      Assert.AreSame (d1, d2);
    }

    [Test]
    public void ForcedGenerationIsNotPersistent ()
    {
      Assert.IsFalse (MixinConfiguration.ActiveConfiguration.ClassContexts.ContainsWithInheritance (typeof (object)));
      TargetClassDefinition d1 = TargetClassDefinitionUtility.GetActiveConfiguration (typeof (object), GenerationPolicy.ForceGeneration);
      TargetClassDefinition d2 = TargetClassDefinitionUtility.GetActiveConfiguration (typeof (object), GenerationPolicy.GenerateOnlyIfConfigured);
      Assert.IsNotNull (d1);
      Assert.IsNull (d2);
    }

    [Test]
    public void NoTypeGeneratedIfNoConfigByDefault ()
    {
      Assert.IsFalse (MixinConfiguration.ActiveConfiguration.ClassContexts.ContainsWithInheritance (typeof (object)));
      Assert.AreSame (typeof (object), TypeFactory.GetConcreteType (typeof (object)));
    }

    [Test]
    public void NoTypeGeneratedIfGeneratedTypeIsGivenByDefault ()
    {
      Type concreteType = TypeFactory.GetConcreteType (typeof (BaseType1));
      Assert.AreSame (concreteType, TypeFactory.GetConcreteType (concreteType));
    }

    [Test]
    public void TypeGeneratedIfNoConfigViaPolicy ()
    {
      Assert.IsFalse (MixinConfiguration.ActiveConfiguration.ClassContexts.ContainsWithInheritance (typeof (object)));
      Type concreteType = TypeFactory.GetConcreteType (typeof (object), GenerationPolicy.ForceGeneration);
      Assert.AreNotSame (typeof (object), concreteType);
      Assert.AreSame (typeof (object), concreteType.BaseType);
    }

    [Test]
    public void TypeGeneratedIfGeneratedTypeIsGivenViaPolicy ()
    {
      Type concreteType = TypeFactory.GetConcreteType (typeof (BaseType1));
      Type concreteType2 = TypeFactory.GetConcreteType (concreteType, GenerationPolicy.ForceGeneration);
      Assert.AreNotSame (concreteType, concreteType2);
      Assert.AreSame (concreteType, concreteType2.BaseType);
    }

    [Test]
    public void InitializeUnconstructedInstance ()
    {
      Type concreteType = TypeFactory.GetConcreteType (typeof (BaseType3));
      BaseType3 bt3 = (BaseType3) FormatterServices.GetSafeUninitializedObject (concreteType);
      TypeFactory.InitializeUnconstructedInstance (bt3 as IMixinTarget);
      BT3Mixin1 bt3m1 = Mixin.Get<BT3Mixin1> (bt3);
      Assert.IsNotNull (bt3m1, "Mixin must have been created");
      Assert.AreSame (bt3, bt3m1.This, "Mixin must have been initialized");
    }
  }
}

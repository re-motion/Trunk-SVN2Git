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
using System.Reflection;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Mixins;
using Remotion.Mixins.CodeGeneration;
using Remotion.Mixins.Definitions;
using Remotion.Mixins.Utilities;
using Remotion.UnitTests.Mixins.CodeGeneration.TestDomain;
using Remotion.UnitTests.Mixins.SampleTypes;
using Rhino.Mocks;

namespace Remotion.UnitTests.Mixins.CodeGeneration.IntegrationTests.MixinTypeCodeGeneration
{
  [TestFixture]
  public class TypeFeatureTest : CodeGenerationBaseTest
  {
    [Test]
    public void GeneratedTypeImplementsMarkerInterface ()
    {
      MixinDefinition mixinDefinition =
          TargetClassDefinitionUtility.GetActiveConfiguration (typeof (ClassOverridingMixinMembers)).Mixins[typeof (MixinWithAbstractMembers)];
      Assert.IsNotNull (mixinDefinition);
      Type generatedType = ConcreteTypeBuilder.Current.GetConcreteMixinType (mixinDefinition).GeneratedType;
      Assert.IsTrue (typeof (IGeneratedMixinType).IsAssignableFrom (generatedType));
    }

    [Test]
    public void GeneratedMixinTypeHasMixinTypeAttribute ()
    {
      MixinDefinition mixinDefinition =
          TargetClassDefinitionUtility.GetActiveConfiguration (typeof (ClassOverridingMixinMembers)).Mixins[typeof (MixinWithAbstractMembers)];
      Assert.IsNotNull (mixinDefinition);

      Type generatedType = ConcreteTypeBuilder.Current.GetConcreteMixinType (mixinDefinition).GeneratedType;
      Assert.IsTrue (generatedType.IsDefined (typeof (ConcreteMixinTypeAttribute), false));

      var attributes = (ConcreteMixinTypeAttribute[]) generatedType.GetCustomAttributes (typeof (ConcreteMixinTypeAttribute), false);
      Assert.AreEqual (1, attributes.Length);
    }

    [Test]
    public void MixinTypeAttributeCanBeUsedToGetMixinDefinition ()
    {
      MixinDefinition mixinDefinition =
          TargetClassDefinitionUtility.GetActiveConfiguration (typeof (ClassOverridingMixinMembers)).Mixins[typeof (MixinWithAbstractMembers)];
      Assert.IsNotNull (mixinDefinition);

      Type generatedType = ConcreteTypeBuilder.Current.GetConcreteMixinType (mixinDefinition).GeneratedType;
      Assert.IsTrue (generatedType.IsDefined (typeof (ConcreteMixinTypeAttribute), false));

      var attributes = (ConcreteMixinTypeAttribute[]) generatedType.GetCustomAttributes (typeof (ConcreteMixinTypeAttribute), false);
      Assert.AreSame (mixinDefinition, attributes[0].GetMixinDefinition (TargetClassDefinitionCache.Current));
    }

    [Test]
    public void NameProviderIsUsedWhenTypeIsGenerated ()
    {
      var builder = new ConcreteTypeBuilder { Scope = SavedTypeBuilder.Scope };
      var repository = new MockRepository ();
      var nameProviderMock = repository.StrictMock<INameProvider> ();
      builder.MixinTypeNameProvider = nameProviderMock;
      ConcreteTypeBuilder.SetCurrent (builder);

      MixinDefinition mixinDefinition =
          TargetClassDefinitionUtility.GetActiveConfiguration (typeof (ClassOverridingMixinMembers)).Mixins[typeof (MixinWithAbstractMembers)];
      Assert.IsNotNull (mixinDefinition);

      Expect.Call (nameProviderMock.GetNewTypeName (mixinDefinition)).Return ("Bra");

      repository.ReplayAll ();

      Type generatedType = ConcreteTypeBuilder.Current.GetConcreteMixinType (mixinDefinition).GeneratedType;

      Assert.AreEqual ("Bra", generatedType.FullName);

      repository.VerifyAll ();
    }

    [Test]
    public void NamesOfNestedTypesAreFlattened ()
    {
      var builder = new ConcreteTypeBuilder { Scope = SavedTypeBuilder.Scope };
      var repository = new MockRepository ();
      var nameProviderMock = repository.StrictMock<INameProvider> ();
      builder.MixinTypeNameProvider = nameProviderMock;
      ConcreteTypeBuilder.SetCurrent (builder);

      MixinDefinition mixinDefinition =
          TargetClassDefinitionUtility.GetActiveConfiguration (typeof (ClassOverridingMixinMembers)).Mixins[typeof (MixinWithAbstractMembers)];
      Assert.IsNotNull (mixinDefinition);

      Expect.Call (nameProviderMock.GetNewTypeName (mixinDefinition)).Return ("Bra+Oof");

      repository.ReplayAll ();

      Type generatedType = ConcreteTypeBuilder.Current.GetConcreteMixinType (mixinDefinition).GeneratedType;

      Assert.AreEqual ("Bra/Oof", generatedType.FullName);

      repository.VerifyAll ();
    }

    [Test]
    public void AttributesAreReplicatedIfNecessary ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<NullTarget> ().Clear().AddMixins (typeof (MixinWithProtectedOverriderAndAttributes)).EnterScope())
      {
        MixinDefinition mixinDefinition =
            TargetClassDefinitionUtility.GetActiveConfiguration (typeof (NullTarget)).Mixins[typeof (MixinWithProtectedOverriderAndAttributes)];
        Assert.IsNotNull (mixinDefinition);
        Type generatedType = ConcreteTypeBuilder.Current.GetConcreteMixinType (mixinDefinition).GeneratedType;
        Assert.AreNotSame (typeof (MixinWithProtectedOverriderAndAttributes), generatedType);

        object[] inheritableAttributes = generatedType.GetCustomAttributes (typeof (InheritableAttribute), true);
        object[] nonInheritableAttributes = generatedType.GetCustomAttributes (typeof (NonInheritableAttribute), true);
        
        Assert.AreEqual (1, inheritableAttributes.Length);
        Assert.That (inheritableAttributes, Is.EquivalentTo (typeof (MixinWithProtectedOverriderAndAttributes).GetCustomAttributes (typeof (InheritableAttribute), true)));

        Assert.AreEqual (1, nonInheritableAttributes.Length);
        Assert.That (nonInheritableAttributes, Is.EquivalentTo (typeof (MixinWithProtectedOverriderAndAttributes).GetCustomAttributes (typeof (NonInheritableAttribute), true)));
      }
    }

    [Test]
    public void CopyTemplatesAreNotReplicated ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<NullTarget> ().Clear().AddMixins (typeof (MixinWithProtectedOverriderAndAttributes)).EnterScope())
      {
        MixinDefinition mixinDefinition =
            TargetClassDefinitionUtility.GetActiveConfiguration (typeof (NullTarget)).Mixins[typeof (MixinWithProtectedOverriderAndAttributes)];
        Assert.IsNotNull (mixinDefinition);
        Type generatedType = ConcreteTypeBuilder.Current.GetConcreteMixinType (mixinDefinition).GeneratedType;
        Assert.AreNotSame (typeof (MixinWithProtectedOverriderAndAttributes), generatedType);

        object[] copiedAttributes = generatedType.GetCustomAttributes (typeof (SampleCopyTemplateAttribute), true);
        Assert.AreEqual (0, copiedAttributes.Length);
      }
    }
  }
}

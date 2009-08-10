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
using System.Collections.Generic;
using System.Resources;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Collections;
using Remotion.Globalization;
using Remotion.Mixins;
using Remotion.Mixins.Globalization;
using Remotion.UnitTests.Mixins.Globalization.SampleTypes;
using Remotion.UnitTests.Mixins.SampleTypes;
using Remotion.Utilities;

namespace Remotion.UnitTests.Mixins.Globalization
{
  [TestFixture]
  public class MixedResourceManagerResolverTest
  {
    private MixedResourceManagerResolver<MultiLingualResourcesAttribute> _resolver;
  	private readonly MultiLingualResourcesAttribute[] _noAttributes = new MultiLingualResourcesAttribute[0];

  	[SetUp]
    public void SetUp ()
    {
      _resolver = new MixedResourceManagerResolver<MultiLingualResourcesAttribute> ();
    }

    [Test]
    public void GetResourceDefinitions_NoSuccessOnType_NoSuccessOnMixin ()
    {
      using (MixinConfiguration.BuildNew().ForClass<ClassWithoutMultiLingualResourcesAttributes>().AddMixin<NullMixin>().EnterScope())
      {
        ResourceDefinition<MultiLingualResourcesAttribute>[] definitions =
						EnumerableUtility.ToArray (_resolver.GetResourceDefinitionStream (typeof (ClassWithoutMultiLingualResourcesAttributes), false));
				Assert.That (definitions.Length, Is.EqualTo (0));
      }
    }

    [Test]
    public void GetResourceDefinitions_NoSuccessOnType_NoMixins ()
    {
      ResourceDefinition<MultiLingualResourcesAttribute>[] definitions =
          EnumerableUtility.ToArray (_resolver.GetResourceDefinitionStream (typeof (ClassWithoutMultiLingualResourcesAttributes), false));
      Assert.That (definitions.Length, Is.EqualTo (0));
    }

    [Test]
    public void GetResourceDefinitions_SuccessOnType_NoSuccessOnMixin ()
    {
      using (MixinConfiguration.BuildNew().ForClass<ClassWithMultiLingualResourcesAttributes>().AddMixin<NullMixin>().EnterScope())
      {
        ResourceDefinition<MultiLingualResourcesAttribute>[] definitions =
            EnumerableUtility.ToArray (_resolver.GetResourceDefinitionStream (typeof (ClassWithMultiLingualResourcesAttributes), false));
        Assert.That (definitions.Length, Is.EqualTo (1));
        CheckDefinition (definitions[0], typeof (ClassWithMultiLingualResourcesAttributes), 
            AttributesFor<ClassWithMultiLingualResourcesAttributes>());
      }
    }

    [Test]
    public void GetResourceDefinitions_NoSuccessOnType_SuccessOnMixin ()
    {
      using (MixinConfiguration.BuildNew().ForClass<ClassWithoutMultiLingualResourcesAttributes>().AddMixin<MixinAddingMultiLingualResourcesAttributes1>().EnterScope())
      {
        ResourceDefinition<MultiLingualResourcesAttribute>[] definitions =
						EnumerableUtility.ToArray (_resolver.GetResourceDefinitionStream (typeof (ClassWithoutMultiLingualResourcesAttributes), false));
        Assert.That (definitions.Length, Is.EqualTo (1));
        CheckDefinition (definitions[0], typeof (ClassWithoutMultiLingualResourcesAttributes), 
            _noAttributes,
						TupleFor<MixinAddingMultiLingualResourcesAttributes1>());
      }
    }

    [Test]
    public void GetResourceDefinitions_SuccessOnType_SuccessOnMixin ()
    {
      using (MixinConfiguration.BuildNew().ForClass<ClassWithMultiLingualResourcesAttributes>().AddMixin<MixinAddingMultiLingualResourcesAttributes1>().EnterScope())
      {
        ResourceDefinition<MultiLingualResourcesAttribute>[] definitions =
            EnumerableUtility.ToArray (_resolver.GetResourceDefinitionStream (typeof (ClassWithMultiLingualResourcesAttributes), false));
        Assert.That (definitions.Length, Is.EqualTo (1));
        CheckDefinition (definitions[0], typeof (ClassWithMultiLingualResourcesAttributes), 
            AttributesFor<ClassWithMultiLingualResourcesAttributes>(),
            TupleFor<MixinAddingMultiLingualResourcesAttributes1>());
      }
    }

    [Test]
    public void GetResourceDefinitions_SuccessOnType_SuccessOnMultipleMixins ()
    {
      using (MixinConfiguration.BuildNew()
          .ForClass<ClassWithMultiLingualResourcesAttributes>()
          .AddMixin<MixinAddingMultiLingualResourcesAttributes1>()
          .AddMixin<MixinAddingMultiLingualResourcesAttributes2>()
          .EnterScope())
      {
        ResourceDefinition<MultiLingualResourcesAttribute>[] definitions =
            EnumerableUtility.ToArray (_resolver.GetResourceDefinitionStream (typeof (ClassWithMultiLingualResourcesAttributes), false));
        Assert.That (definitions.Length, Is.EqualTo (1));

				CheckDefinition (definitions[0], typeof (ClassWithMultiLingualResourcesAttributes), 
            AttributesFor<ClassWithMultiLingualResourcesAttributes>(),
						TupleFor<MixinAddingMultiLingualResourcesAttributes1>(), 
						TupleFor<MixinAddingMultiLingualResourcesAttributes2>());
      }
    }

    [Test]
    public void GetResourceDefinitions_SuccessOnType_SuccessOnMultipleMixins_WithDependency1 ()
    {
      using (MixinConfiguration.BuildNew ()
          .ForClass<ClassWithMultiLingualResourcesAttributes> ()
          .AddMixin<MixinAddingMultiLingualResourcesAttributes1> ()
          .AddMixin<MixinAddingMultiLingualResourcesAttributes2> ().WithDependency<MixinAddingMultiLingualResourcesAttributes1>()
          .EnterScope ())
      {
        ResourceDefinition<MultiLingualResourcesAttribute>[] definitions =
            EnumerableUtility.ToArray (_resolver.GetResourceDefinitionStream (typeof (ClassWithMultiLingualResourcesAttributes), false));
        Assert.That (definitions.Length, Is.EqualTo (1));

        CheckDefinition (definitions[0], typeof (ClassWithMultiLingualResourcesAttributes),
            AttributesFor<ClassWithMultiLingualResourcesAttributes> (),
            TupleFor<MixinAddingMultiLingualResourcesAttributes2> (),
            TupleFor<MixinAddingMultiLingualResourcesAttributes1> ());
      }
    }

    [Test]
    public void GetResourceDefinitions_SuccessOnType_SuccessOnMultipleMixins_WithDependency2 ()
    {
      using (MixinConfiguration.BuildNew ()
          .ForClass<ClassWithMultiLingualResourcesAttributes> ()
          .AddMixin<MixinAddingMultiLingualResourcesAttributes1> ().WithDependency<MixinAddingMultiLingualResourcesAttributes2> ()
          .AddMixin<MixinAddingMultiLingualResourcesAttributes2> ()
          .EnterScope ())
      {
        ResourceDefinition<MultiLingualResourcesAttribute>[] definitions =
            EnumerableUtility.ToArray (_resolver.GetResourceDefinitionStream (typeof (ClassWithMultiLingualResourcesAttributes), false));
        Assert.That (definitions.Length, Is.EqualTo (1));

        CheckDefinition (definitions[0], typeof (ClassWithMultiLingualResourcesAttributes),
            AttributesFor<ClassWithMultiLingualResourcesAttributes> (),
            TupleFor<MixinAddingMultiLingualResourcesAttributes1> (),
            TupleFor<MixinAddingMultiLingualResourcesAttributes2> ());
      }
    }

    [Test]
    public void GetResourceDefinitions_SuccessOnBaseType_NoSuccessOnMixin ()
    {
      using (MixinConfiguration.BuildNew().ForClass<InheritedClassWithoutMultiLingualResourcesAttributes>().AddMixin<NullMixin>().EnterScope())
      {
        ResourceDefinition<MultiLingualResourcesAttribute>[] definitions =
            EnumerableUtility.ToArray (_resolver.GetResourceDefinitionStream (typeof (InheritedClassWithoutMultiLingualResourcesAttributes), false));
        Assert.That (definitions.Length, Is.EqualTo (1));
        CheckDefinition (definitions[0], typeof (ClassWithMultiLingualResourcesAttributes), 
            AttributesFor<ClassWithMultiLingualResourcesAttributes>());
      }
    }

    [Test]
    public void GetResourceDefinitions_NoSuccessOnType_SuccessOnInheritingMixin ()
    {
      using (MixinConfiguration.BuildNew().ForClass<ClassWithoutMultiLingualResourcesAttributes>().AddMixin<InheritedMixinWithoutMultiLingualResourcesAttributes1>().EnterScope())
      {
        ResourceDefinition<MultiLingualResourcesAttribute>[] definitions =
            EnumerableUtility.ToArray (_resolver.GetResourceDefinitionStream (typeof (ClassWithoutMultiLingualResourcesAttributes), false));
        Assert.That (definitions.Length, Is.EqualTo (1));
        CheckDefinition (definitions[0], typeof (ClassWithoutMultiLingualResourcesAttributes),
						_noAttributes,
            TupleFor<MixinAddingMultiLingualResourcesAttributes1>());
      }
    }

    [Test]
    public void GetResourceDefinitions_SuccessOnBaseType_SuccessOnMixin ()
    {
      using (MixinConfiguration.BuildNew().ForClass<InheritedClassWithoutMultiLingualResourcesAttributes>().AddMixin<MixinAddingMultiLingualResourcesAttributes1>().EnterScope())
      {
        ResourceDefinition<MultiLingualResourcesAttribute>[] definitions =
            EnumerableUtility.ToArray (_resolver.GetResourceDefinitionStream (typeof (InheritedClassWithoutMultiLingualResourcesAttributes), false));
        Assert.That (definitions.Length, Is.EqualTo (1));
        CheckDefinition (definitions[0], typeof (InheritedClassWithoutMultiLingualResourcesAttributes),
						_noAttributes,
            TupleFor<MixinAddingMultiLingualResourcesAttributes1>());
      }
    }

    [Test]
    public void GetResourceDefinitions_SuccessOnBaseType_SuccessOnInheritingMixin ()
    {
      using (MixinConfiguration.BuildNew().ForClass<InheritedClassWithoutMultiLingualResourcesAttributes>().AddMixin<InheritedMixinWithoutMultiLingualResourcesAttributes1>().EnterScope())
      {
        ResourceDefinition<MultiLingualResourcesAttribute>[] definitions =
            EnumerableUtility.ToArray (_resolver.GetResourceDefinitionStream (typeof (InheritedClassWithoutMultiLingualResourcesAttributes), false));
        Assert.That (definitions.Length, Is.EqualTo (1));
        CheckDefinition (definitions[0], typeof (InheritedClassWithoutMultiLingualResourcesAttributes),
						_noAttributes,
            TupleFor<MixinAddingMultiLingualResourcesAttributes1>());
      }
    }

    [Test]
    public void GetResourceDefinitions_SuccessOnType_SuccessOnInheritingMixin ()
    {
      using (MixinConfiguration.BuildNew().ForClass<ClassWithMultiLingualResourcesAttributes>().AddMixin<InheritedMixinWithoutMultiLingualResourcesAttributes1>().EnterScope())
      {
        ResourceDefinition<MultiLingualResourcesAttribute>[] definitions =
            EnumerableUtility.ToArray (_resolver.GetResourceDefinitionStream (typeof (ClassWithMultiLingualResourcesAttributes), false));
        Assert.That (definitions.Length, Is.EqualTo (1));
        CheckDefinition (definitions[0], typeof (ClassWithMultiLingualResourcesAttributes), 
            AttributesFor<ClassWithMultiLingualResourcesAttributes>(), TupleFor<MixinAddingMultiLingualResourcesAttributes1>());
      }
    }

    [Test]
    public void GetResourceDefinitions_InheritanceFalse_SuccessOnTypeAndBase_SuccessOnMixinAndBase ()
    {
      using (MixinConfiguration.BuildNew().ForClass<InheritedClassWithMultiLingualResourcesAttributes>().AddMixin<InheritedMixinAddingMultiLingualResourcesAttributes2>().EnterScope())
      {
        ResourceDefinition<MultiLingualResourcesAttribute>[] definitions =
            EnumerableUtility.ToArray (_resolver.GetResourceDefinitionStream (typeof (InheritedClassWithMultiLingualResourcesAttributes), false));
        Assert.That (definitions.Length, Is.EqualTo (1));
				CheckDefinition (definitions[0], typeof (InheritedClassWithMultiLingualResourcesAttributes),
						AttributesFor<InheritedClassWithMultiLingualResourcesAttributes> (),
						TupleFor<InheritedMixinAddingMultiLingualResourcesAttributes2> (),
						TupleFor<MixinAddingMultiLingualResourcesAttributes2> ());
      }
    }

    [Test]
    public void GetResourceDefinitions_InheritanceTrue_SuccessOnTypeAndBase_SuccessOnMixinAndBase ()
    {
      using (MixinConfiguration.BuildNew()
					.ForClass<InheritedClassWithMultiLingualResourcesAttributes>()
					.AddMixin<InheritedMixinAddingMultiLingualResourcesAttributes2>()
					.EnterScope())
      {
        ResourceDefinition<MultiLingualResourcesAttribute>[] definitions =
            EnumerableUtility.ToArray (_resolver.GetResourceDefinitionStream (typeof (InheritedClassWithMultiLingualResourcesAttributes), true));
        Assert.That (definitions.Length, Is.EqualTo (2));
        CheckDefinition (definitions[0], typeof (InheritedClassWithMultiLingualResourcesAttributes), 
            AttributesFor<InheritedClassWithMultiLingualResourcesAttributes>(),
            TupleFor<InheritedMixinAddingMultiLingualResourcesAttributes2>(),
						TupleFor<MixinAddingMultiLingualResourcesAttributes2>());
				CheckDefinition (definitions[1], typeof (ClassWithMultiLingualResourcesAttributes), 
            AttributesFor<ClassWithMultiLingualResourcesAttributes>());
      }
    }

		[Test]
		public void GetResourceDefinitions_InheritanceFalse_NoSuccessOnType_SuccessOnMixinFromBase ()
		{
			using (MixinConfiguration.BuildNew ().ForClass<ClassWithMultiLingualResourcesAttributes> ()
				.AddMixin<MixinAddingMultiLingualResourcesAttributes1> ().EnterScope ())
			{
				ResourceDefinition<MultiLingualResourcesAttribute>[] definitions =
						EnumerableUtility.ToArray (_resolver.GetResourceDefinitionStream (typeof (InheritedClassWithoutMultiLingualResourcesAttributes), false));
				Assert.That (definitions.Length, Is.EqualTo (1));
				CheckDefinition (definitions[0], typeof (InheritedClassWithoutMultiLingualResourcesAttributes),
						_noAttributes,
						TupleFor<MixinAddingMultiLingualResourcesAttributes1> ());
			}
		}

		[Test]
		public void GetResourceDefinitions_InheritanceTrue_NoSuccessOnType_SuccessOnMixinFromBase_MixinsAreOnlyCheckedAtTopLevel ()
		{
			using (MixinConfiguration.BuildNew ().ForClass<ClassWithMultiLingualResourcesAttributes> ()
				.AddMixin<MixinAddingMultiLingualResourcesAttributes1> ().EnterScope ())
			{
				ResourceDefinition<MultiLingualResourcesAttribute>[] definitions =
						EnumerableUtility.ToArray (_resolver.GetResourceDefinitionStream (typeof (InheritedClassWithoutMultiLingualResourcesAttributes), true));
				Assert.That (definitions.Length, Is.EqualTo (2));
				CheckDefinition (definitions[0], typeof (InheritedClassWithoutMultiLingualResourcesAttributes),
						_noAttributes, 
						TupleFor<MixinAddingMultiLingualResourcesAttributes1> ());
				CheckDefinition (definitions[1], typeof (ClassWithMultiLingualResourcesAttributes),
						AttributesFor<ClassWithMultiLingualResourcesAttributes> ());
			}
		}

		[Test]
		public void GetResourceManager_InheritanceFalse_SuccessOnTypeAndBase_SuccessOnMixin ()
		{
			using (MixinConfiguration.BuildNew ()
					.ForClass<InheritedClassWithMultiLingualResourcesAttributes> ().AddMixin<MixinAddingMultiLingualResourcesAttributes1> ().EnterScope ())
			{
				ResourceManagerSet resourceManagerSet =
						(ResourceManagerSet) _resolver.GetResourceManager (typeof (InheritedClassWithMultiLingualResourcesAttributes), false);
				Assert.AreEqual (2, resourceManagerSet.Count);
				Assert.AreEqual ("OnMixin1", resourceManagerSet[0].Name);
				Assert.AreEqual ("OnInherited", resourceManagerSet[1].Name);
			}
		}


  	[Test]
    public void GetResourceManager_InheritanceFalse_SuccessOnTypeAndBase_NoMixin ()
    {
      ResourceManagerSet resourceManagerSet =
          (ResourceManagerSet) _resolver.GetResourceManager (typeof (InheritedClassWithMultiLingualResourcesAttributes), false);
      Assert.AreEqual (1, resourceManagerSet.Count);
      Assert.AreEqual ("OnInherited", resourceManagerSet[0].Name);
    }

    [Test]
    public void GetResourceManager_InheritanceTrue_SuccessOnTypeAndBase_SuccessOnMixin ()
    {
      using (MixinConfiguration.BuildNew ()
          .ForClass<InheritedClassWithMultiLingualResourcesAttributes> ()
          .AddMixin<MixinAddingMultiLingualResourcesAttributes1> ()
					.ForClass<ClassWithMultiLingualResourcesAttributes> ()
					.AddMixin<MixinAddingMultiLingualResourcesAttributes2> ()
					.EnterScope ())
      {
        ResourceManagerSet resourceManagerSet =
            (ResourceManagerSet) _resolver.GetResourceManager (typeof (InheritedClassWithMultiLingualResourcesAttributes), true);
        Assert.AreEqual (5, resourceManagerSet.Count);
				string[] names = new string[] {resourceManagerSet[0].Name, resourceManagerSet[1].Name, resourceManagerSet[2].Name,
						resourceManagerSet[3].Name, resourceManagerSet[4].Name};
				Assert.That (names, Is.EquivalentTo (new string[] { "OnMixin2b", "OnMixin2a", "OnTarget", "OnMixin1", "OnInherited" }));
				Assert.That (Array.IndexOf (names, "OnTarget"), Is.LessThan (Array.IndexOf (names, "OnInherited")));
      }
    }

    [Test]
    [ExpectedException (typeof (ResourceException), ExpectedMessage = "Type Remotion.UnitTests.Mixins.Globalization.SampleTypes."
        + "ClassWithoutMultiLingualResourcesAttributes and its base classes do not define the attribute MultiLingualResourcesAttribute.")]
		public void GetResourceManager_NoSuccess ()
    {
      _resolver.GetResourceManager (typeof (ClassWithoutMultiLingualResourcesAttributes), true);
    }

    [Test]
    [ExpectedException (typeof (MissingManifestResourceException), ExpectedMessage = "Could not find any resources appropriate for the specified "
        + "culture or the neutral culture.  Make sure \"OnTarget.resources\" was correctly embedded or linked into assembly "
        + "\"Remotion.UnitTests\" at compile time, or that all the satellite assemblies required are loadable and fully signed.")]
    public void GetResourceManager_ForGeneratedType_GetString ()
    {
      using (MixinConfiguration.BuildNew ().ForClass<ClassWithMultiLingualResourcesAttributes> ().AddMixin<NullMixin>().EnterScope ())
      {
        IResourceManager resourceManager = _resolver.GetResourceManager (TypeFactory.GetConcreteType (typeof (ClassWithMultiLingualResourcesAttributes)), true);
        resourceManager.GetString ("Foo");
      }
    }

		private MultiLingualResourcesAttribute[] AttributesFor<T> ()
		{
			return AttributeUtility.GetCustomAttributes<MultiLingualResourcesAttribute> (typeof (T), false);
		}

		private Tuple<Type, MultiLingualResourcesAttribute[]> TupleFor<T> ()
		{
			return Tuple.NewTuple (typeof (T), AttributesFor<T> ());
		}

    private void CheckDefinition (
        ResourceDefinition<MultiLingualResourcesAttribute> definition, 
        Type expectedDefinitionType, 
				MultiLingualResourcesAttribute[] expectedOwn, 
        params Tuple<Type, MultiLingualResourcesAttribute[]>[] expectedSupplementing)
    {
      Assert.That (definition.Type, Is.SameAs (expectedDefinitionType));
      Assert.That (definition.OwnAttributes, Is.EquivalentTo (expectedOwn));
			
			Assert.That (definition.SupplementingAttributes.Count, Is.EqualTo (expectedSupplementing.Length));
      for (int i = 0; i < expectedSupplementing.Length; i++)
      {
        Assert.That (definition.SupplementingAttributes[i].A, Is.EqualTo (expectedSupplementing[i].A));
        Assert.That (definition.SupplementingAttributes[i].B, Is.EqualTo (expectedSupplementing[i].B));
      }
    }
  }
}

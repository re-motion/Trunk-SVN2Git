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
using System.Linq;
using System.Resources;
using NUnit.Framework;
using Remotion.Collections;
using Remotion.Globalization;
using Remotion.Mixins.Globalization;
using Remotion.Mixins.UnitTests.Core.Globalization.TestDomain;
using Remotion.Mixins.UnitTests.Core.TestDomain;
using Remotion.Utilities;

namespace Remotion.Mixins.UnitTests.Core.Globalization
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
						_resolver.GetResourceDefinitionStream (typeof (ClassWithoutMultiLingualResourcesAttributes), false).ToArray ();
				Assert.That (definitions.Length, Is.EqualTo (0));
      }
    }

    [Test]
    public void GetResourceDefinitions_NoSuccessOnType_NoMixins ()
    {
      ResourceDefinition<MultiLingualResourcesAttribute>[] definitions =
          _resolver.GetResourceDefinitionStream (typeof (ClassWithoutMultiLingualResourcesAttributes), false).ToArray ();
      Assert.That (definitions.Length, Is.EqualTo (0));
    }

    [Test]
    public void GetResourceDefinitions_SuccessOnType_NoSuccessOnMixin ()
    {
      using (MixinConfiguration.BuildNew().ForClass<ClassWithMultiLingualResourcesAttributes>().AddMixin<NullMixin>().EnterScope())
      {
        ResourceDefinition<MultiLingualResourcesAttribute>[] definitions =
            _resolver.GetResourceDefinitionStream (typeof (ClassWithMultiLingualResourcesAttributes), false).ToArray ();
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
						_resolver.GetResourceDefinitionStream (typeof (ClassWithoutMultiLingualResourcesAttributes), false).ToArray ();
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
            _resolver.GetResourceDefinitionStream (typeof (ClassWithMultiLingualResourcesAttributes), false).ToArray ();
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
            _resolver.GetResourceDefinitionStream (typeof (ClassWithMultiLingualResourcesAttributes), false).ToArray ();
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
            _resolver.GetResourceDefinitionStream (typeof (ClassWithMultiLingualResourcesAttributes), false).ToArray ();
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
            _resolver.GetResourceDefinitionStream (typeof (ClassWithMultiLingualResourcesAttributes), false).ToArray ();
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
            _resolver.GetResourceDefinitionStream (typeof (InheritedClassWithoutMultiLingualResourcesAttributes), false).ToArray ();
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
            _resolver.GetResourceDefinitionStream (typeof (ClassWithoutMultiLingualResourcesAttributes), false).ToArray ();
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
            _resolver.GetResourceDefinitionStream (typeof (InheritedClassWithoutMultiLingualResourcesAttributes), false).ToArray ();
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
            _resolver.GetResourceDefinitionStream (typeof (InheritedClassWithoutMultiLingualResourcesAttributes), false).ToArray ();
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
            _resolver.GetResourceDefinitionStream (typeof (ClassWithMultiLingualResourcesAttributes), false).ToArray ();
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
            _resolver.GetResourceDefinitionStream (typeof (InheritedClassWithMultiLingualResourcesAttributes), false).ToArray ();
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
            _resolver.GetResourceDefinitionStream (typeof (InheritedClassWithMultiLingualResourcesAttributes), true).ToArray ();
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
						_resolver.GetResourceDefinitionStream (typeof (InheritedClassWithoutMultiLingualResourcesAttributes), false).ToArray ();
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
						_resolver.GetResourceDefinitionStream (typeof (InheritedClassWithoutMultiLingualResourcesAttributes), true).ToArray ();
				Assert.That (definitions.Length, Is.EqualTo (2));
				CheckDefinition (definitions[0], typeof (InheritedClassWithoutMultiLingualResourcesAttributes),
						_noAttributes, 
						TupleFor<MixinAddingMultiLingualResourcesAttributes1> ());
				CheckDefinition (definitions[1], typeof (ClassWithMultiLingualResourcesAttributes),
						AttributesFor<ClassWithMultiLingualResourcesAttributes> ());
			}
		}

    [Test]
    public void GetResourceDefinitions_ForGeneratedType ()
    {
      using (MixinConfiguration.BuildNew ().ForClass<ClassWithMultiLingualResourcesAttributes> ().AddMixin<NullMixin> ().EnterScope ())
      {
        var generatedType = TypeFactory.GetConcreteType (typeof (ClassWithMultiLingualResourcesAttributes));
        var definitions = _resolver.GetResourceDefinitionStream (generatedType, true);
        Assert.That (definitions.ToArray (), Is.Not.Empty);
      }
    }

		[Test]
		public void GetResourceManager_InheritanceFalse_SuccessOnTypeAndBase_SuccessOnMixin ()
		{
			using (MixinConfiguration.BuildNew ()
					.ForClass<InheritedClassWithMultiLingualResourcesAttributes> ().AddMixin<MixinAddingMultiLingualResourcesAttributes1> ().EnterScope ())
			{
				var resourceManagerSet =
						(ResourceManagerSet) _resolver.GetResourceManager (typeof (InheritedClassWithMultiLingualResourcesAttributes), false);
			  Assert.That (resourceManagerSet.Count, Is.EqualTo (2));
			  Assert.That (resourceManagerSet[0].Name, Is.EqualTo ("OnMixin1"));
			  Assert.That (resourceManagerSet[1].Name, Is.EqualTo ("OnInherited"));
			}
		}


  	[Test]
    public void GetResourceManager_InheritanceFalse_SuccessOnTypeAndBase_NoMixin ()
    {
      var resourceManagerSet =
          (ResourceManagerSet) _resolver.GetResourceManager (typeof (InheritedClassWithMultiLingualResourcesAttributes), false);
  	  Assert.That (resourceManagerSet.Count, Is.EqualTo (1));
  	  Assert.That (resourceManagerSet[0].Name, Is.EqualTo ("OnInherited"));
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
        var resourceManagerSet =
            (ResourceManagerSet) _resolver.GetResourceManager (typeof (InheritedClassWithMultiLingualResourcesAttributes), true);
        Assert.That (resourceManagerSet.Count, Is.EqualTo (5));
        var names = new[] {resourceManagerSet[0].Name, resourceManagerSet[1].Name, resourceManagerSet[2].Name,
						resourceManagerSet[3].Name, resourceManagerSet[4].Name};
				Assert.That (names, Is.EquivalentTo (new[] { "OnMixin2b", "OnMixin2a", "OnTarget", "OnMixin1", "OnInherited" }));
				Assert.That (Array.IndexOf (names, "OnTarget"), Is.LessThan (Array.IndexOf (names, "OnInherited")));
      }
    }

    [Test]
    [ExpectedException (typeof (ResourceException), ExpectedMessage = "Type Remotion.Mixins.UnitTests.Core.Globalization.TestDomain."
        + "ClassWithoutMultiLingualResourcesAttributes and its base classes do not define the attribute MultiLingualResourcesAttribute.")]
		public void GetResourceManager_NoSuccess ()
    {
      _resolver.GetResourceManager (typeof (ClassWithoutMultiLingualResourcesAttributes), true);
    }

    [Test]
    [ExpectedException (typeof (MissingManifestResourceException), ExpectedMessage = "Could not find any resources appropriate for the specified "
        + "culture or the neutral culture.  Make sure \"OnTarget.resources\" was correctly embedded or linked into assembly "
        + "\"Remotion.Mixins.UnitTests\" at compile time, or that all the satellite assemblies required are loadable and fully signed.")]
    public void GetResourceManager_ForGeneratedType_GetString ()
    {
      using (MixinConfiguration.BuildNew ().ForClass<ClassWithMultiLingualResourcesAttributes> ().AddMixin<NullMixin>().EnterScope ())
      {
        var generatedType = TypeFactory.GetConcreteType (typeof (ClassWithMultiLingualResourcesAttributes));
        IResourceManager resourceManager = _resolver.GetResourceManager (generatedType, true);
        resourceManager.GetString ("Foo");
      }
    }

		private MultiLingualResourcesAttribute[] AttributesFor<T> ()
		{
			return AttributeUtility.GetCustomAttributes<MultiLingualResourcesAttribute> (typeof (T), false);
		}

		private Tuple<Type, MultiLingualResourcesAttribute[]> TupleFor<T> ()
		{
			return Tuple.Create (typeof (T), AttributesFor<T> ());
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
        Assert.That (definition.SupplementingAttributes[i].Item1, Is.EqualTo (expectedSupplementing[i].Item1));
        Assert.That (definition.SupplementingAttributes[i].Item2, Is.EqualTo (expectedSupplementing[i].Item2));
      }
    }
  }
}

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
using Remotion.Mixins;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.UnitTests.Core.BindableObject.IntergrationTests.Globalization.TestDomain;

namespace Remotion.ObjectBinding.UnitTests.Core.BindableObject.IntergrationTests.Globalization
{
  [TestFixture]
  public class GlobalizationIntegrationTests : TestBase
  {
    private BindableObjectMetadataFactory _factory;

    public override void SetUp ()
    {
      base.SetUp();

      _factory = BindableObjectMetadataFactory.Create();
    }

    [Test]
    public void PropertyDisplayName_NotInherited_NotOverridden_NotMixed ()
    {
      var resourceString = GetResourceStringForType (typeof (TargetClassForGlobalization), "Property2");

      Assert.That (resourceString, Is.EqualTo ("Property2 display name from TargetClassForGlobalization"));
    }

    [Test]
    public void PropertyDisplayName_InheritedAndOverridden_NotMixed_PropertyEvaluatedForBaseClass ()
    {
      var resourceString = GetResourceStringForType (typeof (TargetClassForGlobalization), "Property1");

      Assert.That (resourceString, Is.EqualTo ("Property1 display name from TargetClassForGlobalization"));
    }

    [Test]
    public void PropertyDisplayName_InheritedAndOverridden_NotMixed_PropertyEvaluatedForDerivedClass ()
    {
      var resourceString = GetResourceStringForType (typeof (DerivedTargetClassForGlobalization), "Property1");

      Assert.That (resourceString, Is.EqualTo ("Property1 display name from DerivedTargetClassForGlobalization"));
    }

    [Test]
    public void PropertyDisplayName_ResourceOveriddenByMixin ()
    {
      using (MixinConfiguration.BuildFromActive()
          .ForClass<TargetClassForGlobalization>()
          .AddMixin<MixinAddingResources>()
          .EnterScope())
      {
        var resourceString = GetResourceStringForType (typeof (TargetClassForGlobalization), "PropertyForMixinOverrideTest");

        var ct = MixinTypeUtility.GetConcreteMixedType (typeof (TargetClassForGlobalization));
        var a = ct.GetCustomAttributes(false);

        Assert.That (resourceString, Is.EqualTo ("PropertyForMixinOverrideTest display name from MixinAddingResources"));
      }
    }

    [Test]
    public void PropertyDisplayName_PropertyAddedViaMixin ()
    {
      using (MixinConfiguration.BuildFromActive()
          .ForClass<TargetClassForGlobalization>()
          .AddMixin<MixinAddingResources>()
          .EnterScope())
      {
        var resourceString = GetResourceStringForType (typeof (TargetClassForGlobalization), "MixedProperty1");

        Assert.That (resourceString, Is.EqualTo ("MixedProperty1 display name from TargetClassForGlobalization"));
      }
    }

    [Test]
    public void PropertyDisplayName_MixedProperty ()
    {
      using (MixinConfiguration.BuildFromActive()
          .ForClass<TargetClassForGlobalization>()
          .AddMixin<MixinAddingResources>()
          .EnterScope())
      {
        var resourceString = GetResourceStringForType (typeof (TargetClassForGlobalization), "MixedProperty2");

        Assert.That (resourceString, Is.EqualTo ("MixedProperty2 display name from MixinAddingResources"));
      }
    }

    [Test]
    public void PropertyDisplayName_ImplicitInterfaceProperty ()
    {
      var resourceString = GetResourceStringForType (typeof (TargetClassForGlobalization), "ImplicitImplementedProperty");

      Assert.That (resourceString, Is.EqualTo ("ImplicitImplementedProperty display name from TargetClassForGlobalization"));
    }

    [Test]
    public void PropertyDisplayName_ExplicitInterfaceProperty ()
    {
      var resourceString = GetResourceStringForType (typeof (TargetClassForGlobalization), "ExplicitImplementedProperty");

      Assert.That (resourceString, Is.EqualTo ("ExplicitImplementedProperty display name from TargetClassForGlobalization"));
    }

    [Test]
    public void PropertyDisplayName_MixedExplicitProperty ()
    {
      using (MixinConfiguration.BuildFromActive()
          .ForClass<TargetClassForGlobalization>()
          .AddMixin<MixinAddingResources>()
          .EnterScope())
      {
        var resourceString = GetResourceStringForType (typeof (TargetClassForGlobalization), "MixedExplicitProperty");

        Assert.That (resourceString, Is.EqualTo ("MixedExplicitProperty display name from TargetClassForGlobalization"));
      }
    }

    [Test]
    public void PropertyDisplayName_MixedPropertyInDerivedClass ()
    {
      using (MixinConfiguration.BuildFromActive()
          .ForClass<DerivedTargetClassForGlobalization>()
          .AddMixin<DerivedMixinAddingResources>()
          .EnterScope())
      {
        var resourceString = GetResourceStringForType (typeof (DerivedTargetClassForGlobalization), "MixedProperty3");

        Assert.That (resourceString, Is.EqualTo ("MixedProperty3 display name from DerivedMixinAddingResources"));
      }
    }

    [Test]
    public void PropertyDisplayName_OverridenMixedPropertyInDerivedClass ()
    {
      using (MixinConfiguration.BuildFromActive()
          .ForClass<DerivedTargetClassForGlobalization>()
          .AddMixin<DerivedMixinAddingResources>()
          .EnterScope())
      {
        var resourceString = GetResourceStringForType (typeof (DerivedTargetClassForGlobalization), "MixedProperty2");

        Assert.That (resourceString, Is.EqualTo ("MixedProperty2 display name from DerivedMixinAddingResources"));
      }
    }

    [Test]
    public void PropertyDisplayName_PropertyAddedInDerivedClass_OverridenInDerivedMixin ()
    {
      using (MixinConfiguration.BuildFromActive()
          .ForClass<DerivedTargetClassForGlobalization>()
          .AddMixin<DerivedMixinAddingResources>()
          .EnterScope())
      {
        var resourceString = GetResourceStringForType (typeof (DerivedTargetClassForGlobalization), "Property4");

        Assert.That (resourceString, Is.EqualTo ("Property4 display name from DerivedMixinAddingResources"));
      }
    }

    [Test]
    public void PropertyDisplayName_PropertyOverridenInMixinOfMixin ()
    {
      using (MixinConfiguration.BuildFromActive()
          .ForClass<TargetClassForGlobalization>()
          .AddMixin<MixinAddingResources>()
          .ForClass<MixinAddingResources>()
          .AddMixin<MixinOfMixinAddingResources>()
          .EnterScope())
      {
        var resourceString = GetResourceStringForType (typeof (TargetClassForGlobalization), "Property1");

        Assert.That (resourceString, Is.EqualTo ("Property1 display name from MixinOfMixinAddingResources"));
      }
    }

    [Test]
    public void PropertyDisplayName_MixedPropertyOverridenInMixinOfMixin ()
    {
      using (MixinConfiguration.BuildFromActive()
          .ForClass<TargetClassForGlobalization>()
          .AddMixin<MixinAddingResources>()
          .ForClass<MixinAddingResources>()
          .AddMixin<MixinOfMixinAddingResources>()
          .EnterScope())
      {
        var resourceString = GetResourceStringForType (typeof (TargetClassForGlobalization), "MixedProperty2");

        Assert.That (resourceString, Is.EqualTo ("MixedProperty2 display name from MixinOfMixinAddingResources"));
      }
    }

    private string GetResourceStringForType (Type targetType, string propertyName)
    {
      var classReflector = _factory.CreateClassReflector (targetType, new BindableObjectProvider());
      var @class = classReflector.GetMetadata();
      return @class.GetPropertyDefinition (propertyName).DisplayName;
    }
  }
}
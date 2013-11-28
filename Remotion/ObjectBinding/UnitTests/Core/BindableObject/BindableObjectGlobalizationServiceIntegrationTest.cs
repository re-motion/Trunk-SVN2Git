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
using System.Globalization;
using System.Threading;
using NUnit.Framework;
using Remotion.Mixins;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.BindableObject.Properties;
using Remotion.ObjectBinding.UnitTests.Core.TestDomain;
using Remotion.Reflection;
using Remotion.ServiceLocation;

namespace Remotion.ObjectBinding.UnitTests.Core.BindableObject
{
  [TestFixture]
  public class BindableObjectGlobalizationServiceIntegrationTest : TestBase
  {
    private IBindableObjectGlobalizationService _globalizationService;
    private CultureInfo _uiCultureBackup;
    private DefaultServiceLocator _defaultServiceLocator;
    private TypeAdapter _targetType;

    public override void SetUp ()
    {
      base.SetUp();

      _defaultServiceLocator = new DefaultServiceLocator();
      _globalizationService = _defaultServiceLocator.GetInstance<IBindableObjectGlobalizationService>();
      var bindableObjectClass = BindableObjectProviderTestHelper.GetBindableObjectClass (typeof (ClassWithResources));
      _targetType = TypeAdapter.Create (bindableObjectClass.TargetType);

      _uiCultureBackup = Thread.CurrentThread.CurrentUICulture;
      Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;
    }

    public override void TearDown ()
    {
      base.TearDown();
      Thread.CurrentThread.CurrentUICulture = _uiCultureBackup;
    }

    [Test]
    public void GetBooleanValueDisplayName ()
    {
      Assert.That (_globalizationService.GetBooleanValueDisplayName (true), Is.EqualTo ("Yes"));
      Assert.That (_globalizationService.GetBooleanValueDisplayName (false), Is.EqualTo ("No"));
    }

    [Test]
    public void GetEnumerationValueDisplayName ()
    {
      Assert.That (_globalizationService.GetEnumerationValueDisplayName (EnumWithResources.Value1), Is.EqualTo ("Value 1"));
      Assert.That (_globalizationService.GetEnumerationValueDisplayName (EnumWithResources.Value2), Is.EqualTo ("Value 2"));
      Assert.That (_globalizationService.GetEnumerationValueDisplayName (EnumWithResources.ValueWithoutResource), Is.EqualTo ("ValueWithoutResource"));
    }

    [Test]
    public void GetEnumerationValueDisplayName_WithDescription ()
    {
      Assert.That (_globalizationService.GetEnumerationValueDisplayName (EnumWithDescription.Value1), Is.EqualTo ("Value I"));
      Assert.That (_globalizationService.GetEnumerationValueDisplayName (EnumWithDescription.Value2), Is.EqualTo ("Value II"));
      Assert.That (
          _globalizationService.GetEnumerationValueDisplayName (EnumWithDescription.ValueWithoutDescription),
          Is.EqualTo ("ValueWithoutDescription"));
    }

    [Test]
    public void GetEnumerationValueDisplayName_WithoutResources ()
    {
      Assert.That (_globalizationService.GetEnumerationValueDisplayName (TestEnum.Value1), Is.EqualTo ("Value1"));
      Assert.That (_globalizationService.GetEnumerationValueDisplayName (TestEnum.Value2), Is.EqualTo ("Value2"));
      Assert.That (_globalizationService.GetEnumerationValueDisplayName (TestEnum.Value3), Is.EqualTo ("Value3"));
    }

    [Test]
    public void GetExtensibleEnumerationValueDisplayName ()
    {
      Assert.That (_globalizationService.GetExtensibleEnumerationValueDisplayName (ExtensibleEnumWithResources.Values.Value1()), Is.EqualTo ("Wert1"));
      Assert.That (_globalizationService.GetExtensibleEnumerationValueDisplayName (ExtensibleEnumWithResources.Values.Value2()), Is.EqualTo ("Wert2"));
      Assert.That (
          _globalizationService.GetExtensibleEnumerationValueDisplayName (ExtensibleEnumWithResources.Values.ValueWithoutResource()),
          Is.EqualTo ("Remotion.ObjectBinding.UnitTests.Core.TestDomain.ExtensibleEnumWithResourcesExtensions.ValueWithoutResource"));
    }

    [Test]
    public void GetPropertyDisplayName ()
    {
      IPropertyInformation propertyInformation = GetPropertyInfo (typeof (ClassWithResources), "Value1");
      Assert.That (_globalizationService.GetPropertyDisplayName (_targetType, propertyInformation), Is.EqualTo ("Value 1"));
    }

    [Test]
    public void GetPropertyDisplayName_LongPropertyName ()
    {
      IPropertyInformation propertyInformation = GetPropertyInfo (typeof (ClassWithResources), "Value2");
      Assert.That (_globalizationService.GetPropertyDisplayName (_targetType, propertyInformation), Is.EqualTo ("Value 2"));
    }

    [Test]
    public void GetPropertyDisplayName_WithoutMultiLingualResourcesAttribute ()
    {
      IPropertyInformation propertyInformation = GetPropertyInfo (typeof (SimpleBusinessObjectClass), "String");
      Assert.That (_globalizationService.GetPropertyDisplayName (_targetType, propertyInformation), Is.EqualTo ("String"));
    }

    [Test]
    public void GetPropertyDisplayName_WithoutResourceForProperty ()
    {
      IPropertyInformation propertyInformation = GetPropertyInfo (typeof (ClassWithResources), "ValueWithoutResource");
      Assert.That (_globalizationService.GetPropertyDisplayName (_targetType, propertyInformation), Is.EqualTo ("ValueWithoutResource"));
    }

    [Test]
    public void GetPropertyDisplayName_WithMixin ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<SimpleBusinessObjectClass>().AddMixin<MixinAddingResources>().EnterScope())
      {
        IPropertyInformation propertyInformation = GetPropertyInfo (typeof (SimpleBusinessObjectClass), "String");
        Assert.That (
            _globalizationService.GetPropertyDisplayName (TypeAdapter.Create (typeof (MixinAddingResources)), propertyInformation),
            Is.EqualTo ("Resource from mixin"));
      }
    }

    [Test]
    public void GetPropertyDisplayName_WithMixin_ResourceEntryIsOverriddenByMixin ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<SimpleBusinessObjectClass>().AddMixin<MixinAddingResources>().EnterScope())
      {
        IPropertyInformation propertyInformation = GetPropertyInfo (typeof (SimpleBusinessObjectClass), "PropertyForMixinOverriddeTest");
        Assert.That (
            _globalizationService.GetPropertyDisplayName (TypeAdapter.Create (typeof (MixinAddingResources)), propertyInformation),
            Is.EqualTo ("overridden by mixin"));
      }
    }

    [Test]
    public void GetPropertyDisplayName_WithMixin_LongPropertyName ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<SimpleBusinessObjectClass>().AddMixin<MixinAddingResources>().EnterScope())
      {
        IPropertyInformation propertyInformation = GetPropertyInfo (typeof (SimpleBusinessObjectClass), "StringForLongPropertyName");
        Assert.That (
            _globalizationService.GetPropertyDisplayName (TypeAdapter.Create (typeof (MixinAddingResources)), propertyInformation),
            Is.EqualTo ("Resource from mixin for long property name"));
      }
    }

    [Test]
    [Ignore ("BindableObjectGlobalizationService currently does not support dynamic changes to the mixin configuration. (TODO: change that)")]
    public void GetPropertyDisplayName_WithTwoMixins_WithDependency_IntegrationTest ()
    {
      using (MixinConfiguration.BuildFromActive()
          .ForClass<SimpleBusinessObjectClass>()
          .AddMixin<MixinAddingResources>()
          .AddMixin<MixinAddingResources2>().WithDependency<MixinAddingResources>()
          .EnterScope())
      {
        IPropertyInformation propertyInformation = GetPropertyInfo (typeof (SimpleBusinessObjectClass), "String");
        Assert.That (
            _globalizationService.GetPropertyDisplayName (TypeAdapter.Create (typeof (MixinAddingResources2)), propertyInformation),
            Is.EqualTo ("Resource from mixin2"));
      }

      using (MixinConfiguration.BuildFromActive()
          .ForClass<SimpleBusinessObjectClass>()
          .AddMixin<MixinAddingResources>().WithDependency<MixinAddingResources2>()
          .AddMixin<MixinAddingResources2>()
          .EnterScope())
      {
        IPropertyInformation propertyInformation = GetPropertyInfo (typeof (SimpleBusinessObjectClass), "String");
        Assert.That (
            _globalizationService.GetPropertyDisplayName (TypeAdapter.Create (typeof (MixinAddingResources)), propertyInformation),
            Is.EqualTo ("Resource from mixin"));
      }
    }

    [Test]
    public void GetPropertyDisplayName_WithTwoMixins_WithDependency1_IntegrationTest ()
    {
      using (MixinConfiguration.BuildFromActive()
          .ForClass<SimpleBusinessObjectClass>()
          .AddMixin<MixinAddingResources>()
          .AddMixin<MixinAddingResources2>().WithDependency<MixinAddingResources>()
          .EnterScope())
      {
        IPropertyInformation propertyInformation = GetPropertyInfo (typeof (SimpleBusinessObjectClass), "String");
        Assert.That (
            _globalizationService.GetPropertyDisplayName (TypeAdapter.Create (typeof (MixinAddingResources2)), propertyInformation),
            Is.EqualTo ("Resource from mixin2"));
      }
    }

    [Test]
    public void GetPropertyDisplayName_WithTwoMixins_WithDependency2_IntegrationTest ()
    {
      using (MixinConfiguration.BuildFromActive()
          .ForClass<SimpleBusinessObjectClass>()
          .AddMixin<MixinAddingResources>().WithDependency<MixinAddingResources2>()
          .AddMixin<MixinAddingResources2>()
          .EnterScope())
      {
        IPropertyInformation propertyInformation = GetPropertyInfo (typeof (SimpleBusinessObjectClass), "String");
        Assert.That (
            _globalizationService.GetPropertyDisplayName (TypeAdapter.Create (typeof (MixinAddingResources)), propertyInformation),
            Is.EqualTo ("Resource from mixin"));
      }
    }

    [Test]
    public void GetPropertyDisplayName_WithPropertyAddedByMixin ()
    {
      BindableObjectClass bindableClass = BindableObjectProviderTestHelper.GetBindableObjectClass (typeof (ClassWithMixedPropertyAndResources));
      PropertyBase property = (PropertyBase) bindableClass.GetPropertyDefinition ("MixedProperty");

      Assert.That (
          _globalizationService.GetPropertyDisplayName (TypeAdapter.Create (bindableClass.TargetType), property.PropertyInfo),
          Is.EqualTo ("Resourced!"));
    }

    [Test]
    public void GetPropertyDisplayName_WithPropertyAddedByMixin_NoMixinResource ()
    {
      BindableObjectClass bindableClass = BindableObjectProviderTestHelper.GetBindableObjectClass (typeof (ClassWithMixedPropertyAndResources));
      PropertyBase property = (PropertyBase) bindableClass.GetPropertyDefinition ("MixedReadOnlyProperty");

      Assert.That (
          _globalizationService.GetPropertyDisplayName (TypeAdapter.Create (bindableClass.TargetType), property.PropertyInfo),
          Is.EqualTo ("MixedReadOnlyProperty"));
    }
  }
}
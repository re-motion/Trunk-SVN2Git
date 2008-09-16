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
using System.Globalization;
using System.Threading;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Mixins;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.BindableObject.Properties;
using Remotion.ObjectBinding.UnitTests.Core.TestDomain;

namespace Remotion.ObjectBinding.UnitTests.Core.BindableObject
{
  [TestFixture]
  public class BindableObjectGlobalizationServiceTest : TestBase
  {
    private IBindableObjectGlobalizationService _globalizationService;
    private CultureInfo _uiCultureBackup;

    public override void SetUp ()
    {
      base.SetUp();

      _globalizationService = new BindableObjectGlobalizationService();

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
    public void GetPropertyDisplayName ()
    {
      IPropertyInformation IPropertyInformation = GetPropertyInfo (typeof (ClassWithResources), "Value1");
      Assert.That (_globalizationService.GetPropertyDisplayName (IPropertyInformation), Is.EqualTo ("Value 1"));
    }

    [Test]
    public void GetPropertyDisplayName_WithoutMultiLingualResourcesAttribute ()
    {
      IPropertyInformation IPropertyInformation = GetPropertyInfo (typeof (SimpleBusinessObjectClass), "String");
      Assert.That (_globalizationService.GetPropertyDisplayName (IPropertyInformation), Is.EqualTo ("String"));
    }

    [Test]
    public void GetPropertyDisplayName_WithoutResourceForProperty ()
    {
      IPropertyInformation IPropertyInformation = GetPropertyInfo (typeof (ClassWithResources), "ValueWithoutResource");
      Assert.That (_globalizationService.GetPropertyDisplayName (IPropertyInformation), Is.EqualTo ("ValueWithoutResource"));
    }

    [Test]
    public void GetPropertyDisplayName_WithMixin ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<SimpleBusinessObjectClass>().AddMixin<MixinAddingResources>().EnterScope())
      {
        IPropertyInformation IPropertyInformation = GetPropertyInfo (typeof (SimpleBusinessObjectClass), "String");
        Assert.That (_globalizationService.GetPropertyDisplayName (IPropertyInformation), Is.EqualTo ("Resource from mixin"));
      }
    }

    [Test]
    [Ignore ("BindableObjectGlobalizationService currently does not support dynamic changes to the mixin configuration. (TODO: change that)")]
    public void GetPropertyDisplayName_WithTwoMixins_WithDependency_IntegrationTest ()
    {
      using (MixinConfiguration.BuildFromActive ()
          .ForClass<SimpleBusinessObjectClass> ()
          .AddMixin<MixinAddingResources> ()
          .AddMixin<MixinAddingResources2>().WithDependency<MixinAddingResources>()
          .EnterScope ())
      {
        IPropertyInformation propertyInformation = GetPropertyInfo (typeof (SimpleBusinessObjectClass), "String");
        Assert.That (_globalizationService.GetPropertyDisplayName (propertyInformation), Is.EqualTo ("Resource from mixin2"));
      }

      using (MixinConfiguration.BuildFromActive ()
          .ForClass<SimpleBusinessObjectClass> ()
          .AddMixin<MixinAddingResources> ().WithDependency<MixinAddingResources2> ()
          .AddMixin<MixinAddingResources2> ()
          .EnterScope ())
      {
        IPropertyInformation propertyInformation = GetPropertyInfo (typeof (SimpleBusinessObjectClass), "String");
        Assert.That (_globalizationService.GetPropertyDisplayName (propertyInformation), Is.EqualTo ("Resource from mixin"));
      }
    }

    [Test]
    public void GetPropertyDisplayName_WithTwoMixins_WithDependency1_IntegrationTest ()
    {
      using (MixinConfiguration.BuildFromActive ()
          .ForClass<SimpleBusinessObjectClass> ()
          .AddMixin<MixinAddingResources> ()
          .AddMixin<MixinAddingResources2> ().WithDependency<MixinAddingResources> ()
          .EnterScope ())
      {
        IPropertyInformation propertyInformation = GetPropertyInfo (typeof (SimpleBusinessObjectClass), "String");
        Assert.That (_globalizationService.GetPropertyDisplayName (propertyInformation), Is.EqualTo ("Resource from mixin2"));
      }
    }

    [Test]
    public void GetPropertyDisplayName_WithTwoMixins_WithDependency2_IntegrationTest ()
    {
      using (MixinConfiguration.BuildFromActive ()
          .ForClass<SimpleBusinessObjectClass> ()
          .AddMixin<MixinAddingResources> ().WithDependency<MixinAddingResources2> ()
          .AddMixin<MixinAddingResources2> ()
          .EnterScope ())
      {
        IPropertyInformation propertyInformation = GetPropertyInfo (typeof (SimpleBusinessObjectClass), "String");
        Assert.That (_globalizationService.GetPropertyDisplayName (propertyInformation), Is.EqualTo ("Resource from mixin"));
      }
    }

    [Test]
    public void GetPropertyDisplayName_WithPropertyAddedByMixin ()
    {
      BindableObjectClass bindableClass = BindableObjectProvider.GetBindableObjectClass (typeof (ClassWithMixedPropertyAndResources));
      PropertyBase property = (PropertyBase) bindableClass.GetPropertyDefinition ("MixedProperty");

      Assert.That (_globalizationService.GetPropertyDisplayName (property.PropertyInfo), Is.EqualTo ("Resourced!"));
    }
  }
}

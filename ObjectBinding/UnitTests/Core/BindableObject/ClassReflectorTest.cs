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
using NUnit.Framework.SyntaxHelpers;
using Remotion.Mixins;
using Rhino.Mocks;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.BindableObject.Properties;
using Remotion.ObjectBinding.UnitTests.Core.TestDomain;

namespace Remotion.ObjectBinding.UnitTests.Core.BindableObject
{
  [TestFixture]
  public class ClassReflectorTest : TestBase
  {
    private BindableObjectProvider _businessObjectProvider;
    private Type _type;
    private IClassReflector _classReflector;
    private BindableObjectMetadataFactory _metadataFactory;

    public override void SetUp ()
    {
      base.SetUp();

      _type = typeof (DerivedBusinessObjectClass);
      _businessObjectProvider = new BindableObjectProvider();
      _metadataFactory = BindableObjectMetadataFactory.Create ();
      _classReflector = new ClassReflector (_type, _businessObjectProvider, _metadataFactory);
    }

    [Test]
    public void Initialize ()
    {
      Assert.That (_classReflector.TargetType, Is.SameAs (_type));
      Assert.That (((ClassReflector) _classReflector).ConcreteType, Is.Not.SameAs (_type));
      Assert.That (((ClassReflector) _classReflector).ConcreteType, Is.SameAs (Mixins.TypeUtility.GetConcreteMixedType (_type)));
      Assert.That (_classReflector.BusinessObjectProvider, Is.SameAs (_businessObjectProvider));
    }

    [Test]
    public void GetMetadata ()
    {
      BindableObjectClass bindableObjectClass = _classReflector.GetMetadata();

      Assert.That (bindableObjectClass, Is.InstanceOfType (typeof (IBusinessObjectClass)));
      Assert.That (bindableObjectClass.TargetType, Is.SameAs (_type));
      Assert.That (bindableObjectClass.GetPropertyDefinitions().Length, Is.EqualTo (1));
      Assert.That (bindableObjectClass.GetPropertyDefinitions()[0].Identifier, Is.EqualTo ("Public"));
      Assert.That (((PropertyBase) bindableObjectClass.GetPropertyDefinitions()[0]).PropertyInfo.DeclaringType, Is.SameAs (_type));
      Assert.That (bindableObjectClass.GetPropertyDefinitions()[0].BusinessObjectProvider, Is.SameAs (_businessObjectProvider));
    }

    [Test]
    public void GetMetadata_ForBindableObjectWithIdentity ()
    {
      ClassReflector classReflector = new ClassReflector (typeof (ClassWithIdentity), _businessObjectProvider, _metadataFactory);
      BindableObjectClass bindableObjectClass = classReflector.GetMetadata();

      Assert.That (bindableObjectClass, Is.InstanceOfType (typeof (IBusinessObjectClassWithIdentity)));
      Assert.That (bindableObjectClass.TargetType, Is.SameAs (typeof (ClassWithIdentity)));
      Assert.That (bindableObjectClass.GetPropertyDefinitions()[0].BusinessObjectProvider, Is.SameAs (_businessObjectProvider));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Type 'Remotion.ObjectBinding.UnitTests.Core.TestDomain."
        + "ClassWithManualIdentity' does not implement the 'Remotion.ObjectBinding.IBusinessObject' interface via the 'Remotion.ObjectBinding."
        + "BindableObject.BindableObjectMixinBase`1'.\r\nParameter name: concreteType")]
    public void GetMetadata_ForBindableObjectWithManualIdentity ()
    {
      ClassReflector classReflector = new ClassReflector (typeof (ClassWithManualIdentity), _businessObjectProvider, _metadataFactory);
      classReflector.GetMetadata ();
    }

    [Test]
    public void GetMetadata_UsesFactory ()
    {
      MockRepository mockRepository = new MockRepository ();
      IMetadataFactory factoryMock = mockRepository.CreateMock<IMetadataFactory> ();

      IPropertyInformation dummyProperty1 = GetPropertyInfo (typeof (DateTime), "Now");
      IPropertyInformation dummyProperty2 = GetPropertyInfo (typeof (Environment), "TickCount");

      PropertyReflector dummyReflector1 = PropertyReflector.Create(GetPropertyInfo (typeof (DateTime), "Ticks"), _businessObjectProvider);
      PropertyReflector dummyReflector2 = PropertyReflector.Create(GetPropertyInfo (typeof (Environment), "NewLine"), _businessObjectProvider);

      IPropertyFinder propertyFinderMock = mockRepository.CreateMock<IPropertyFinder> ();

      ClassReflector otherClassReflector = new ClassReflector (_type, _businessObjectProvider, factoryMock);

      Type concreteType = Mixins.TypeUtility.GetConcreteMixedType (_type);

      Expect.Call (factoryMock.CreatePropertyFinder (concreteType)).Return (propertyFinderMock);
      Expect.Call (propertyFinderMock.GetPropertyInfos ()).Return (new IPropertyInformation[] { dummyProperty1, dummyProperty2 });
      Expect.Call (factoryMock.CreatePropertyReflector (concreteType, dummyProperty1, _businessObjectProvider)).Return (dummyReflector1);
      Expect.Call (factoryMock.CreatePropertyReflector (concreteType, dummyProperty2, _businessObjectProvider)).Return (dummyReflector2);

      mockRepository.ReplayAll ();

      BindableObjectClass theClass = otherClassReflector.GetMetadata ();
      Assert.IsTrue (theClass.HasPropertyDefinition ("Ticks"));
      Assert.IsTrue (theClass.HasPropertyDefinition ("NewLine"));
      
      Assert.IsFalse (theClass.HasPropertyDefinition ("Now"));
      Assert.IsFalse (theClass.HasPropertyDefinition ("TickCount"));

      mockRepository.VerifyAll ();
    }


    [Test]
    [ExpectedException (typeof (NotSupportedException), ExpectedMessage = "Type '.*ClassWithMixedPropertyOfSameName' has two properties called "
        + "'MixedProperty', this is currently not supported.", MatchType = MessageMatch.Regex)]
    public void GetMetadata_ForMixedPropertyWithSameName ()
    {
      ClassReflector classReflector = new ClassReflector (typeof (ClassWithMixedPropertyOfSameName), _businessObjectProvider,
          _metadataFactory);
      classReflector.GetMetadata ();
    }

    [Test]
    public void ClassReflector_CreatesBaseClass_CompatibleWithDerivedInstances ()
    {
      var classReflector = new ClassReflector (typeof (BaseBusinessObjectClass), _businessObjectProvider, _metadataFactory);
      var bindableObjectClass = classReflector.GetMetadata ();
      var derivedBusinessObject = ObjectFactory.Create<DerivedBusinessObjectClass> ().With();

      ((BaseBusinessObjectClass) derivedBusinessObject).Public = "p";
      var propertyDefinition = bindableObjectClass.GetPropertyDefinition ("Public");
      var businessObject = (IBusinessObject) derivedBusinessObject;
      Assert.That (businessObject.GetProperty (propertyDefinition), Is.EqualTo ("p"));
    }

    [Test]
    public void ClassReflector_CreatesBaseClass_CompatibleWithDerivedInstances_WithMixins ()
    {
      using (MixinConfiguration.BuildNew ()
          .ForClass<BaseBusinessObjectClass> ().AddMixin<MixinAddingProperty> ()
          .ForClass<BaseBusinessObjectClass> ().AddMixin<BindableObjectMixin> ()
          .EnterScope ())
      {
        var classReflector = new ClassReflector (typeof (BaseBusinessObjectClass), _businessObjectProvider, _metadataFactory);
        var bindableObjectClass = classReflector.GetMetadata();
        var derivedBusinessObject = ObjectFactory.Create<DerivedBusinessObjectClass>().With();

        Assert.That (derivedBusinessObject, Is.InstanceOfType (typeof (IMixinAddingProperty)));

        ((BaseBusinessObjectClass) derivedBusinessObject).Public = "p";
        var propertyDefinition = bindableObjectClass.GetPropertyDefinition ("Public");
        var businessObject = (IBusinessObject) derivedBusinessObject;
        Assert.That (businessObject.GetProperty (propertyDefinition), Is.EqualTo ("p"));
      }
    }

    [Test]
    public void ClassReflector_CreatesBaseClass_CompatibleWithDerivedInstances_WithMixins_WithMixedProperty ()
    {
      using (MixinConfiguration.BuildNew ()
          .ForClass<BaseBusinessObjectClass> ().AddMixin<MixinAddingProperty> ()
          .ForClass<BaseBusinessObjectClass> ().AddMixin<BindableObjectMixin> ()
          .EnterScope ())
      {
        var classReflector = new ClassReflector (typeof (BaseBusinessObjectClass), _businessObjectProvider, _metadataFactory);
        var bindableObjectClass = classReflector.GetMetadata ();
        var derivedBusinessObject = ObjectFactory.Create<DerivedBusinessObjectClass> ().With ();

        ((IMixinAddingProperty) derivedBusinessObject).MixedProperty = "p";
        var propertyDefinition = bindableObjectClass.GetPropertyDefinition ("MixedProperty");
        Assert.That (propertyDefinition, Is.Not.Null);
        
        var businessObject = (IBusinessObject) derivedBusinessObject;
        Assert.That (businessObject.GetProperty (propertyDefinition), Is.EqualTo ("p"));
      }
    }
  }
}

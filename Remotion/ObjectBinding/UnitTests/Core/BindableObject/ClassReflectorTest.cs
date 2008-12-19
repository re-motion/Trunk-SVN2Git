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
      Assert.That (((ClassReflector) _classReflector).ConcreteType, Is.SameAs (MixinTypeUtility.GetConcreteMixedType (_type)));
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
      var classReflector = new ClassReflector (typeof (ClassWithIdentity), _businessObjectProvider, _metadataFactory);
      BindableObjectClass bindableObjectClass = classReflector.GetMetadata();

      Assert.That (bindableObjectClass, Is.InstanceOfType (typeof (IBusinessObjectClassWithIdentity)));
      Assert.That (bindableObjectClass.TargetType, Is.SameAs (typeof (ClassWithIdentity)));
      Assert.That (bindableObjectClass.GetPropertyDefinitions()[0].BusinessObjectProvider, Is.SameAs (_businessObjectProvider));
    }

    [Test]
    public void GetMetadata_ForBindableObjectWithManualIdentity ()
    {
      var classReflector = new ClassReflector (typeof (ClassWithManualIdentity), _businessObjectProvider, _metadataFactory);
      var bindableObjectClass = classReflector.GetMetadata ();

      Assert.That (bindableObjectClass, Is.InstanceOfType (typeof (IBusinessObjectClassWithIdentity)));
      Assert.That (bindableObjectClass.TargetType, Is.SameAs (typeof (ClassWithManualIdentity)));
    }

    [Test]
    public void GetMetadata_UsesFactory ()
    {
      var mockRepository = new MockRepository ();
      var factoryMock = mockRepository.StrictMock<IMetadataFactory> ();

      IPropertyInformation dummyProperty1 = GetPropertyInfo (typeof (DateTime), "Now");
      IPropertyInformation dummyProperty2 = GetPropertyInfo (typeof (Environment), "TickCount");

      PropertyReflector dummyReflector1 = PropertyReflector.Create(GetPropertyInfo (typeof (DateTime), "Ticks"), _businessObjectProvider);
      PropertyReflector dummyReflector2 = PropertyReflector.Create(GetPropertyInfo (typeof (Environment), "NewLine"), _businessObjectProvider);

      var propertyFinderMock = mockRepository.StrictMock<IPropertyFinder> ();

      var otherClassReflector = new ClassReflector (_type, _businessObjectProvider, factoryMock);

      Type concreteType = MixinTypeUtility.GetConcreteMixedType (_type);

      Expect.Call (factoryMock.CreatePropertyFinder (concreteType)).Return (propertyFinderMock);
      Expect.Call (propertyFinderMock.GetPropertyInfos ()).Return (new[] { dummyProperty1, dummyProperty2 });
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
      var classReflector = new ClassReflector (typeof (ClassWithMixedPropertyOfSameName), _businessObjectProvider, _metadataFactory);
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

    [Test]
    [Ignore ("TODO: COMMONS-936")]
    public void ClassReflector_CreatesBaseClass_CompatibleWithDerivedInstances_WithMixins_WithOverriddenProperty ()
    {
      using (MixinConfiguration.BuildNew ()
          .ForClass<BaseBusinessObjectClass> ().AddMixin<MixinOverridingProperty> ()
          .ForClass<BaseBusinessObjectClass> ().AddMixin<BindableObjectMixin> ()
          .EnterScope ())
      {
        var classReflector = new ClassReflector (typeof (BaseBusinessObjectClass), _businessObjectProvider, _metadataFactory);
        var bindableObjectClass = classReflector.GetMetadata ();
        var derivedBusinessObject = ObjectFactory.Create<DerivedBusinessObjectClass> ().With ();

        derivedBusinessObject.Public = "p";
        Mixin.Get<MixinOverridingProperty> (derivedBusinessObject).Public += "q";
        
        var propertyDefinition = bindableObjectClass.GetPropertyDefinition ("Public");
        Assert.That (propertyDefinition, Is.Not.Null);

        var businessObject = (IBusinessObject) derivedBusinessObject;
        Assert.That (businessObject.GetProperty (propertyDefinition), Is.EqualTo ("pq"));
      }
    }
  }
}

using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Rhino.Mocks;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.BindableObject.Properties;
using Remotion.ObjectBinding.UnitTests.Core.BindableObject.TestDomain;
using System.Reflection;

namespace Remotion.ObjectBinding.UnitTests.Core.BindableObject
{
  [TestFixture]
  public class ClassReflectorTest : TestBase
  {
    private BindableObjectProvider _businessObjectProvider;
    private Type _type;
    private ClassReflector _classReflector;

    public override void SetUp ()
    {
      base.SetUp();

      _type = typeof (DerivedBusinessObjectClass);
      _businessObjectProvider = new BindableObjectProvider();
      _classReflector = new ClassReflector (_type, _businessObjectProvider, DefaultMetadataFactory.Instance);
    }

    [Test]
    public void Initialize ()
    {
      Assert.That (_classReflector.TargetType, Is.SameAs (_type));
      Assert.That (_classReflector.ConcreteType, Is.Not.SameAs (_type));
      Assert.That (_classReflector.ConcreteType, Is.SameAs (Mixins.TypeUtility.GetConcreteMixedType (_type)));
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
      ClassReflector classReflector = new ClassReflector (typeof (ClassWithIdentity), _businessObjectProvider, DefaultMetadataFactory.Instance);
      BindableObjectClass bindableObjectClass = classReflector.GetMetadata();

      Assert.That (bindableObjectClass, Is.InstanceOfType (typeof (IBusinessObjectClassWithIdentity)));
      Assert.That (bindableObjectClass.TargetType, Is.SameAs (typeof (ClassWithIdentity)));
      Assert.That (bindableObjectClass.GetPropertyDefinitions()[0].BusinessObjectProvider, Is.SameAs (_businessObjectProvider));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Type 'Remotion.ObjectBinding.UnitTests.Core.BindableObject.TestDomain."
        + "ClassWithManualIdentity' does not implement the 'Remotion.ObjectBinding.IBusinessObject' interface via the 'Remotion.ObjectBinding."
        + "BindableObject.BindableObjectMixinBase`1'.\r\nParameter name: concreteType")]
    public void GetMetadata_ForBindableObjectWithManualIdentity ()
    {
      ClassReflector classReflector = new ClassReflector (typeof (ClassWithManualIdentity), _businessObjectProvider, DefaultMetadataFactory.Instance);
      classReflector.GetMetadata ();
    }

    [Test]
    public void GetMetadata_UsesFactory ()
    {
      MockRepository mockRepository = new MockRepository ();
      IMetadataFactory factoryMock = mockRepository.CreateMock<IMetadataFactory> ();

      IPropertyInformation dummyProperty1 = GetPropertyInfo (typeof (DateTime), "Now");
      IPropertyInformation dummyProperty2 = GetPropertyInfo (typeof (Environment), "TickCount");

      PropertyReflector dummyReflector1 = new PropertyReflector (GetPropertyInfo (typeof (DateTime), "Ticks"), _businessObjectProvider);
      PropertyReflector dummyReflector2 = new PropertyReflector (GetPropertyInfo (typeof (Environment), "NewLine"), _businessObjectProvider);

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
          DefaultMetadataFactory.Instance);
      classReflector.GetMetadata ();
    }

  }
}
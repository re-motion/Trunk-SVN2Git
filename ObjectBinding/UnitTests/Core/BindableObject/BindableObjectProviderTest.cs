using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Rhino.Mocks;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.UnitTests.Core.BindableObject.TestDomain;

namespace Remotion.ObjectBinding.UnitTests.Core.BindableObject
{
  [TestFixture]
  public class BindableObjectProviderTest : TestBase
  {
    private BindableObjectProvider _provider;
    private MockRepository _mockRepository;

    public override void SetUp ()
    {
      base.SetUp();

      _provider = new BindableObjectProvider();
      _mockRepository = new MockRepository();
    }

    [Test]
    public void GetInstance ()
    {
      Assert.That (BindableObjectProvider.Current, Is.TypeOf (typeof (BindableObjectProvider)));
      Assert.That (
          BindableObjectProvider.Current.GetService<IBindableObjectGlobalizationService>(),
          Is.TypeOf (typeof (BindableObjectGlobalizationService)));
    }

    [Test]
    public void GetInstance_SameTwice ()
    {
      Assert.That (BindableObjectProvider.Current, Is.SameAs (BindableObjectProvider.Current));
    }

    [Test]
    public void SetInstance ()
    {
      BindableObjectProvider.SetCurrent (_provider);
      Assert.That (BindableObjectProvider.Current, Is.SameAs (_provider));
    }

    [Test]
    public void AddAndGetService ()
    {
      IBusinessObjectService expectedService = _mockRepository.Stub<IBusinessObjectService>();
      Assert.That (_provider.GetService (expectedService.GetType()), Is.Null);

      _provider.AddService (expectedService.GetType(), expectedService);

      Assert.That (_provider.GetService (expectedService.GetType()), Is.SameAs (expectedService));
    }

    [Test]
    public void GetServiceFromGeneric ()
    {
      _provider.AddService (typeof (IBusinessObjectService), _mockRepository.Stub<IBusinessObjectService>());

      Assert.That (_provider.GetService<IBusinessObjectService>(), Is.SameAs (_provider.GetService (typeof (IBusinessObjectService))));
    }

    [Test]
    public void GetDefaultServices ()
    {
      IBusinessObjectProvider currentProvider = BindableObjectProvider.Current;

      Assert.That (
        currentProvider.GetService (typeof (IBusinessObjectStringFormatterService)), 
        Is.InstanceOfType (typeof (BusinessObjectStringFormatterService)));

      Assert.That (
        currentProvider.GetService (typeof (IBindableObjectGlobalizationService)),
        Is.InstanceOfType (typeof (BindableObjectGlobalizationService)));
    }

    [Test]
    public void GetBindableObjectClass ()
    {
      BindableObjectClass actual = _provider.GetBindableObjectClass (typeof (SimpleBusinessObjectClass));

      Assert.That (actual, Is.Not.Null);
      Assert.That (actual.TargetType, Is.SameAs (typeof (SimpleBusinessObjectClass)));
      Assert.That (actual.BusinessObjectProvider, Is.SameAs (_provider));
    }

    [Test]
    public void GetBindableObjectClass_SameTwice ()
    {
      Assert.That (_provider.GetBindableObjectClass (typeof (SimpleBusinessObjectClass)), Is.SameAs (_provider.GetBindableObjectClass (typeof (SimpleBusinessObjectClass))));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException),
       ExpectedMessage =
       "Type 'Remotion.ObjectBinding.UnitTests.Core.BindableObject.TestDomain.SimpleReferenceType' does not implement the "
       + "'Remotion.ObjectBinding.IBusinessObject' interface via the 'Remotion.ObjectBinding.BindableObject.BindableObjectMixinBase`1'.\r\n"
       + "Parameter name: concreteType")]
    public void GetBindableObjectClass_WithTypeNotUsingBindableObjectMixin ()
    {
      _provider.GetBindableObjectClass (typeof (SimpleReferenceType));
    }

    [Test]
    public void GetMetadataFactoryForType_DefaultMetadataFactoryByDefault ()
    {
      Assert.AreSame (DefaultMetadataFactory.Instance, _provider.GetMetadataFactoryForType (typeof (SimpleBusinessObjectClass)));
    }

    [Test]
    public void GetMetadataFactoryForType_SpecialMetadataFactoryViaAttribute ()
    {
      Assert.AreNotSame (DefaultMetadataFactory.Instance, _provider.GetMetadataFactoryForType (typeof (ClassWithSpecialFactory)));
      Assert.AreSame (SpecialMetadataFactory.Instance, _provider.GetMetadataFactoryForType (typeof (ClassWithSpecialFactory)));
      Assert.IsTrue (_provider.GetBindableObjectClass (typeof (ClassWithSpecialFactory)).HasPropertyDefinition ("PrivateString"));
    }
  }
}
using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects.ObjectBinding;
using Remotion.Mixins;
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.BindableObject;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.ObjectBinding
{
  [TestFixture]
  public class BindableDomainObjectProviderTest
  {
    public class MixinStub
    { }

    [Test]
    public void Instantiate_WithDefaultValues ()
    {
      BindableDomainObjectProvider provider = new BindableDomainObjectProvider();
      Assert.IsInstanceOfType (typeof (BindableDomainObjectMetadataFactory), provider.MetadataFactory);
      Assert.IsInstanceOfType (typeof (BindableObjectServiceFactory), provider.ServiceFactory);
    }

    [Test]
    public void Instantiate_WithMixin ()
    {
      using (MixinConfiguration.BuildNew ().ForClass (typeof (BindableDomainObjectMetadataFactory)).AddMixin<MixinStub> ().EnterScope ())
      {
        BindableDomainObjectProvider provider = new BindableDomainObjectProvider ();
        Assert.That (provider.MetadataFactory, Is.InstanceOfType (typeof (BindableDomainObjectMetadataFactory)));
        Assert.That (provider.MetadataFactory, Is.InstanceOfType (typeof (IMixinTarget)));
        Assert.That (provider.ServiceFactory, Is.InstanceOfType (typeof (BindableObjectServiceFactory)));
      }
    }

    [Test]
    public void Instantiate_WithCustomValues ()
    {
      IMetadataFactory metadataFactoryStub = MockRepository.GenerateStub<IMetadataFactory>();
      IBusinessObjectServiceFactory serviceFactoryStub = MockRepository.GenerateStub<IBusinessObjectServiceFactory>();
      BindableDomainObjectProvider provider = new BindableDomainObjectProvider (metadataFactoryStub, serviceFactoryStub);
      
      Assert.AreSame (metadataFactoryStub, provider.MetadataFactory);
      Assert.AreSame (serviceFactoryStub, provider.ServiceFactory);
    }
  }
}
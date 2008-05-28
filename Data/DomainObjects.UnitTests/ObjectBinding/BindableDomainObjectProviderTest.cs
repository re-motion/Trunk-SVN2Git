using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects.ObjectBinding;
using Remotion.ObjectBinding.BindableObject;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.ObjectBinding
{
  [TestFixture]
  public class BindableDomainObjectProviderTest
  {
    [Test]
    public void Instantiate_WithDefaultValues ()
    {
      BindableDomainObjectProvider provider = new BindableDomainObjectProvider();
      Assert.AreSame (BindableDomainObjectMetadataFactory.Instance, provider.MetadataFactory);
    }

    [Test]
    public void Instantiate_WithCustomValues ()
    {
      IMetadataFactory metadataFactoryStub = MockRepository.GenerateStub<IMetadataFactory>();
      BindableDomainObjectProvider provider = new BindableDomainObjectProvider (metadataFactoryStub);
      Assert.AreSame (metadataFactoryStub, provider.MetadataFactory);
    }
  }
}
using System;
using Remotion.ObjectBinding.BindableObject;

namespace Remotion.Data.DomainObjects.ObjectBinding
{
  [AttributeUsage (AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
  public class UseBindableDomainObjectMetadataFactoryAttribute : UseCustomMetadataFactoryAttribute
  {
    public override IMetadataFactory GetFactoryInstance ()
    {
      return BindableDomainObjectMetadataFactory.Instance;
    }
  }
}
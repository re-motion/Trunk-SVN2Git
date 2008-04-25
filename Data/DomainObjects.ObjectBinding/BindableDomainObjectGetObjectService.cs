using System;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.BindableObject;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.ObjectBinding
{
  public class BindableDomainObjectGetObjectService : IGetObjectService
  {
    public IBusinessObjectWithIdentity GetObject (BindableObjectClassWithIdentity classWithIdentity, string uniqueIdentifier)
    {
      ArgumentUtility.CheckNotNull ("classWithIdentity", classWithIdentity);
      ArgumentUtility.CheckNotNullOrEmpty ("uniqueIdentifier", uniqueIdentifier);

      return (IBusinessObjectWithIdentity) RepositoryAccessor.GetObject (ObjectID.Parse (uniqueIdentifier), false);
    }
  }
}
using System;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.ObjectBinding;

namespace Remotion.SecurityManager.Domain
{
  [Serializable]
  public abstract class BaseSecurityManagerObject : BindableDomainObject
  {
    public static BaseSecurityManagerObject GetObject (ObjectID id)
    {
      return DomainObject.GetObject<BaseSecurityManagerObject> (id);
    }

    protected BaseSecurityManagerObject ()
    {
    }

    public new void Delete ()
    {
      base.Delete();
    }
  }
}

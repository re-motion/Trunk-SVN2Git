using System;
using Remotion.Data.DomainObjects.ObjectBinding;

namespace Remotion.Data.DomainObjects.Web.Test.Domain
{
  [Serializable]
  [DBTable ("TableWithoutColumns")]
  [Instantiable]
  [DBStorageGroup]
  public abstract class ClassWithoutProperties: BindableDomainObject
  {
    public static ClassWithoutProperties NewObject()
    {
      return DomainObject.NewObject<ClassWithoutProperties> ().With ();
    }

    protected ClassWithoutProperties()
    {
    }
  }
}
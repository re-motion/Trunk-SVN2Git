using System;
using Remotion.Data.DomainObjects.ObjectBinding;

namespace Remotion.Data.DomainObjects.Web.Test.Domain
{
  [Serializable]
  [DBTable ("TableWithUndefinedEnum")]
  [Instantiable]
  [DBStorageGroup]
  public abstract class ClassWithUndefinedEnum: BindableDomainObject
  {
    public static ClassWithUndefinedEnum NewObject ()
    {
      return DomainObject.NewObject<ClassWithUndefinedEnum> ().With ();
    }

    public static ClassWithUndefinedEnum GetObject (ObjectID id)
    {
      return DomainObject.GetObject<ClassWithUndefinedEnum> (id);
    }

    protected ClassWithUndefinedEnum()
    {
    }

    public abstract UndefinedEnum UndefinedEnum { get; set; }
  }
}
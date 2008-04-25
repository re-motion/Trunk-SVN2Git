using System;

namespace Remotion.Security.UnitTests.Core.SampleDomain
{
  public class DerivedSecurableObject : SecurableObject
  {
    //[RequiredMethodPermission (GeneralAccessTypes.Edit)]
    //public static new string GetObjectName (SecurableObject securableObject)
    //{
    //  return null;
    //}

    public DerivedSecurableObject ()
    {
    }

    public DerivedSecurableObject (IObjectSecurityStrategy objectSecurityStrategy)
      : base (objectSecurityStrategy)
    {
    }

    [DemandMethodPermission (GeneralAccessTypes.Read)]
    public new void Send ()
    {
    }

    [DemandMethodPermission (GeneralAccessTypes.Create)]
    public override void Print ()
    {
      base.Print ();
    }
  }
}

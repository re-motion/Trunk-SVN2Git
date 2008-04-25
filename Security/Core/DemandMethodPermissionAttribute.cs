using System;

namespace Remotion.Security
{
  [AttributeUsage (AttributeTargets.Method, AllowMultiple=false)]
  public class DemandMethodPermissionAttribute : BaseDemandPermissionAttribute
  {
    public DemandMethodPermissionAttribute (object accessType1)
        : base (new object[] { accessType1 })
    {
    }

    public DemandMethodPermissionAttribute (object accessType1, object accessType2)
        : base (new object[] { accessType1, accessType2 })
    {
    }

    public DemandMethodPermissionAttribute (object accessType1, object accessType2, object accessType3)
        : base (new object[] { accessType1, accessType2, accessType3 })
    {
    }

    public DemandMethodPermissionAttribute (object accessType1, object accessType2, object accessType3, object accessType4)
        : base (new object[] { accessType1, accessType2, accessType3, accessType4 })
    {
    }

    public DemandMethodPermissionAttribute (object accessType1, object accessType2, object accessType3, object accessType4, object accessType5)
        : base (new object[] { accessType1, accessType2, accessType3, accessType4, accessType5 })
    {
    }
  }
}

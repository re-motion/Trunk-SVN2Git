using System;

namespace Remotion.Security
{
  [AttributeUsage (AttributeTargets.Property, AllowMultiple=false)]
  public class DemandPropertyReadPermissionAttribute : BaseDemandPermissionAttribute
  {
    public DemandPropertyReadPermissionAttribute (object accessType1)
        : base (new object[] { accessType1 })
    {
    }

    public DemandPropertyReadPermissionAttribute (object accessType1, object accessType2)
        : base (new object[] { accessType1, accessType2 })
    {
    }

    public DemandPropertyReadPermissionAttribute (object accessType1, object accessType2, object accessType3)
        : base (new object[] { accessType1, accessType2, accessType3 })
    {
    }

    public DemandPropertyReadPermissionAttribute (object accessType1, object accessType2, object accessType3, object accessType4)
        : base (new object[] { accessType1, accessType2, accessType3, accessType4 })
    {
    }

    public DemandPropertyReadPermissionAttribute (object accessType1, object accessType2, object accessType3, object accessType4, object accessType5)
        : base (new object[] { accessType1, accessType2, accessType3, accessType4, accessType5 })
    {
    }
  }
}

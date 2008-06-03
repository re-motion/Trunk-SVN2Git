/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

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

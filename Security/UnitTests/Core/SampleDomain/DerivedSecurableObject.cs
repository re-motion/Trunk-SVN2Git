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

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
using Remotion.Security;

namespace Remotion.Web.UnitTests.Security.Domain
{
  public class DerivedSecurableObject : SecurableObject
  {
    // types

    public new enum Method
    {
      Create,
      Delete,
      Show,
      Search
    }

    // static members

    // member fields

    // construction and disposing

    public DerivedSecurableObject (IObjectSecurityStrategy securityStrategy)
      : base (securityStrategy)
    {
    }

    // methods and properties

    [DemandMethodPermission (GeneralAccessTypes.Read)]
    public void ShowSpecial ()
    {
    }
  }
}

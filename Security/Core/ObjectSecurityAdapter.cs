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

using Remotion.Utilities;

namespace Remotion.Security
{
  public class ObjectSecurityAdapter : IObjectSecurityAdapter
  {
    // types

    // static members

    // member fields

    // construction and disposing

    public ObjectSecurityAdapter ()
    {
    }

    // methods and properties

    public bool HasAccessOnGetAccessor (ISecurableObject securableObject, string propertyName)
    {
      ArgumentUtility.CheckNotNull ("securableObject", securableObject);

      if (SecurityFreeSection.IsActive)
        return true;

      SecurityClient securityClient = SecurityClient.CreateSecurityClientFromConfiguration ();
      return securityClient.HasPropertyReadAccess (securableObject, propertyName);
    }

    public bool HasAccessOnSetAccessor (ISecurableObject securableObject, string propertyName)
    {
      ArgumentUtility.CheckNotNull ("securableObject", securableObject);

      if (SecurityFreeSection.IsActive)
        return true;

      SecurityClient securityClient = SecurityClient.CreateSecurityClientFromConfiguration ();
      return securityClient.HasPropertyWriteAccess (securableObject, propertyName);
    }
  }
}

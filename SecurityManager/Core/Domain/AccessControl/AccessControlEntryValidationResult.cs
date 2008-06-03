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

namespace Remotion.SecurityManager.Domain.AccessControl
{
  public class AccessControlEntryValidationResult
  {
    // types

    // static members

    // member fields

    private bool _isValid = true;
    private bool _isSpecificTenantMissing = false;

    // construction and disposing

    public AccessControlEntryValidationResult ()
    {
    }

    // methods and properties

    public bool IsValid
    {
      get { return _isValid; }
    }

    public bool IsSpecificTenantMissing
    {
      get { return _isSpecificTenantMissing; }
    }

    public void SetSpecificTenantMissing ()
    {
      _isValid = false;
      _isSpecificTenantMissing = true;
    }
  }
}

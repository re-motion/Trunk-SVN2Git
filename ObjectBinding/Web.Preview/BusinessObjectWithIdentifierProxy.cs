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

namespace Remotion.ObjectBinding.Web
{
  public sealed class BusinessObjectWithIdentityProxy
  {
    private readonly string _uniqueIdentifier;
    private readonly string _displayName;

    private BusinessObjectWithIdentityProxy ()
    {
    }

    public BusinessObjectWithIdentityProxy (IBusinessObjectWithIdentity obj)
    {
      ArgumentUtility.CheckNotNull ("obj", obj);

      _uniqueIdentifier = obj.UniqueIdentifier;
      _displayName = obj.DisplayNameSafe;
    }

    public BusinessObjectWithIdentityProxy (string uniqueIdentifier, string displayName)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("uniqueIdentifier", uniqueIdentifier);
      ArgumentUtility.CheckNotNullOrEmpty ("displayName", displayName);

      _uniqueIdentifier = uniqueIdentifier;
      _displayName = displayName;
    }

    public string UniqueIdentifier
    {
      get { return _uniqueIdentifier; }
    }

    public string DisplayName
    {
      get { return _displayName; }
    }
  }
}

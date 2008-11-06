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
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.Utilities;

namespace Remotion.SecurityManager.Domain.AccessControl
{
  public class AccessInformation
  {
    private readonly AccessTypeDefinition[] _allowedAccessTypes;
    private readonly AccessTypeDefinition[] _deniedAccessTypes;

    public AccessInformation (AccessTypeDefinition[] allowedAccessTypes, AccessTypeDefinition[] deniedAccessTypes)
    {
      ArgumentUtility.CheckNotNullOrItemsNull ("allowedAccessTypes", allowedAccessTypes);
      ArgumentUtility.CheckNotNullOrItemsNull ("deniedAccessTypes", deniedAccessTypes);

      _allowedAccessTypes = allowedAccessTypes;
      _deniedAccessTypes = deniedAccessTypes;
    }

    public AccessTypeDefinition[] AllowedAccessTypes
    {
      get { return _allowedAccessTypes; }
    }

    public AccessTypeDefinition[] DeniedAccessTypes
    {
      get { return _deniedAccessTypes; }
    }
  }
}
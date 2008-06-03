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
using System.Collections.Specialized;
using Remotion.Configuration;
using Remotion.Security.Metadata;

namespace Remotion.Security.UnitTests.Core.Configuration
{
  public class PermissionProviderMock : ExtendedProviderBase, IPermissionProvider
  {
    public PermissionProviderMock (string name, NameValueCollection config)
        : base (name, config)
    {
    }

    public Enum[] GetRequiredMethodPermissions (Type type, string methodName)
    {
      throw new NotImplementedException();
    }

    public Enum[] GetRequiredStaticMethodPermissions (Type type, string methodName)
    {
      throw new NotImplementedException();
    }

    public Enum[] GetRequiredPropertyReadPermissions (Type type, string methodName)
    {
      throw new NotImplementedException();
    }

    public Enum[] GetRequiredPropertyWritePermissions (Type type, string methodName)
    {
      throw new NotImplementedException();
    }
  }
}

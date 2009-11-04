// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
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

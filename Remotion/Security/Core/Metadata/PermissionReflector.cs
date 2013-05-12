// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using System.Collections.Generic;
using System.Collections.Specialized;
using Remotion.Collections;
using Remotion.Configuration;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Security.Metadata
{
  /// <summary>
  /// Implements the <see cref="IPermissionProvider"/> for a reflection-based security declaration.
  /// </summary>
  public class PermissionReflector : ExtendedProviderBase, IPermissionProvider
  {
    private struct CacheKey : IEquatable<CacheKey>
    {
      public readonly Type Type;
      public readonly IMethodInformation MethodInformation;

      public CacheKey (Type type, IMethodInformation methodInformation)
      {
        Assertion.DebugAssert (type != null, "Parameter 'type' is null.");
        Assertion.DebugAssert (methodInformation != null, "Parameter 'methodInformation' is null.");

        Type = type;
        MethodInformation = methodInformation;
      }

      public override int GetHashCode ()
      {
        return MethodInformation.GetHashCode();
      }

      public bool Equals (CacheKey other)
      {
        return Type == other.Type
               && MethodInformation.Equals (other.MethodInformation);
      }
    }

    private static readonly ICache<CacheKey, Enum[]> s_cache = CacheFactory.CreateWithLocking<CacheKey, Enum[]>();
    private readonly Func<CacheKey, Enum[]> _cacheValueFactory;

    public PermissionReflector ()
        : this ("Reflection", new NameValueCollection())
    {
    }

    public PermissionReflector (string name, NameValueCollection config)
        : base (name, config)
    {
      _cacheValueFactory = key => GetPermissions (key.MethodInformation);
    }

    public Enum[] GetRequiredMethodPermissions (Type type, IMethodInformation methodInformation)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      ArgumentUtility.CheckNotNull ("methodInformation", methodInformation);
      
      return GetPermissionsFromCache (type, methodInformation);
    }

    public Enum[] GetPermissions (IMethodInformation methodInformation)
    {
      var permissionAttribute = methodInformation.GetCustomAttribute<DemandPermissionAttribute>(true);

      if (permissionAttribute == null)
        return new Enum[0];

      var permissions = new List<Enum> ();
      foreach (Enum accessTypeEnum in permissionAttribute.GetAccessTypes ())
      {
        if (!permissions.Contains (accessTypeEnum))
          permissions.Add (accessTypeEnum);
      }

      return permissions.ToArray ();
    }

    private Enum[] GetPermissionsFromCache (Type type, IMethodInformation methodInformation)
    {
      var cacheKey = new CacheKey (type, methodInformation);
      return s_cache.GetOrCreateValue (cacheKey, _cacheValueFactory);
    }
  }
}
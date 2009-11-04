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
using Remotion.Collections;
using Remotion.Reflection.TypeDiscovery;
using Remotion.Utilities;

namespace Remotion.ExtensibleEnums.Infrastructure
{
  /// <summary>
  /// Caches <see cref="ExtensibleEnumDefinition{T}"/> instances for non-generic access.
  /// </summary>
  public class ExtensibleEnumDefinitionCache
  {
    public static readonly ExtensibleEnumDefinitionCache Instance = new ExtensibleEnumDefinitionCache();

    private readonly InterlockedCache<Type, IExtensibleEnumDefinition> _cache = new InterlockedCache<Type, IExtensibleEnumDefinition>();
    private readonly IExtensibleEnumValueDiscoveryService _valueDiscoveryService;

    private ExtensibleEnumDefinitionCache ()
    {
      _valueDiscoveryService = new ExtensibleEnumValueDiscoveryService (ContextAwareTypeDiscoveryUtility.GetTypeDiscoveryService ());
    }

    public IExtensibleEnumValueDiscoveryService ValueDiscoveryService
    {
      get { return _valueDiscoveryService; }
    }

    public IExtensibleEnumDefinition GetDefinition (Type extensibleEnumType)
    {
      ArgumentUtility.CheckNotNull ("extensibleEnumType", extensibleEnumType);

      return _cache.GetOrCreateValue (extensibleEnumType, CreateDefinition);
    }

    private IExtensibleEnumDefinition CreateDefinition (Type extensibleEnumType)
    {
      Type definitionType;
      try
      {
        definitionType = typeof (ExtensibleEnumDefinition<>).MakeGenericType (extensibleEnumType);
      }
      catch (ArgumentException ex) // constraint violation
      {
        var message = string.Format ("Type '{0}' is not an extensible enum type directly derived from ExtensibleEnum<T>.", extensibleEnumType);
        throw new ArgumentException (message, "extensibleEnumType", ex);
      }
      return (IExtensibleEnumDefinition) Activator.CreateInstance (definitionType, new[] { ValueDiscoveryService });
    }
  }
}

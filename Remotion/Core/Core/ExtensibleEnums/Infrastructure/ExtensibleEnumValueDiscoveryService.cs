// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
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
using System.ComponentModel.Design;
using System.Linq;

namespace Remotion.ExtensibleEnums.Infrastructure
{
  /// <summary>
  /// Implements <see cref="IExtensibleEnumValueDiscoveryService"/> by discovering and invoking extension methods defining extensible enum values
  /// via reflection and <see cref="ITypeDiscoveryService"/>.
  /// </summary>
  public class ExtensibleEnumValueDiscoveryService : IExtensibleEnumValueDiscoveryService
  {
    private readonly ITypeDiscoveryService _typeDiscoveryService;
    private readonly ExtensibleEnumValueDiscoveryServiceImplementation _implementation = new ExtensibleEnumValueDiscoveryServiceImplementation ();

    public ExtensibleEnumValueDiscoveryService (ITypeDiscoveryService typeDiscoveryService)
    {
      _typeDiscoveryService = typeDiscoveryService;
    }

    public ITypeDiscoveryService TypeDiscoveryService
    {
      get { return _typeDiscoveryService; }
    }

    public IEnumerable<T> GetValues<T> (ExtensibleEnumDefinition<T> definition) where T: ExtensibleEnum<T>
    {
      var types = _typeDiscoveryService.GetTypes (null, false).Cast<Type>();
      return _implementation.GetValues (definition, types);
    }
  }
}
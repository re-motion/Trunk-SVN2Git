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
using Remotion.BridgeInterfaces;
using Remotion.Implementation;

namespace Remotion
{
  /// <summary>Used to register <see cref="IAdapter"/> instances.</summary>
  /// <remarks>Used by those modules of the framework that do not have binary depedencies to another module to access information from this module.</remarks>
  /// <seealso cref="T:Remotion.Security.ISecurityAdapter"/>
  public static class AdapterRegistry
  {
    public static IAdapterRegistryImplementation Instance
    {
      get { return VersionDependentImplementationBridge<IAdapterRegistryImplementation>.Implementation; }
    }
  }
}

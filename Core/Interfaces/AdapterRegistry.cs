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
using Remotion.Implementation;
using Remotion.BridgeInterfaces;

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

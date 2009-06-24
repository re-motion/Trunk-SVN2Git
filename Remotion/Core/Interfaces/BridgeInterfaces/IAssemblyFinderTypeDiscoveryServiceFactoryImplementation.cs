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
using System.ComponentModel.Design;
using Remotion.Implementation;

namespace Remotion.BridgeInterfaces
{
  /// <summary>
  /// This interface is used to separate the <see cref="T:Remotion.BridgeImplementations.AssemblyFinderTypeDiscoveryServiceFactoryImplementation"/>
  /// from it's instantiation in the <see cref="VersionDependentImplementationBridge{T}"/>.
  /// </summary>
  [ConcreteImplementation ("Remotion.BridgeImplementations.AssemblyFinderTypeDiscoveryServiceFactoryImplementation, Remotion, Version=<version>, Culture=neutral, PublicKeyToken=<publicKeyToken>")]
  public interface IAssemblyFinderTypeDiscoveryServiceFactoryImplementation
  {
    ITypeDiscoveryService CreateTypeDiscoveryService ();
  }
}

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
using System.IO;
using Remotion.Mixins.CodeGeneration;
using Remotion.Utilities;

namespace Remotion.Mixins.MixerTool
{
  public class ConcreteTypeBuilderFactory
  {
    private readonly INameProvider _typeNameProvider;
    private readonly string _signedAssemblyName;
    private readonly string _unsignedAssemblyName;

    public ConcreteTypeBuilderFactory (
        INameProvider typeNameProvider, 
        string signedAssemblyName, 
        string unsignedAssemblyName)
    {
      ArgumentUtility.CheckNotNull ("typeNameProvider", typeNameProvider);
      ArgumentUtility.CheckNotNullOrEmpty ("signedAssemblyName", signedAssemblyName);
      ArgumentUtility.CheckNotNullOrEmpty ("unsignedAssemblyName", unsignedAssemblyName);

      _typeNameProvider = typeNameProvider;
      _signedAssemblyName = signedAssemblyName;
      _unsignedAssemblyName = unsignedAssemblyName;
    }

    public ConcreteTypeBuilder CreateTypeBuilder (string assemblyOutputDirectory)
    {
      var builder = new ConcreteTypeBuilder ();
      builder.TypeNameProvider = _typeNameProvider;

      builder.Scope.SignedAssemblyName = _signedAssemblyName;
      builder.Scope.SignedModulePath = GetSignedModulePath (assemblyOutputDirectory);

      builder.Scope.UnsignedAssemblyName = _unsignedAssemblyName;
      builder.Scope.UnsignedModulePath = GetUnsignedModulePath(assemblyOutputDirectory);

      return builder;
    }

    public string GetSignedModulePath (string assemblyOutputDirectory)
    {
      return Path.Combine (assemblyOutputDirectory, _signedAssemblyName + ".dll");
    }

    public string GetUnsignedModulePath (string assemblyOutputDirectory)
    {
      return Path.Combine (assemblyOutputDirectory, _unsignedAssemblyName + ".dll");
    }
  }
}
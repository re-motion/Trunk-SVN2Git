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
using System.IO;
using Remotion.Mixins.CodeGeneration;
using Remotion.Mixins.CodeGeneration.DynamicProxy;
using Remotion.Utilities;

namespace Remotion.Mixins.MixerTools
{
  public class ConcreteTypeBuilderFactory : IConcreteTypeBuilderFactory
  {
    private class ModuleManagerFactory : IModuleManagerFactory
    {
      private readonly ConcreteTypeBuilderFactory _concreteTypeBuilderFactory;
      private readonly string _assemblyOutputDirectory;

      public ModuleManagerFactory (ConcreteTypeBuilderFactory concreteTypeBuilderFactory, string assemblyOutputDirectory)
      {
        ArgumentUtility.CheckNotNull ("concreteTypeBuilderFactory", concreteTypeBuilderFactory);
        ArgumentUtility.CheckNotNullOrEmpty ("assemblyOutputDirectory", assemblyOutputDirectory)
          ;
        _concreteTypeBuilderFactory = concreteTypeBuilderFactory;
        _assemblyOutputDirectory = assemblyOutputDirectory;
      }

      public IModuleManager Create ()
      {
        return new ModuleManager
               {
                   SignedAssemblyName = _concreteTypeBuilderFactory.SignedAssemblyName,
                   SignedModulePath = _concreteTypeBuilderFactory.GetSignedModulePath (_assemblyOutputDirectory),
                   UnsignedAssemblyName = _concreteTypeBuilderFactory.UnsignedAssemblyName,
                   UnsignedModulePath = _concreteTypeBuilderFactory.GetUnsignedModulePath (_assemblyOutputDirectory)
               };
      }
    }

    private readonly IConcreteMixedTypeNameProvider _typeNameProvider;
    private readonly string _signedAssemblyName;
    private readonly string _unsignedAssemblyName;

    public ConcreteTypeBuilderFactory (
        IConcreteMixedTypeNameProvider typeNameProvider, 
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

    public IConcreteMixedTypeNameProvider TypeNameProvider
    {
      get { return _typeNameProvider; }
    }

    public string SignedAssemblyName
    {
      get { return _signedAssemblyName; }
    }

    public string UnsignedAssemblyName
    {
      get { return _unsignedAssemblyName; }
    }

    public IConcreteTypeBuilder CreateTypeBuilder (string assemblyOutputDirectory)
    {
      var moduleManagerFactory = new ModuleManagerFactory (this, assemblyOutputDirectory);
      return new ConcreteTypeBuilder (moduleManagerFactory, _typeNameProvider, new GuidNameProvider());
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

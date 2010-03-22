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
using System.IO;
using System.Reflection;

namespace Remotion.Utilities
{
  /// <summary>
  /// Helper class for resolving assemblies when executing code in a separate <see cref="AppDomain"/>. Register the <see cref="ResolveAssembly"/>
  /// method as a handler for the <see cref="AppDomain.AssemblyResolve"/> event of the <see cref="AppDomain"/>.
  /// </summary>
  [Serializable]
  public sealed class AppDomainAssemblyResolver
  {
    private readonly string _parentApplicationBase;
    private readonly string _dynamicDirectory;

    public AppDomainAssemblyResolver (string parentApplicationBase, string dynamicDirectory)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("parentApplicationBase", parentApplicationBase);
      ArgumentUtility.CheckNotNullOrEmpty ("dynamicDirectory", dynamicDirectory);
      
      _parentApplicationBase = parentApplicationBase;

      _dynamicDirectory = dynamicDirectory;

      if (Directory.Exists (_dynamicDirectory))
        Directory.Delete (_dynamicDirectory, true);
      Directory.CreateDirectory (_dynamicDirectory);
      CopyAssembly (_dynamicDirectory, typeof (AppDomainAssemblyResolver).Assembly.Location);
    }

    public Assembly ResolveAssembly (object sender, ResolveEventArgs args)
    {
      var assemblyName = new AssemblyName (args.Name);
      string localAssemblyLocation = CopyAssemblyToDynamicDirectory (args, assemblyName.Name + ".dll");
      if (localAssemblyLocation == null)
        localAssemblyLocation = CopyAssemblyToDynamicDirectory (args, assemblyName.Name + ".exe");
      if (localAssemblyLocation == null)
        throw CreateFileNotFoundException (args.Name);

      return Assembly.LoadFrom (localAssemblyLocation);
      //throw CreateFileNotFoundException (args.Name);
    }

    private string CopyAssemblyToDynamicDirectory (ResolveEventArgs args, string assemblyFileName)
    {
      string assemblyLocation = Path.Combine (_parentApplicationBase, assemblyFileName);
      if (!File.Exists (assemblyLocation))
        return null;
      
      AssemblyName assemblyName = AssemblyName.GetAssemblyName (assemblyLocation);
      if (assemblyName.FullName != args.Name)
        throw CreateFileLoadException (args.Name);

      return CopyAssembly (_dynamicDirectory, assemblyLocation);
    }

    private string CopyAssembly (string dynamicDirectory, string assemblyLocation)
    {
      string destinationFileName = Path.Combine (dynamicDirectory, Path.GetFileName (assemblyLocation));
      if (File.Exists (destinationFileName))
        File.Delete (destinationFileName);
      File.Copy (assemblyLocation, destinationFileName);
      File.SetAttributes (destinationFileName, FileAttributes.Normal);

      return destinationFileName;
    }

    private FileLoadException CreateFileLoadException (string assemblyName)
    {
      return new FileLoadException (
          string.Format (
              "Could not load file or assembly '{0}'. The located assembly's manifest definition does not match the assembly reference.", 
              assemblyName));
    }

    private FileNotFoundException CreateFileNotFoundException (string assemblyName)
    {
      return new FileNotFoundException (
          string.Format ("Could not load file or assembly '{0}'. The system cannot find the file specified.", assemblyName));
    }
  }
}

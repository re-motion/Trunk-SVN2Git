// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using Castle.DynamicProxy;
using Remotion.Utilities;
using Remotion.Reflection.CodeGeneration.DPExtensions;
using System.IO;

namespace Remotion.Data.DomainObjects.Infrastructure.Interception
{
  public class ModuleManager
  {
    public const string StrongAssemblyName = "Remotion.Data.DomainObjects.Generated.Signed";
    public const string WeakAssemblyName = "Remotion.Data.DomainObjects.Generated.Unsigned";

    private readonly string _directory;
    private ModuleScope _scope;

    public ModuleManager (string directory)
    {
      ArgumentUtility.CheckNotNull ("directory", directory);

      _directory = directory;
      _scope = CreateModuleScope();
    }

    private ModuleScope CreateModuleScope ()
    {
      return new ModuleScope (true, StrongAssemblyName, Path.Combine (_directory, StrongAssemblyName + ".dll"),
        WeakAssemblyName, Path.Combine (_directory, WeakAssemblyName + ".dll"));
    }

    public TypeGenerator CreateTypeGenerator (Type publicDomainObjectType, Type typeToDeriveFrom)
    {
      ArgumentUtility.CheckNotNull ("publicDomainObjectType", publicDomainObjectType);
      ArgumentUtility.CheckNotNull ("typeToDeriveFrom", typeToDeriveFrom);
      return new TypeGenerator (publicDomainObjectType, typeToDeriveFrom, _scope);
    }

    public string[] SaveAssemblies ()
    {
      string[] paths = AssemblySaver.SaveAssemblies (_scope);
      _scope = CreateModuleScope ();
      return paths;
    }
  }
}

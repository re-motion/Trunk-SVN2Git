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

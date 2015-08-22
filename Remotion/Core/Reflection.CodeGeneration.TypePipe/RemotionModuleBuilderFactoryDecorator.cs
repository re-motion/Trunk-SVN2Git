﻿// This file is part of the re-motion Core Framework (www.re-motion.org)
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
using System.Reflection;
using Remotion.Reflection.TypeDiscovery;
using Remotion.TypePipe.CodeGeneration.ReflectionEmit;
using Remotion.TypePipe.CodeGeneration.ReflectionEmit.Abstractions;
using Remotion.TypePipe.MutableReflection;
using Remotion.Utilities;

namespace Remotion.Reflection.CodeGeneration.TypePipe
{
  /// <summary>
  /// Decorates an instance of <see cref="IModuleBuilderFactory"/> and adds the <see cref="NonApplicationAssemblyAttribute"/> to the
  /// <see cref="IAssemblyBuilder"/> whenever a <see cref="IModuleBuilder"/> is created.
  /// </summary>
  /// <threadsafety static="true" instance="true"/>
  public class RemotionModuleBuilderFactoryDecorator : IModuleBuilderFactory
  {
    private static readonly ConstructorInfo s_nonApplicationAssemblyAttributeConstructor =
        MemberInfoFromExpressionUtility.GetConstructor (() => new NonApplicationAssemblyAttribute());

    private readonly IModuleBuilderFactory _moduleBuilderFactory;

    [CLSCompliant (false)]
    public RemotionModuleBuilderFactoryDecorator (IModuleBuilderFactory moduleBuilderFactory)
    {
      ArgumentUtility.CheckNotNull ("moduleBuilderFactory", moduleBuilderFactory);

      _moduleBuilderFactory = moduleBuilderFactory;
    }

    [CLSCompliant (false)]
    public IModuleBuilder CreateModuleBuilder (string assemblyName, string assemblyDirectoryOrNull, bool strongNamed, string keyFilePathOrNull)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("assemblyName", assemblyName);

      var moduleBuilder = _moduleBuilderFactory.CreateModuleBuilder (assemblyName, assemblyDirectoryOrNull, strongNamed, keyFilePathOrNull);

      var attribute = new CustomAttributeDeclaration (s_nonApplicationAssemblyAttributeConstructor, new object[0]);
      moduleBuilder.AssemblyBuilder.SetCustomAttribute (attribute);

      return moduleBuilder;
    }
  }
}
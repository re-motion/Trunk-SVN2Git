// Copyright (c) rubicon IT GmbH, www.rubicon.eu
//
// See the NOTICE file distributed with this work for additional information
// regarding copyright ownership.  rubicon licenses this file to you under 
// the Apache License, Version 2.0 (the "License"); you may not use this 
// file except in compliance with the License.  You may obtain a copy of the 
// License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software 
// distributed under the License is distributed on an "AS IS" BASIS, WITHOUT 
// WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.  See the 
// License for the specific language governing permissions and limitations
// under the License.
// 
using System;
using System.Reflection;
using System.Reflection.Emit;
using Remotion.Utilities;

namespace Remotion.TypePipe.CodeGeneration.ReflectionEmit.Abstractions
{
  /// <summary>
  /// Adapts <see cref="ModuleBuilder"/> with the <see cref="IModuleBuilder"/> interface.
  /// </summary>
  public class ModuleBuilderAdapter : IModuleBuilder
  {
    private readonly ModuleBuilder _moduleBuilder;

    public string ScopeName
    {
      get { return _moduleBuilder.ScopeName; }
    }

    public ModuleBuilderAdapter (ModuleBuilder moduleBuilder)
    {
      ArgumentUtility.CheckNotNull ("moduleBuilder", moduleBuilder);

      _moduleBuilder = moduleBuilder;
    }

    [CLSCompliant (false)]
    public ITypeBuilder DefineType (string name, TypeAttributes attr, Type parent)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("name", name);
      ArgumentUtility.CheckNotNull ("parent", parent);

      var typeBuilder = _moduleBuilder.DefineType (name, attr, parent);
      return new TypeBuilderAdapter (typeBuilder);
    }

    public string SaveToDisk ()
    {
      throw new NotImplementedException();
    }
  }
}
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
using System.Reflection.Emit;
using System.Runtime.Serialization;
using Remotion.Mixins.Definitions;

namespace Remotion.Mixins.CodeGeneration
{
  public interface IModuleManager : ICodeGenerationModule
  {
    ITypeGenerator CreateTypeGenerator (
        CodeGenerationCache codeGenerationCache, 
        TargetClassDefinition configuration, 
        IConcreteMixedTypeNameProvider nameProvider, 
        IConcreteMixinTypeNameProvider mixinNameProvider);
    IMixinTypeGenerator CreateMixinTypeGenerator (MixinDefinition mixinDefinition, IConcreteMixinTypeNameProvider mixinNameProvider);

    string SignedAssemblyName { get; set; }
    string UnsignedAssemblyName { get; set; }

    string SignedModulePath { get; set; }
    string UnsignedModulePath { get; set; }

    ModuleBuilder SignedModule { get; }
    ModuleBuilder UnsignedModule { get; }

    bool HasAssemblies { get; }
    bool HasSignedAssembly { get; }
    bool HasUnsignedAssembly { get; }

    string[] SaveAssemblies ();

    void InitializeMixinTarget (IMixinTarget target);
    void InitializeDeserializedMixinTarget (IMixinTarget instance, object[] mixinInstances);

    IObjectReference BeginDeserialization (Func<Type, Type> typeTransformer, SerializationInfo info, StreamingContext context);
    void FinishDeserialization (IObjectReference objectReference);
  }
}

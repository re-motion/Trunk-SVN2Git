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
using System.Runtime.Serialization;
using Remotion.Mixins.CodeGeneration.DynamicProxy;
using Remotion.Mixins.Definitions;
using Remotion.ServiceLocation;

namespace Remotion.Mixins.CodeGeneration
{
  [ConcreteImplementation (typeof (ModuleManager))]
  public interface IModuleManager : ICodeGenerationModule
  {
    ITypeGenerator CreateTypeGenerator (TargetClassDefinition configuration, IConcreteMixedTypeNameProvider nameProvider, IConcreteMixinTypeProvider concreteMixinTypeProvider);
    IMixinTypeGenerator CreateMixinTypeGenerator (ConcreteMixinTypeIdentifier concreteMixinTypeIdentifier, IConcreteMixinTypeNameProvider mixinNameProvider);

    string SignedAssemblyName { get; set; }
    string UnsignedAssemblyName { get; set; }

    string SignedModulePath { get; set; }
    string UnsignedModulePath { get; set; }

    bool HasAssemblies { get; }
    bool HasSignedAssembly { get; }
    bool HasUnsignedAssembly { get; }

    string[] SaveAssemblies ();
    void Reset ();

    // Must be implemented in a thread-safe way
    void InitializeMixinTarget (IMixinTarget target);
    // Must be implemented in a thread-safe way
    void InitializeDeserializedMixinTarget (IMixinTarget instance, object[] mixinInstances);

    // Must be implemented in a thread-safe way
    IObjectReference BeginDeserialization (Func<Type, Type> typeTransformer, SerializationInfo info, StreamingContext context);
    // Must be implemented in a thread-safe way
    void FinishDeserialization (IObjectReference objectReference);
  }
}

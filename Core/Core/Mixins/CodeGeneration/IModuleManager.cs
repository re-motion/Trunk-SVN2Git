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
using System.Runtime.Serialization;
using Remotion.Mixins.Definitions;

namespace Remotion.Mixins.CodeGeneration
{
  public interface IModuleManager
  {
    ITypeGenerator CreateTypeGenerator (TargetClassDefinition configuration, INameProvider nameProvider, INameProvider mixinNameProvider);

    string SignedAssemblyName { get; set; }
    string UnsignedAssemblyName { get; set; }

    string SignedModulePath { get; set; }
    string UnsignedModulePath { get; set; }

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

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
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

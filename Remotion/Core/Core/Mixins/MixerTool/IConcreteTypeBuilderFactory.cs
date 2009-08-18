using System;
using Remotion.Mixins.CodeGeneration;

namespace Remotion.Mixins.MixerTool
{
  public interface IConcreteTypeBuilderFactory
  {
    IConcreteTypeBuilder CreateTypeBuilder (string assemblyOutputDirectory);
    string GetSignedModulePath (string assemblyOutputDirectory);
    string GetUnsignedModulePath (string assemblyOutputDirectory);
  }
}
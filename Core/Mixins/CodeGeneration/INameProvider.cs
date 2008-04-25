using Remotion.Mixins.Definitions;

namespace Remotion.Mixins.CodeGeneration
{
  public interface INameProvider
  {
    string GetNewTypeName (ClassDefinitionBase configuration);
  }
}
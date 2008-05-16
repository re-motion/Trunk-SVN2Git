using System.Reflection.Emit;
using Castle.DynamicProxy.Generators.Emitters.CodeBuilders;

namespace Remotion.UnitTests.Mixins.CodeGeneration
{
  public class DynamicMethodCodeBuilder : AbstractCodeBuilder
  {
    public DynamicMethodCodeBuilder (ILGenerator generator)
        : base (generator)
    {
    }
  }
}
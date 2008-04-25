using System.Reflection.Emit;
using Castle.DynamicProxy.Generators.Emitters;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;

namespace Remotion.Reflection.CodeGeneration.DPExtensions
{
  public class PopStatement : Statement
  {
    public override void Emit (IMemberEmitter member, ILGenerator gen)
    {
      gen.Emit (OpCodes.Pop);
    }
  }
}
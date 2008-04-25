using System.Reflection;
using System.Reflection.Emit;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;

namespace Remotion.Mixins.Samples.DynamicMixinBuilding
{
  internal class LoadFunctionExpression : Expression
  {
    private readonly MethodInfo _function;

    public LoadFunctionExpression (MethodInfo function)
    {
      _function = function;
    }

    public override void Emit (Castle.DynamicProxy.Generators.Emitters.IMemberEmitter member, ILGenerator gen)
    {
      gen.Emit (OpCodes.Ldftn, _function);
    }
  }
}
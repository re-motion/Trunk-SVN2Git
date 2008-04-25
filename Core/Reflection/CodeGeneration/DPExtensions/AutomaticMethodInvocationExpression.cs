using System;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using Castle.DynamicProxy.Generators.Emitters;
using System.Reflection.Emit;
using System.Reflection;

namespace Remotion.Reflection.CodeGeneration.DPExtensions
{
  public class AutomaticMethodInvocationExpression : TypedMethodInvocationExpression
  {
    private readonly TypeReference _owner;

    public AutomaticMethodInvocationExpression (TypeReference owner, MethodInfo method, params Expression[] arguments)
        : base (owner, method, arguments)
    {
      _owner = owner;
    }

    protected override void EmitCall (IMemberEmitter member, ILGenerator gen)
    {
      if (_owner.Type.IsValueType || Method.IsStatic)
        gen.Emit (OpCodes.Call, Method);
      else
        gen.Emit (OpCodes.Callvirt, Method);
    }
  }
}

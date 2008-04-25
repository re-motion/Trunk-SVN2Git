using System;
using System.Collections.Generic;
using System.Text;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using Castle.DynamicProxy.Generators.Emitters;
using System.Reflection.Emit;
using System.Reflection;

namespace Remotion.Reflection.CodeGeneration.DPExtensions
{
  public class VirtualMethodInvocationExpression : TypedMethodInvocationExpression
  {
    public VirtualMethodInvocationExpression (TypeReference owner, MethodInfo method, params Expression[] arguments)
        : base (owner, method, arguments)
    {
    }

    protected override void EmitCall (IMemberEmitter member, ILGenerator gen)
    {
      gen.Emit (OpCodes.Callvirt, Method);
    }
  }
}

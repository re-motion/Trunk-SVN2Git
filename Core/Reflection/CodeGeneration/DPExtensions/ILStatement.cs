using System;
using System.Reflection.Emit;
using Castle.DynamicProxy.Generators.Emitters;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;

namespace Remotion.Reflection.CodeGeneration.DPExtensions
{
  public class ILStatement : Statement
  {
    private readonly Proc<IMemberEmitter, ILGenerator> _ilSource;

    public ILStatement (Proc<IMemberEmitter, ILGenerator> ilSource)
    {
      _ilSource = ilSource;
    }

    public override void Emit (IMemberEmitter member, ILGenerator gen)
    {
      _ilSource (member, gen);
    }
  }
}
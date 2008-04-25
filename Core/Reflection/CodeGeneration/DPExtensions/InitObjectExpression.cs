using System;
using System.Reflection.Emit;
using Castle.DynamicProxy.Generators.Emitters;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using Remotion.Utilities;

namespace Remotion.Reflection.CodeGeneration.DPExtensions
{
  public class InitObjectExpression : Expression
  {
    private readonly Reference _objectToBeInitialized;
    private readonly Type _type;

    public InitObjectExpression (Reference objectToBeInitialized, Type type)
    {
      ArgumentUtility.CheckNotNull ("objectToBeInitialized", objectToBeInitialized);
      ArgumentUtility.CheckNotNull ("type", type);

      _objectToBeInitialized = objectToBeInitialized;
      _type = type;
    }

    public InitObjectExpression (CustomMethodEmitter method, Type type)
        : this (ArgumentUtility.CheckNotNull ("method", method).DeclareLocal (type), type)
    {
    }

    public override void Emit (IMemberEmitter member, ILGenerator gen)
    {
      _objectToBeInitialized.LoadAddressOfReference (gen);
      gen.Emit (OpCodes.Initobj, _type);
      _objectToBeInitialized.LoadReference (gen);
    }
  }
}

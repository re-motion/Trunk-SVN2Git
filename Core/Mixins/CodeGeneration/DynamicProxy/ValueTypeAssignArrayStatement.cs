using System;
using System.Reflection.Emit;
using Castle.DynamicProxy.Generators.Emitters;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;

namespace Remotion.Mixins.CodeGeneration.DynamicProxy
{
  internal class ValueTypeAssignArrayStatement : Statement
  {
    private readonly Type _elementType;
    private readonly Reference _arrayReference;
    private readonly int _elementIndex;
    private readonly Expression _elementValue;

    public ValueTypeAssignArrayStatement (Type elementType, Reference arrayReference, int elementIndex, Expression elementValue)
    {
      _elementType = elementType;
      _arrayReference = arrayReference;
      _elementIndex = elementIndex;
      _elementValue = elementValue;
    }

    public override void Emit (Castle.DynamicProxy.Generators.Emitters.IMemberEmitter member, ILGenerator il)
    {
      ArgumentsUtil.EmitLoadOwnerAndReference (_arrayReference, il);
      il.Emit (OpCodes.Ldc_I4, _elementIndex);
      _elementValue.Emit (member, il);
      il.Emit (OpCodes.Stelem, _elementType);
    }
  }
}
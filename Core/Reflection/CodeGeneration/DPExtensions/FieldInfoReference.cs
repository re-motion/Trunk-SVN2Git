using System;
using System.Reflection;
using System.Reflection.Emit;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;

namespace Remotion.Reflection.CodeGeneration.DPExtensions
{
  public class FieldInfoReference : TypeReference
  {
    private readonly FieldInfo _field;

    public FieldInfoReference (Reference owner, FieldInfo field)
        : base (owner, field.FieldType)
    {
      _field = field;
    }

    public override void LoadAddressOfReference (ILGenerator gen)
    {
      if (IsStaticField)
        gen.Emit (OpCodes.Ldsflda, _field);
      else
        gen.Emit (OpCodes.Ldflda, _field);
    }

    public override void LoadReference (ILGenerator gen)
    {
      if (IsStaticField)
        gen.Emit (OpCodes.Ldsfld, _field);
      else
        gen.Emit (OpCodes.Ldfld, _field);
    }

    public override void StoreReference (ILGenerator gen)
    {
      if (IsStaticField)
        gen.Emit (OpCodes.Stsfld, _field);
      else
        gen.Emit (OpCodes.Stfld, _field);
    }

    private bool IsStaticField
    {
      get { return _field.IsStatic; }
    }
  }
}
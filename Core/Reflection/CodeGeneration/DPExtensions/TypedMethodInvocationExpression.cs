using System;
using System.Reflection;
using System.Reflection.Emit;
using Castle.DynamicProxy.Generators.Emitters;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using Remotion.Utilities;

namespace Remotion.Reflection.CodeGeneration.DPExtensions
{
  /// <summary>
  /// Replacement for <see cref="MethodInvocationExpression"/> with value type support.
  /// </summary>
  public class TypedMethodInvocationExpression : Expression
  {
    private readonly TypeReference _callTarget;
    private readonly MethodInfo _method;
    private readonly Expression[] _arguments;

    public TypedMethodInvocationExpression (TypeReference callTarget, MethodInfo method, params Expression[] arguments)
    {
      ArgumentUtility.CheckNotNull ("callTarget", callTarget);
      ArgumentUtility.CheckNotNull ("method", method);
      ArgumentUtility.CheckNotNull ("arguments", arguments);

      _callTarget = callTarget;
      _method = method;
      _arguments = arguments;
    }

    public MethodInfo Method
    {
      get { return _method; }
    }

    public override void Emit (IMemberEmitter member, ILGenerator gen)
    {
      LoadOwnersRecursively (_callTarget.OwnerReference, gen);

      if (_callTarget.Type.IsValueType)
        _callTarget.LoadAddressOfReference (gen);
      else
        _callTarget.LoadReference (gen);

      foreach (Expression argument in _arguments)
        argument.Emit (member, gen);

      EmitCall (member, gen);
    }

    private void LoadOwnersRecursively (Reference owner, ILGenerator gen)
    {
      if (owner != null)
      {
        LoadOwnersRecursively (owner.OwnerReference, gen);
        owner.LoadReference (gen);
      }
    }

    protected virtual void EmitCall (IMemberEmitter member, ILGenerator gen)
    {
      gen.Emit (OpCodes.Call, _method);
    }
  }
}
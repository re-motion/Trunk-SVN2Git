using System;
using System.Reflection;
using System.Reflection.Emit;
using Castle.DynamicProxy.Generators.Emitters;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using Remotion.Utilities;

namespace Remotion.Reflection.CodeGeneration.DPExtensions
{
  public class CustomAttributeExpression : Expression
  {
    private static readonly MethodInfo s_getCustomAttributesMethod =
        typeof (ICustomAttributeProvider).GetMethod ("GetCustomAttributes", new Type[] { typeof (Type), typeof (bool) }, null);

    private readonly TypeReference _attributeOwner;
    private readonly Type _attributeType;
    private readonly int _index;
    private readonly bool _inherited;
    private readonly Expression _getAttributeExpression;

    public CustomAttributeExpression (TypeReference attributeOwner, Type attributeType, int index, bool inherited)
    {
      ArgumentUtility.CheckNotNull ("attributeOwner", attributeOwner);
      ArgumentUtility.CheckNotNull ("attributeType", attributeType);

      ArgumentUtility.CheckTypeIsAssignableFrom ("attributeOwner", attributeOwner.Type, typeof (ICustomAttributeProvider));

      _attributeOwner = attributeOwner;
      _attributeType = attributeType;
      _index = index;
      _inherited = inherited;

      Expression getAttributesExpression = new ConvertExpression (
          _attributeType.MakeArrayType (),
          new VirtualMethodInvocationExpression (
              _attributeOwner,
              s_getCustomAttributesMethod,
              new TypeTokenExpression (_attributeType),
              new ConstReference (_inherited).ToExpression ()));
      _getAttributeExpression =
          new LoadCalculatedArrayElementExpression (getAttributesExpression, new ConstReference (_index).ToExpression (), _attributeType);
    }

    public override void Emit (IMemberEmitter member, ILGenerator gen)
    {
      _getAttributeExpression.Emit (member, gen);
    }
  }
}
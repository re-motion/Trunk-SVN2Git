using System;
using System.Reflection;
using System.Reflection.Emit;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;

namespace Remotion.Reflection.CodeGeneration
{
  public interface IMethodEmitter : IAttributableEmitter
  {
    MethodBuilder MethodBuilder { get; }
    ILGenerator ILGenerator { get; }
    string Name { get; }
    ArgumentReference[] ArgumentReferences { get; }
    Type ReturnType { get; }
    Type[] ParameterTypes { get; }
    Expression[] GetArgumentExpressions ();
    CustomMethodEmitter SetParameterTypes (params Type[] parameters);
    CustomMethodEmitter SetReturnType (Type returnType);
    CustomMethodEmitter CopyParametersAndReturnType (MethodInfo method);
    CustomMethodEmitter ImplementByReturning (Expression result);
    CustomMethodEmitter ImplementByReturningVoid ();
    CustomMethodEmitter ImplementByReturningDefault ();
    CustomMethodEmitter ImplementByDelegating (TypeReference implementer, MethodInfo methodToCall);
    CustomMethodEmitter ImplementByBaseCall (MethodInfo baseMethod);
    CustomMethodEmitter ImplementByThrowing (Type exceptionType, string message);
    CustomMethodEmitter AddStatement (Statement statement);
    LocalReference DeclareLocal (Type type);
  }
}
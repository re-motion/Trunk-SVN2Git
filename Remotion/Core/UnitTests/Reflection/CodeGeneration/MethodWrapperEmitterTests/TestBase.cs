using System;
using System.Reflection;
using Remotion.Reflection.CodeGeneration;

namespace Remotion.UnitTests.Reflection.CodeGeneration.MethodWrapperEmitterTests
{
  public class TestBase : MethodGenerationTestBase
  {
    protected IMethodEmitter GetWrapperMethodFromEmitter (MethodBase executingTestMethod, Type[] publicParameterTypes, Type publicReturnType, MethodInfo innerMethod)
    {
      var method = ClassEmitter.CreateMethod (executingTestMethod.Name, MethodAttributes.Public | MethodAttributes.Static)
          .SetParameterTypes (publicParameterTypes)
          .SetReturnType (publicReturnType);

      var emitter = new MethodWrapperEmitter ();
      emitter.EmitMethodBody (method.ILGenerator, innerMethod, publicReturnType, publicParameterTypes);
      return method;
    }
  }
}
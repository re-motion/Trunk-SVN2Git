using System;
using System.Reflection;
using System.Reflection.Emit;
using Castle.DynamicProxy.Generators.Emitters;

namespace Remotion.UnitTests.Mixins.CodeGeneration
{
  public class DynamicMethodEmitter : IMemberEmitter
  {
    private readonly DynamicMethod _dynamicMethod;

    public DynamicMethodEmitter (DynamicMethod dynamicMethod)
    {
      _dynamicMethod = dynamicMethod;
    }

    public void Generate ()
    {
      throw new NotImplementedException ();
    }

    public void EnsureValidCodeBlock ()
    {
      throw new NotImplementedException();
    }

    public MemberInfo Member
    {
      get { return _dynamicMethod; }
    }

    public Type ReturnType
    {
      get { return _dynamicMethod.ReturnType; }
    }
  }
}
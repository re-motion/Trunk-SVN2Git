using System;
using System.Reflection;
using NUnit.Framework;
using Remotion.Reflection.CodeGeneration;
using Remotion.Development.UnitTesting;

namespace Remotion.UnitTests.Reflection.CodeGeneration
{
  [TestFixture]
  public class SnippetGenerationBaseTest : CodeGenerationBaseTest
  {
    private CustomClassEmitter _classEmitter;
    private CustomMethodEmitter _methodEmitter;
    private Type _builtType;
    private object _builtInstance;

    public override void SetUp ()
    {
      base.SetUp ();
      _classEmitter = new CustomClassEmitter (Scope, this.GetType ().Name, typeof (object), Type.EmptyTypes, TypeAttributes.Public, true);
      _methodEmitter = null;
      _builtType = null;
      _builtInstance  = null;
    }

    public CustomClassEmitter ClassEmitter
    {
      get { return _classEmitter; }
    }

    public CustomMethodEmitter GetMethodEmitter (bool isStatic)
    {
      if (_methodEmitter == null)
      {
        MethodAttributes flags = MethodAttributes.Public;
        if (isStatic)
          flags |= MethodAttributes.Static;
        _methodEmitter = ClassEmitter.CreateMethod ("TestMethod", flags);
      }
      return _methodEmitter;
    }

    public Type GetBuiltType ()
    {
      if (_builtType == null)
        _builtType = ClassEmitter.BuildType ();
      return _builtType;
    }

    public object GetBuiltInstance ()
    {
      if (_builtInstance == null)
        _builtInstance = Activator.CreateInstance (GetBuiltType ());
      return _builtInstance;
    }

    public object InvokeMethod (params object[] args)
    {
      if (_methodEmitter == null)
        throw new InvalidOperationException ("No method created.");
      else
      {
        if (_methodEmitter.MethodBuilder.IsStatic)
          return PrivateInvoke.InvokePublicStaticMethod (GetBuiltType (), _methodEmitter.Name, args);
        else
          return PrivateInvoke.InvokePublicMethod (GetBuiltInstance (), _methodEmitter.Name, args);
      }
    }
  }
}
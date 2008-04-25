using System;
using System.IO;
using System.Reflection;
using Castle.DynamicProxy;
using NUnit.Framework;
using Remotion.Reflection.CodeGeneration;
using Remotion.Reflection.CodeGeneration.DPExtensions;

namespace Remotion.UnitTests.Reflection.CodeGeneration
{
  [TestFixture]
  public class AssemblySaverTest
  {
    [Test]
    public void SaveAssemblyNoGeneratedTypes ()
    {
      ModuleScope scope = new ModuleScope (true);
      string[] paths = AssemblySaver.SaveAssemblies (scope);
      Assert.AreEqual (0, paths.Length);
    }

    [Test]
    public void SaveAssemblySigned ()
    {
      ModuleScope scope = new ModuleScope (true);
      CustomClassEmitter emitter = new CustomClassEmitter (scope, "SignedType", typeof (object));
      emitter.BuildType ();
      string[] paths = AssemblySaver.SaveAssemblies (scope);
      Assert.AreEqual (1, paths.Length);
      Assert.AreEqual (Path.Combine (Environment.CurrentDirectory, scope.StrongNamedModuleName), paths[0]);
    }

    [Test]
    public void SaveAssemblyUnsigned ()
    {
      ModuleScope scope = new ModuleScope (true);
      CustomClassEmitter emitter = new CustomClassEmitter (scope, "UnsignedType", typeof (object), Type.EmptyTypes, TypeAttributes.Public, true);
      emitter.BuildType ();
      string[] paths = AssemblySaver.SaveAssemblies (scope);
      Assert.AreEqual (1, paths.Length);
      Assert.AreEqual (Path.Combine (Environment.CurrentDirectory, scope.WeakNamedModuleName), paths[0]);
    }
  }
}
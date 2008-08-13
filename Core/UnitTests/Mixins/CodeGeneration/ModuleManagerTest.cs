/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Development.UnitTesting;
using Remotion.Mixins;
using Remotion.Mixins.CodeGeneration;
using Remotion.Mixins.CodeGeneration.DynamicProxy;
using Remotion.Mixins.Definitions;
using Remotion.Mixins.Utilities;
using Remotion.Reflection;
using Remotion.UnitTests.Mixins.SampleTypes;

namespace Remotion.UnitTests.Mixins.CodeGeneration
{
  [TestFixture]
  public class ModuleManagerTest
  {
    private IModuleManager _moduleManager;
    private const string c_signedAssemblyFileName = "Remotion.Mixins.Generated.ModuleManagerTest.Signed.dll";
    private const string c_unsignedAssemblyFileName = "Remotion.Mixins.Generated.ModuleManagerTest.Unsigned.dll";

    [SetUp]
    public void SetUp ()
    {
      ConcreteTypeBuilder.SetCurrent (null);
      _moduleManager = ConcreteTypeBuilder.Current.Scope;
      _moduleManager.SignedAssemblyName = Path.GetFileNameWithoutExtension (c_signedAssemblyFileName);
      _moduleManager.UnsignedAssemblyName = Path.GetFileNameWithoutExtension (c_unsignedAssemblyFileName);
      _moduleManager.SignedModulePath = c_signedAssemblyFileName;
      _moduleManager.UnsignedModulePath = c_unsignedAssemblyFileName;
      DeleteSavedAssemblies();
    }

    [TearDown]
    public void TearDown ()
    {
      DeleteSavedAssemblies();
    }

    private void DeleteSavedAssemblies ()
    {
      if (File.Exists (c_signedAssemblyFileName))
        File.Delete (c_signedAssemblyFileName);
      if (File.Exists (c_unsignedAssemblyFileName))
        File.Delete (c_unsignedAssemblyFileName);
    }

    [Test]
    public void CreateTypeGenerator ()
    {
      TargetClassDefinition bt1 = TargetClassDefinitionUtility.GetActiveConfiguration (typeof (BaseType1));

      ITypeGenerator generator = _moduleManager.CreateTypeGenerator (bt1, GuidNameProvider.Instance, GuidNameProvider.Instance);
      Assert.IsNotNull (generator);
      Assert.IsTrue (bt1.Type.IsAssignableFrom (generator.GetBuiltType()));
    }

    [Test]
    public void HasAssemblyFromUnsignedType ()
    {
      Assert.IsFalse (_moduleManager.HasUnsignedAssembly);
      Assert.IsFalse (_moduleManager.HasSignedAssembly);
      Assert.IsFalse (_moduleManager.HasAssemblies);

      GetUnsignedConcreteType(); // type from unsigned assembly

      Assert.IsTrue (_moduleManager.HasUnsignedAssembly);
      Assert.IsFalse (_moduleManager.HasSignedAssembly);
      Assert.IsTrue (_moduleManager.HasAssemblies);
    }

    [Test]
    public void HasAssemblyFromSignedType ()
    {
      Assert.IsFalse (_moduleManager.HasUnsignedAssembly);
      Assert.IsFalse (_moduleManager.HasSignedAssembly);
      Assert.IsFalse (_moduleManager.HasAssemblies);

      GetSignedConcreteType(); // type from signed assembly

      Assert.IsFalse (_moduleManager.HasUnsignedAssembly);
      Assert.IsTrue (_moduleManager.HasSignedAssembly);
      Assert.IsTrue (_moduleManager.HasAssemblies);
    }

    [Test]
    public void SaveAssemblies ()
    {
      GetUnsignedConcreteType();
      GetSignedConcreteType();

      Assert.IsFalse (File.Exists (c_signedAssemblyFileName));
      Assert.IsFalse (File.Exists (c_unsignedAssemblyFileName));

      string[] paths = _moduleManager.SaveAssemblies();

      Assert.AreEqual (2, paths.Length);
      Assert.AreEqual (Path.Combine (Environment.CurrentDirectory, c_signedAssemblyFileName), paths[0]);
      Assert.AreEqual (Path.Combine (Environment.CurrentDirectory, c_unsignedAssemblyFileName), paths[1]);

      Assert.IsTrue (File.Exists (c_signedAssemblyFileName));
      Assert.IsTrue (File.Exists (c_unsignedAssemblyFileName));
    }

    [Test]
    public void SaveUnsignedAssemblyWithDifferentNameAndPath ()
    {
      _moduleManager.UnsignedAssemblyName = "Foo";
      string path = Path.GetTempFileName();
      _moduleManager.UnsignedModulePath = path;
      File.Delete (path);

      GetUnsignedConcreteType();

      Assert.IsFalse (File.Exists (path));
      string[] actualPaths = _moduleManager.SaveAssemblies();

      Assert.AreEqual (1, actualPaths.Length);
      Assert.AreEqual (Path.Combine (Environment.CurrentDirectory, path), actualPaths[0]);

      Assert.IsTrue (File.Exists (path));

      try
      {
        AssemblyName name = AssemblyName.GetAssemblyName (path);
        Assert.AreEqual ("Foo", name.Name);
      }
      finally
      {
        File.Delete (path);
      }
    }

    [Test]
    public void SaveSignedAssemblyWithDifferentNameAndPath ()
    {
      _moduleManager.SignedAssemblyName = "Bar";
      string path = Path.GetTempFileName();
      _moduleManager.SignedModulePath = path;
      File.Delete (path);

      GetSignedConcreteType();

      Assert.IsFalse (File.Exists (path));
      string[] actualPaths = _moduleManager.SaveAssemblies();

      Assert.AreEqual (1, actualPaths.Length);
      Assert.AreEqual (Path.Combine (Environment.CurrentDirectory, path), actualPaths[0]);

      Assert.IsTrue (File.Exists (path));

      try
      {
        AssemblyName name = AssemblyName.GetAssemblyName (path);
        Assert.AreEqual ("Bar", name.Name);
      }
      finally
      {
        File.Delete (path);
      }
    }

    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "The name can only be set before the first type is built.")]
    public void SettingSignedNameThrowsWhenTypeGenerated ()
    {
      GetUnsignedConcreteType();
      _moduleManager.SignedAssemblyName = "Foo";
    }

    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "The module path can only be set before the first type is built.")]
    public void SettingSignedPathThrowsWhenTypeGenerated ()
    {
      GetUnsignedConcreteType();
      _moduleManager.SignedModulePath = "Foo.dll";
    }

    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "The name can only be set before the first type is built.")]
    public void SettingUnsignedNameThrowsWhenTypeGenerated ()
    {
      GetUnsignedConcreteType();
      _moduleManager.UnsignedAssemblyName = "Foo";
    }

    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "The module path can only be set before the first type is built.")]
    public void SettingUnsignedPathThrowsWhenTypeGenerated ()
    {
      GetUnsignedConcreteType();
      _moduleManager.UnsignedModulePath = "Foo.dll";
    }

    [Test]
    public void SavedSignedAssemblyHasStrongName ()
    {
      GetSignedConcreteType();

      _moduleManager.SaveAssemblies();
      AssemblyName assemblyName = AssemblyName.GetAssemblyName (c_signedAssemblyFileName);

      Assert.IsTrue (ReflectionUtility.IsAssemblySigned (assemblyName));
    }

    [Test]
    public void SavedUnsignedAssemblyHasWeakName ()
    {
      GetUnsignedConcreteType();

      _moduleManager.SaveAssemblies();
      AssemblyName assemblyName = AssemblyName.GetAssemblyName (c_unsignedAssemblyFileName);

      Assert.IsFalse (ReflectionUtility.IsAssemblySigned (assemblyName));
    }

    [Test]
    public void SavedUnsignedAssemblyHasMixinAssemblyName ()
    {
      GetUnsignedConcreteType();

      _moduleManager.SaveAssemblies();
      AssemblyName assemblyName = AssemblyName.GetAssemblyName (c_unsignedAssemblyFileName);

      Assert.AreEqual (Path.GetFileNameWithoutExtension (c_unsignedAssemblyFileName), assemblyName.Name);
    }

    [Test]
    public void SavedSignedAssemblyHasMixinAssemblyName ()
    {
      GetSignedConcreteType();

      _moduleManager.SaveAssemblies();
      AssemblyName assemblyName = AssemblyName.GetAssemblyName (c_signedAssemblyFileName);

      Assert.AreEqual (Path.GetFileNameWithoutExtension (c_signedAssemblyFileName), assemblyName.Name);
    }

    [Test]
    public void SavedUnsignedAssemblyContainsGeneratedType ()
    {
      Type concreteType = GetUnsignedConcreteType();
      _moduleManager.SaveAssemblies();

      CheckForTypeInAssembly (concreteType.FullName, AssemblyName.GetAssemblyName (c_unsignedAssemblyFileName));
    }

    [Test]
    public void SavedSignedAssemblyContainsGeneratedType ()
    {
      Type concreteType = GetSignedConcreteType();
      _moduleManager.SaveAssemblies();

      CheckForTypeInAssembly (concreteType.FullName, AssemblyName.GetAssemblyName (c_signedAssemblyFileName));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "No types have been built, so no assembly has been generated.")]
    public void SaveThrowsWhenNoTypeCreated ()
    {
      _moduleManager.SaveAssemblies();
    }

    [Test]
    public void GeneratedAssemblies_NonApplicationAssemblyAttribute ()
    {
      Type t1 = GetUnsignedConcreteType();
      Type t2 = GetSignedConcreteType();

      Assert.IsTrue (t1.Assembly.IsDefined (typeof (NonApplicationAssemblyAttribute), false));
      Assert.IsTrue (t2.Assembly.IsDefined (typeof (NonApplicationAssemblyAttribute), false));
    }

    [Test]
    public void SavedAssemblies_NonApplicationAssemblyAttribute ()
    {
      GetUnsignedConcreteType();
      GetSignedConcreteType();

      string[] assemblyPaths = _moduleManager.SaveAssemblies();
      CheckForAttributeOnAssembly (typeof (NonApplicationAssemblyAttribute), AssemblyName.GetAssemblyName (assemblyPaths[0]));
      CheckForAttributeOnAssembly (typeof (NonApplicationAssemblyAttribute), AssemblyName.GetAssemblyName (assemblyPaths[1]));
    }

    [Test]
    public void NewScope_HasNewAssemblyNames ()
    {
      var scope1 = new ModuleManager ();
      var scope2 = new ModuleManager ();

      Assert.That (scope1.SignedAssemblyName, Is.Not.EqualTo (scope2.SignedAssemblyName));
      Assert.That (scope1.UnsignedAssemblyName, Is.Not.EqualTo (scope2.UnsignedAssemblyName));
    }

    [Test]
    public void ModuleNames_MatchAssemblyNames ()
    {
      var scope1 = new ModuleManager ();
      var scope2 = new ModuleManager ();

      Assert.That (Path.GetFileNameWithoutExtension (scope1.SignedModulePath), Is.EqualTo (scope1.SignedAssemblyName));
      Assert.That (Path.GetFileNameWithoutExtension (scope2.SignedModulePath), Is.EqualTo (scope2.SignedAssemblyName));
      Assert.That (Path.GetFileNameWithoutExtension (scope1.UnsignedModulePath), Is.EqualTo (scope1.UnsignedAssemblyName));
      Assert.That (Path.GetFileNameWithoutExtension (scope2.UnsignedModulePath), Is.EqualTo (scope2.UnsignedAssemblyName));
    }

    private Type GetUnsignedConcreteType ()
    {
      Type t = TypeFactory.GetConcreteType (typeof (BaseType1), GenerationPolicy.ForceGeneration);
      Assert.IsFalse (ReflectionUtility.IsAssemblySigned (t.Assembly));
      return t;
    }

    private Type GetSignedConcreteType ()
    {
      Type t = TypeFactory.GetConcreteType (typeof (List<int>), GenerationPolicy.ForceGeneration);
      Assert.IsTrue (ReflectionUtility.IsAssemblySigned (t.Assembly));
      return t;
    }

    private void CheckForTypeInAssembly (string typeName, AssemblyName assemblyName)
    {
      AppDomainRunner.Run (
          delegate (object[] args)
          {
            AssemblyName assemblyToLoad = (AssemblyName) args[0];
            string typeToFind = (string) args[1];

            Assembly loadedAssembly = Assembly.Load (assemblyToLoad);
            Assert.IsNotNull (loadedAssembly.GetType (typeToFind));
          },
          assemblyName,
          typeName);
    }

    private void CheckForAttributeOnAssembly (Type attributeType, AssemblyName assemblyName)
    {
      AppDomainRunner.Run (
          delegate (object[] args)
          {
            AssemblyName assemblyToLoad = (AssemblyName) args[0];
            Type attributeToFind = (Type) args[1];

            Assembly loadedAssembly = Assembly.Load (assemblyToLoad);
            Assert.IsTrue (loadedAssembly.IsDefined (attributeToFind, false));
          },
          assemblyName,
          attributeType);
    }
  }
}

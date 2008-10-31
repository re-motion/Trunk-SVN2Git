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
using System.IO;
using System.Reflection;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Collections;
using Remotion.Development.UnitTesting;
using Remotion.Mixins;
using Remotion.Mixins.CodeGeneration;
using Remotion.Mixins.CodeGeneration.DynamicProxy;
using Remotion.Mixins.Definitions;
using Remotion.Mixins.Utilities;
using Remotion.Reflection;
using Remotion.UnitTests.Mixins.SampleTypes;
using Remotion.Reflection.CodeGeneration;

namespace Remotion.UnitTests.Mixins.CodeGeneration
{
  [TestFixture]
  public class ModuleManagerTest : CodeGenerationBaseTest
  {
    private ModuleManager _emptyModuleManager;
    private const string c_signedAssemblyFileName = "Remotion.Mixins.Generated.ModuleManagerTest.Signed.dll";
    private const string c_unsignedAssemblyFileName = "Remotion.Mixins.Generated.ModuleManagerTest.Unsigned.dll";

    private ModuleManager _savedModuleManager;
    private ModuleManager _unsavedModuleManager;
    private string _signedSavedModulePath;
    private string _unsignedSavedModulePath;
    private string[] _savedModulePaths;
    private Type _signedSavedType;
    private Type _unsignedSavedType;

    [SetUp]
    public override void SetUp ()
    {
      _emptyModuleManager = new ModuleManager ();
    }

    [TearDown]
    public override void TearDown ()
    {
    }

    private void DeleteSavedAssemblies ()
    {
      if (File.Exists (c_signedAssemblyFileName))
        File.Delete (c_signedAssemblyFileName);
      if (File.Exists (c_unsignedAssemblyFileName))
        File.Delete (c_unsignedAssemblyFileName);
    }

    [TestFixtureSetUp]
    public void TestFixtureSetUp ()
    {
      DeleteSavedAssemblies ();

      _unsavedModuleManager = new ModuleManager();

      _savedModuleManager = new ModuleManager
      {
        SignedAssemblyName = Path.GetFileNameWithoutExtension (c_signedAssemblyFileName),
        UnsignedAssemblyName = Path.GetFileNameWithoutExtension (c_unsignedAssemblyFileName),
        SignedModulePath = c_signedAssemblyFileName,
        UnsignedModulePath = c_unsignedAssemblyFileName
      };
      var savedBuilder = new ConcreteTypeBuilder {Scope = _savedModuleManager};

      var signedConfiguration = TargetClassDefinitionUtility.GetActiveConfiguration (typeof (object), GenerationPolicy.ForceGeneration);
      var unsignedConfiguration = TargetClassDefinitionUtility.GetActiveConfiguration (typeof (BaseType1), GenerationPolicy.ForceGeneration);

      _signedSavedType = savedBuilder.GetConcreteType (signedConfiguration);
      _unsignedSavedType = savedBuilder.GetConcreteType (unsignedConfiguration);

      _savedModulePaths = _savedModuleManager.SaveAssemblies ();
      
      _signedSavedModulePath = _savedModulePaths[0];
      _unsignedSavedModulePath = _savedModulePaths[1];
    }

    [TestFixtureTearDown]
    public void TestFixtureTearDown()
    {
      DeleteSavedAssemblies ();
    }

    [Test]
    public void CreateTypeGenerator ()
    {
      TargetClassDefinition bt1 = TargetClassDefinitionUtility.GetActiveConfiguration (typeof (BaseType1));

      ITypeGenerator generator = SavedTypeBuilder.Scope.CreateTypeGenerator (ConcreteTypeBuilder.Current.Cache, bt1, GuidNameProvider.Instance, GuidNameProvider.Instance);
      Assert.IsNotNull (generator);
      Assert.IsTrue (bt1.Type.IsAssignableFrom (generator.GetBuiltType()));
    }

    [Test]
    public void HasAssemblies_False ()
    {
      Assert.That (_emptyModuleManager.HasUnsignedAssembly, Is.False);
      Assert.That (_emptyModuleManager.HasSignedAssembly, Is.False);
      Assert.That (_emptyModuleManager.HasAssemblies, Is.False);
    }

    [Test]
    public void HasAssemblies_True ()
    {
      Assert.That (_savedModuleManager.HasUnsignedAssembly, Is.True);
      Assert.That (_savedModuleManager.HasSignedAssembly, Is.True);
      Assert.That (_savedModuleManager.HasAssemblies, Is.True);
    }

    [Test]
    public void SaveAssemblies ()
    {
      Assert.AreEqual (2, _savedModulePaths.Length);
      Assert.AreEqual (Path.Combine (Environment.CurrentDirectory, c_signedAssemblyFileName), _savedModulePaths[0]);
      Assert.AreEqual (Path.Combine (Environment.CurrentDirectory, c_unsignedAssemblyFileName), _savedModulePaths[1]);

      Assert.IsTrue (File.Exists (_savedModulePaths[0]));
      Assert.IsTrue (File.Exists (_savedModulePaths[1]));
    }

    [Test]
    public void SavedAssemblyNameAndPath ()
    {
      AssemblyName signedName = AssemblyName.GetAssemblyName (_signedSavedModulePath);
      Assert.AreEqual (Path.GetFileNameWithoutExtension (c_signedAssemblyFileName), signedName.Name);

      AssemblyName unsignedName = AssemblyName.GetAssemblyName (_unsignedSavedModulePath);
      Assert.AreEqual (Path.GetFileNameWithoutExtension (c_unsignedAssemblyFileName), unsignedName.Name);
    }

    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "The name can only be set before the first type is built.")]
    public void SettingSignedNameThrowsWhenTypeGenerated ()
    {
      _savedModuleManager.SignedAssemblyName = "Foo";
    }

    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "The module path can only be set before the first type is built.")]
    public void SettingSignedPathThrowsWhenTypeGenerated ()
    {
      _savedModuleManager.SignedModulePath = "Foo.dll";
    }

    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "The name can only be set before the first type is built.")]
    public void SettingUnsignedNameThrowsWhenTypeGenerated ()
    {
      _savedModuleManager.UnsignedAssemblyName = "Foo";
    }

    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "The module path can only be set before the first type is built.")]
    public void SettingUnsignedPathThrowsWhenTypeGenerated ()
    {
      _savedModuleManager.UnsignedModulePath = "Foo.dll";
    }

    [Test]
    public void SavedSignedAssemblyHasStrongName ()
    {
      AssemblyName assemblyName = AssemblyName.GetAssemblyName (_signedSavedModulePath);
      Assert.IsTrue (ReflectionUtility.IsAssemblySigned (assemblyName));
    }

    [Test]
    public void SavedUnsignedAssemblyHasWeakName ()
    {
      AssemblyName assemblyName = AssemblyName.GetAssemblyName (_unsignedSavedModulePath);
      Assert.IsFalse (ReflectionUtility.IsAssemblySigned (assemblyName));
    }

    [Test]
    public void SavedAssembliesContainGeneratedTypes_AndHaveAssemblyAttributes ()
    {
      AppDomainRunner.Run (
          delegate (object[] args)
          {
            foreach (Tuple<string, string> assemblyAndTypeName in args)
            {
              Assembly loadedAssembly = Assembly.LoadFile (assemblyAndTypeName.A);
              Assert.IsNotNull (loadedAssembly.GetType (assemblyAndTypeName.B));
              Assert.That (loadedAssembly.IsDefined (typeof (NonApplicationAssemblyAttribute), false));
            }
          },
          Tuple.NewTuple (_signedSavedModulePath, _signedSavedType.FullName),
          Tuple.NewTuple (_unsignedSavedModulePath, _unsignedSavedType.FullName));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "No types have been built, so no assembly has been generated.")]
    public void SaveThrowsWhenNoTypeCreated ()
    {
      _emptyModuleManager.SaveAssemblies();
    }

    [Test]
    public void GeneratedAssemblies_NonApplicationAssemblyAttribute ()
    {
      Assert.IsTrue (_signedSavedType.Assembly.IsDefined (typeof (NonApplicationAssemblyAttribute), false));
      Assert.IsTrue (_unsignedSavedType.Assembly.IsDefined (typeof (NonApplicationAssemblyAttribute), false));
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

    [Test]
    public void ManualStrongName_CanUseCounterVariable ()
    {
      var scope1 = new ModuleManager {SignedAssemblyName = "abc{counter}", SignedModulePath = "xyz{counter}"};
      var scope2 = new ModuleManager {SignedAssemblyName = "abc{counter}", SignedModulePath = "xyz{counter}"};

      Assert.That (scope1.SignedAssemblyName, Is.Not.EqualTo ("abc{counter}"));
      Assert.That (scope1.SignedModulePath, Is.Not.EqualTo ("xyz{counter}"));
      Assert.That (scope2.SignedAssemblyName, Is.Not.EqualTo (scope1.SignedAssemblyName));
      Assert.That (scope2.SignedModulePath, Is.Not.EqualTo (scope1.SignedModulePath));
    }

    [Test]
    public void ManualWeakName_CanUseCounterVariable ()
    {
      var scope1 = new ModuleManager { UnsignedAssemblyName = "abc{counter}", UnsignedModulePath = "xyz{counter}" };
      var scope2 = new ModuleManager { UnsignedAssemblyName = "abc{counter}", UnsignedModulePath = "xyz{counter}" };

      Assert.That (scope1.UnsignedAssemblyName, Is.Not.EqualTo ("abc{counter}"));
      Assert.That (scope1.UnsignedModulePath, Is.Not.EqualTo ("xyz{counter}"));
      Assert.That (scope2.UnsignedAssemblyName, Is.Not.EqualTo (scope1.UnsignedAssemblyName));
      Assert.That (scope2.UnsignedModulePath, Is.Not.EqualTo (scope1.UnsignedModulePath));
    }

    [Test]
    public void SignedModule_Null ()
    {
      Assert.That (_emptyModuleManager.SignedModule, Is.Null);
    }

    [Test]
    public void UnsignedModule_Null ()
    {
      Assert.That (_emptyModuleManager.UnsignedModule, Is.Null);
    }

    [Test]
    public void SignedModule_NonNull ()
    {
      Assert.That (_savedModuleManager.SignedModule, Is.Not.Null);
      Assert.That (_savedModuleManager.SignedModule.FullyQualifiedName, Is.EqualTo (Path.GetFullPath (_savedModuleManager.SignedModulePath)));
    }

    [Test]
    public void UnsignedModule_NonNull ()
    {
      Assert.That (_savedModuleManager.UnsignedModule, Is.Not.Null);
      Assert.That (_savedModuleManager.UnsignedModule.FullyQualifiedName, Is.EqualTo (Path.GetFullPath (_savedModuleManager.UnsignedModulePath)));
    }

    [Test]
    public void CreatedAssemblyBuilders ()
    {
      Assert.That (ModuleManager.CreatedAssemblies.Contains (_signedSavedType.Assembly), Is.True);
      Assert.That (ModuleManager.CreatedAssemblies.Contains (_unsignedSavedType.Assembly), Is.True);
    }

    [Test]
    public void CreateClassEmitter ()
    {
      IClassEmitter emitter = _unsavedModuleManager.CreateClassEmitter ("X", typeof (BaseType2), new[] { typeof (IMarkerInterface) }, TypeAttributes.Public, true);
      Type type = emitter.BuildType();
      Assert.That (emitter, Is.InstanceOfType (typeof (CustomClassEmitter)));
      Assert.That (type.BaseType, Is.SameAs (typeof (BaseType2)));
      Assert.That (type.GetInterface (typeof (IMarkerInterface).FullName), Is.SameAs (typeof (IMarkerInterface)));
      Assert.That (type.Attributes, Is.EqualTo (TypeAttributes.Public));
      Assert.That (ReflectionUtility.IsAssemblySigned (type.Assembly), Is.False);
    }
  }
}

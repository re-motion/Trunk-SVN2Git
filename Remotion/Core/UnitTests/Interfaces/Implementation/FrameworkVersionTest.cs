// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;
using System.IO;
using System.Reflection;
using Castle.DynamicProxy;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Implementation;
using Remotion.Reflection.CodeGeneration;
using Remotion.Reflection.CodeGeneration.DPExtensions;
using Remotion.Utilities;

namespace Remotion.UnitTests.Interfaces.Implementation
{
  [TestFixture]
  public class FrameworkVersionTest
  {
    [SetUp]
    public void SetUp ()
    {
      FrameworkVersion.Reset();
    }

    [TearDown]
    public void TearDown ()
    {
      FrameworkVersion.Reset ();
    }

    [Test]
    public void SetAndAccessValue ()
    {
      FrameworkVersion.Value = new Version (1, 2, 3, 4);
      Assert.That (FrameworkVersion.Value, Is.EqualTo (new Version (1, 2, 3, 4)));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "The framework version has already been set to 1.2.3.4. It can "
        + "only be set once.")]
    public void SetValue_Twice ()
    {
      FrameworkVersion.Value = new Version (1, 2, 3, 4);
      FrameworkVersion.Value = new Version (1, 2, 3, 5);
    }

    [Test]
    public void SetValue_Twice_SameVersion ()
    {
      FrameworkVersion.Value = new Version (1, 2, 3, 4);
      FrameworkVersion.Value = new Version (1, 2, 3, 4);
      Assert.That (FrameworkVersion.Value, Is.EqualTo (new Version (1, 2, 3, 4)));
    }

    [Test]
    [ExpectedException (typeof (ArgumentNullException))]
    public void SetValue_Null ()
    {
      FrameworkVersion.Value = null;
    }

    [Test]
    public void AutomaticVersionDiscovery ()
    {
      Assert.AreEqual (typeof (INullObject).Assembly.GetName().Version, FrameworkVersion.Value);
    }

    [Test]
    [ExpectedException (typeof (FrameworkVersionNotFoundException), ExpectedMessage = "Remotion is neither loaded nor referenced, and trying to load "
        + "it by name ('Remotion') didn't work either. Add a reference to the framework implementation assemblies or manually set "
        + "Remotion.Implementation.FrameworkVersion.Value to specify what version should be used.")]
    public void FailedAutomaticDiscovery ()
    {
      var scope = new ModuleScope (true, "VersionAccessorAssembly", "VersionAccessorAssembly.dll", "x", "x");
      var versionAccessorTypeBuilder = new CustomClassEmitter (scope, "VersionAccessor", typeof (object));
      var versionAccessorMethod = versionAccessorTypeBuilder.CreateMethod ("AccessVersion", MethodAttributes.Public | MethodAttributes.Static);
      versionAccessorMethod.AddStatement (
          new ExpressionStatement (new MethodInvocationExpression (null, typeof (FrameworkVersion).GetProperty ("Value").GetGetMethod())));
      versionAccessorMethod.AddStatement (new PopStatement());
      versionAccessorMethod.ImplementByReturningVoid();

      var tempPath = Path.Combine (Path.GetTempPath (), Guid.NewGuid().ToString());
      Directory.CreateDirectory (tempPath);

      try
      {
        var appDomainSetup = AppDomain.CurrentDomain.SetupInformation;
        appDomainSetup.ApplicationBase = tempPath;
        AppDomain newDomain = AppDomain.CreateDomain ("Test", null, appDomainSetup);

        Type versionAccessorType = versionAccessorTypeBuilder.BuildType ();
        var assemblyPath = scope.SaveAssembly (true);
        File.Move (assemblyPath, Path.Combine (tempPath, Path.GetFileName (assemblyPath)));
        File.Delete (assemblyPath.Replace (".dll", ".pdb"));

        var interfaceAssemblyPath = typeof (FrameworkVersion).Assembly.Location;
        File.Copy (interfaceAssemblyPath, Path.Combine (tempPath, Path.GetFileName (interfaceAssemblyPath)));

        try
        {
          var action = (CrossAppDomainDelegate) Delegate.CreateDelegate (typeof (CrossAppDomainDelegate), versionAccessorType, "AccessVersion");
          newDomain.DoCallBack (action);
        }
        finally
        {
          AppDomain.Unload (newDomain);
        }
      }
      finally
      {
        Directory.Delete (tempPath, true);
      }
    }

    [Test]
    public void Reset ()
    {
      FrameworkVersion.Value = new Version (1, 2, 3, 4);
      FrameworkVersion.Reset();
      FrameworkVersion.Value = new Version (4, 3, 2, 1);
      Assert.That (FrameworkVersion.Value, Is.EqualTo (new Version (4, 3, 2, 1)));
    }

    [Test]
    public void RetrieveFromType()
    {
      FrameworkVersion.RetrieveFromType (typeof (FrameworkVersionTest));
      Assert.That (FrameworkVersion.Value, Is.EqualTo (typeof (FrameworkVersionTest).Assembly.GetName ().Version));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "The framework version has already been set to " 
        + "1.2.3.45252345. It can only be set once.")]
    public void RetrieveFromType_AfterSet ()
    {
      FrameworkVersion.Value = new Version (1, 2, 3, 45252345);
      FrameworkVersion.RetrieveFromType (typeof (FrameworkVersionTest));
    }

    [Test]
    public void RetrieveFromType_AfterSet_SameValue ()
    {
      FrameworkVersion.Value = typeof (FrameworkVersionTest).Assembly.GetName().Version;
      FrameworkVersion.RetrieveFromType (typeof (FrameworkVersionTest));
      Assert.That (FrameworkVersion.Value, Is.EqualTo (typeof (FrameworkVersionTest).Assembly.GetName ().Version));
    }
  }
}

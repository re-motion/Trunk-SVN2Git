﻿// Copyright (c) rubicon IT GmbH, www.rubicon.eu
//
// See the NOTICE file distributed with this work for additional information
// regarding copyright ownership.  rubicon licenses this file to you under 
// the Apache License, Version 2.0 (the "License"); you may not use this 
// file except in compliance with the License.  You may obtain a copy of the 
// License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software 
// distributed under the License is distributed on an "AS IS" BASIS, WITHOUT 
// WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.  See the 
// License for the specific language governing permissions and limitations
// under the License.
// 

using System;
using System.IO;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.ServiceLocation;
using Remotion.TypePipe;
using Remotion.TypePipe.CodeGeneration;
using Remotion.TypePipe.CodeGeneration.ReflectionEmit;

namespace TypePipe.IntegrationTests
{
  [TestFixture]
  public class FlushGeneratedCodeTest : ObjectFactoryIntegrationTestBase
  {
    private IObjectFactory _objectFactory;
    private ICodeGenerator _codeGenerator;

    public override void SetUp ()
    {
      base.SetUp();

      _objectFactory = CreateObjectFactory();
      _codeGenerator = _objectFactory.CodeGenerator;
    }

    [Test]
    public void FlushGeneratedCode ()
    {
      var assembledType1 = RequestType (typeof (RequestedType));
      var path1 = Flush();

      var assembledType2 = RequestType (typeof (object));
      var path2 = Flush();

      Assert.That (path1, Is.Not.EqualTo (path2));
      Assert.That (assembledType1.FullName, Is.Not.EqualTo (assembledType2.FullName));

      CheckSavedAssembly (path1, assembledType1.FullName);
      CheckSavedAssembly (path2, assembledType2.FullName);
    }

    [Test]
    public void FlushGeneratedCode_NoNewTypes ()
    {
      Assert.That (_codeGenerator.FlushCodeToDisk(), Is.Null);

      RequestTypeAndFlush();
      RequestType();

      Assert.That (_codeGenerator.FlushCodeToDisk(), Is.Null);
    }

    [Test]
    public void StandardNameAndDirectory_Initial ()
    {
      // Get code generator directly to avoid having assembly name and directory set by the integration test setup.
      var codeGenerator = SafeServiceLocator.Current.GetInstance<IReflectionEmitCodeGenerator>();
      Assert.That (codeGenerator.AssemblyDirectory, Is.Null);
      Assert.That (codeGenerator.AssemblyName, Is.StringMatching (@"TypePipe_GeneratedAssembly_\d+"));
    }

    [Test]
    public void StandardNameAndDirectory_Unique ()
    {
      var oldAssemblyName = _codeGenerator.AssemblyName;

      RequestTypeAndFlush();

      Assert.That (_codeGenerator.AssemblyName, Is.Not.EqualTo (oldAssemblyName));
    }

    [Test]
    public void CustomNameAndDirectory ()
    {
      // The assembly will be saved in a directory that lacks the needed references for peverify.
      SkipPeVerification();

      var directory = Path.GetTempPath();
      _codeGenerator.SetAssemblyDirectory (directory);
      _codeGenerator.SetAssemblyName ("Abc");

      Assert.That (_codeGenerator.AssemblyDirectory, Is.EqualTo (directory));
      Assert.That (_codeGenerator.AssemblyName, Is.EqualTo ("Abc"));

      var path = RequestTypeAndFlush();

      Assert.That (path, Is.EqualTo (Path.Combine (directory, "Abc.dll")));
      Assert.That (File.Exists (path), Is.True);
    }

    [Test]
    public void SetNameAndDirectory_AfterFlush ()
    {
      RequestType();

      var message1 = "Cannot set assembly directory after a type has been defined (use FlushCodeToDisk() to start a new assembly).";
      var message2 = "Cannot set assembly name after a type has been defined (use FlushCodeToDisk() to start a new assembly).";
      Assert.That (() => _codeGenerator.SetAssemblyDirectory ("Uio"), Throws.InvalidOperationException.With.Message.EqualTo (message1));
      Assert.That (() => _codeGenerator.SetAssemblyName ("Xyz"), Throws.InvalidOperationException.With.Message.EqualTo (message2));

      Flush();

      _codeGenerator.SetAssemblyDirectory ("Uio");
      _codeGenerator.SetAssemblyName ("Xyz");

      Assert.That (_codeGenerator.AssemblyDirectory, Is.EqualTo ("Uio"));
      Assert.That (_codeGenerator.AssemblyName, Is.EqualTo ("Xyz"));
    }

    private Type RequestType (Type requestedType = null)
    {
      requestedType = requestedType ?? typeof (RequestedType);
      return _objectFactory.GetAssembledType (requestedType);
    }

    private string RequestTypeAndFlush ()
    {
      RequestType();
      return Flush();
    }

    private string Flush ()
    {
      var assemblyPath = FlushAndTrackFilesForCleanup();
      Assert.That (assemblyPath, Is.Not.Null);

      Assert.That (File.Exists (assemblyPath), Is.True);
      Assert.That (File.Exists (Path.ChangeExtension (assemblyPath, "pdb")), Is.True);

      return assemblyPath;
    }

    private void CheckSavedAssembly (string assemblyPath, string assembledTypeFullName)
    {
      Assert.That (File.Exists (assemblyPath), Is.True);

      AppDomainRunner.Run (
          args =>
          {
            var path = (string) args[0];
            var typeName = (string) args[1];

            var assembly = Assembly.LoadFrom (path);
            var typeNames = assembly.GetExportedTypes().Select (t => t.FullName);

            Assert.That (typeNames, Is.EqualTo (new[] { typeName }));
          },
          assemblyPath,
          assembledTypeFullName);
    }

    public class RequestedType { }
  }
}
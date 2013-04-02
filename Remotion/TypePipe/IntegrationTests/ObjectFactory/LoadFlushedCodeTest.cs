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

using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using NUnit.Framework;

namespace Remotion.TypePipe.IntegrationTests.ObjectFactory
{
  [TestFixture]
  public class LoadFlushedCodeTest : IntegrationTestBase
  {
    private Assembly _assembly1;
    private Assembly _assembly2;

    private IObjectFactory _objectFactory;

    public override void TestFixtureSetUp ()
    {
      base.TestFixtureSetUp();

      PreGenerateAssemblies();
    }

    public override void SetUp ()
    {
      base.SetUp();

      _objectFactory = CreateObjectFactory();
    }

    [Test]
    public void LoadMultipleAssemblies ()
    {
      _objectFactory.LoadFlushedCode (_assembly1);
      _objectFactory.LoadFlushedCode (_assembly2);

      var assembledType1 = _objectFactory.GetAssembledType (typeof (DomainType1));
      var assembledType2 = _objectFactory.GetAssembledType (typeof (DomainType2));

      Assert.That (assembledType1.Assembly, Is.SameAs (_assembly1));
      Assert.That (assembledType2.Assembly, Is.SameAs (_assembly2));
      Assert.That (Flush(), Is.Null, "No new code should generated.");
    }

    [Test]
    public void LoadAssembly_ThenContinueGenerating ()
    {
      _objectFactory.LoadFlushedCode (_assembly1);

      var assembledType1 = _objectFactory.GetAssembledType (typeof (DomainType1));

      Assert.That (assembledType1.Assembly, Is.SameAs (_assembly1));
      Assert.That (Flush(), Is.Null, "No new code should be generated.");

      var assembledType2 = _objectFactory.GetAssembledType (typeof (DomainType2));

      Assert.That (assembledType2.Assembly, Is.TypeOf<AssemblyBuilder>());
      Assert.That (Flush(), Is.Not.Null);
    }

    [Test]
    public void LoadAlreadyCachedType_DoesNothing ()
    {
      // Load and get type 1.
      _objectFactory.LoadFlushedCode (_assembly1);
      var assembledType1 = _objectFactory.GetAssembledType (typeof (DomainType1));
      // Generate and get type 2.
      var assembledType2 = _objectFactory.GetAssembledType (typeof (DomainType2));

      _objectFactory.LoadFlushedCode (_assembly1);
      _objectFactory.LoadFlushedCode (_assembly2);

      Assert.That (_objectFactory.GetAssembledType (typeof (DomainType1)), Is.SameAs (assembledType1));
      Assert.That (_objectFactory.GetAssembledType (typeof (DomainType2)), Is.SameAs (assembledType2));
    }

    private void PreGenerateAssemblies ()
    {
      var objectFactory = CreateObjectFactory();

      var assembledType1 = objectFactory.GetAssembledType (typeof (DomainType1));
      var assemblyPath1 = Flush();
      var assembledType2 = objectFactory.GetAssembledType (typeof (DomainType2));
      var assemblyPath2 = Flush();

      Assert.That (assembledType1.Assembly, Is.Not.SameAs (assembledType2.Assembly));
      Assert.That (assemblyPath1, Is.Not.Null.And.Not.EqualTo (assemblyPath2));

      // Load via raw bytes so that the file is not locked and can be deleted.
      _assembly1 = Assembly.Load (File.ReadAllBytes (assemblyPath1));
      _assembly2 = Assembly.Load (File.ReadAllBytes (assemblyPath2));
    }

    public class DomainType1 {}
    public class DomainType2 {}
  }
}
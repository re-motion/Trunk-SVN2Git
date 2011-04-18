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
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Persistence.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping;
using Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.SqlServer.SchemaGeneration;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Rhino.Mocks;
using System.Linq;
using File = System.IO.File;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.SchemaGeneration
{
  [TestFixture]
  public class FileBuilderTest : SchemaGenerationTestBase
  {
    private TestableFileBuilder _fileBuilder;
    private ScriptBuilderBase _scriptBuilderMock;

    private ClassDefinition _classDefinition1;
    private ClassDefinition _classDefinition2;
    private ClassDefinition _classDefinition3;

    private IEntityDefinition _firstProviderStorageEntityDefinitionStub;
    private IEntityDefinition _secondProviderStorageEntityDefinitionStub;

    public override void TestFixtureSetUp ()
    {
      base.TestFixtureSetUp();

      if (Directory.Exists ("TestDirectory"))
        Directory.Delete ("TestDirectory", true);
    }

    public override void SetUp ()
    {
      base.SetUp();

      _scriptBuilderMock = MockRepository.GenerateStrictMock<ScriptBuilderBase> (SchemaGenerationFirstStorageProviderDefinition);
      _fileBuilder = new TestableFileBuilder (_scriptBuilderMock);

      _classDefinition1 = ClassDefinitionFactory.CreateClassDefinition (typeof (Order));
      _classDefinition2 = ClassDefinitionFactory.CreateClassDefinition (typeof (OrderItem));
      _classDefinition3 = ClassDefinitionFactory.CreateClassDefinition (typeof (Customer));

      _firstProviderStorageEntityDefinitionStub = MockRepository.GenerateStub<IEntityDefinition> ();
      _firstProviderStorageEntityDefinitionStub.Stub (stub => stub.StorageProviderDefinition).Return (SchemaGenerationFirstStorageProviderDefinition);

      _secondProviderStorageEntityDefinitionStub = MockRepository.GenerateStub<IEntityDefinition> ();
      _secondProviderStorageEntityDefinitionStub.Stub (stub => stub.StorageProviderDefinition).Return (SchemaGenerationSecondStorageProviderDefinition);
    }

    public override void TearDown ()
    {
      base.TearDown();

      if (Directory.Exists ("TestDirectory"))
        Directory.Delete ("TestDirectory", true);
    }

    [Test]
    public void Build_Static_ExecutesFileBuilder_WithSingleFile ()
    {
      var fileBuilderMock1 = MockRepository.GenerateStrictMock<IFileBuilder> ();
      Func<RdbmsProviderDefinition, IFileBuilder> fileBuilderFactory = providerDefinition => fileBuilderMock1;

      var classDefinitions = new[] { _classDefinition1, _classDefinition2 };

      fileBuilderMock1.Expect (mock => mock.Build (classDefinitions, @"TestDirectory\SetupDB.sql"));
      fileBuilderMock1.Replay ();

      FileBuilder.Build (
          classDefinitions,
          new[] { SchemaGenerationFirstStorageProviderDefinition },
          "TestDirectory",
          fileBuilderFactory);

      fileBuilderMock1.VerifyAllExpectations ();
    }

    [Test]
    public void Build_Static_ExecutesFileBuilders_WithMultipleFiles ()
    {
      var fileBuilderMock1 = MockRepository.GenerateStrictMock<IFileBuilder> ();
      var fileBuilderMock2 = MockRepository.GenerateStrictMock<IFileBuilder> ();
      Func<RdbmsProviderDefinition, IFileBuilder> fileBuilderFactory =
          providerDefinition => providerDefinition == SchemaGenerationFirstStorageProviderDefinition ? fileBuilderMock1 : fileBuilderMock2;

      var classDefinitions = new[] { _classDefinition1, _classDefinition2 };

      fileBuilderMock1.Expect (mock => mock.Build (classDefinitions, @"TestDirectory\SetupDB_SchemaGenerationFirstStorageProvider.sql"));
      fileBuilderMock1.Replay();

      fileBuilderMock2.Expect (mock => mock.Build (classDefinitions, @"TestDirectory\SetupDB_SchemaGenerationSecondStorageProvider.sql"));
      fileBuilderMock2.Replay();

      FileBuilder.Build (
          classDefinitions, 
          new[] { SchemaGenerationFirstStorageProviderDefinition, SchemaGenerationSecondStorageProviderDefinition }, 
          "TestDirectory", 
          fileBuilderFactory);

      fileBuilderMock1.VerifyAllExpectations ();
      fileBuilderMock2.VerifyAllExpectations ();
    }

    [Test]
    public void Build_Static_WithNonRdbmsProvider ()
    {
      Func<RdbmsProviderDefinition, IFileBuilder> fileBuilderFactory =
          storageProvider => { throw new InvalidOperationException ("Should not be called."); };

    var classDefinitions = new[] { _classDefinition1, _classDefinition2 };

      var fakeStorageProviderDefinition = new UnitTestStorageProviderStubDefinition ("Test");
      FileBuilder.Build (
          classDefinitions,
          new StorageProviderDefinition[] { fakeStorageProviderDefinition },
          "TestDirectory",
          fileBuilderFactory);
    }

    [Test]
    public void Build_Static_CreatesDirectory ()
    {
      Assert.That (Directory.Exists ("TestDirectory"), Is.False);

      Func<RdbmsProviderDefinition, IFileBuilder> fileBuilderFactory =
    storageProvider => { throw new InvalidOperationException ("Should not be called."); };

      var classDefinitions = new[] { _classDefinition1, _classDefinition2 };

      var fakeStorageProviderDefinition = new UnitTestStorageProviderStubDefinition ("Test");
      FileBuilder.Build (
          classDefinitions,
          new StorageProviderDefinition[] { fakeStorageProviderDefinition },
          "TestDirectory",
          fileBuilderFactory);

      Assert.That (Directory.Exists ("TestDirectory"), Is.True);
    }

    [Test]
    public void Build_Static_WithEmptyDirectory ()
    {
      var fileBuilderMock1 = MockRepository.GenerateStrictMock<IFileBuilder> ();
      Func<RdbmsProviderDefinition, IFileBuilder> fileBuilderFactory = providerDefinition => fileBuilderMock1;

      var classDefinitions = new[] { _classDefinition1, _classDefinition2 };

      fileBuilderMock1.Expect (mock => mock.Build (classDefinitions, @"SetupDB.sql"));
      fileBuilderMock1.Replay ();

      FileBuilder.Build (
          classDefinitions,
          new[] { SchemaGenerationFirstStorageProviderDefinition },
          "",
          fileBuilderFactory);

      fileBuilderMock1.VerifyAllExpectations ();
    }

    [Test]
    public void GetFileName ()
    {
      var result = FileBuilder.GetFileName (SchemaGenerationFirstStorageProviderDefinition, "TestOutputPath", false);

      Assert.That (result, Is.EqualTo ("TestOutputPath\\SetupDB.sql"));
    }

    [Test]
    public void GetFileName_MultipleStorageProviders ()
    {
      var result = FileBuilder.GetFileName (SchemaGenerationFirstStorageProviderDefinition, "TestOutputPath", true);

      Assert.That (result, Is.EqualTo ("TestOutputPath\\SetupDB_SchemaGenerationFirstStorageProvider.sql"));
    }

    [Test]
    public void Build_InstanceMethod ()
    {
      _classDefinition1.SetStorageEntity (_firstProviderStorageEntityDefinitionStub);

      _scriptBuilderMock
          .Expect (mock => mock.GetScript (Arg<IEnumerable<IEntityDefinition>>.List.Equal (new[] { _firstProviderStorageEntityDefinitionStub })))
          .Return ("Dummy");
      
      _fileBuilder.Build (new[] { _classDefinition1 }, @"FileBuilderTest_Build_InstanceMethod.sql");

      Assert.That (File.Exists (@"FileBuilderTest_Build_InstanceMethod.sql"), Is.True);
      try
      {
        Assert.That (File.ReadAllText (@"FileBuilderTest_Build_InstanceMethod.sql"), Is.EqualTo ("Dummy"));
      }
      finally
      {
        File.Delete (@"FileBuilderTest_Build_InstanceMethod.sql");
      }
    }
    
    [Test]
    public void GetScript_NoIEntityDefinition ()
    {
      var storageEntityDefinitionStub = MockRepository.GenerateStub<IStorageEntityDefinition>();
      storageEntityDefinitionStub.Stub (stub => stub.StorageProviderDefinition).Return (SchemaGenerationFirstStorageProviderDefinition);
      _classDefinition1.SetStorageEntity (storageEntityDefinitionStub);

      _scriptBuilderMock
          .Expect (mock => mock.GetScript (Arg<IEnumerable<IEntityDefinition>>.List.Equal (new IEntityDefinition[0])))
          .Return ("result");
      _scriptBuilderMock.Replay();

      var result = _fileBuilder.GetScript (new[] { _classDefinition1 });

      _scriptBuilderMock.VerifyAllExpectations();
      Assert.That (result, Is.EqualTo ("result"));
    }

    [Test]
    public void GetScript_WithIEntityDefinition ()
    {
      _classDefinition1.SetStorageEntity (_firstProviderStorageEntityDefinitionStub);

      _scriptBuilderMock
          .Expect (mock => mock.GetScript (Arg<IEnumerable<IEntityDefinition>>.List.Equal (new[] { _firstProviderStorageEntityDefinitionStub })))
          .Return ("result");
      _scriptBuilderMock.Replay ();

      var result = _fileBuilder.GetScript (new[] { _classDefinition1 });

      _scriptBuilderMock.VerifyAllExpectations ();
      Assert.That (result, Is.EqualTo ("result"));
    }

    [Test]
    public void GetScript_WithWrongStorageProvider ()
    {
      _classDefinition1.SetStorageEntity (_secondProviderStorageEntityDefinitionStub);

      _scriptBuilderMock
          .Expect (mock => mock.GetScript (Arg<IEnumerable<IEntityDefinition>>.List.Equal (new IEntityDefinition[0])))
          .Return ("result");
      _scriptBuilderMock.Replay ();

      var result = _fileBuilder.GetScript (new[] { _classDefinition1 });

      _scriptBuilderMock.VerifyAllExpectations();
      Assert.That (result, Is.EqualTo ("result"));
    }

    [Test]
    public void GetClassesInStorageProvider ()
    {
      _classDefinition1.SetStorageEntity (_firstProviderStorageEntityDefinitionStub);
      _classDefinition2.SetStorageEntity (_firstProviderStorageEntityDefinitionStub);
      _classDefinition3.SetStorageEntity (_secondProviderStorageEntityDefinitionStub);

      var classesInFirstStorageProvider = _fileBuilder.GetClassesInStorageProvider (
          new[] { _classDefinition1, _classDefinition2, _classDefinition3 },
          SchemaGenerationFirstStorageProviderDefinition);

      Assert.That (classesInFirstStorageProvider, Is.EqualTo (new[] { _classDefinition1, _classDefinition2 }));
    }

    [Test]
    public void GetEntityDefinitions ()
    {
      var entityDefinition1 = MockRepository.GenerateStub<IEntityDefinition>();
      var entityDefinition2 = MockRepository.GenerateStub<IEntityDefinition> ();
      var storageEntityDefinition = MockRepository.GenerateStub<IStorageEntityDefinition>();

      _classDefinition1.SetStorageEntity (entityDefinition1);
      _classDefinition2.SetStorageEntity (storageEntityDefinition);
      _classDefinition3.SetStorageEntity (entityDefinition2);

      var result = _fileBuilder.GetEntityDefinitions (new[] { _classDefinition1, _classDefinition2, _classDefinition3 });

      Assert.That (result.ToArray(), Is.EqualTo (new[] { entityDefinition1, entityDefinition2 }));
  }
  }
}
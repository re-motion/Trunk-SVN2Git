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
using NUnit.Framework;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.SchemaGeneration;
using Remotion.Development.UnitTesting.Resources;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.SqlServer.SchemaGeneration
{
  //TODO: Derive ClassWithAllDataTypes from an abstract class to ensure that all data types are selected in a UNION
  [TestFixture]
  public class ScriptBuilderTest : SchemaGenerationTestBase
  {
    private ScriptBuilder _scriptBuilderForFirstStorageProvider;
    private ScriptBuilder _scriptBuilderForSecondStorageProvider;
    private string _firstStorageProviderSetupDBScriptWithoutTables;

    public override void SetUp ()
    {
      base.SetUp();

      _scriptBuilderForFirstStorageProvider = new ScriptBuilder (SchemaGenerationFirstStorageProviderDefinition);
      _scriptBuilderForSecondStorageProvider = new ScriptBuilder (SchemaGenerationSecondStorageProviderDefinition);
      _firstStorageProviderSetupDBScriptWithoutTables = ResourceUtility.GetResourceString (typeof (ScriptBuilderTest), "TestData.SetupDB_FirstStorageProviderWithoutTables.sql");
    }

    [Test]
    public void GetDatabaseName ()
    {
      Assert.AreEqual ("SchemaGenerationTestDomain1", _scriptBuilderForFirstStorageProvider.GetDatabaseName());
      Assert.AreEqual ("SchemaGenerationTestDomain2", _scriptBuilderForSecondStorageProvider.GetDatabaseName());
    }

    [Test]
    public void GetScript_NoEntities ()
    {
      var result = _scriptBuilderForFirstStorageProvider.GetScript (new IEntityDefinition[0]);

      Assert.That (result, Is.EqualTo (_firstStorageProviderSetupDBScriptWithoutTables));
    }
  }
}
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
using System.Data.SqlClient;
using Microsoft.Practices.ServiceLocation;
using NUnit.Framework;
using Remotion.Data.UnitTests.DomainObjects.Core.TableInheritance;
using Remotion.Data.UnitTests.DomainObjects.Database;
using Remotion.Data.UnitTests.DomainObjects.Factories;
using Remotion.Development.UnitTesting.Data.SqlClient;

namespace Remotion.Data.UnitTests.DomainObjects
{
  [SetUpFixture]
  public class SetUpFixture
  {
    private StandardMappingDatabaseAgent _standardMappingDatabaseAgent;

    [SetUp]
    public void SetUp ()
    {
      try
      {
        ServiceLocator.SetLocatorProvider (() => null);

        StandardConfiguration.Initialize();

        SqlConnection.ClearAllPools();

        DatabaseAgent masterAgent = new DatabaseAgent (DatabaseTest.MasterConnectionString);
        masterAgent.ExecuteBatchFile ("DataDomainObjects_CreateDB.sql", false);
        DatabaseAgent testDomainAgent = new DatabaseAgent (DatabaseTest.TestDomainConnectionString);
        testDomainAgent.ExecuteBatchFile ("DataDomainObjects_SetupDB.sql", true);

        _standardMappingDatabaseAgent = new StandardMappingDatabaseAgent (DatabaseTest.TestDomainConnectionString);
        _standardMappingDatabaseAgent.ExecuteBatchFile (StandardMappingTest.CreateTestDataFileName, true);
        _standardMappingDatabaseAgent.ExecuteBatchFile (TableInheritanceMappingTest.CreateTestDataFileName, true);
        _standardMappingDatabaseAgent.SetDatabaseReadOnly (DatabaseTest.DatabaseName);
      }
      catch (Exception ex)
      {
        Console.WriteLine ("SetUpFixture failed: " + ex);
        Console.WriteLine ();
        throw;
      }
    }

    [TearDown]
    public void TearDown ()
    {
      _standardMappingDatabaseAgent.SetDatabaseReadWrite (DatabaseTest.DatabaseName);
      SqlConnection.ClearAllPools();
    }
  }
}

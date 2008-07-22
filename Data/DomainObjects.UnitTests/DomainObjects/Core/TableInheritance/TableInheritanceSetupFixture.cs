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
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Development.UnitTesting.Data.SqlClient;

namespace Remotion.Data.UnitTests.DomainObjects.Core.TableInheritance
{
  [SetUpFixture]
  public class TableInheritanceSetUpFixture
  {
    private DatabaseAgent _databaseAgent;

    [SetUp]
    public void SetUp()
    {
      TableInheritanceConfiguration.Initialize();

      _databaseAgent = new DatabaseAgent (DatabaseTest.TestDomainConnectionString);
      _databaseAgent.SetDatabaseReadWrite (DatabaseTest.DatabaseName);
      _databaseAgent.ExecuteBatch (TableInheritanceMappingTest.CreateTestDataFileName, true);
      _databaseAgent.SetDatabaseReadOnly (DatabaseTest.DatabaseName);
    }
  }
}

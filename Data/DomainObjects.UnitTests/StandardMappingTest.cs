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
using Remotion.Configuration;
using Remotion.Data.DomainObjects.Configuration;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.UnitTests.Database;
using Remotion.Data.DomainObjects.UnitTests.Factories;

namespace Remotion.Data.DomainObjects.UnitTests
{
  public class StandardMappingTest: DatabaseTest
  {
    public const string CreateTestDataFileName = "DataDomainObjects_CreateTestData.sql";

    protected StandardMappingTest()
        : base (new StandardMappingDatabaseAgent (TestDomainConnectionString), CreateTestDataFileName)
    {
    }

    public override void TestFixtureSetUp ()
    {
      base.TestFixtureSetUp ();
      DomainObjectsConfiguration.SetCurrent (StandardConfiguration.Instance.GetDomainObjectsConfiguration ());
      MappingConfiguration.SetCurrent (StandardConfiguration.Instance.GetMappingConfiguration());
      ConfigurationWrapper.SetCurrent (null);      
      TestMappingConfiguration.Reset();
    }

    public override void SetUp()
    {
      base.SetUp();
      DomainObjectsConfiguration.SetCurrent (StandardConfiguration.Instance.GetDomainObjectsConfiguration ());
      MappingConfiguration.SetCurrent (StandardConfiguration.Instance.GetMappingConfiguration());
      ConfigurationWrapper.SetCurrent (null);
    }

    protected DomainObjectIDs DomainObjectIDs
    {
      get { return StandardConfiguration.Instance.GetDomainObjectIDs(); }
    }
  }
}

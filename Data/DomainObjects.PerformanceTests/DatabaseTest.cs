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
using Remotion.Data.DomainObjects.PerformanceTests.Database;

namespace Remotion.Data.DomainObjects.PerformanceTests
{
  public class DatabaseTest
  {
    [TestFixtureSetUp]
    public virtual void TestFixtureSetUp()
    {
      StandardConfiguration.Initialize();
    }

    [TestFixtureTearDown]
    public virtual void TestFixtureTearDown()
    {
    }

    [SetUp]
    public virtual void SetUp()
    {
      using (TestDataLoader loader = new TestDataLoader (StandardConfiguration.ConnectionString))
      {
        loader.Load();
      }
    }

    [TearDown]
    public virtual void TearDown()
    {
    }
  }
}

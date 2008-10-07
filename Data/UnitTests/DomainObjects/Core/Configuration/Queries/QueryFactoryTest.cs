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
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects.Configuration;
using Remotion.Data.DomainObjects.Linq;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.Linq.SqlGeneration;
using Remotion.Data.Linq.SqlGeneration.SqlServer;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Rhino.Mocks;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Development.UnitTesting;
using Remotion.Data.DomainObjects.Mapping;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Configuration.Queries
{
  [TestFixture]
  public class QueryFactoryTest : StandardMappingTest
  {
    [Test]
    public void CreateQueryable_WithSqlGenerator ()
    {
      var sqlGeneratorMock = MockRepository.GenerateMock<ISqlGenerator> ();
      var queryable = QueryFactory.CreateQueryable<Order> (sqlGeneratorMock);
      Assert.That (queryable, Is.Not.Null);
      Assert.That (queryable.GetExecutor (), Is.InstanceOfType (typeof (QueryExecutor<Order>)));
      Assert.That (queryable.GetExecutor ().SqlGenerator, Is.SameAs (sqlGeneratorMock));
    }

    [Test]
    public void CreateQueryable_WithImplicitSqlGenerator ()
    {
      var queryable = QueryFactory.CreateQueryable<Order> ();
      Assert.That (queryable, Is.Not.Null);
      Assert.That (queryable.GetExecutor (), Is.InstanceOfType (typeof (QueryExecutor<Order>)));
      Assert.That (queryable.GetExecutor ().SqlGenerator, Is.Not.Null);

      Assert.That (queryable.GetExecutor ().SqlGenerator, Is.SameAs (QueryFactory.GetDefaultSqlGenerator (typeof (Order))));
    }

    [Test]
    public void GetDefaultSqlGenerator ()
    {
      var providerID = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (Order)).StorageProviderID;
      var providerDefinition = DomainObjectsConfiguration.Current.Storage.StorageProviderDefinitions[providerID];
      Assert.That (QueryFactory.GetDefaultSqlGenerator (typeof (Order)), Is.SameAs (providerDefinition.LinqSqlGenerator));
    }
  }
}
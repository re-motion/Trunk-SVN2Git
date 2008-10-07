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
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Configuration.StorageProviders;
using Remotion.Data.DomainObjects.Linq;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.Linq.Parsing.Details.WhereConditionParsing;
using Remotion.Data.Linq.SqlGeneration;
using Remotion.Data.Linq.SqlGeneration.SqlServer;
using Remotion.Mixins;
using Remotion.Utilities;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Configuration.StorageProviders
{
  [TestFixture]
  public class RdbmsProviderDefinitionTest
  {
    [Test]
    public void Initialize()
    {
      var providerDefinition = new RdbmsProviderDefinition ("Provider", typeof (SqlProvider), "ConnectionString");

      Assert.That (providerDefinition.Name, Is.EqualTo ("Provider"));
      Assert.That (providerDefinition.StorageProviderType, Is.SameAs (typeof (SqlProvider)));
      Assert.That (providerDefinition.ConnectionString, Is.EqualTo ("ConnectionString"));
    }

    [Test]
    public void GetTypeConversionProvider ()
    {
      var providerDefinition = new RdbmsProviderDefinition ("Provider", typeof (SqlProvider), "ConnectionString");
      Assert.That (providerDefinition.TypeConversionProvider, Is.InstanceOfType (typeof (TypeConversionProvider)));
    }

    [Test]
    public void GetTypeProvider ()
    {
      var providerDefinition = new RdbmsProviderDefinition ("Provider", typeof (SqlProvider), "ConnectionString");
      Assert.That (providerDefinition.TypeProvider, Is.InstanceOfType (typeof (TypeProvider)));
    }

    [Test]
    public void LinqSqlGenerator()
    {
      var providerDefinition = new RdbmsProviderDefinition ("Provider", typeof (SqlProvider), "ConnectionString");
      Assert.That (providerDefinition.LinqSqlGenerator, Is.InstanceOfType (typeof (SqlServerGenerator)));
    }

    [Test]
    public void LinqSqlGenerator_CanBeMixed ()
    {
      using (MixinConfiguration.BuildNew ().ForClass<SqlServerGenerator> ().AddMixin<object> ().EnterScope ())
      {
        var providerDefinition = new RdbmsProviderDefinition ("Provider", typeof (SqlProvider), "ConnectionString");
        Assert.That (providerDefinition.LinqSqlGenerator, Is.InstanceOfType (typeof (SqlServerGenerator)));
        Assert.That (Mixin.Get<object> (providerDefinition.LinqSqlGenerator), Is.Not.Null);
      }
    }

    [Test]
    public void ResetLinqSqlGenerator ()
    {
      var providerDefinition = new RdbmsProviderDefinition ("Provider", typeof (SqlProvider), "ConnectionString");
      ISqlGenerator generator = providerDefinition.LinqSqlGenerator;
      providerDefinition.ResetLinqSqlGenerator ();

      ISqlGenerator generator2 = providerDefinition.LinqSqlGenerator;

      Assert.That (generator2, Is.Not.Null);
      Assert.That (generator2, Is.Not.SameAs (generator));
    }

    [Test]
    public void SqlGenerator_HasOPFDetailParsers ()
    {
      var providerDefinition = new RdbmsProviderDefinition ("Provider", typeof (SqlProvider), "ConnectionString");
      ISqlGenerator generator = providerDefinition.LinqSqlGenerator;
      IEnumerable<IWhereConditionParser> whereConditionParsers = generator.DetailParserRegistries.WhereConditionParser.GetParsers (
          typeof (MethodCallExpression));
      IEnumerable<Type> parserTypes = from p in whereConditionParsers select p.GetType ();
      Assert.That (parserTypes.ToArray (), List.Contains (typeof (ContainsObjectParser)));
    }

  }
}

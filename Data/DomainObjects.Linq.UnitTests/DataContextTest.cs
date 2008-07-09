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
using Remotion.Data.Linq.Parsing.Details.WhereConditionParsing;
using Remotion.Data.Linq.SqlGeneration;
using Remotion.Data.Linq.SqlGeneration.SqlServer;
using Remotion.Mixins;
using Rhino.Mocks;
using Remotion.Data.DomainObjects.UnitTests;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.Linq.UnitTests
{
  [TestFixture]
  public class DataContextTest : ClientTransactionBaseTest
  {
    public override void TearDown ()
    {
      DataContext.ResetSqlGenerator ();
      base.TearDown ();
    }

    [Test]
    public void Entity()
    {
      Assert.IsNotNull (DataContext.Entity<Order>());
    }

    [Test]
    public void SqlGenerator_CanBeMixed ()
    {
      using (MixinConfiguration.BuildNew ().ForClass<SqlServerGenerator> ().AddMixin<object> ().EnterScope())
      {
        DataContext.ResetSqlGenerator();
        Assert.That (Mixin.Get<object> (DataContext.SqlGenerator), Is.Not.Null);
      }
    }

    [Test]
    public void SqlGenerator_AutoInitialization ()
    {
      ISqlGeneratorBase generator = DataContext.SqlGenerator;
      Assert.That (generator, Is.Not.Null);
    }

    [Test]
    public void ResetSqlGenerator ()
    {
      ISqlGeneratorBase generator = DataContext.SqlGenerator;

      DataContext.ResetSqlGenerator ();
      ISqlGeneratorBase generator2 = DataContext.SqlGenerator;

      Assert.That (generator2, Is.Not.Null);
      Assert.That (generator2, Is.Not.SameAs (generator));
    }

    [Test]
    public void SqlGenerator_HasOPFDetailParsers ()
    {
      ISqlGeneratorBase generator = DataContext.SqlGenerator;
      IEnumerable<IWhereConditionParser> whereConditionParsers = generator.DetailParser.WhereConditionParser.GetParsers (
          typeof (MethodCallExpression));
      IEnumerable<Type> parserTypes = from p in whereConditionParsers select p.GetType();
      Assert.That (parserTypes.ToArray(), List.Contains (typeof (ContainsObjectParser)));
    }
  }
}

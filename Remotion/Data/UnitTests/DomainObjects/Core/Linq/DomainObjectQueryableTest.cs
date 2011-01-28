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
using System.Linq.Expressions;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects.Linq;
using Remotion.Data.Linq;
using Remotion.Data.Linq.Parsing.Structure;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Rhino.Mocks;


namespace Remotion.Data.UnitTests.DomainObjects.Core.Linq
{
  [TestFixture]
  public class DomainObjectQueryableTest : ClientTransactionBaseTest
  {
    private IQueryExecutor _executorStub;
    private IQueryParser _queryParserStub;

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp();

      _executorStub = MockRepository.GenerateStub<IQueryExecutor>();
      _queryParserStub = MockRepository.GenerateStub<IQueryParser> ();
    }

    [Test]
    public void Provider_AutoInitialized ()
    {
      var queryableWithOrder = new DomainObjectQueryable<Order> (_executorStub, _queryParserStub);

      Assert.That (queryableWithOrder.Provider, Is.Not.Null);
      Assert.That (queryableWithOrder.Provider, Is.InstanceOfType (typeof (DefaultQueryProvider)));
      Assert.That (((DefaultQueryProvider) queryableWithOrder.Provider).QueryableType, Is.SameAs (typeof (DomainObjectQueryable<>)));
      Assert.That (queryableWithOrder.Provider.Executor, Is.SameAs (_executorStub));
      Assert.That (queryableWithOrder.Provider.QueryParser, Is.SameAs (_queryParserStub));
    }
    
    [Test]
    public void Provider_PassedIn ()
    {
      var expectedProvider = new DefaultQueryProvider (
          typeof (DomainObjectQueryable<>),
          _executorStub,
          _queryParserStub);

      var queryable = new DomainObjectQueryable<Order> (expectedProvider, Expression.Constant (null, typeof (DomainObjectQueryable<Order>)));
      
      Assert.That (queryable.Provider, Is.Not.Null);
      Assert.That (queryable.Provider, Is.SameAs (expectedProvider));
    }

    
  }
}
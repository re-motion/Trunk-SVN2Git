// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.Security;
using Remotion.Data.UnitTests.DomainObjects.Security.TestDomain;
using Remotion.Security;

namespace Remotion.Data.UnitTests.DomainObjects.Security.SecurityClientTransactionExtensionTests
{
  [TestFixture]
  public class FilterQueryResultTest
  {
    private TestHelper _testHelper;
    private IClientTransactionExtension _extension;

    [SetUp]
    public void SetUp ()
    {
      _testHelper = new TestHelper ();
      _extension = new SecurityClientTransactionExtension ();

      _testHelper.SetupSecurityConfiguration ();
    }

    [TearDown]
    public void TearDown ()
    {
      _testHelper.TearDownSecurityConfiguration ();
    }

    [Test]
    public void Test_WithNullObject ()
    {
      IQuery query = QueryFactory.CreateQueryFromConfiguration ("Dummy");
      var queryResult = new QueryResult<DomainObject> (query, new DomainObject[] { null });
      _testHelper.AddExtension (_extension);
      _testHelper.ReplayAll ();

      var finalResult = _extension.FilterQueryResult (_testHelper.Transaction, queryResult);

      _testHelper.VerifyAll ();
      Assert.That (finalResult, Is.SameAs (queryResult));
    }

    [Test]
    public void Test_WithOneAllowedObject ()
    {
      SecurableObject allowedObject = _testHelper.CreateSecurableObject ();
      IQuery query = QueryFactory.CreateQueryFromConfiguration ("Dummy");
      var queryResult = new QueryResult<DomainObject> (query, new DomainObject[] { allowedObject });
      _testHelper.AddExtension (_extension);
      _testHelper.ExpectObjectSecurityStrategyHasAccess (allowedObject, GeneralAccessTypes.Find, true);
      _testHelper.ReplayAll ();

      var finalResult = _extension.FilterQueryResult (_testHelper.Transaction, queryResult);

      _testHelper.VerifyAll ();
      Assert.That (finalResult, Is.SameAs (queryResult));
    }

    [Test]
    public void Test_WithNoObjects ()
    {
      _extension = new SecurityClientTransactionExtension ();
      IQuery query = QueryFactory.CreateQueryFromConfiguration ("Dummy");
      var queryResult = new QueryResult<DomainObject> (query, new DomainObject[0]);
      _testHelper.AddExtension (_extension);
      _testHelper.ReplayAll ();

      var finalResult = _extension.FilterQueryResult (_testHelper.Transaction, queryResult);

      _testHelper.VerifyAll ();
      Assert.That (finalResult, Is.SameAs (queryResult));
      Assert.That (finalResult.Count, Is.EqualTo (0));
    }

    [Test]
    public void Test_WithOneDeniedObject ()
    {
      SecurableObject deniedObject = _testHelper.CreateSecurableObject ();
      IQuery query = QueryFactory.CreateQueryFromConfiguration ("Dummy");
      var queryResult = new QueryResult<DomainObject> (query, new DomainObject[] { deniedObject });
      _testHelper.AddExtension (_extension);
      _testHelper.ExpectObjectSecurityStrategyHasAccess (deniedObject, GeneralAccessTypes.Find, false);
      _testHelper.ReplayAll ();

      var finalResult = _extension.FilterQueryResult (_testHelper.Transaction, queryResult);

      _testHelper.VerifyAll ();
      Assert.That (finalResult, Is.Not.SameAs (queryResult));
      Assert.That (finalResult.Count, Is.EqualTo (0));
    }

    [Test]
    public void Test_WithOneAllowedAndOneDeniedObject ()
    {
      SecurableObject allowedObject = _testHelper.CreateSecurableObject ();
      SecurableObject deniedObject = _testHelper.CreateSecurableObject ();
      IQuery query = QueryFactory.CreateQueryFromConfiguration ("Dummy");
      var queryResult = new QueryResult<DomainObject> (query, new DomainObject[] { allowedObject, deniedObject });
      _testHelper.AddExtension (_extension);
      _testHelper.ExpectObjectSecurityStrategyHasAccess (allowedObject, GeneralAccessTypes.Find, true);
      _testHelper.ExpectObjectSecurityStrategyHasAccess (deniedObject, GeneralAccessTypes.Find, false);
      _testHelper.ReplayAll ();

      var finalResult = _extension.FilterQueryResult (_testHelper.Transaction, queryResult);

      _testHelper.VerifyAll ();
      Assert.That (finalResult, Is.Not.SameAs (queryResult));
      Assert.That (finalResult.Count, Is.EqualTo (1));
      Assert.That (finalResult.ToArray(), Has.Member (allowedObject));
    }

    [Test]
    public void Test_WithNonSecurableObject ()
    {
      NonSecurableObject nonSecurableObject = _testHelper.CreateNonSecurableObject ();
      IQuery query = QueryFactory.CreateQueryFromConfiguration ("Dummy");
      var queryResult = new QueryResult<DomainObject> (query, new DomainObject[] { nonSecurableObject });
      _testHelper.AddExtension (_extension);
      _testHelper.ReplayAll ();

      var finalResult = _extension.FilterQueryResult (_testHelper.Transaction, queryResult);

      _testHelper.VerifyAll ();
      Assert.That (finalResult, Is.SameAs (queryResult));
      Assert.That (finalResult.Count, Is.EqualTo (1));
      Assert.That (finalResult.ToArray(), Has.Member (nonSecurableObject));
    }

    [Test]
    public void Test_WithinSecurityFreeSection ()
    {
      SecurableObject allowedObject = _testHelper.CreateSecurableObject ();
      SecurableObject deniedObject = _testHelper.CreateSecurableObject ();
      IQuery query = QueryFactory.CreateQueryFromConfiguration ("Dummy");
      var queryResult = new QueryResult<DomainObject> (query, new DomainObject[] { allowedObject, deniedObject });
      _testHelper.AddExtension (_extension);
      _testHelper.ReplayAll ();

      QueryResult<DomainObject> finalResult;
      using (new SecurityFreeSection ())
      {
        finalResult = _extension.FilterQueryResult (_testHelper.Transaction, queryResult);
      }

      _testHelper.VerifyAll ();
      Assert.That (finalResult, Is.SameAs (queryResult));
      Assert.That (queryResult.Count, Is.EqualTo (2));
      Assert.That (queryResult.ToArray(), Has.Member (allowedObject));
      Assert.That (queryResult.ToArray (), Has.Member (deniedObject));
    }

    [Test]
    public void Test_RecursiveSecurity ()
    {
      SecurableObject securableObject = _testHelper.CreateSecurableObject ();
      IQuery query = QueryFactory.CreateQueryFromConfiguration ("Dummy");
      var queryResult = new QueryResult<DomainObject> (query, new DomainObject[] { securableObject });
      _testHelper.AddExtension (_extension);
      HasAccessDelegate hasAccess = delegate
      {
        _testHelper.Transaction.QueryManager.GetCollection (QueryFactory.CreateQueryFromConfiguration ("GetSecurableObjects"));
        return true;
      };
      _testHelper.ExpectObjectSecurityStrategyHasAccess (securableObject, GeneralAccessTypes.Find, hasAccess);
      _testHelper.ReplayAll ();

      var finalResult = _extension.FilterQueryResult (_testHelper.Transaction, queryResult);

      _testHelper.VerifyAll ();

      Assert.That (finalResult, Is.SameAs (queryResult));
    }

    [Test]
    public void Test_AccessedViaDomainObject ()
    {
      _testHelper.AddExtension (_extension);
      _testHelper.ExpectSecurityProviderGetAccess (SecurityContext.CreateStateless(typeof (SecurableObject)), GeneralAccessTypes.Find);
      _testHelper.ReplayAll ();

      _testHelper.Transaction.QueryManager.GetCollection (QueryFactory.CreateQueryFromConfiguration ("GetSecurableObjects"));

      _testHelper.VerifyAll ();
    }

    [Test]
    public void Test_WithSubtransaction_EventIsIgnored ()
    {
      SecurableObject deniedObject = _testHelper.CreateSecurableObject ();
      IQuery query = QueryFactory.CreateQueryFromConfiguration ("Dummy");
      var queryResult = new QueryResult<DomainObject> (query, new DomainObject[] { deniedObject });
      _testHelper.AddExtension (_extension);
      _testHelper.ReplayAll ();

      var subTransaction = _testHelper.Transaction.CreateSubTransaction ();
      var finalResult = _extension.FilterQueryResult (subTransaction, queryResult);
      subTransaction.Discard ();

      _testHelper.VerifyAll ();
      Assert.That (finalResult, Is.SameAs (queryResult));
    }

    [Test]
    public void Test_WithSubtransaction_ViaDomainObjectQuery_EventIsExecutedInRoot ()
    {
      _testHelper.AddExtension (_extension);
      _testHelper.ExpectSecurityProviderGetAccess (SecurityContext.CreateStateless (typeof (SecurableObject)), GeneralAccessTypes.Find);
      _testHelper.ReplayAll ();

      _testHelper.Transaction.QueryManager.GetCollection (QueryFactory.CreateQueryFromConfiguration ("GetSecurableObjects"));

      _testHelper.VerifyAll ();
    }

    [Test]
    public void Test_WithActiveTransactionMatchingTransactionPassedAsArgument_DoesNotCreateScope ()
    {
      SecurableObject securableObject = _testHelper.CreateSecurableObject();
      IQuery query = QueryFactory.CreateQueryFromConfiguration ("Dummy");
      var queryResult = new QueryResult<DomainObject> (query, new DomainObject[] { securableObject });

      using (var scope = _testHelper.Transaction.EnterNonDiscardingScope())
      {
        _testHelper.AddExtension (_extension);
        _testHelper.ExpectObjectSecurityStrategyHasAccessWithMatchingScope (securableObject, scope);
        _testHelper.ReplayAll();

        _extension.FilterQueryResult (_testHelper.Transaction, queryResult);
      }

      _testHelper.VerifyAll();
    }

    [Test]
    public void Test_WithActiveTransactionNotMatchingTransactionPassedAsArgument_CreatesScope ()
    {
      SecurableObject securableObject = _testHelper.CreateSecurableObject();
      IQuery query = QueryFactory.CreateQueryFromConfiguration ("Dummy");
      var queryResult = new QueryResult<DomainObject> (query, new DomainObject[] { securableObject });
      _testHelper.AddExtension (_extension);
      _testHelper.ExpectObjectSecurityStrategyHasAccess (securableObject, GeneralAccessTypes.Find, true);
      _testHelper.ReplayAll();

      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        _extension.FilterQueryResult (_testHelper.Transaction, queryResult);
      }

      _testHelper.VerifyAll();
    }

    [Test]
    public void Test_WithInactiveTransaction_CreatesScope ()
    {
      SecurableObject securableObject = _testHelper.CreateSecurableObject();
      IQuery query = QueryFactory.CreateQueryFromConfiguration ("Dummy");
      var queryResult = new QueryResult<DomainObject> (query, new DomainObject[] { securableObject });
      _testHelper.AddExtension (_extension);
      _testHelper.ExpectObjectSecurityStrategyHasAccess (securableObject, GeneralAccessTypes.Find, true);
      _testHelper.ReplayAll();

      using (_testHelper.Transaction.EnterNonDiscardingScope())
      {
        using (_testHelper.MakeInactive())
        {
          _extension.FilterQueryResult (_testHelper.Transaction, queryResult);

        }
      }
      _testHelper.VerifyAll();
    }
  }
}

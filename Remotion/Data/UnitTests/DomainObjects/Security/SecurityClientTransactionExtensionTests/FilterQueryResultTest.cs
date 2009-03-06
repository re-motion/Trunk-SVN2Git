// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
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
using NUnit.Framework.SyntaxHelpers;
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
    private SecurityClientTransactionExtensionTestHelper _testHelper;
    private IClientTransactionExtension _extension;
    private IDisposable _transactionScope;

    [SetUp]
    public void SetUp ()
    {
      _testHelper = new SecurityClientTransactionExtensionTestHelper ();
      _extension = new SecurityClientTransactionExtension ();

      _testHelper.SetupSecurityConfiguration ();
      _transactionScope = _testHelper.Transaction.EnterDiscardingScope ();
    }

    [TearDown]
    public void TearDown ()
    {
      _testHelper.TearDownSecurityConfiguration ();
      _transactionScope.Dispose ();
    }

    [Test]
    public void Test_WithNullObject ()
    {
      IQuery query = QueryFactory.CreateQueryFromConfiguration ("Dummy");
      var queryResult = new QueryResult<DomainObject> (query, new DomainObject[] { null });
      _testHelper.AddExtension (_extension);
      _testHelper.ReplayAll ();

      var finalResult = _extension.FilterQueryResult (ClientTransaction.CreateRootTransaction (), queryResult);

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

      var finalResult = _extension.FilterQueryResult (ClientTransaction.CreateRootTransaction (), queryResult);

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

      var finalResult = _extension.FilterQueryResult (ClientTransaction.CreateRootTransaction (), queryResult);

      _testHelper.VerifyAll ();
      Assert.That (finalResult, Is.SameAs (queryResult));
      Assert.AreEqual (0, finalResult.Count);
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

      var finalResult = _extension.FilterQueryResult (ClientTransaction.CreateRootTransaction (), queryResult);

      _testHelper.VerifyAll ();
      Assert.That (finalResult, Is.Not.SameAs (queryResult));
      Assert.AreEqual (0, finalResult.Count);
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

      var finalResult = _extension.FilterQueryResult (ClientTransaction.CreateRootTransaction (), queryResult);

      _testHelper.VerifyAll ();
      Assert.That (finalResult, Is.Not.SameAs (queryResult));
      Assert.AreEqual (1, finalResult.Count);
      Assert.Contains (allowedObject, finalResult.ToArray());
    }

    [Test]
    public void Test_WithNonSecurableObject ()
    {
      NonSecurableObject nonSecurableObject = _testHelper.CreateNonSecurableObject ();
      IQuery query = QueryFactory.CreateQueryFromConfiguration ("Dummy");
      var queryResult = new QueryResult<DomainObject> (query, new DomainObject[] { nonSecurableObject });
      _testHelper.AddExtension (_extension);
      _testHelper.ReplayAll ();

      var finalResult = _extension.FilterQueryResult (ClientTransaction.CreateRootTransaction (), queryResult);

      _testHelper.VerifyAll ();
      Assert.That (finalResult, Is.SameAs (queryResult));
      Assert.AreEqual (1, finalResult.Count);
      Assert.Contains (nonSecurableObject, finalResult.ToArray());
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
        finalResult = _extension.FilterQueryResult (ClientTransaction.CreateRootTransaction (), queryResult);
      }

      _testHelper.VerifyAll ();
      Assert.That (finalResult, Is.SameAs (queryResult));
      Assert.AreEqual (2, queryResult.Count);
      Assert.Contains (allowedObject, queryResult.ToArray());
      Assert.Contains (deniedObject, queryResult.ToArray ());
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

      var finalResult = _extension.FilterQueryResult (ClientTransaction.CreateRootTransaction (), queryResult);

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
  }
}

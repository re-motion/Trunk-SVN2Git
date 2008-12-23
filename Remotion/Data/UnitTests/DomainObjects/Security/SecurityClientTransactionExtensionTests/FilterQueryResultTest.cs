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
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Infrastructure;
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
    public void Test_WithOneAllowedObject ()
    {
      SecurableObject allowedObject = _testHelper.CreateSecurableObject ();
      DomainObjectCollection collection = new DomainObjectCollection ();
      collection.Add (allowedObject);
      IQuery query = QueryFactory.CreateQueryFromConfiguration ("Dummy");
      _testHelper.AddExtension (_extension);
      _testHelper.ExpectObjectSecurityStrategyHasAccess (allowedObject, GeneralAccessTypes.Find, true);
      _testHelper.ReplayAll ();

      _extension.FilterQueryResult (ClientTransaction.CreateRootTransaction (), collection, query);

      _testHelper.VerifyAll ();
      Assert.AreEqual (1, collection.Count);
      Assert.Contains (allowedObject, collection);
    }

    [Test]
    public void Test_WithNoObjects ()
    {
      _extension = new SecurityClientTransactionExtension ();
      DomainObjectCollection collection = new DomainObjectCollection ();
      IQuery query = QueryFactory.CreateQueryFromConfiguration ("Dummy");
      _testHelper.AddExtension (_extension);
      _testHelper.ReplayAll ();

      _extension.FilterQueryResult (ClientTransaction.CreateRootTransaction (), collection, query);

      _testHelper.VerifyAll ();
      Assert.AreEqual (0, collection.Count);
    }

    [Test]
    public void Test_WithOneDeniedObject ()
    {
      SecurableObject deniedObject = _testHelper.CreateSecurableObject ();
      DomainObjectCollection collection = new DomainObjectCollection ();
      collection.Add (deniedObject);
      IQuery query = QueryFactory.CreateQueryFromConfiguration ("Dummy");
      _testHelper.AddExtension (_extension);
      _testHelper.ExpectObjectSecurityStrategyHasAccess (deniedObject, GeneralAccessTypes.Find, false);
      _testHelper.ReplayAll ();

      _extension.FilterQueryResult (ClientTransaction.CreateRootTransaction (), collection, query);

      _testHelper.VerifyAll ();
      Assert.AreEqual (0, collection.Count);
    }

    [Test]
    public void Test_WithOneAllowedAndOneDeniedObject ()
    {
      SecurableObject allowedObject = _testHelper.CreateSecurableObject ();
      SecurableObject deniedObject = _testHelper.CreateSecurableObject ();
      DomainObjectCollection collection = new DomainObjectCollection ();
      collection.Add (allowedObject);
      collection.Add (deniedObject);
      IQuery query = QueryFactory.CreateQueryFromConfiguration ("Dummy");
      _testHelper.AddExtension (_extension);
      _testHelper.ExpectObjectSecurityStrategyHasAccess (allowedObject, GeneralAccessTypes.Find, true);
      _testHelper.ExpectObjectSecurityStrategyHasAccess (deniedObject, GeneralAccessTypes.Find, false);
      _testHelper.ReplayAll ();

      _extension.FilterQueryResult (ClientTransaction.CreateRootTransaction (), collection, query);

      _testHelper.VerifyAll ();
      Assert.AreEqual (1, collection.Count);
      Assert.Contains (allowedObject, collection);
    }

    [Test]
    public void Test_WithNonSecurableObject ()
    {
      NonSecurableObject nonSecurableObject = _testHelper.CreateNonSecurableObject ();
      DomainObjectCollection collection = new DomainObjectCollection ();
      collection.Add (nonSecurableObject);
      IQuery query = QueryFactory.CreateQueryFromConfiguration ("Dummy");
      _testHelper.AddExtension (_extension);
      _testHelper.ReplayAll ();

      _extension.FilterQueryResult (ClientTransaction.CreateRootTransaction (), collection, query);

      _testHelper.VerifyAll ();
      Assert.AreEqual (1, collection.Count);
      Assert.Contains (nonSecurableObject, collection);
    }

    [Test]
    public void Test_WithinSecurityFreeSection ()
    {
      SecurableObject allowedObject = _testHelper.CreateSecurableObject ();
      SecurableObject deniedObject = _testHelper.CreateSecurableObject ();
      DomainObjectCollection collection = new DomainObjectCollection ();
      collection.Add (allowedObject);
      collection.Add (deniedObject);
      IQuery query = QueryFactory.CreateQueryFromConfiguration ("Dummy");
      _testHelper.AddExtension (_extension);
      _testHelper.ReplayAll ();

      using (new SecurityFreeSection ())
      {
        _extension.FilterQueryResult (ClientTransaction.CreateRootTransaction (), collection, query);
      }

      _testHelper.VerifyAll ();
      Assert.AreEqual (2, collection.Count);
      Assert.Contains (allowedObject, collection);
      Assert.Contains (deniedObject, collection);
    }

    [Test]
    public void Test_RecursiveSecurity ()
    {
      SecurableObject securableObject = _testHelper.CreateSecurableObject ();
      DomainObjectCollection collection = new DomainObjectCollection ();
      collection.Add (securableObject);
      IQuery query = QueryFactory.CreateQueryFromConfiguration ("Dummy");
      _testHelper.AddExtension (_extension);
      HasAccessDelegate hasAccess = delegate
      {
        _testHelper.Transaction.QueryManager.GetCollection (QueryFactory.CreateQueryFromConfiguration ("GetSecurableObjects"));
        return true;
      };
      _testHelper.ExpectObjectSecurityStrategyHasAccess (securableObject, GeneralAccessTypes.Find, hasAccess);
      _testHelper.ReplayAll ();

      _extension.FilterQueryResult (ClientTransaction.CreateRootTransaction (), collection, query);

      _testHelper.VerifyAll ();
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

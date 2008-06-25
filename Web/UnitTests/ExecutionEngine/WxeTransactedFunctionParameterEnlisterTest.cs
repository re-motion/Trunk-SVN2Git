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
using Remotion.Collections;
using Remotion.Data;
using Remotion.Web.ExecutionEngine;
using Rhino.Mocks;

namespace Remotion.Web.UnitTests.ExecutionEngine
{
  using Enlister = WxeTransactedFunctionParameterEnlister<ITransaction, ITransactionScope<ITransaction>, 
    ITransactionScopeManager<ITransaction, ITransactionScope<ITransaction>>>;
  using NUnit.Framework.SyntaxHelpers;

  [TestFixture]
  public class WxeTransactedFunctionParameterEnlisterTest
  {
    private MockRepository _mockRepository;
    private ITransaction _transactionMock;
    private ITransactionScopeManager<ITransaction, ITransactionScope<ITransaction>> _scopeManagerMock;
    private ITransactionScope<ITransaction> _scopeMock;
    private WxeParameterDeclaration _parameter1;
    private WxeParameterDeclaration _parameter2;
    private object _objectToEnlist1a;
    private object _objectToEnlist1b;
    private object _objectToEnlist2;

    private Enlister _enlister;

    [SetUp]
    public void SetUp ()
    {
      _mockRepository = new MockRepository ();
      _transactionMock = _mockRepository.CreateMock<ITransaction> ();
      _scopeManagerMock = _mockRepository.CreateMock<ITransactionScopeManager<ITransaction, ITransactionScope<ITransaction>>> ();
      _scopeMock = _mockRepository.CreateMock<ITransactionScope<ITransaction>> ();
      _enlister = new Enlister (_transactionMock, _scopeManagerMock);
      _parameter1 = new WxeParameterDeclaration ("one", false, WxeParameterDirection.In, typeof (object));
      _parameter2 = new WxeParameterDeclaration ("two", false, WxeParameterDirection.In, typeof (object));
      _objectToEnlist1a = new object ();
      _objectToEnlist1b = new object ();
      _objectToEnlist2 = new object ();
    }

    [Test]
    public void TryEnlistAsSingleObject_True ()
    {
      Expect.Call (_scopeManagerMock.TryEnlistObject (_transactionMock, _objectToEnlist1a)).Return (true);
      _mockRepository.ReplayAll ();
      Assert.That (_enlister.TryEnlistAsSingleObject (_parameter1, _objectToEnlist1a), Is.True);
      _mockRepository.VerifyAll ();

      Assert.That (_enlister.EnlistedObjects, List.Contains (new Tuple<WxeParameterDeclaration, object> (_parameter1, _objectToEnlist1a)));
    }

    [Test]
    public void TryEnlistAsSingleObject_False ()
    {
      Expect.Call (_scopeManagerMock.TryEnlistObject (_transactionMock, _objectToEnlist1a)).Return (false);
      _mockRepository.ReplayAll ();
      Assert.That (_enlister.TryEnlistAsSingleObject (_parameter1, _objectToEnlist1a), Is.False);
      _mockRepository.VerifyAll ();

      Assert.That (_enlister.EnlistedObjects, Is.Empty);
    }

    [Test]
    public void TryEnlistAsEnumerable_True ()
    {
      Expect.Call (_scopeManagerMock.TryEnlistObject (_transactionMock, _objectToEnlist1a)).Return (true);
      Expect.Call (_scopeManagerMock.TryEnlistObject (_transactionMock, _objectToEnlist1b)).Return (true);

      _mockRepository.ReplayAll ();
      Assert.That (_enlister.TryEnlistAsEnumerable (_parameter1, new object[] { _objectToEnlist1a, _objectToEnlist1b }), Is.True);
      _mockRepository.VerifyAll ();

      Assert.That (_enlister.EnlistedObjects, List.Contains (new Tuple<WxeParameterDeclaration, object> (_parameter1, _objectToEnlist1a)));
      Assert.That (_enlister.EnlistedObjects, List.Contains (new Tuple<WxeParameterDeclaration, object> (_parameter1, _objectToEnlist1b)));
    }

    [Test]
    public void TryEnlistAsEnumerable_False ()
    {
      _mockRepository.ReplayAll ();
      Assert.That (_enlister.TryEnlistAsEnumerable (_parameter1, _objectToEnlist1a), Is.False);
      _mockRepository.VerifyAll ();

      Assert.That (_enlister.EnlistedObjects, Is.Empty);
    }

    [Test]
    public void EnlistParameter_SingleObject ()
    {
      Expect.Call (_scopeManagerMock.TryEnlistObject (_transactionMock, _objectToEnlist1a)).Return (true);
      
      _mockRepository.ReplayAll ();
      _enlister.EnlistParameter (_parameter1, _objectToEnlist1a);
      _mockRepository.VerifyAll ();

      Assert.That (_enlister.EnlistedObjects, List.Contains (new Tuple<WxeParameterDeclaration, object> (_parameter1, _objectToEnlist1a)));
    }

    [Test]
    public void EnlistParameter_Enumerable ()
    {
      object[] objects = new object[] { _objectToEnlist1a, _objectToEnlist1b };
      Expect.Call (_scopeManagerMock.TryEnlistObject (_transactionMock, objects)).Return (false);
      Expect.Call (_scopeManagerMock.TryEnlistObject (_transactionMock, objects[0])).Return (true);
      Expect.Call (_scopeManagerMock.TryEnlistObject (_transactionMock, objects[1])).Return (true);

      _mockRepository.ReplayAll ();
      _enlister.EnlistParameter (_parameter1, objects);
      _mockRepository.VerifyAll ();

      Assert.That (_enlister.EnlistedObjects, List.Contains (new Tuple<WxeParameterDeclaration, object> (_parameter1, _objectToEnlist1a)));
      Assert.That (_enlister.EnlistedObjects, List.Contains (new Tuple<WxeParameterDeclaration, object> (_parameter1, _objectToEnlist1b)));
    }

		[Test]
		public void EnlistParameter_WithNullValue ()
		{
			// no calls to scope
			_mockRepository.ReplayAll ();
			_enlister.EnlistParameter (_parameter1, null);
			_mockRepository.VerifyAll ();
		}

    [Test]
    public void EnlistParameter_NotEnlistable ()
    {
      Expect.Call (_scopeManagerMock.TryEnlistObject (_transactionMock, _objectToEnlist1a)).Return (false);

      _mockRepository.ReplayAll ();
      _enlister.EnlistParameter (_parameter1, _objectToEnlist1a);
      _mockRepository.VerifyAll ();

      Assert.That (_enlister.EnlistedObjects, Is.Empty);
    }

    [Test]
    public void EnlistParameters ()
    {
      NameObjectCollection values = new NameObjectCollection ();
      object[] objectArray = new object[] { _objectToEnlist1a, _objectToEnlist1b };
      values.Add (_parameter1.Name, objectArray);
      values.Add (_parameter2.Name, _objectToEnlist2);
      values.Add ("undeclaredValue", new object());

      Expect.Call (_scopeManagerMock.TryEnlistObject (_transactionMock, objectArray)).Return (false);
      Expect.Call (_scopeManagerMock.TryEnlistObject (_transactionMock, _objectToEnlist1a)).Return (true);
      Expect.Call (_scopeManagerMock.TryEnlistObject (_transactionMock, _objectToEnlist1b)).Return (true);
      Expect.Call (_scopeManagerMock.TryEnlistObject (_transactionMock, _objectToEnlist2)).Return (true);

      _mockRepository.ReplayAll ();
      _enlister.EnlistParameters (new WxeParameterDeclaration[] { _parameter1, _parameter2 }, values);
      _mockRepository.VerifyAll ();

      Assert.That (_enlister.EnlistedObjects, List.Contains (new Tuple<WxeParameterDeclaration, object> (_parameter1, _objectToEnlist1a)));
      Assert.That (_enlister.EnlistedObjects, List.Contains (new Tuple<WxeParameterDeclaration, object> (_parameter1, _objectToEnlist1b)));
      Assert.That (_enlister.EnlistedObjects, List.Contains (new Tuple<WxeParameterDeclaration, object> (_parameter2, _objectToEnlist2)));
    }

    [Test]
    public void LoadAllEnlistedObjects_Success ()
    {
      NameObjectCollection values = new NameObjectCollection ();
      values.Add (_parameter1.Name, _objectToEnlist1a);
      values.Add (_parameter2.Name, _objectToEnlist2);
      values.Add ("undeclaredValue", new object ());

      Expect.Call (_scopeManagerMock.TryEnlistObject (_transactionMock, _objectToEnlist1a)).Return (true);
      Expect.Call (_scopeManagerMock.TryEnlistObject (_transactionMock, _objectToEnlist2)).Return (true);

      Expect.Call (_scopeManagerMock.EnterScope (_transactionMock)).Return (_scopeMock);
      _scopeManagerMock.EnsureEnlistedObjectIsLoaded (_transactionMock, _objectToEnlist1a); // expectation
      _scopeManagerMock.EnsureEnlistedObjectIsLoaded (_transactionMock, _objectToEnlist2); // expectation
      _scopeMock.Leave(); // expectation

      _mockRepository.ReplayAll ();
      _enlister.EnlistParameters (new WxeParameterDeclaration[] { _parameter1, _parameter2 }, values);
      _enlister.LoadAllEnlistedObjects();
      _mockRepository.VerifyAll ();
    }

    [Test]
    public void LoadAllEnlistedObjects_Failure ()
    {
      NameObjectCollection values = new NameObjectCollection ();
      values.Add (_parameter1.Name, _objectToEnlist1a);

      Expect.Call (_scopeManagerMock.TryEnlistObject (_transactionMock, _objectToEnlist1a)).Return (true);

      Expect.Call (_scopeManagerMock.EnterScope (_transactionMock)).Return (_scopeMock);
      _scopeManagerMock.EnsureEnlistedObjectIsLoaded (_transactionMock, _objectToEnlist1a); // expectation
      Exception exception = new Exception ("Test");
      LastCall.Throw (exception);
      _scopeMock.Leave (); // expectation

      _mockRepository.ReplayAll ();
      _enlister.EnlistParameters (new WxeParameterDeclaration[] { _parameter1 }, values);
      try
      {
        _enlister.LoadAllEnlistedObjects ();
        Assert.Fail ("Expected ArgumentException");
      }
      catch (ArgumentException ex)
      {
        Assert.That (ex.InnerException, Is.SameAs (exception));
        Assert.That (ex.Message, Is.EqualTo ("The object 'System.Object' cannot be enlisted in the function's transaction. Maybe it was newly created "
            + "and has not yet been committed, or it was deleted.\r\nParameter name: one"));
      }
      _mockRepository.VerifyAll ();
    }
  }
}

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
using System.Reflection;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Infrastructure;
using System.Linq;
using Remotion.Data.DomainObjects.Infrastructure.HierarchyManagement;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Infrastructure.HierarchyManagement
{
  [TestFixture]
  public class InactiveClientTransactionListenerTest
  {
    private InactiveClientTransactionListener _listener;
    private TestableClientTransaction _clientTransaction;

    private readonly string[] _neverThrowingMethodNames =
        {
            "TransactionInitialize",
            "TransactionDiscard",
            "ObjectsLoading",
            "ObjectsLoaded",
            "ObjectsNotFound",
            "PropertyValueReading",
            "PropertyValueRead",
            "RelationReading",
            "RelationRead",
            "RelationRead",
            "FilterQueryResult",
            "FilterCustomQueryResult",
            "ObjectsUnloading",
            "ObjectsUnloaded",
            "RelationEndPointMapRegistering",
            "RelationEndPointMapUnregistering",
            "RelationEndPointBecomingIncomplete",
            "DataContainerMapRegistering",
            "DataContainerMapUnregistering",
        };

    private readonly string[] _methodsExpectingInactivenessNames = {
          "SubTransactionCreated", 
          "SubTransactionInitialize"
    };

    private MethodInfo[] _allMethods;
    private MethodInfo[] _neverThrowingMethods;
    private MethodInfo[] _throwingMethods;
    private MethodInfo[] _methodsAssertingInactiveness;
    private MethodInfo[] _methodsNotAssertingReadonlyness;

    [SetUp]
    public void SetUp ()
    {
      _listener = new InactiveClientTransactionListener();
      _clientTransaction = new TestableClientTransaction();

      _allMethods = typeof (InactiveClientTransactionListener).GetMethods (BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance);
      _neverThrowingMethods = _allMethods.Where (n => _neverThrowingMethodNames.Contains (n.Name)).ToArray();
      _throwingMethods = _allMethods
          .Where (n => !_neverThrowingMethodNames.Contains (n.Name))
          .Where (n => !_methodsExpectingInactivenessNames.Contains (n.Name)).ToArray ();
      _methodsAssertingInactiveness = _allMethods.Where (n => _methodsExpectingInactivenessNames.Contains (n.Name)).ToArray ();
      _methodsNotAssertingReadonlyness = _allMethods.Except (_methodsAssertingInactiveness).ToArray ();
    }

    [Test]
    public void ClientTransactionInactive_ThrowingMethods ()
    {
      ClientTransactionTestHelper.SetIsActive (_clientTransaction, false);

      Assert.That (_throwingMethods, Has.Length.EqualTo (17));
      
      foreach (var method in _throwingMethods)
      {
        object[] arguments = Array.ConvertAll (method.GetParameters (), p => GetDefaultValue (p.ParameterType));

        ExpectException (method, arguments);
      }
    }

    [Test]
    public void ClientTransactionInactive_MethodsExpectingInactive ()
    {
      ClientTransactionTestHelper.SetIsActive (_clientTransaction, true);

      Assert.That (_methodsAssertingInactiveness, Has.Length.EqualTo (2));

      foreach (var method in _methodsAssertingInactiveness)
      {
        object[] arguments = Array.ConvertAll (method.GetParameters (), p => GetDefaultValue (p.ParameterType));

        ExpectAssertException (method, arguments);
      }
    }

    [Test]
    public void ClientTransactionInactive_NotThrowingMethods ()
    {
      ClientTransactionTestHelper.SetIsActive (_clientTransaction, true);

      Assert.That (_neverThrowingMethods, Has.Length.EqualTo (19));

      foreach (var method in _neverThrowingMethods)
      {
        var concreteMethod = GetCallableMethod (method);
        object[] arguments = Array.ConvertAll (concreteMethod.GetParameters (), p => GetDefaultValue (p.ParameterType));

        ExpectNoException (concreteMethod, arguments);
      }
    }

    [Test]
    public void ClientTransactionNotInactive_NoMethodThrows ()
    {
      ClientTransactionTestHelper.SetIsActive (_clientTransaction, true);

      Assert.That (_methodsNotAssertingReadonlyness, Has.Length.EqualTo (36));

      foreach (var method in _methodsNotAssertingReadonlyness)
      {
        var concreteMethod = GetCallableMethod (method);
        object[] arguments = Array.ConvertAll (concreteMethod.GetParameters (), p => GetDefaultValue (p.ParameterType));

        ExpectNoException (concreteMethod, arguments);
      }
    }
    
    private void ExpectException (MethodInfo method, object[] arguments)
    {
      string message = string.Format (
            "The operation cannot be executed because the ClientTransaction is inactive. "
            + "Offending transaction modification: {0}.",
            method.Name);

      Assert.That (
        () => method.Invoke (_listener, arguments),
        Throws.TargetInvocationException.With.InnerException.TypeOf<ClientTransactionInactiveException> ().And.InnerException.Message.EqualTo (message),
        "Expected exception to be thrown by method '{0}'.",
        method.Name);
    }

    private void ExpectNoException (MethodInfo method, object[] arguments)
    {
      Assert.That (
        () => method.Invoke (_listener, arguments),
        Throws.Nothing,
        "Expected no exception to be thrown by method '{0}'.",
        method.Name);
    }

    private void ExpectAssertException (MethodInfo method, object[] arguments)
    {
      string message = "Assertion failed: Expression evaluates to true.";

      Assert.That (
        () => method.Invoke (_listener, arguments),
        Throws.TargetInvocationException.With.InnerException.TypeOf<Remotion.Utilities.AssertionException> ()
            .And.InnerException.Message.EqualTo (message),
        "Expected AssertionException to be thrown by method '{0}'.",
        method.Name);
    }

    private object GetDefaultValue (Type t)
    {
      if (t.IsValueType)
        return Activator.CreateInstance (t);
      else if (t == typeof (ClientTransaction))
        return _clientTransaction;
      else
        return null;
    }

    private MethodInfo GetCallableMethod (MethodInfo method)
    {
      return method.Name == "FilterQueryResult" || method.Name == "FilterCustomQueryResult"
                 ? method.MakeGenericMethod (typeof (Order))
                 : method;
    }
  }
}
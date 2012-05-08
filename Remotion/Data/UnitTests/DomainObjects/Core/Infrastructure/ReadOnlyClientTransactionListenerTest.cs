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
using Remotion.Data.UnitTests.DomainObjects.TestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Infrastructure
{
  [TestFixture]
  public class ReadOnlyClientTransactionListenerTest
  {
    private ReadOnlyClientTransactionListener _listener;
    private TestableClientTransaction _clientTransaction;

    private readonly string[] _neverThrowingMethodNames = {
        "TransactionInitialize",
        "TransactionDiscard",
        "DataContainerStateUpdated",
        "VirtualRelationEndPointStateUpdated",
        "PropertyValueReading",
        "PropertyValueRead",
        "RelationReading", 
        "RelationRead", 
        "ObjectMarkedInvalid", 
        "ObjectMarkedNotInvalid", 
        "DataContainerStateUpdated", 
        "VirtualRelationEndPointStateUpdated",
        "FilterQueryResult",
        "FilterCustomQueryResult"};

    private readonly string[] _methodsExpectingReadOnlynessNames = {
          "SubTransactionCreated", 
          "SubTransactionInitialize"
    };

    private MethodInfo[] _allMethods;
    private MethodInfo[] _neverThrowingMethods;
    private MethodInfo[] _throwingMethods;
    private MethodInfo[] _methodsAssertingReadOnlyness;
    private MethodInfo[] _methodsNotExpectingReadonlyness;

    [SetUp]
    public void SetUp ()
    {
      _listener = new ReadOnlyClientTransactionListener();
      _clientTransaction = new TestableClientTransaction();

      _allMethods = typeof (ReadOnlyClientTransactionListener).GetMethods (BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance);
      _neverThrowingMethods = _allMethods.Where (n => _neverThrowingMethodNames.Contains (n.Name)).ToArray();
      _throwingMethods = _allMethods
          .Where (n => !_neverThrowingMethodNames.Contains (n.Name))
          .Where (n => !_methodsExpectingReadOnlynessNames.Contains (n.Name)).ToArray ();
      _methodsAssertingReadOnlyness = _allMethods.Where (n => _methodsExpectingReadOnlynessNames.Contains (n.Name)).ToArray ();
      _methodsNotExpectingReadonlyness = _allMethods.Except (_methodsAssertingReadOnlyness).ToArray ();
    }

    [Test]
    public void ClientTransactionReadOnly_ThrowingMethods ()
    {
      _clientTransaction.IsReadOnly = true;

      Assert.That (_throwingMethods, Has.Length.EqualTo (22));
      
      foreach (var method in _throwingMethods)
      {
        object[] arguments = Array.ConvertAll (method.GetParameters (), p => GetDefaultValue (p.ParameterType));

        ExpectException (method, arguments);
      }
    }

    [Test]
    public void ClientTransactionReadOnly_MethodsExpectingReadOnly ()
    {
      _clientTransaction.IsReadOnly = false;

      Assert.That (_methodsAssertingReadOnlyness, Has.Length.EqualTo (2));

      foreach (var method in _methodsAssertingReadOnlyness)
      {
        object[] arguments = Array.ConvertAll (method.GetParameters (), p => GetDefaultValue (p.ParameterType));

        ExpectAssertException (method, arguments);
      }
    }

    [Test]
    public void ClientTransactionReadOnly_NotThrowingMethods ()
    {
      _clientTransaction.IsReadOnly = true;

      Assert.That (_neverThrowingMethods, Has.Length.EqualTo (13));

      foreach (var method in _neverThrowingMethods)
      {
        var concreteMethod = GetCallableMethod (method);
        object[] arguments = Array.ConvertAll (concreteMethod.GetParameters (), p => GetDefaultValue (p.ParameterType));

        ExpectNoException (concreteMethod, arguments);
      }
    }

    [Test]
    public void ClientTransactionNotReadOnly_NoMethodThrows ()
    {
      _clientTransaction.IsReadOnly = false;

      Assert.That (_methodsNotExpectingReadonlyness, Has.Length.EqualTo (35));

      foreach (var method in _methodsNotExpectingReadonlyness)
      {
        var concreteMethod = GetCallableMethod (method);
        object[] arguments = Array.ConvertAll (concreteMethod.GetParameters (), p => GetDefaultValue (p.ParameterType));

        ExpectNoException (concreteMethod, arguments);
      }
    }
    
    private void ExpectException (MethodInfo method, object[] arguments)
    {
      string message = string.Format (
            "The operation cannot be executed because the ClientTransaction is read-only. "
            + "Offending transaction modification: {0}.",
            method.Name);

      Assert.That (
        () => method.Invoke (_listener, arguments),
        Throws.TargetInvocationException.With.InnerException.TypeOf<ClientTransactionReadOnlyException> ().And.InnerException.Message.EqualTo (message),
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
      string message = "Assertion failed: Expression evaluates to false.";

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
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
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Infrastructure;
using System.Linq;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Infrastructure
{
  [TestFixture]
  public class ReadOnlyClientTransactionListenerTest
  {
    private ReadOnlyClientTransactionListener _listener;
    private TestableClientTransaction _clientTransaction;

    private readonly string[] _neverThrowingMethods = {
        "TransactionInitialize",
        "TransactionDiscard",
        "DataContainerStateUpdated",
        "VirtualRelationEndPointStateUpdated",
        "RelationReading", 
        "RelationRead", 
        "ObjectMarkedInvalid", 
        "ObjectMarkedNotInvalid", 
        "DataContainerStateUpdated", 
        "VirtualRelationEndPointStateUpdated",
        "FilterQueryResult"};

    private readonly string[] _methodsExpectingReadOnly = {
          "SubTransactionCreated", 
          "SubTransactionInitialize"
    };

    [SetUp]
    public void SetUp ()
    {
      _listener = new ReadOnlyClientTransactionListener();
      _clientTransaction = new TestableClientTransaction();
    }

    [Test]
    public void ClientTransactionReadOnly_ThrowingMethods ()
    {
      _clientTransaction.IsReadOnly = true;
      MethodInfo[] methods =
          typeof (ReadOnlyClientTransactionListener).GetMethods (BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance);
      var result = methods
        .Where (n => !_neverThrowingMethods.Contains (n.Name))
        .Where (n => !_methodsExpectingReadOnly.Contains (n.Name)).ToList();

      Assert.That (result.Count (), Is.EqualTo (25));
      
      foreach (var method in result)
      {
        object[] arguments = Array.ConvertAll (method.GetParameters (), p => GetDefaultValue (p.ParameterType));

        ExpectException (method, arguments);
      }
    }

    [Test]
    public void ClientTransactionReadOnly_MethodsExpectingReadOnly ()
    {
      MethodInfo[] methods =
          typeof (ReadOnlyClientTransactionListener).GetMethods (BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance);
      IEnumerable<MethodInfo> result = methods.Where (n => _methodsExpectingReadOnly.Contains (n.Name));

      Assert.That (result.Count (), Is.EqualTo (2));

      foreach (var method in result)
      {
        object[] arguments = Array.ConvertAll (method.GetParameters (), p => GetDefaultValue (p.ParameterType));

        ExpectAssertException (method, arguments);
      }
    }

    [Test]
    public void ClientTransactionReadOnly_NotThrowingMethods ()
    {
      MethodInfo[] methods =
          typeof (ReadOnlyClientTransactionListener).GetMethods (BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance);
      IEnumerable<MethodInfo> result = methods.Where (n => _neverThrowingMethods.Contains (n.Name));
      
      Assert.That (result.Count(), Is.EqualTo (10));
    }

    [Test]
    public void ClientTransactionNotReadOnly_NoMethodThrows ()
    {
      _clientTransaction.IsReadOnly = false;
      MethodInfo[] methods =
          typeof (ReadOnlyClientTransactionListener).GetMethods (BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance);
      IEnumerable<MethodInfo> result = methods
        .Where (n => !_neverThrowingMethods.Contains (n.Name))
        .Where (n => !_methodsExpectingReadOnly.Contains (n.Name));
      
      Assert.That (result.Count (), Is.EqualTo (25));

      foreach (var method in result)
      {
        object[] arguments = Array.ConvertAll (method.GetParameters (), p => GetDefaultValue (p.ParameterType));

        ExpectNoException (method, arguments);
      }
    }
    
    private void ExpectException (MethodInfo method, object[] arguments)
    {
      string message = string.Format (
            "The operation cannot be executed because the ClientTransaction is read-only. "
            + "Offending transaction modification: {0}.",
            method.Name);

      try
      {
        if (method.ContainsGenericParameters)
          method = method.MakeGenericMethod (typeof (object));

        method.Invoke (_listener, arguments);
      }
      catch (Exception ex)
      {
        Assert.That (ex.InnerException.Message, Is.EqualTo (message));
        return;
      }
    }

    private void ExpectNoException (MethodInfo method, object[] arguments)
    {
      if (method.ContainsGenericParameters)
        method = method.MakeGenericMethod (typeof (object));
      method.Invoke (_listener, arguments);
    }

    private void ExpectAssertException (MethodInfo method, object[] arguments)
    {
      string message = "Assertion failed: Expression evaluates to false.";

      try
      {
        method.Invoke (_listener, arguments);
      }
      catch (Exception ex)
      {
        Assert.That (ex.InnerException.Message, Is.EqualTo (message));
        return;
      }
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
  }
}
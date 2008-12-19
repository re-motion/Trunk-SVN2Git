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
using System.Reflection;
using Castle.DynamicProxy;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Collections;
using Remotion.Development.UnitTesting;
using Remotion.Utilities;

namespace Remotion.UnitTests.Collections
{
  [TestFixture]
  public class InterlockedDataStoreTest
  {
    private IDataStore<string, int> _innerStore;
    private MonitorCheckingInterceptor _innerStoreInterceptor;
    private InterlockedDataStore<string, int> _store;

    [SetUp]
    public void SetUp ()
    {
      ProxyGenerator generator = new ProxyGenerator();
      _innerStoreInterceptor = new MonitorCheckingInterceptor ();
      _innerStore = generator.CreateInterfaceProxyWithoutTarget<IDataStore<string, int>> (_innerStoreInterceptor);
      _store = new InterlockedDataStore<string, int> (_innerStore);
      _innerStoreInterceptor.Monitor = PrivateInvoke.GetNonPublicField (_store, "_lock");
    }

    [Test]
    public void DefaultConstructor ()
    {
      InterlockedDataStore<string, int> store = new InterlockedDataStore<string, int> ();
      Assert.IsInstanceOfType (typeof (SimpleDataStore<string, int>), PrivateInvoke.GetNonPublicField (store, "_innerStore"));
    }

    [Test]
    public void IsNull ()
    {
      Assert.That (((INullObject) _store).IsNull, Is.False);
    }

    [Test]
    public void ContainsKey ()
    {
      ExpectSynchronizedDelegation (true, "ContainsKey", "a");
    }

    [Test]
    public void Add ()
    {
      ExpectSynchronizedDelegation (null, "Add", "a", 1);
    }

    [Test]
    public void Remove ()
    {
      ExpectSynchronizedDelegation (true, "Remove", "b");
    }

    [Test]
    public void Clear ()
    {
      ExpectSynchronizedDelegation (null, "Clear");
    }

    [Test]
    public void Get_Value ()
    {
      ExpectSynchronizedDelegation (47, "get_Item", "c");
    }

    [Test]
    public void Set_Value ()
    {
      ExpectSynchronizedDelegation (null, "set_Item", "c", 17);
    }

    [Test]
    public void GetValueOrDefault ()
    {
      ExpectSynchronizedDelegation (7, "GetValueOrDefault", "hugo");
    }

    [Test]
    public void TryGetValue ()
    {
      ExpectSynchronizedDelegation (true, "TryGetValue", "hugo", 45);
    }

    [Test]
    public void GetOrCreateValue ()
    {
      ExpectSynchronizedDelegation (17, "GetOrCreateValue", "hugo", (Func<string, int>) delegate { return 3; });
    }

    [Test]
    public void Serializable ()
    {
      Serializer.SerializeAndDeserialize (new InterlockedDataStore<string, int>());
    }

    private void ExpectSynchronizedDelegation (object result, string methodName, params object[] args)
    {
      _innerStoreInterceptor.ExpectedArguments = args;
      _innerStoreInterceptor.ReturnValue = result;

      MethodInfo method = typeof (IDataStore<string, int>).GetMethod (methodName);
      object actualResult;
      try
      {
        actualResult = method.Invoke (_store, args);
      }
      catch (TargetInvocationException ex)
      {
        throw ex.InnerException.PreserveStackTrace ();
      }
      Assert.That (_innerStoreInterceptor.Executed);
      Assert.That (actualResult, Is.EqualTo (result));
    }
  }
}

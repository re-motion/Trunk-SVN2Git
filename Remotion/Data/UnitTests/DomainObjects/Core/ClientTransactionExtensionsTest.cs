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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;

namespace Remotion.Data.UnitTests.DomainObjects.Core
{
  [TestFixture]
  public class ClientTransactionExtensionsTest : StandardMappingTest
  {
    private ClientTransaction _transaction;

    public override void SetUp ()
    {
      base.SetUp ();

      _transaction = ClientTransaction.CreateRootTransaction ();
    }

    [Test]
    public void Execute_Action_RunsDelegate ()
    {
      bool delegateRun = false;
      Action action = () => delegateRun = true;

      _transaction.Execute (action);

      Assert.That (delegateRun, Is.True);
    }

    [Test]
    public void Execute_Action_SetsCurrentTx ()
    {
      Assert.That (ClientTransaction.Current, Is.Null);
      
      ClientTransaction currentInDelegate = null;
      Action action = () => currentInDelegate = ClientTransaction.Current;

      _transaction.Execute (action);

      Assert.That (currentInDelegate, Is.SameAs (_transaction));
      Assert.That (ClientTransaction.Current, Is.Null);
    }

    [Test]
    public void Execute_Action_ReusesScopeIfPossible ()
    {
      using (var scope = _transaction.EnterNonDiscardingScope ())
      {
        Assert.That (ClientTransactionScope.ActiveScope, Is.SameAs (scope));

        ClientTransactionScope scopeInDelegate = null;
        Action action = () => scopeInDelegate = ClientTransactionScope.ActiveScope;

        _transaction.Execute (action);

        Assert.That (scopeInDelegate, Is.SameAs (scope));
        Assert.That (ClientTransactionScope.ActiveScope, Is.SameAs (scope));
      }
    }

    [Test]
    public void Execute_Func_RunsDelegate ()
    {
      Func<int> func = () => 17;
      var result = _transaction.Execute (func);

      Assert.That (result, Is.EqualTo (17));
    }

    [Test]
    public void Execute_Func_SetsCurrentTx ()
    {
      Assert.That (ClientTransaction.Current, Is.Null);

      ClientTransaction currentInDelegate = null;
      Func<int> func = () =>
      {
        currentInDelegate = ClientTransaction.Current;
        return 4;
      };

      _transaction.Execute (func);

      Assert.That (currentInDelegate, Is.SameAs (_transaction));
      Assert.That (ClientTransaction.Current, Is.Null);
    }

    [Test]
    public void Execute_Func_ReusesScopeIfPossible ()
    {
      using (var scope = _transaction.EnterNonDiscardingScope ())
      {
        Assert.That (ClientTransactionScope.ActiveScope, Is.SameAs (scope));

        ClientTransactionScope scopeInDelegate = null;
        Func<int> func = () =>
        {
          scopeInDelegate = ClientTransactionScope.ActiveScope;
          return 4;
        };

        _transaction.Execute (func);

        Assert.That (scopeInDelegate, Is.SameAs (scope));
        Assert.That (ClientTransactionScope.ActiveScope, Is.SameAs (scope));
      }
    }
  }
}
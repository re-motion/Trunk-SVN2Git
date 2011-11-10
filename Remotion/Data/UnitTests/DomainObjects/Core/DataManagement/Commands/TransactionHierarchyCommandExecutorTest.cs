// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) rubicon IT GmbH, www.rubicon.eu
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
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.Commands;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.Commands
{
  [TestFixture]
  public class TransactionHierarchyCommandExecutorTest : StandardMappingTest
  {
    public interface ICommandFactory
    {
      IDataManagementCommand Create (ClientTransaction tx);
    }

    private ClientTransaction _leafRootTransaction;

    private ClientTransaction _rootTransactionWithSub;
    private ClientTransaction _leafSubTransaction;

    private ClientTransactionMock _readOnlyTransaction;

    private MockRepository _mockRepository;
    private ICommandFactory _commandFactoryMock;
    private IDataManagementCommand _commandMock1;
    private IDataManagementCommand _commandMock2;
    private TransactionHierarchyCommandExecutor _executor;

    public override void SetUp ()
    {
      base.SetUp ();

      _leafRootTransaction = ClientTransaction.CreateRootTransaction ();

      _rootTransactionWithSub = ClientTransaction.CreateRootTransaction ();
      _leafSubTransaction = _rootTransactionWithSub.CreateSubTransaction();

      _readOnlyTransaction = new ClientTransactionMock { IsReadOnly = true };

      _mockRepository = new MockRepository();
      _commandFactoryMock = _mockRepository.StrictMock<ICommandFactory>();
      _commandMock1 = _mockRepository.StrictMock<IDataManagementCommand> ();
      _commandMock2 = _mockRepository.StrictMock<IDataManagementCommand> ();

      _executor = new TransactionHierarchyCommandExecutor (_commandFactoryMock.Create);
    }

    [Test]
    public void TryExecuteCommandForTransactionHierarchy_LeafRootTransaction_True ()
    {
      _commandFactoryMock
          .Expect (mock => mock.Create (_leafRootTransaction))
          .Return (_commandMock1);
      _commandMock1.Stub (stub => stub.GetAllExceptions()).Return (new Exception[0]);
      ExpectNotifyAndPerform (_commandMock1);

      _mockRepository.ReplayAll ();

      var result = _executor.TryExecuteCommandForTransactionHierarchy (_leafRootTransaction);

      _mockRepository.VerifyAll ();
      Assert.That (result, Is.True);
    }

    [Test]
    public void TryExecuteCommandForTransactionHierarchy_LeafRootTransaction_False ()
    {
      _commandFactoryMock
          .Expect (mock => mock.Create (_leafRootTransaction))
          .Return (_commandMock1);
      _commandMock1.Stub (stub => stub.GetAllExceptions ()).Return (new[] { new Exception() });

      _mockRepository.ReplayAll ();

      var result = _executor.TryExecuteCommandForTransactionHierarchy (_leafRootTransaction);

      _mockRepository.VerifyAll ();
      Assert.That (result, Is.False);
    }

    [Test]
    public void TryExecuteCommandForTransactionHierarchy_Hierarchy_ApplyToRoot ()
    {
      using (_mockRepository.Ordered ())
      {
        _commandFactoryMock
            .Expect (mock => mock.Create (_leafSubTransaction))
            .Return (_commandMock1);
        _commandMock1.Stub (stub => stub.GetAllExceptions ()).Return (new Exception[0]);
        ExpectNotifyAndPerform (_commandMock1);

        _commandFactoryMock
            .Expect (mock => mock.Create (_rootTransactionWithSub))
            .Return (_commandMock2);
        _commandMock2.Stub (stub => stub.GetAllExceptions ()).Return (new Exception[0]);
        ExpectNotifyAndPerform (_commandMock2);
      }

      _mockRepository.ReplayAll ();

      _executor.TryExecuteCommandForTransactionHierarchy (_rootTransactionWithSub);

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void TryExecuteCommandForTransactionHierarchy_Hierarchy_ApplyToLeaf ()
    {
      using (_mockRepository.Ordered ())
      {
        _commandFactoryMock
            .Expect (mock => mock.Create (_leafSubTransaction))
            .Return (_commandMock1);
        _commandMock1.Stub (stub => stub.GetAllExceptions ()).Return (new Exception[0]);
        ExpectNotifyAndPerform (_commandMock1);

        _commandFactoryMock
            .Expect (mock => mock.Create (_rootTransactionWithSub))
            .Return (_commandMock2);
        _commandMock2.Stub (stub => stub.GetAllExceptions ()).Return (new Exception[0]);
        ExpectNotifyAndPerform (_commandMock2);
      }

      _mockRepository.ReplayAll ();

      _executor.TryExecuteCommandForTransactionHierarchy (_leafSubTransaction);

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void TryExecuteCommandForTransactionHierarchy_Hierarchy_StopsAtException ()
    {
      var exception = new Exception ("Oh no!");
      using (_mockRepository.Ordered ())
      {
        _commandFactoryMock
            .Expect (mock => mock.Create (_leafSubTransaction))
            .Return (_commandMock1);
        _commandMock1.Stub (stub => stub.GetAllExceptions ()).Return (new[] { exception });
      }

      _mockRepository.ReplayAll ();

      var result = _executor.TryExecuteCommandForTransactionHierarchy (_rootTransactionWithSub);

      _mockRepository.VerifyAll ();
      Assert.That (result, Is.False);
    }

    [Test]
    public void TryExecuteCommandForTransactionHierarchy_UnlocksReadOnlyTransaction ()
    {
      _commandFactoryMock
          .Expect (mock => mock.Create (_readOnlyTransaction))
          .Return (_commandMock1);

      _commandMock1.Stub (stub => stub.GetAllExceptions ()).Return (new Exception[0]);
      using (_mockRepository.Ordered ())
      {
        _commandMock1
            .Expect (mock => mock.NotifyClientTransactionOfBegin ())
            .WhenCalled (mi => Assert.That (_readOnlyTransaction.IsReadOnly, Is.False));
        _commandMock1
            .Expect (mock => mock.Begin ())
            .WhenCalled (mi => Assert.That (_readOnlyTransaction.IsReadOnly, Is.False));
        _commandMock1
            .Expect (mock => mock.Perform ())
            .WhenCalled (mi => Assert.That (_readOnlyTransaction.IsReadOnly, Is.False));
        _commandMock1
            .Expect (mock => mock.End ())
            .WhenCalled (mi => Assert.That (_readOnlyTransaction.IsReadOnly, Is.False));
        _commandMock1
            .Expect (mock => mock.NotifyClientTransactionOfEnd ())
            .WhenCalled (mi => Assert.That (_readOnlyTransaction.IsReadOnly, Is.False));
      }

      _mockRepository.ReplayAll ();

      Assert.That (_readOnlyTransaction.IsReadOnly, Is.True);
      _executor.TryExecuteCommandForTransactionHierarchy (_readOnlyTransaction);
      Assert.That (_readOnlyTransaction.IsReadOnly, Is.True);

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void ExecuteCommandForTransactionHierarchy_LeafRootTransaction ()
    {
      _commandFactoryMock
          .Expect (mock => mock.Create (_leafRootTransaction))
          .Return (_commandMock1);
      ExpectNotifyAndPerform (_commandMock1);

      _mockRepository.ReplayAll();

      _executor.ExecuteCommandForTransactionHierarchy (_leafRootTransaction);

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void ExecuteCommandForTransactionHierarchy_Hierarchy_ApplyToRoot ()
    {
      using (_mockRepository.Ordered ())
      {
        _commandFactoryMock
            .Expect (mock => mock.Create (_leafSubTransaction))
            .Return (_commandMock1);
        ExpectNotifyAndPerform (_commandMock1);

        _commandFactoryMock
            .Expect (mock => mock.Create (_rootTransactionWithSub))
            .Return (_commandMock2);
        ExpectNotifyAndPerform (_commandMock2);
      }

      _mockRepository.ReplayAll ();

      _executor.ExecuteCommandForTransactionHierarchy (_rootTransactionWithSub);

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void ExecuteCommandForTransactionHierarchy_Hierarchy_ApplyToLeaf ()
    {
      using (_mockRepository.Ordered ())
      {
        _commandFactoryMock
            .Expect (mock => mock.Create (_leafSubTransaction))
            .Return (_commandMock1);
        ExpectNotifyAndPerform (_commandMock1);

        _commandFactoryMock
            .Expect (mock => mock.Create (_rootTransactionWithSub))
            .Return (_commandMock2);
        ExpectNotifyAndPerform (_commandMock2);
      }

      _mockRepository.ReplayAll ();

      _executor.ExecuteCommandForTransactionHierarchy (_leafSubTransaction);

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void ExecuteCommandForTransactionHierarchy_Hierarchy_StopsAtException ()
    {
      var exception = new Exception ("Oh no!");
      using (_mockRepository.Ordered ())
      {
        _commandFactoryMock
            .Expect (mock => mock.Create (_leafSubTransaction))
            .Return (_commandMock1);
        _commandMock1.Expect (mock => mock.NotifyClientTransactionOfBegin ()).Throw (exception);
      }

      _mockRepository.ReplayAll ();

      Assert.That (() => _executor.ExecuteCommandForTransactionHierarchy (_rootTransactionWithSub), Throws.Exception.SameAs (exception));

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void ExecuteCommandForTransactionHierarchy_UnlocksReadOnlyTransaction ()
    {
      _commandFactoryMock
          .Expect (mock => mock.Create (_readOnlyTransaction))
          .Return (_commandMock1);
      
      using (_mockRepository.Ordered ())
      {
        _commandMock1
            .Expect (mock => mock.NotifyClientTransactionOfBegin())
            .WhenCalled (mi => Assert.That (_readOnlyTransaction.IsReadOnly, Is.False));
        _commandMock1
            .Expect (mock => mock.Begin ())
            .WhenCalled (mi => Assert.That (_readOnlyTransaction.IsReadOnly, Is.False));
        _commandMock1
            .Expect (mock => mock.Perform ())
            .WhenCalled (mi => Assert.That (_readOnlyTransaction.IsReadOnly, Is.False));
        _commandMock1
            .Expect (mock => mock.End ())
            .WhenCalled (mi => Assert.That (_readOnlyTransaction.IsReadOnly, Is.False));
        _commandMock1
            .Expect (mock => mock.NotifyClientTransactionOfEnd ())
            .WhenCalled (mi => Assert.That (_readOnlyTransaction.IsReadOnly, Is.False));
      }

      _mockRepository.ReplayAll ();

      Assert.That (_readOnlyTransaction.IsReadOnly, Is.True);
      _executor.ExecuteCommandForTransactionHierarchy (_readOnlyTransaction);
      Assert.That (_readOnlyTransaction.IsReadOnly, Is.True);

      _mockRepository.VerifyAll ();
    }

    private void ExpectNotifyAndPerform (IDataManagementCommand commandMock)
    {
      using (_mockRepository.Ordered ())
      {
        commandMock.Expect (mock => mock.NotifyClientTransactionOfBegin ());
        commandMock.Expect (mock => mock.Begin ());
        commandMock.Expect (mock => mock.Perform ());
        commandMock.Expect (mock => mock.End ());
        commandMock.Expect (mock => mock.NotifyClientTransactionOfEnd ());
      }
    }

  }
}
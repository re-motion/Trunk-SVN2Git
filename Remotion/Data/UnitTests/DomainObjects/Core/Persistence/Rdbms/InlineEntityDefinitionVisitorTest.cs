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
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.Model;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms
{
  [TestFixture]
  public class InlineEntityDefinitionVisitorTest : StandardMappingTest
  {
    public interface IVisitorCallReceiver
    {
      void HandleTableDefinition (TableDefinition table, Action<IEntityDefinition> continuation);
      void HandleFilterViewDefinition (FilterViewDefinition filterView, Action<IEntityDefinition> continuation);
      void HandleUnionViewDefinition (UnionViewDefinition unionView, Action<IEntityDefinition> continuation);
      void HandleNullEntityDefinition (NullEntityDefinition nullEntity, Action<IEntityDefinition> continuation);
    }

    public interface IVisitorCallReceiver<T>
    {
      T HandleTableDefinition (TableDefinition table, Func<IEntityDefinition, T> continuation);
      T HandleFilterViewDefinition (FilterViewDefinition filterView, Func<IEntityDefinition, T> continuation);
      T HandleUnionViewDefinition (UnionViewDefinition unionView, Func<IEntityDefinition, T> continuation);
      T HandleNullEntityDefinition (NullEntityDefinition nullEntity, Func<IEntityDefinition, T> continuation);
    }

    private TableDefinition _tableDefinition;
    private FilterViewDefinition _filterViewDefinition;
    private UnionViewDefinition _unionViewDefinition;
    private NullEntityDefinition _nullEntityDefinition;

    private object _fakeResult;

    private MockRepository _mockRepository;
    private IVisitorCallReceiver _voidReceiverMock;
    private IVisitorCallReceiver<string> _nonVoidReceiverMock;

    public override void SetUp ()
    {
      base.SetUp();

      _tableDefinition = TableDefinitionObjectMother.Create (TestDomainStorageProviderDefinition, new EntityNameDefinition (null, "Table"));
      _filterViewDefinition = FilterViewDefinitionObjectMother.Create (
          TestDomainStorageProviderDefinition, new EntityNameDefinition (null, "FilterView"), _tableDefinition);
      _unionViewDefinition = UnionViewDefinitionObjectMother.Create (
          TestDomainStorageProviderDefinition, new EntityNameDefinition (null, "UnionView"), new[] { _tableDefinition });
      _nullEntityDefinition = new NullEntityDefinition (TestDomainStorageProviderDefinition);

      _fakeResult = new object();

      _mockRepository = new MockRepository();
      _voidReceiverMock = _mockRepository.StrictMock<IVisitorCallReceiver> ();
      _nonVoidReceiverMock = _mockRepository.StrictMock<IVisitorCallReceiver<string>> ();
    }

    [Test]
    public void Visit_WithoutResult_TableDefinition ()
    {
      _voidReceiverMock.Expect (mock => mock.HandleTableDefinition (Arg.Is (_tableDefinition), Arg<Action<IEntityDefinition>>.Is.Anything));
      _voidReceiverMock.Replay();

      InlineEntityDefinitionVisitor.Visit (
          _tableDefinition,
          _voidReceiverMock.HandleTableDefinition,
          _voidReceiverMock.HandleFilterViewDefinition,
          _voidReceiverMock.HandleUnionViewDefinition,
          _voidReceiverMock.HandleNullEntityDefinition);

      _voidReceiverMock.VerifyAllExpectations();
    }

    [Test]
    public void Visit_WithoutResult_FilterViewDefinition ()
    {
      _voidReceiverMock.Expect (mock => mock.HandleFilterViewDefinition (Arg.Is (_filterViewDefinition), Arg<Action<IEntityDefinition>>.Is.Anything));
      _voidReceiverMock.Replay ();

      InlineEntityDefinitionVisitor.Visit (
          _filterViewDefinition,
          _voidReceiverMock.HandleTableDefinition,
          _voidReceiverMock.HandleFilterViewDefinition,
          _voidReceiverMock.HandleUnionViewDefinition,
          _voidReceiverMock.HandleNullEntityDefinition);

      _voidReceiverMock.VerifyAllExpectations ();
    }

    [Test]
    public void Visit_WithoutResult_UnionViewDefinition ()
    {
      _voidReceiverMock.Expect (mock => mock.HandleUnionViewDefinition (Arg.Is (_unionViewDefinition), Arg<Action<IEntityDefinition>>.Is.Anything));
      _voidReceiverMock.Replay ();
 
      InlineEntityDefinitionVisitor.Visit (
          _unionViewDefinition,
          _voidReceiverMock.HandleTableDefinition,
          _voidReceiverMock.HandleFilterViewDefinition,
          _voidReceiverMock.HandleUnionViewDefinition,
          _voidReceiverMock.HandleNullEntityDefinition);

      _voidReceiverMock.VerifyAllExpectations ();
    }

    [Test]
    public void Visit_WithoutResult_NullEntityDefinition ()
    {
      _voidReceiverMock.Expect (mock => mock.HandleNullEntityDefinition (Arg.Is (_nullEntityDefinition), Arg<Action<IEntityDefinition>>.Is.Anything));
      _voidReceiverMock.Replay ();

      InlineEntityDefinitionVisitor.Visit (
          _nullEntityDefinition,
          _voidReceiverMock.HandleTableDefinition,
          _voidReceiverMock.HandleFilterViewDefinition,
          _voidReceiverMock.HandleUnionViewDefinition,
          _voidReceiverMock.HandleNullEntityDefinition);

      _voidReceiverMock.VerifyAllExpectations ();
    }

    [Test]
    public void Visit_WithoutResult_Continuation ()
    {
      using (_mockRepository.Ordered ())
      {
        _voidReceiverMock
            .Expect (mock => mock.HandleTableDefinition (Arg.Is (_tableDefinition), Arg<Action<IEntityDefinition>>.Is.Anything))
            .WhenCalled (
                mi =>
                {
                  var continuation = (Action<IEntityDefinition>) mi.Arguments[1];
                  continuation (_filterViewDefinition);
                });
        _voidReceiverMock
            .Expect (mock => mock.HandleFilterViewDefinition (Arg.Is (_filterViewDefinition), Arg<Action<IEntityDefinition>>.Is.Anything))
            .WhenCalled (
                mi =>
                {
                  var continuation = (Action<IEntityDefinition>) mi.Arguments[1];
                  continuation (_unionViewDefinition);
                });
        _voidReceiverMock
            .Expect (mock => mock.HandleUnionViewDefinition (Arg.Is (_unionViewDefinition), Arg<Action<IEntityDefinition>>.Is.Anything))
            .WhenCalled (
                mi =>
                {
                  var continuation = (Action<IEntityDefinition>) mi.Arguments[1];
                  continuation (_nullEntityDefinition);
                });
        _voidReceiverMock
            .Expect (mock => mock.HandleNullEntityDefinition (Arg.Is (_nullEntityDefinition), Arg<Action<IEntityDefinition>>.Is.Anything))
            .WhenCalled (
                mi =>
                {
                  var continuation = (Action<IEntityDefinition>) mi.Arguments[1];
                  continuation (_tableDefinition);
                });
        _voidReceiverMock.Expect (mock => mock.HandleTableDefinition (Arg.Is (_tableDefinition), Arg<Action<IEntityDefinition>>.Is.Anything));
      }

      _voidReceiverMock.Replay ();

      InlineEntityDefinitionVisitor.Visit (
          _tableDefinition,
          _voidReceiverMock.HandleTableDefinition,
          _voidReceiverMock.HandleFilterViewDefinition,
          _voidReceiverMock.HandleUnionViewDefinition,
          _voidReceiverMock.HandleNullEntityDefinition);

      _voidReceiverMock.VerifyAllExpectations ();
    }

    [Test]
    public void Visit_WithResult_TableDefinition ()
    {
      _nonVoidReceiverMock
        .Expect (mock => mock.HandleTableDefinition(Arg.Is (_tableDefinition), Arg<Func<IEntityDefinition, string>>.Is.Anything))
        .Return("test");
      _nonVoidReceiverMock.Replay ();

      var result = InlineEntityDefinitionVisitor.Visit<string> (
          _tableDefinition,
          _nonVoidReceiverMock.HandleTableDefinition,
          _nonVoidReceiverMock.HandleFilterViewDefinition,
          _nonVoidReceiverMock.HandleUnionViewDefinition,
          _nonVoidReceiverMock.HandleNullEntityDefinition);

      _nonVoidReceiverMock.VerifyAllExpectations();
      Assert.That (result, Is.EqualTo ("test"));
    }

    [Test]
    public void Visit_WithResult_FilterViewDefinition ()
    {
      _nonVoidReceiverMock
        .Expect (mock => mock.HandleFilterViewDefinition (Arg.Is (_filterViewDefinition), Arg<Func<IEntityDefinition, string>>.Is.Anything))
        .Return ("test");
      _nonVoidReceiverMock.Replay();

      var result = InlineEntityDefinitionVisitor.Visit<string> (
          _filterViewDefinition,
          _nonVoidReceiverMock.HandleTableDefinition,
          _nonVoidReceiverMock.HandleFilterViewDefinition,
          _nonVoidReceiverMock.HandleUnionViewDefinition,
          _nonVoidReceiverMock.HandleNullEntityDefinition);

      _nonVoidReceiverMock.VerifyAllExpectations ();
      Assert.That (result, Is.EqualTo ("test"));
    }

    [Test]
    public void Visit_WithResult_UnionViewDefinition ()
    {
      _nonVoidReceiverMock
        .Expect (mock => mock.HandleUnionViewDefinition (Arg.Is (_unionViewDefinition), Arg<Func<IEntityDefinition, string>>.Is.Anything))
        .Return ("test");
      _nonVoidReceiverMock.Replay ();

      var result = InlineEntityDefinitionVisitor.Visit<string> (
          _unionViewDefinition,
          _nonVoidReceiverMock.HandleTableDefinition,
          _nonVoidReceiverMock.HandleFilterViewDefinition,
          _nonVoidReceiverMock.HandleUnionViewDefinition,
          _nonVoidReceiverMock.HandleNullEntityDefinition);

      _nonVoidReceiverMock.VerifyAllExpectations ();
      Assert.That (result, Is.EqualTo ("test"));
    }

    [Test]
    public void Visit_WithResult_NullEntityDefinition ()
    {
      _nonVoidReceiverMock
        .Expect (mock => mock.HandleNullEntityDefinition (Arg.Is (_nullEntityDefinition), Arg<Func<IEntityDefinition, string>>.Is.Anything))
        .Return ("test");
      _nonVoidReceiverMock.Replay ();

      var result = InlineEntityDefinitionVisitor.Visit<string> (
          _nullEntityDefinition,
          _nonVoidReceiverMock.HandleTableDefinition,
          _nonVoidReceiverMock.HandleFilterViewDefinition,
          _nonVoidReceiverMock.HandleUnionViewDefinition,
          _nonVoidReceiverMock.HandleNullEntityDefinition);

      _nonVoidReceiverMock.VerifyAllExpectations ();
      Assert.That (result, Is.EqualTo ("test"));
    }

    [Test]
    public void Visit_WithResult_Continuation ()
    {
      using (_mockRepository.Ordered ())
      {
        _nonVoidReceiverMock
            .Expect (mock => mock.HandleTableDefinition (Arg.Is (_tableDefinition), Arg<Func<IEntityDefinition, string>>.Is.Anything))
            .WhenCalled (
                mi =>
                {
                  var continuation = (Func<IEntityDefinition, string>) mi.Arguments[1];
                  var value = continuation (_filterViewDefinition);
                  Assert.That (value, Is.EqualTo ("3"));
                })
            .Return ("4");
        _nonVoidReceiverMock
            .Expect (mock => mock.HandleFilterViewDefinition (Arg.Is (_filterViewDefinition), Arg<Func<IEntityDefinition, string>>.Is.Anything))
            .WhenCalled (
                mi =>
                {
                  var continuation = (Func<IEntityDefinition, string>) mi.Arguments[1];
                  var value = continuation (_unionViewDefinition);
                  Assert.That (value, Is.EqualTo ("2"));
                })
            .Return ("3");
        _nonVoidReceiverMock
            .Expect (mock => mock.HandleUnionViewDefinition (Arg.Is (_unionViewDefinition), Arg<Func<IEntityDefinition, string>>.Is.Anything))
            .WhenCalled (
                mi =>
                {
                  var continuation = (Func<IEntityDefinition, string>) mi.Arguments[1];
                  var value = continuation (_nullEntityDefinition);
                  Assert.That (value, Is.EqualTo ("1"));
                })
            .Return ("2");
        _nonVoidReceiverMock
            .Expect (mock => mock.HandleNullEntityDefinition (Arg.Is (_nullEntityDefinition), Arg<Func<IEntityDefinition, string>>.Is.Anything))
            .WhenCalled (
                mi =>
                {
                  var continuation = (Func<IEntityDefinition, string>) mi.Arguments[1];
                  var value = continuation (_tableDefinition);
                  Assert.That (value, Is.EqualTo ("0"));
                })
            .Return ("1");
        _nonVoidReceiverMock
            .Expect (mock => mock.HandleTableDefinition (Arg.Is (_tableDefinition), Arg<Func<IEntityDefinition, string>>.Is.Anything))
            .Return ("0");
      }

      _nonVoidReceiverMock.Replay ();

      var result = InlineEntityDefinitionVisitor.Visit<string> (
          _tableDefinition,
          _nonVoidReceiverMock.HandleTableDefinition,
          _nonVoidReceiverMock.HandleFilterViewDefinition,
          _nonVoidReceiverMock.HandleUnionViewDefinition,
          _nonVoidReceiverMock.HandleNullEntityDefinition);

      _nonVoidReceiverMock.VerifyAllExpectations ();
      Assert.That (result, Is.EqualTo ("4"));
    }
  }
}
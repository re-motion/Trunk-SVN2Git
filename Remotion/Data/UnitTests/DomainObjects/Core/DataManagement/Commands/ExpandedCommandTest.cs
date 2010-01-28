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
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.Commands;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.Commands
{
  [TestFixture]
  public class ExpandedCommandTest
  {
    private MockRepository _mockRepository;
    private IDataManagementCommand _commandMock1;
    private IDataManagementCommand _commandMock2;
    private IDataManagementCommand _commandMock3;
    private ExpandedCommand _expanded;

    [SetUp]
    public void SetUp ()
    {
      _mockRepository = new MockRepository ();

      _commandMock1 = _mockRepository.StrictMock<IDataManagementCommand> ();
      _commandMock2 = _mockRepository.StrictMock<IDataManagementCommand> ();
      _commandMock3 = _mockRepository.StrictMock<IDataManagementCommand> ();

      _expanded = new ExpandedCommand (_commandMock1, _commandMock2, _commandMock3);
    }

    [Test]
    public void GetNestedCommands ()
    {
      Assert.That (_expanded.GetNestedCommands (), Is.EqualTo (new[] { _commandMock1, _commandMock2, _commandMock3 }));
    }

    [Test]
    public void Begin ()
    {
      _commandMock1.Begin ();
      _commandMock2.Begin ();
      _commandMock3.Begin ();

      _mockRepository.ReplayAll ();

      _expanded.Begin ();

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void Perform ()
    {
      _commandMock1.Perform ();
      _commandMock2.Perform ();
      _commandMock3.Perform ();

      _mockRepository.ReplayAll ();

      _expanded.Perform ();

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void End ()
    {
      _commandMock1.End ();
      _commandMock2.End ();
      _commandMock3.End ();

      _mockRepository.ReplayAll ();

      _expanded.End ();

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void NotifyClientTransactionOfBegin ()
    {
      _commandMock1.NotifyClientTransactionOfBegin ();
      _commandMock2.NotifyClientTransactionOfBegin ();
      _commandMock3.NotifyClientTransactionOfBegin ();

      _mockRepository.ReplayAll ();

      _expanded.NotifyClientTransactionOfBegin ();

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void NotifyClientTransactionOfEnd ()
    {
      _commandMock1.NotifyClientTransactionOfEnd ();
      _commandMock2.NotifyClientTransactionOfEnd ();
      _commandMock3.NotifyClientTransactionOfEnd ();

      _mockRepository.ReplayAll ();

      _expanded.NotifyClientTransactionOfEnd ();

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void ExtendToRelatedObjects ()
    {
      var result = ((IDataManagementCommand) _expanded).ExpandToAllRelatedObjects ();

      Assert.That (result, Is.SameAs (_expanded));
    }

    [Test]
    public void CombineWith ()
    {
      var otherCommandStub = MockRepository.GenerateStub<IDataManagementCommand> ();
      var result = _expanded.CombineWith (otherCommandStub);

      Assert.That (result, Is.Not.SameAs (_expanded));
      Assert.That (result.GetNestedCommands (), Is.EqualTo (new[] { _commandMock1, _commandMock2, _commandMock3, otherCommandStub }));
      Assert.That (_expanded.GetNestedCommands (), Is.EqualTo (new[] { _commandMock1, _commandMock2, _commandMock3 }));
    }

  }
}
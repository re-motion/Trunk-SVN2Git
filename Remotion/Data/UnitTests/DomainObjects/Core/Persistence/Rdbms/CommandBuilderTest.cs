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
using System.Data;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Rhino.Mocks;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms
{
  [TestFixture]
  public class CommandBuilderTest : SqlProviderBaseTest
  {
    private CommandBuilder _commandBuilder;

    public override void SetUp ()
    {
      base.SetUp ();
      Provider.Connect ();
      _commandBuilder = new StubCommandBuilder (Provider);
    }

    [Test]
    public void AddCommandParameter_ForExtensibleEnum ()
    {
      var commandStub = MockRepository.GenerateStub<IDbCommand> ();
      var parameterMock = MockRepository.GenerateMock<IDbDataParameter> ();
      var parameterCollectionStub = MockRepository.GenerateStub<IDataParameterCollection> ();

      commandStub.Stub (stub => stub.CreateParameter ()).Return (parameterMock);
      commandStub.Stub (stub => stub.Parameters).Return (parameterCollectionStub);
      commandStub.Replay ();

      parameterCollectionStub.Stub (stub => stub.Add (parameterMock)).Return (0);
      parameterCollectionStub.Replay ();

      parameterMock.Expect (mock => mock.Value = Color.Values.Red ().ID); // string!
      parameterMock.Replay ();

      _commandBuilder.AddCommandParameter (commandStub, "x", Color.Values.Red ());

      parameterMock.VerifyAllExpectations ();
    }
  }
}
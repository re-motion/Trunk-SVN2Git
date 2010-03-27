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
using System.Data;
using System.Text;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Development.UnitTesting;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms
{
  [TestFixture]
  public class WhereClauseBuilderTest : SqlProviderBaseTest
  {
    private Guid _guid1;
    private Guid _guid2;
    private Guid _guid3;
    private ICommandBuilder _commandBuilderMock;
    private IDbCommand _commandMock;

    public override void SetUp ()
    {
      base.SetUp ();

      _guid1 = new Guid ("11111111-68DB-4e68-BC69-AC69B15FA23F");
      _guid2 = new Guid ("22222222-68DB-4e68-BC69-AC69B15FA23F");
      _guid3 = new Guid ("33333333-68DB-4e68-BC69-AC69B15FA23F");

      _commandBuilderMock = MockRepository.GenerateStrictMock<ICommandBuilder> ();
      _commandMock = MockRepository.GenerateStrictMock<IDbCommand> ();

      _commandBuilderMock.Stub (mock => mock.Provider).Return (Provider);
    }

    [Test]
    public void SetInExpression ()
    {
      var builder = WhereClauseBuilder.Create (_commandBuilderMock, _commandMock);

      var expectedXml = "<L><I>" + _guid1 + "</I><I>" + _guid2 + "</I><I>" + _guid3 + "</I></L>";
      var parameterMock = MockRepository.GenerateStrictMock<IDataParameter> ();
      _commandBuilderMock.Expect (mock => mock.AddCommandParameter (_commandMock, "@WhateverID", expectedXml)).Return (parameterMock);
      _commandBuilderMock.Replay ();

      parameterMock.Expect (mock => mock.DbType = DbType.Xml);
      parameterMock.Replay ();

      builder.SetInExpression ("WhateverID", "uniqueidentifier", new object[] { _guid1, _guid2, _guid3 });
      var resultText = builder.ToString();

      _commandBuilderMock.VerifyAllExpectations ();
      parameterMock.VerifyAllExpectations ();
      Assert.That (resultText, Is.EqualTo ("[WhateverID] IN (SELECT T.c.value('.', 'uniqueidentifier') FROM @WhateverID.nodes('/L/I') T(c))"));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "SetInExpression can only be used with an empty WhereClauseBuilder.")]
    public void SetInExpression_ThrowsWhenNotEmpty ()
    {
      var builder = WhereClauseBuilder.Create (_commandBuilderMock, _commandMock);

      ((StringBuilder) PrivateInvoke.GetNonPublicProperty(builder, "Builder")).Append ("Holerö");

      builder.SetInExpression ("WhateverID", "uniqueIdentifier", new object[] { _guid1, _guid2, _guid3 });
    }
  }
}
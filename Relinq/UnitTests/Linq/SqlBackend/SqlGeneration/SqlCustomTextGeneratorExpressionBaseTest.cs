// This file is part of the re-linq project (relinq.codeplex.com)
// Copyright (C) rubicon IT GmbH, www.rubicon.eu
// 
// re-linq is free software; you can redistribute it and/or modify it under 
// the terms of the GNU Lesser General Public License as published by the 
// Free Software Foundation; either version 2.1 of the License, 
// or (at your option) any later version.
// 
// re-linq is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-linq; if not, see http://www.gnu.org/licenses.
// 
using System;
using NUnit.Framework;
using Remotion.Linq.UnitTests.Linq.Core.TestDomain;
using Remotion.Linq.Parsing;
using Remotion.Linq.SqlBackend.SqlGeneration;
using Rhino.Mocks;

namespace Remotion.Linq.UnitTests.Linq.SqlBackend.SqlGeneration
{
  [TestFixture]
  public class SqlCustomTextGeneratorExpressionBaseTest
  {
    [Test]
    public void Accept ()
    {
      var mockRepository = new MockRepository ();
      var visitorMock = mockRepository.StrictMultiMock<ExpressionTreeVisitor> (typeof (ISqlCustomTextGeneratorExpressionVisitor));

      var customTextGeneratorExpression = new TestableSqlCustomTextGeneratorExpression (typeof (Cook));

      visitorMock
          .Expect (mock => ((ISqlCustomTextGeneratorExpressionVisitor)mock).VisitSqlCustomTextGeneratorExpression (customTextGeneratorExpression))
          .Return (customTextGeneratorExpression);
      visitorMock.Replay();

      customTextGeneratorExpression.Accept (visitorMock);

      visitorMock.VerifyAllExpectations();
    }
  }
}
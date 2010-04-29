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
using Remotion.Data.Linq.Clauses.ResultOperators;
using Remotion.Data.Linq.SqlBackend.SqlPreparation;
using Remotion.Data.Linq.SqlBackend.SqlPreparation.ResultOperatorHandlers;
using Rhino.Mocks;

namespace Remotion.Data.Linq.UnitTests.Linq.SqlBackend.SqlPreparation
{
  [TestFixture]
  public class ResultOperatorHandlerRegistryTest
  {
    [Test]
    public void CreateDefault ()
    {
      var registry = ResultOperatorHandlerRegistry.CreateDefault ();

      Assert.That (registry.GetHandler (typeof (CastResultOperator)), Is.TypeOf (typeof (CastResultOperatorHandler)));
      Assert.That (registry.GetHandler (typeof (ContainsResultOperator)), Is.TypeOf (typeof (ContainsResultOperatorHandler)));
      Assert.That (registry.GetHandler (typeof (CountResultOperator)), Is.TypeOf (typeof (CountResultOperatorHandler)));
      Assert.That (registry.GetHandler (typeof (DistinctResultOperator)), Is.TypeOf (typeof (DistinctResultOperatorHandler)));
      Assert.That (registry.GetHandler (typeof (FirstResultOperator)), Is.TypeOf (typeof (FirstResultOperatorHandler)));
      Assert.That (registry.GetHandler (typeof (OfTypeResultOperator)), Is.TypeOf (typeof (OfTypeResultOperatorHandler)));
      Assert.That (registry.GetHandler (typeof (SingleResultOperator)), Is.TypeOf (typeof (SingleResultOperatorHandler)));
      Assert.That (registry.GetHandler (typeof (TakeResultOperator)), Is.TypeOf (typeof (TakeResultOperatorHandler)));
      Assert.That (registry.GetHandler (typeof (AnyResultOperator)), Is.TypeOf (typeof (AnyResultOperatorHandler)));
    }

    [Test]
    public void Register ()
    {
      var registry = new ResultOperatorHandlerRegistry();
      var handlerMock = MockRepository.GenerateMock<IResultOperatorHandler>();
      registry.Register (typeof (CastResultOperator), handlerMock);

      Assert.That (registry.GetHandler (typeof (CastResultOperator)), Is.SameAs(handlerMock));
    }
  }
}
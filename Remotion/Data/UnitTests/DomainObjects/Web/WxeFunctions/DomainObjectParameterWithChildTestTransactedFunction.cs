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
using Remotion.Data.DomainObjects;
using Remotion.Data.UnitTests.DomainObjects.Factories;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Web.ExecutionEngine;

namespace Remotion.Data.UnitTests.DomainObjects.Web.WxeFunctions
{
  public class DomainObjectParameterWithChildTestTransactedFunction : CreateRootWithChildTestTransactedFunctionBase
  {
    public DomainObjectParameterWithChildTestTransactedFunction ()
        : base (
            WxeTransactionMode<ClientTransactionFactory>.CreateRootWithAutoCommit,
            new DomainObjectParameterTestTransactedFunction (
                WxeTransactionMode<ClientTransactionFactory>.CreateChildIfParentWithAutoCommit, null, null))
    {
      Insert (0, new WxeMethodStep (FirstStep));
      Add (new WxeMethodStep (LastStep));
    }

    private void FirstStep ()
    {
      Assert.AreSame (Transaction.GetNativeTransaction<ClientTransaction>(), ClientTransactionScope.CurrentTransaction);

      DomainObjectParameterTestTransactedFunction childFunction = (DomainObjectParameterTestTransactedFunction) ChildFunction;
      ClassWithAllDataTypes inParameter = ClassWithAllDataTypes.GetObject (new DomainObjectIDs().ClassWithAllDataTypes2);
      inParameter.Int32Property = 47;

      ClassWithAllDataTypes[] inParameterArray = new[] { inParameter };

      childFunction.InParameter = inParameter;
      childFunction.InParameterArray = inParameterArray;
    }

    private void LastStep ()
    {
      Assert.AreSame (Transaction.GetNativeTransaction<ClientTransaction>(), ClientTransactionScope.CurrentTransaction);

      var childFunction = (DomainObjectParameterTestTransactedFunction) ChildFunction;
      ClassWithAllDataTypes outParameter = childFunction.OutParameter;
      ClassWithAllDataTypes[] outParameterArray = childFunction.OutParameterArray;

      Assert.IsTrue (ClientTransaction.Current.IsEnlisted (outParameter));
      Assert.AreEqual (52, outParameter.Int32Property); // 47 + 5

      Assert.IsTrue (ClientTransaction.Current.IsEnlisted (outParameterArray[0]));
      Assert.AreEqual (52, outParameterArray[0].Int32Property); // 48 + 5
    }
  }
}

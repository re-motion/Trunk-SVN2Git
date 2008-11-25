/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

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

      DomainObjectParameterTestTransactedFunction childFunction = (DomainObjectParameterTestTransactedFunction) ChildFunction;
      ClassWithAllDataTypes outParameter = childFunction.OutParameter;
      ClassWithAllDataTypes[] outParameterArray = childFunction.OutParameterArray;

      Assert.IsTrue (outParameter.CanBeUsedInTransaction);
      Assert.AreEqual (52, outParameter.Int32Property); // 47 + 5

      Assert.IsTrue (outParameterArray[0].CanBeUsedInTransaction);
      Assert.AreEqual (52, outParameterArray[0].Int32Property); // 48 + 5
    }
  }
}
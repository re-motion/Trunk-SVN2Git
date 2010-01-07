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
  [Serializable]
  public class DomainObjectParameterTestTransactedFunction : WxeFunction
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    public DomainObjectParameterTestTransactedFunction (
        ITransactionMode transactionMode,
        ClassWithAllDataTypes inParameter,
        ClassWithAllDataTypes[] inParameterArray)
        : base (transactionMode, inParameter, inParameterArray)
    {
    }

    // methods and properties

    [WxeParameter (1, false, WxeParameterDirection.In)]
    public ClassWithAllDataTypes InParameter
    {
      get { return (ClassWithAllDataTypes) Variables["InParameter"]; }
      set { Variables["InParameter"] = value; }
    }

    [WxeParameter (2, false, WxeParameterDirection.In)]
    public ClassWithAllDataTypes[] InParameterArray
    {
      get { return (ClassWithAllDataTypes[]) Variables["InParameterArray"]; }
      set { Variables["InParameterArray"] = value; }
    }

    [WxeParameter (3, false, WxeParameterDirection.Out)]
    public ClassWithAllDataTypes OutParameter
    {
      get { return (ClassWithAllDataTypes) Variables["OutParameter"]; }
      set { Variables["OutParameter"] = value; }
    }

    [WxeParameter (4, false, WxeParameterDirection.Out)]
    public ClassWithAllDataTypes[] OutParameterArray
    {
      get { return (ClassWithAllDataTypes[]) Variables["OutParameterArray"]; }
      set { Variables["OutParameterArray"] = value; }
    }

    private void Step1 ()
    {
      var clientTransaction = Transaction.GetNativeTransaction<ClientTransaction> ();
      Assert.IsTrue (clientTransaction == null || clientTransaction == ClientTransactionScope.CurrentTransaction);
      Assert.IsTrue (ClientTransactionScope.CurrentTransaction.IsEnlisted (InParameter));
      Assert.IsTrue (ClientTransactionScope.CurrentTransaction.IsEnlisted (InParameterArray[0]));

      OutParameter = ClassWithAllDataTypes.GetObject (new DomainObjectIDs().ClassWithAllDataTypes1);
      OutParameter.Int32Property = InParameter.Int32Property + 5;

      OutParameterArray = new[] {ClassWithAllDataTypes.GetObject (new DomainObjectIDs().ClassWithAllDataTypes2)};
      OutParameterArray[0].Int32Property = InParameterArray[0].Int32Property + 5;
    }
  }
}

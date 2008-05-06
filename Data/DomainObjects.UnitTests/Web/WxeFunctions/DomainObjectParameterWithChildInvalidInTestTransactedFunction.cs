using System;
using NUnit.Framework;
using Remotion.Data.DomainObjects.UnitTests.Factories;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Web.ExecutionEngine;

namespace Remotion.Data.DomainObjects.UnitTests.Web.WxeFunctions
{
  public class DomainObjectParameterWithChildInvalidInTestTransactedFunction : CreateRootWithChildTestTransactedFunctionBase
  {
    public DomainObjectParameterWithChildInvalidInTestTransactedFunction ()
        : base (WxeTransactionMode.CreateRoot, new DomainObjectParameterTestTransactedFunction (WxeTransactionMode.CreateChildIfParent, null, null))
    {
      Insert (0, new WxeMethodStep (FirstStep));
    }

    private void FirstStep ()
    {
      Assert.AreSame (MyTransaction, ClientTransactionScope.CurrentTransaction);

      DomainObjectParameterTestTransactedFunction childFunction = (DomainObjectParameterTestTransactedFunction) ChildFunction;
      ClassWithAllDataTypes inParameter = ClassWithAllDataTypes.GetObject (new DomainObjectIDs().ClassWithAllDataTypes2);
      inParameter.Delete ();
      childFunction.InParameter = inParameter;

      ClassWithAllDataTypes[] inParameterArray =
          new ClassWithAllDataTypes[] { inParameter };
      childFunction.InParameterArray = inParameterArray;
    }
  }
}

using System;
using NUnit.Framework;
using Remotion.Data.DomainObjects.UnitTests.Factories;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Web.ExecutionEngine;

namespace Remotion.Data.DomainObjects.UnitTests.Web.WxeFunctions
{
  public class DomainObjectParameterWithChildTestTransactedFunction : CreateRootWithChildTestTransactedFunctionBase
  {
    public DomainObjectParameterWithChildTestTransactedFunction ()
        : base (WxeTransactionMode.CreateRoot, new DomainObjectParameterTestTransactedFunction (WxeTransactionMode.CreateChildIfParent, null, null))
    {
      Insert (0, new WxeMethodStep (FirstStep));
      Add (new WxeMethodStep (LastStep));
    }

    private void FirstStep ()
    {
      Assert.AreSame (MyTransaction, ClientTransactionScope.CurrentTransaction);

      DomainObjectParameterTestTransactedFunction childFunction = (DomainObjectParameterTestTransactedFunction) ChildFunction;
      ClassWithAllDataTypes inParameter = ClassWithAllDataTypes.GetObject (new DomainObjectIDs().ClassWithAllDataTypes2);
      inParameter.Int32Property = 47;

      ClassWithAllDataTypes[] inParameterArray = new ClassWithAllDataTypes[] { inParameter };

      childFunction.InParameter = inParameter;
      childFunction.InParameterArray = inParameterArray;
    }

    private void LastStep ()
    {
      Assert.AreSame (MyTransaction, ClientTransactionScope.CurrentTransaction);

      DomainObjectParameterTestTransactedFunction childFunction = (DomainObjectParameterTestTransactedFunction) ChildFunction;
      ClassWithAllDataTypes outParameter = childFunction.OutParameter;
      ClassWithAllDataTypes[] outParameterArray = childFunction.OutParameterArray;

      Assert.IsTrue (outParameter.CanBeUsedInTransaction (ClientTransactionScope.CurrentTransaction));
      Assert.AreEqual (52, outParameter.Int32Property); // 47 + 5

      Assert.IsTrue (outParameterArray[0].CanBeUsedInTransaction (ClientTransactionScope.CurrentTransaction));
      Assert.AreEqual (52, outParameterArray[0].Int32Property); // 48 + 5
    }
  }
}

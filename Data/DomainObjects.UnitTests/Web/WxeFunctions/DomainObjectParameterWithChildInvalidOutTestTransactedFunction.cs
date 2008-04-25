using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Remotion.Data.DomainObjects.UnitTests.Factories;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Data.DomainObjects.Web.ExecutionEngine;
using Remotion.Web.ExecutionEngine;

namespace Remotion.Data.DomainObjects.UnitTests.Web.WxeFunctions
{
  public class DomainObjectParameterWithChildInvalidOutTestTransactedFunction : CreateRootWithChildTestTransactedFunctionBase
  {
    public DomainObjectParameterWithChildInvalidOutTestTransactedFunction ()
        : base (WxeTransactionMode.CreateRoot, new DomainObjectParameterInvalidOutTestTransactedFunction (WxeTransactionMode.CreateChildIfParent))
    {
    }
  }
}

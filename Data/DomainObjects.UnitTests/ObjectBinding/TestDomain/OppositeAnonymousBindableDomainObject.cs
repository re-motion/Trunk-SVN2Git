using System;
using System.Collections.Generic;
using System.Text;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.ObjectBinding.TestDomain
{
  [TestDomain]
  [Instantiable]
  [Serializable]
  [DBTable]
  public abstract class OppositeAnonymousBindableDomainObject : DomainObject
  {
  }
}

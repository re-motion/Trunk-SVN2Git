using System;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence
{
  public class StubValueConverterBase : ValueConverterBase
  {
    public StubValueConverterBase (TypeConversionProvider typeConversionProvider)
        : base(typeConversionProvider)
    {
    }
  }
}

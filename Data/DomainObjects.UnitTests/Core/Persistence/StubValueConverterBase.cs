using System;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.UnitTests.Core.Persistence
{
  public class StubValueConverterBase : ValueConverterBase
  {
    public StubValueConverterBase (TypeConversionProvider typeConversionProvider)
        : base(typeConversionProvider)
    {
    }
  }
}

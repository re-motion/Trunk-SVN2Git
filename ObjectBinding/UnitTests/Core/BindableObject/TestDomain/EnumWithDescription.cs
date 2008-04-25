using System;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.UnitTests.Core.BindableObject.TestDomain
{
  public enum EnumWithDescription
  {
    [EnumDescription ("Value I")]
    Value1 = 1,
     [EnumDescription ("Value II")]
    Value2 = 2,
    ValueWithoutDescription = 3
 }
}
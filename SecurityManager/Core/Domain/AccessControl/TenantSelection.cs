using System;
using Remotion.Utilities;

namespace Remotion.SecurityManager.Domain.AccessControl
{
  [EnumDescriptionResource ("Remotion.SecurityManager.Globalization.Domain.AccessControl.TenantSelection")]
  public enum TenantSelection
  {
    All = 0,
    OwningTenant = 1,
    SpecificTenant = 2
  }
}

using System;
using Remotion.Security;
using Remotion.Utilities;

namespace Remotion.SecurityManager.Domain.OrganizationalStructure
{
  [SecurityState]
  [EnumDescriptionResource ("Remotion.SecurityManager.Globalization.Domain.OrganizationalStructure.Delegation")]
  public enum Delegation
  {
    Disabled = 0,
    Enabled = 1
  }
}
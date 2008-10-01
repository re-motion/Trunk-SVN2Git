using System;
using System.Collections.Generic;
using Remotion.SecurityManager.Domain.AccessControl;

namespace Remotion.SecurityManager.AclTools.Expansion
{
  public interface IAclExpanderAclFinder
  {
    List<AccessControlList> FindAccessControlLists ();
  }
}
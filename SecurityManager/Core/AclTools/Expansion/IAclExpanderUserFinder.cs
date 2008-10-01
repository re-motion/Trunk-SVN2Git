using System;
using System.Collections.Generic;
using Remotion.SecurityManager.Domain.OrganizationalStructure;

namespace Remotion.SecurityManager.AclTools.Expansion
{
  public interface IAclExpanderUserFinder
  {
    List<User> FindUsers ();
  }
}
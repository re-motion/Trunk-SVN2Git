using System;
using System.Collections.Generic;
using System.Linq;
using Remotion.Data.DomainObjects.Linq;
using Remotion.SecurityManager.Domain.OrganizationalStructure;

namespace Remotion.SecurityManager.AclTools.Expansion
{
  public class AclExpanderUserFinder : IAclExpanderUserFinder
  {
    private List<User> _users;

    public List<User> Users {
      get {
        if (_users == null)
        {
          var result = from u in DataContext.Entity<User>()
                       orderby u.LastName , u.FirstName
                       select u;

          _users = result.ToList ();
        }
        return _users;
      }
    }
  }
}
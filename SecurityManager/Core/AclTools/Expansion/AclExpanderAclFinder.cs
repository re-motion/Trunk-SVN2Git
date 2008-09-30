using System;
using System.Collections.Generic;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.Domain.Metadata;

namespace Remotion.SecurityManager.AclTools.Expansion
{
  public class AclExpanderAclFinder : IAclExpanderAclFinder
  {
    private List<AccessControlList> _acls;

    public List<AccessControlList> AccessControlLists
    {
      get
      {
        if (_acls == null)
        {
          _acls = new List<AccessControlList> ();
          foreach (SecurableClassDefinition securableClassDefinition in SecurableClassDefinition.FindAll ())
          {
            _acls.AddRange (securableClassDefinition.AccessControlLists);
          }
        }
        return _acls;
      }
    }
  }
}
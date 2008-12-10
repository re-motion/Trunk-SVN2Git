// Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
// All rights reserved.
//
using Remotion.Development.UnitTesting;
using Remotion.SecurityManager.AclTools.Expansion;
using Remotion.SecurityManager.AclTools.Expansion.Infrastructure;

namespace Remotion.SecurityManager.UnitTests.AclTools.Expansion.TestClasses
{
  internal class AclExpanderXray 
  {
    public static IUserRoleAclAceCombinationFinder GetUserRoleAclAceCombinationFinder(AclExpander instance)
    {
      return (IUserRoleAclAceCombinationFinder) PrivateInvoke.GetNonPublicField (instance, "_userRoleAclAceCombinationFinder");
    }
  }
}
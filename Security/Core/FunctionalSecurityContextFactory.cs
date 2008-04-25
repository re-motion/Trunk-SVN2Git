using System;

namespace Remotion.Security
{
  [Serializable]
  public class FunctionalSecurityContextFactory : ISecurityContextFactory
  {
    // types

    // static members

    // member fields

    private readonly SecurityContext _context;

    // construction and disposing

    public FunctionalSecurityContextFactory (Type classType)
    {
      _context = new SecurityContext (classType);
    }
    
    // methods and properties

    public SecurityContext CreateSecurityContext ()
    {
      return _context;
    }
  }
}
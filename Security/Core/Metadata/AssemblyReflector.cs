using System;
using System.Reflection;
using Remotion.Utilities;

namespace Remotion.Security.Metadata
{

  public class AssemblyReflector
  {
    // types

    // static members

    // member fields

    private IAccessTypeReflector _accessTypeReflector;
    private IClassReflector _classReflector;
    private IAbstractRoleReflector _abstractRoleReflector;
    
    // construction and disposing

    public AssemblyReflector () : this (new AccessTypeReflector(), new ClassReflector (), new AbstractRoleReflector ())
    {
    }

    public AssemblyReflector (IAccessTypeReflector accessTypeReflector, IClassReflector classReflector, IAbstractRoleReflector abstractRoleReflector)
    {
      ArgumentUtility.CheckNotNull ("accessTypeReflector", accessTypeReflector);
      ArgumentUtility.CheckNotNull ("classReflector", classReflector);
      ArgumentUtility.CheckNotNull ("abstractRoleReflector", abstractRoleReflector);

      _accessTypeReflector = accessTypeReflector;
      _classReflector = classReflector;
      _abstractRoleReflector = abstractRoleReflector;
    }

    // methods and properties

    public IAccessTypeReflector AccessTypeReflector
    {
      get { return _accessTypeReflector; }
    }

    public IClassReflector ClassReflector
    {
      get { return _classReflector; }
    }

    public IAbstractRoleReflector AbstractRoleReflector
    {
      get { return _abstractRoleReflector; }
    }

    public void GetMetadata (Assembly assembly, MetadataCache cache)
    {
      ArgumentUtility.CheckNotNull ("assembly", assembly);
      ArgumentUtility.CheckNotNull ("cache", cache);
      
      Assembly securityAssembly = GetType().Assembly;
      _accessTypeReflector.GetAccessTypesFromAssembly (securityAssembly, cache);
      _accessTypeReflector.GetAccessTypesFromAssembly (assembly, cache);

      _abstractRoleReflector.GetAbstractRoles (securityAssembly, cache);
      _abstractRoleReflector.GetAbstractRoles (assembly, cache);

      foreach (Type type in assembly.GetTypes ())
      {
        if (typeof (ISecurableObject).IsAssignableFrom (type))
          _classReflector.GetMetadata (type, cache);
      }

    }
  }
}
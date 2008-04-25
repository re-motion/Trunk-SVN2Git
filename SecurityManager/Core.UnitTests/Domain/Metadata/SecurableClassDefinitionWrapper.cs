using System;
using System.Reflection;
using Remotion.Data.DomainObjects;
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.Utilities;

namespace Remotion.SecurityManager.UnitTests.Domain.Metadata
{
  public class SecurableClassDefinitionWrapper
  {
    // types

    // static members

    // member fields

    private SecurableClassDefinition _securableClassDefinition;
    private PropertyInfo _accessTypeReferencesPropertyInfo;

    // construction and disposing

    public SecurableClassDefinitionWrapper (SecurableClassDefinition securableClassDefinition)
    {
      ArgumentUtility.CheckNotNull ("securableClassDefinition", securableClassDefinition);

      _securableClassDefinition = securableClassDefinition;
      _accessTypeReferencesPropertyInfo = _securableClassDefinition.GetPublicDomainObjectType().GetProperty (
          "AccessTypeReferences",
          BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
    }

    // methods and properties

    public SecurableClassDefinition SecurableClassDefinition
    {
      get { return _securableClassDefinition; }
    }

    public DomainObjectCollection AccessTypeReferences
    {
      get { return (DomainObjectCollection) _accessTypeReferencesPropertyInfo.GetValue (_securableClassDefinition, new object[0]); }
    }
  }
}
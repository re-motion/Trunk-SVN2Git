using System;
using Remotion.Data.DomainObjects.Mapping;

namespace Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader
{
  /// <summary>
  /// The <see cref="RdbmsClassReflector"/> is used introduce <b>RDBMS</b> specific information into the building the 
  /// <see cref="ReflectionBasedClassDefinition"/> and the <see cref="RelationDefinition"/> objects.
  /// </summary>
  public class RdbmsClassReflector : ClassReflector
  {
    public RdbmsClassReflector (Type type)
        : base (type)
    {
    }

    protected override string GetStorageSpecificIdentifier()
    {
      if (IsTable())
        return base.GetStorageSpecificIdentifier();
      return null;
    }

    private bool IsTable()
    {
      return Attribute.IsDefined (Type, typeof (DBTableAttribute), false);
    }
  }
}
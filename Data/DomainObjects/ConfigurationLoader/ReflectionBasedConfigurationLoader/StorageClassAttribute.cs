using System;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader
{
  /// <summary>Defines if and how a property is managed by the persistence framework.</summary>
  [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
  public class StorageClassAttribute : Attribute, IMappingAttribute
  {
    private StorageClass _storageClass;

    public StorageClassAttribute (StorageClass storageClass)
    {
      ArgumentUtility.CheckValidEnumValue ("storageClass", storageClass);
      _storageClass = storageClass;
    }

    public StorageClass StorageClass
    {
      get { return _storageClass; }
    }
  }
}
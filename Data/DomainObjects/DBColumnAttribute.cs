using System;
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects
{
  /// <summary>Overrides the name used as the column name in the <b>RDBMS</b>.</summary>
  [AttributeUsage (AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
  public class DBColumnAttribute : Attribute, IStorageSpecificIdentifierAttribute
  {
    private string _name;

    /// <summary>Initializes a new instance of the <see cref="DBColumnAttribute"/> class.</summary>
    /// <param name="name">The name. Must not be <see langword="null" /> or empty.</param>
    public DBColumnAttribute (string name)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("name", name);
      _name = name;
    }

    /// <summary>
    /// Gets the column name defined by this <see cref="DBColumnAttribute"/>.
    /// </summary>
    public string Name
    {
      get { return _name; }
    }

    string IStorageSpecificIdentifierAttribute.Identifier
    {
      get { return Name; }
    }
  }
}
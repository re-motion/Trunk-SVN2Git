using System;
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects
{
  /// <summary>
  /// Use the <see cref="DBTableAttribute"/> to define the distribution of the types within a persistence hierarchy into one or more database tables.
  /// </summary>
  [AttributeUsage (AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
  public class DBTableAttribute: Attribute, IStorageSpecificIdentifierAttribute
  {
    private string _name;

    /// <summary>
    /// Initializes a new instance of the <see cref="DBTableAttribute"/> class.
    /// </summary>
    public DBTableAttribute ()
    {
    }

    /// <summary>Initializes a new instance of the <see cref="DBTableAttribute"/> class with a custom table name.</summary>
    /// <param name="name">The name of the table. Must not be <see langword="null" /> or empty.</param>
    public DBTableAttribute (string name)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("name", name);
      _name = name;
    }

    /// <summary>
    /// Gets the table name defined by this <see cref="DBTableAttribute"/>.
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
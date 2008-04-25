using System;

namespace Remotion.Data.DomainObjects
{
  /// <summary>The <see cref="StorageGroupAttribute"/> is the base class for defining storage groups in the domain layer.</summary>
  /// <remarks>
  /// <para>
  /// A storage group is a logical grouping of all classes within a domain that should be persisted with the same storage provider.
  /// </para><para>
  /// Define the <see cref="StorageGroupAttribute"/> at the base classes of the domain layer to signify the root for the persistence hierarchy.
  /// </para><para>
  /// If no storage group is deifned for a persistence hierarchy, the domain object classes are assigned to the default storage provider.
  /// </para> 
  /// </remarks>
  [AttributeUsage (AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
  public abstract class StorageGroupAttribute: Attribute
  {
    protected StorageGroupAttribute()
    {
    }
  }
}
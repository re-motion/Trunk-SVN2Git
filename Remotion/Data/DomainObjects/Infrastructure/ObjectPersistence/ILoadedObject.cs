namespace Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence
{
  /// <summary>
  /// Represents an object loaded via an implementation of <see cref="IPersistenceStrategy"/>.
  /// </summary>
  public interface ILoadedObject
  {
    ObjectID ObjectID { get; }

    void Accept (ILoadedObjectVisitor visitor);
  }
}
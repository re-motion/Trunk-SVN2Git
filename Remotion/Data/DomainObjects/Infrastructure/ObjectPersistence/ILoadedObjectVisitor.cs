namespace Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence
{
  /// <summary>
  /// Provides a method to dispatch on <see cref="ILoadedObject"/> implementations.
  /// </summary>
  public interface ILoadedObjectVisitor
  {
    void VisitFreshlyLoadedObject (FreshlyLoadedObject freshlyLoadedObject);
    void VisitAlreadyExistingLoadedObject (AlreadyExistingLoadedObject alreadyExistingLoadedObject);
  }
}
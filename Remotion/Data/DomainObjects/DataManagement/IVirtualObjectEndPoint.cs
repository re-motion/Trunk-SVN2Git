namespace Remotion.Data.DomainObjects.DataManagement
{
  /// <summary>
  /// Represents a virtual <see cref="IRelationEndPoint"/> holding a single opposite <see cref="ObjectID"/>, i.e. the non-foreign key side in a 
  /// 1:1 relation.
  /// </summary>
  public interface IVirtualObjectEndPoint : IVirtualEndPoint<ObjectID>, IObjectEndPoint
  {
    void MarkDataComplete (DomainObject item);
  }
}
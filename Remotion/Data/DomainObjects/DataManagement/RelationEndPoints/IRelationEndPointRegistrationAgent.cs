using System;

namespace Remotion.Data.DomainObjects.DataManagement.RelationEndPoints
{
  /// <summary>
  /// Defines an interface for classes registering and unregistering end-points in/from a <see cref="RelationEndPointMap"/>.
  /// </summary>
  public interface IRelationEndPointRegistrationAgent
  {
    void RegisterEndPoint (IRelationEndPoint endPoint);
    void UnregisterEndPoint (IRelationEndPoint endPoint);
    bool IsUnregisterable (IRelationEndPoint endPoint);
  }
}
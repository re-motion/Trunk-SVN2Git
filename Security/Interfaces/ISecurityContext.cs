using System;

namespace Remotion.Security
{
  public interface ISecurityContext : IEquatable<ISecurityContext>
  {
    string Class { get; }

    string Owner { get; }

    string OwnerGroup { get; }

    string OwnerTenant { get; }

    EnumWrapper[] AbstractRoles { get; }

    EnumWrapper GetState (string propertyName);
    bool ContainsState (string propertyName);

    bool IsStateless { get; }

    int GetNumberOfStates ();
  }
}
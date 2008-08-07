using System;
using Remotion.SecurityManager.Domain.Metadata;

namespace Remotion.SecurityManager.Domain.AccessControl
{
  public interface IStateCombinationBuilder
  {
    SecurableClassDefinition ClassDefinition { get; }
    PropertyStateTuple[][] CreatePropertyProduct ();
  }
}
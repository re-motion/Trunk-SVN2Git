using System;

namespace Remotion.Data.DomainObjects.UnitTests.Core.EventReceiver
{
  [Serializable]
  public class ObjectDeletionState : ChangeState
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    public ObjectDeletionState (object sender)
      : this (sender, null)
    {
    }

    public ObjectDeletionState (object sender, string message)
      : base (sender, message)
    {
    }

    // methods and properties
  }
}

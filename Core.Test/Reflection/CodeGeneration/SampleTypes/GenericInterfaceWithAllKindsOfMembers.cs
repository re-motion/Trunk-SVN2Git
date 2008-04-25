using System;

namespace Remotion.UnitTests.Reflection.CodeGeneration.SampleTypes
{
  public interface GenericInterfaceWithAllKindsOfMembers<T>
  {
    string Method (T t);
    T Property { get; }
    event Func<T> Event;
  }
}
using System;
using System.Reflection;
using Remotion.Collections;
using Remotion.Security.Metadata;

namespace Remotion.Security.UnitTests.Core.Metadata.PermissionReflectorTests
{
  public class TestPermissionReflector : PermissionReflector
  {
    // types

    // static members

    public new static Cache<Tuple<Type, Type, string, BindingFlags>, Enum[]> Cache
    {
      get { return PermissionReflector.Cache; }
    }

    // member fields

    // construction and disposing

    public TestPermissionReflector ()
    {
    }

    // methods and properties
  }
}
using System;

namespace Remotion.UnitTests.Utilities.AttributeUtilityTests
{
  [AttributeUsage (AttributeTargets.All, Inherited = false, AllowMultiple = false)]
  public class NotInheritedNotMultipleAttribute : Attribute, ICustomAttribute
  { }
}
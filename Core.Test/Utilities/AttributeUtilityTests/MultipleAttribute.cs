using System;

namespace Remotion.UnitTests.Utilities.AttributeUtilityTests
{
  [AttributeUsage(AttributeTargets.All, Inherited = true, AllowMultiple = true)]
  public class MultipleAttribute : Attribute, ICustomAttribute
  { }
}
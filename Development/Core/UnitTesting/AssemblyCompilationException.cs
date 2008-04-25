using System;
using System.Runtime.Serialization;

namespace Remotion.Development.UnitTesting
{
  /// <summary>
  /// The <see cref="AssemblyCompilationException"/> is thrown by the <see cref="AssemblyCompiler"/> type when compilation errors occured.
  /// </summary>
  [Serializable]
  public class AssemblyCompilationException : Exception
  {
    public AssemblyCompilationException (string message)
        : base (message)
    {
    }

    public AssemblyCompilationException (string message, Exception inner)
        : base (message, inner)
    {
    }

    protected AssemblyCompilationException (SerializationInfo info, StreamingContext context)
        : base (info, context)
    {
    }
  }
}
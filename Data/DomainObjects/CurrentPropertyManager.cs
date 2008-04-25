using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects
{
  /// <summary>
  /// Manages a stack of property names per thread.
  /// </summary>
  static class CurrentPropertyManager
  {
    [ThreadStatic]
    private static Stack<string> _currentPropertyNames;

    private static Stack<string> CurrentPropertyNames
    {
      get
      {
        if (_currentPropertyNames == null)
        {
          _currentPropertyNames = new Stack<string> ();
        }
        return _currentPropertyNames;
      }
    }

    /// <summary>
    /// Returns the property name last put on this thread's stack, or null if the stack is empty.
    /// </summary>
    public static string CurrentPropertyName
    {
      get {
        if (CurrentPropertyNames.Count == 0)
        {
          return null;
        }
        else
        {
          return _currentPropertyNames.Peek ();
        }
      }
    }

    /// <summary>
    /// Pushes a new property name on this thread's stack.
    /// </summary>
    /// <param name="propertyName">The name to put on the stack.</param>
    /// <exception cref="System.ArgumentNullException"><paramref name="propertyName"/> is <see langword="null"/>.</exception>
    /// <exception cref="Remotion.Utilities.ArgumentEmptyException"><paramref name="propertyName"/> is an empty string.</exception>
    public static void PreparePropertyAccess (string propertyName)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);
      CurrentPropertyNames.Push (propertyName);
    }

    /// <summary>
    /// Pops the last property name from this thread's stack.
    /// </summary>
    /// <exception cref="InvalidOperationException">There is no property name on the stack.</exception>
    public static void PropertyAccessFinished ()
    {
      if (CurrentPropertyNames.Count == 0)
      {
        throw new InvalidOperationException ("There is no property to finish.");
      }
      CurrentPropertyNames.Pop();
    }
  }
}

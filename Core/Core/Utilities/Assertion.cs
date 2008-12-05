// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Remotion.Utilities
{
  /// <summary>
  /// This exception is thrown when an assertion fails.
  /// </summary>
  /// <seealso cref="Assertion"/>
  [Serializable]
  public class AssertionException : Exception
  {
    public AssertionException (SerializationInfo info, StreamingContext context)
      : base (info, context)
    {
    }

    public AssertionException (string message, Exception innerException)
      : base (message, innerException)
    {
    }

    public AssertionException (string message)
      : this (message, null)
    {
    }
  }

  /// <summary>
  /// Provides methods that throw an <see cref="AssertionException"/> if an assertion fails.
  /// </summary>
  /// <remarks>
  ///   <para>
  ///   This class contains methods that are conditional to the DEBUG and TRACE attributes (<see cref="DebugAssert"/> and <see cref="TraceAssert"/>). 
  ///   </para><para>
  ///   Note that assertion expressions passed to these methods are not evaluated (read: executed) if the respective symbol are not defined during
  ///   compilation, nor are the methods called. This increases performance for production builds, but make sure that your assertion expressions do
  ///   not cause any side effects! See <see cref="ConditionalAttribute"/> or <see cref="Debug"/> and <see cref="Trace"/> the for more information 
  ///   about conditional compilation.
  ///   </para><para>
  ///   Assertions are no replacement for checking input parameters of public methods (see <see cref="ArgumentUtility"/>).  
  ///   </para>
  /// </remarks>
  public static class Assertion
  {
    private const string c_msgIsTrue = "Assertion failed: Expression evaluates to true.";
    private const string c_msgIsFalse = "Assertion failed: Expression evaluates to false.";
    private const string c_msgIsNull = "Assertion failed: Expression evaluates to a null reference.";
    private const string c_msgIsNotNull = "Assertion failed: Expression does not evaluate to a null reference.";

    [Conditional ("DEBUG")]
    public static void DebugAssert (bool assertion, string message)
    {
      IsTrue (assertion, message);
    }

    [Conditional ("DEBUG")]
    public static void DebugAssert (bool assertion, string message, params object[] arguments)
    {
      IsTrue (assertion, message, arguments);
    }

    [Conditional ("DEBUG")]
    public static void DebugAssert (bool assertion)
    {
      IsTrue (assertion);
    }

    [Conditional ("TRACE")]
    public static void TraceAssert (bool assertion, string message)
    {
      IsTrue (assertion, message);
    }

    [Conditional ("TRACE")]
    public static void TraceAssert (bool assertion, string message, params object[] arguments)
    {
      IsTrue (assertion, message, arguments);
    }

    [Conditional ("TRACE")]
    public static void TraceAssert (bool assertion)
    {
      IsTrue (assertion);
    }


    public static void IsTrue (bool assertion, string message)
    {
      if (! assertion)
        throw new AssertionException (message);
    }

    public static void IsTrue (bool assertion, string message, params object[] arguments)
    {
      IsTrue (assertion, string.Format (message, arguments));
    }

    public static void IsTrue (bool assertion)
    {
      IsTrue (assertion, c_msgIsFalse);
    }

    
    public static void IsFalse (bool expression, string message)
    {
      if (expression)
        throw new AssertionException (message);
    }

    public static void IsFalse (bool expression)
    {
      IsFalse (expression, c_msgIsTrue);
    }

    public static void IsFalse (bool expression, string message, params bool[] arguments)
    {
      IsFalse (expression, string.Format (message, arguments));
    }


    public static void IsNotNull (object obj, string message)
    {
      if (obj == null)
        throw new AssertionException (message);
    }

    public static void IsNotNull (object obj)
    {
      IsNotNull (obj, c_msgIsNull);
    }

    public static void IsNotNull (object obj, string message, params object[] arguments)
    {
      IsNotNull (obj, string.Format (message, arguments));
    }


    public static void IsNull (object obj, string message)
    {
      if (obj != null)
        throw new AssertionException (message);
    }

    public static void IsNull (object obj)
    {
      IsNull (obj, c_msgIsNotNull);
    }

    public static void IsNull (object obj, string message, params object[] arguments)
    {
      IsNull (obj, string.Format (message, arguments));
    }
  }
}

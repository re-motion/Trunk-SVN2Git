// This file is part of the re-linq project (relinq.codeplex.com)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// re-linq is free software; you can redistribute it and/or modify it under 
// the terms of the GNU Lesser General Public License as published by the 
// Free Software Foundation; either version 2.1 of the License, 
// or (at your option) any later version.
// 
// re-linq is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-linq; if not, see http://www.gnu.org/licenses.
// 

using System;
using System.Diagnostics;
using JetBrains.Annotations;

// ReSharper disable once CheckNamespace
namespace Remotion.Utilities
{
  /// <summary>
  /// Provides methods that throw an <see cref="InvalidOperationException"/> if an assertion fails.
  /// </summary>
  /// <remarks>
  ///   <para>
  ///   This class contains methods that are conditional to the DEBUG and TRACE attributes (<see cref="DebugAssert(bool)"/> and <see cref="TraceAssert(bool)"/>). 
  ///   </para><para>
  ///   Note that assertion expressions passed to these methods are not evaluated (read: executed) if the respective symbol are not defined during
  ///   compilation, nor are the methods called. This increases performance for production builds, but make sure that your assertion expressions do
  ///   not cause any side effects! See <see cref="ConditionalAttribute"/> or <see cref="Debug"/> and <see cref="Trace"/> the for more information 
  ///   about conditional compilation.
  ///   </para><para>
  ///   Assertions are no replacement for checking input parameters of public methods (see <see cref="ArgumentUtility"/>).  
  ///   </para>
  /// </remarks>
  static partial class Assertion
  {
    private const string c_msgIsTrue = "Assertion failed: Expression evaluates to true.";
    private const string c_msgIsFalse = "Assertion failed: Expression evaluates to false.";
    private const string c_msgIsNull = "Assertion failed: Expression evaluates to a null reference.";
    private const string c_msgIsNotNull = "Assertion failed: Expression does not evaluate to a null reference.";
    private static readonly object[] s_emptyArguments = new object[0];

    [Conditional ("DEBUG")]
    [AssertionMethod]
    public static void DebugAssert ([AssertionCondition (AssertionConditionType.IS_TRUE)] bool assertion, string message)
    {
      IsTrue (assertion, message);
    }

    [Conditional ("DEBUG")]
    [AssertionMethod]
    [StringFormatMethod("message")]
    public static void DebugAssert ([AssertionCondition (AssertionConditionType.IS_TRUE)] bool assertion, string message, params object[] arguments)
    {
      IsTrue (assertion, message, arguments);
    }

    [Conditional ("DEBUG")]
    [AssertionMethod]
    public static void DebugAssert ([AssertionCondition (AssertionConditionType.IS_TRUE)] bool assertion)
    {
      IsTrue (assertion);
    }

    [Conditional ("TRACE")]
    [AssertionMethod]
    public static void TraceAssert ([AssertionCondition (AssertionConditionType.IS_TRUE)] bool assertion, string message)
    {
      IsTrue (assertion, message);
    }

    [Conditional ("TRACE")]
    [AssertionMethod]
    [StringFormatMethod ("message")]
    public static void TraceAssert ([AssertionCondition (AssertionConditionType.IS_TRUE)] bool assertion, string message, params object[] arguments)
    {
      IsTrue (assertion, message, arguments);
    }

    [Conditional ("TRACE")]
    [AssertionMethod]
    public static void TraceAssert ([AssertionCondition (AssertionConditionType.IS_TRUE)] bool assertion)
    {
      IsTrue (assertion);
    }

    [AssertionMethod]
    public static void IsTrue ([AssertionCondition (AssertionConditionType.IS_TRUE)] bool assertion, string message)
    {
      IsTrue (assertion, message, s_emptyArguments);
    }

    [AssertionMethod]
    [StringFormatMethod("message")]
    public static void IsTrue ([AssertionCondition (AssertionConditionType.IS_TRUE)] bool assertion, string message, params object[] arguments)
    {
      if (!assertion)
        throw new InvalidOperationException (string.Format (message, arguments));
    }

    [AssertionMethod]
    public static void IsTrue ([AssertionCondition (AssertionConditionType.IS_TRUE)] bool assertion)
    {
      IsTrue (assertion, c_msgIsFalse);
    }

    [AssertionMethod]
    public static void IsFalse ([AssertionCondition (AssertionConditionType.IS_FALSE)] bool expression, string message)
    {
      IsFalse (expression, message, s_emptyArguments);
    }

    [AssertionMethod]
    public static void IsFalse ([AssertionCondition (AssertionConditionType.IS_FALSE)] bool expression)
    {
      IsFalse (expression, c_msgIsTrue);
    }

    [AssertionMethod]
    [StringFormatMethod ("message")]
    public static void IsFalse ([AssertionCondition (AssertionConditionType.IS_FALSE)] bool expression, string message, params object[] arguments)
    {
      if (expression)
        throw new InvalidOperationException (string.Format (message, arguments));
    }

    [AssertionMethod]
    public static T IsNotNull<T> ([AssertionCondition (AssertionConditionType.IS_NOT_NULL)] T obj, string message)
    {
      return IsNotNull (obj, message, s_emptyArguments);
    }

    [AssertionMethod]
    public static T IsNotNull<T> ([AssertionCondition (AssertionConditionType.IS_NOT_NULL)] T obj)
    {
      return IsNotNull (obj, c_msgIsNull);
    }

    [AssertionMethod]
    [StringFormatMethod ("message")]
    public static T IsNotNull<T> ([AssertionCondition (AssertionConditionType.IS_NOT_NULL)] T obj, string message, params object[] arguments)
    {
      // ReSharper disable CompareNonConstrainedGenericWithNull
      if (obj == null)
        // ReSharper restore CompareNonConstrainedGenericWithNull
        throw new InvalidOperationException (string.Format (message, arguments));

      return obj;
    }

    [AssertionMethod]
    public static void IsNull ([AssertionCondition (AssertionConditionType.IS_NULL)] object obj, string message)
    {
      IsNull (obj, message, s_emptyArguments);
    }

    [AssertionMethod]
    public static void IsNull ([AssertionCondition (AssertionConditionType.IS_NULL)] object obj)
    {
      IsNull (obj, c_msgIsNotNull);
    }

    [AssertionMethod]
    [StringFormatMethod("message")]
    public static void IsNull ([AssertionCondition (AssertionConditionType.IS_NULL)] object obj, string message, params object[] arguments)
    {
      if (obj != null)
        throw new InvalidOperationException (string.Format (message, arguments));
    }
  }
}

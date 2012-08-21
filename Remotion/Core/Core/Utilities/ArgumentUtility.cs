// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
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
using System.Collections;
using JetBrains.Annotations;

namespace Remotion.Utilities
{
  /// <summary>
  /// This utility class provides methods for checking arguments.
  /// </summary>
  /// <remarks>
  /// Some methods of this class return the value of the parameter. In some cases, this is useful because the value will be converted to another 
  /// type:
  /// <code><![CDATA[
  /// void foo (object o) 
  /// {
  ///   int i = ArgumentUtility.CheckNotNullAndType<int> ("o", o);
  /// }
  /// ]]></code>
  /// In some other cases, the input value is returned unmodified. This makes it easier to use the argument checks in calls to base class constructors
  /// or property setters:
  /// <code><![CDATA[
  /// class MyType : MyBaseType
  /// {
  ///   public MyType (string name) : base (ArgumentUtility.CheckNotNullOrEmpty ("name", name))
  ///   {
  ///   }
  /// 
  ///   public override Name
  ///   {
  ///     set { base.Name = ArgumentUtility.CheckNotNullOrEmpty ("value", value); }
  ///   }
  /// }
  /// ]]></code>
  /// </remarks>
  public static class ArgumentUtility
  {
    // Duplicated in Remotion.Linq.Utilities.ArgumentUtility
    [AssertionMethod]
    public static T CheckNotNull<T> (
        [InvokerParameterName] string argumentName, 
        [AssertionCondition (AssertionConditionType.IS_NOT_NULL)] T actualValue)
    {
      // ReSharper disable CompareNonConstrainedGenericWithNull
      if (actualValue == null)
          // ReSharper restore CompareNonConstrainedGenericWithNull
        throw new ArgumentNullException (argumentName);

      return actualValue;
    }

    // Duplicated in Remotion.Linq.Utilities.ArgumentUtility
    [AssertionMethod]
    public static string CheckNotNullOrEmpty (
        [InvokerParameterName] string argumentName, 
        [AssertionCondition (AssertionConditionType.IS_NOT_NULL)] string actualValue)
    {
      CheckNotNull (argumentName, actualValue);
      if (actualValue.Length == 0)
        throw new ArgumentEmptyException (argumentName);

      return actualValue;
    }

    [AssertionMethod]
    public static T CheckNotNullOrEmpty<T> (
        [InvokerParameterName] string argumentName, 
        [AssertionCondition (AssertionConditionType.IS_NOT_NULL)] T enumerable)
        where T: IEnumerable
    {
      CheckNotNull (argumentName, enumerable);
      CheckNotEmpty (argumentName, enumerable);

      return enumerable;
    }

    [AssertionMethod]
    public static T CheckNotNullOrItemsNull<T> (
        [InvokerParameterName] string argumentName, 
        [AssertionCondition (AssertionConditionType.IS_NOT_NULL)] T enumerable)
        where T: IEnumerable
    {
      CheckNotNull (argumentName, enumerable);

      int i = 0;
      foreach (object item in enumerable)
      {
        if (item == null)
          throw new ArgumentItemNullException (argumentName, i);
        ++i;
      }

      return enumerable;
    }

    [AssertionMethod]
    public static T CheckNotNullOrEmptyOrItemsNull<T> (
        [InvokerParameterName] string argumentName, 
        [AssertionCondition (AssertionConditionType.IS_NOT_NULL)] T enumerable)
        where T: IEnumerable
    {
      CheckNotNullOrItemsNull (argumentName, enumerable);
      CheckNotEmpty (argumentName, enumerable);

      return enumerable;
    }

    [AssertionMethod]
    public static string CheckNotEmpty ([InvokerParameterName] string argumentName, string actualValue)
    {
      if (actualValue != null && actualValue.Length == 0)
        throw new ArgumentEmptyException (argumentName);

      return actualValue;
    }

    [AssertionMethod]
    public static T CheckNotEmpty<T> ([InvokerParameterName] string argumentName, T enumerable)
        where T: IEnumerable
    {
      // ReSharper disable CompareNonConstrainedGenericWithNull
      if (enumerable != null)
          // ReSharper restore CompareNonConstrainedGenericWithNull
      {
        var collection = enumerable as ICollection;
        if (collection != null)
        {
          if (collection.Count == 0)
            throw new ArgumentEmptyException (argumentName);
          else
            return enumerable;
        }

        IEnumerator enumerator = enumerable.GetEnumerator();
        var disposableEnumerator = enumerator as IDisposable;
        using (disposableEnumerator) // using (null) is allowed in C#
        {
          if (!enumerator.MoveNext())
            throw new ArgumentEmptyException (argumentName);
        }
      }

      return enumerable;
    }

    [AssertionMethod]
    public static Guid CheckNotEmpty ([InvokerParameterName] string argumentName, Guid actualValue)
    {
      if (actualValue == Guid.Empty)
        throw new ArgumentEmptyException (argumentName);

      return actualValue;
    }

    public static void ThrowEnumArgumentOutOfRangeException ([InvokerParameterName] string argumentName, Enum actualValue)
    {
      string message = string.Format (
          "The value of argument {0} is not a valid value of the type {1}. Actual value was {2}.",
          argumentName,
          actualValue.GetType(),
          actualValue);
      throw new ArgumentOutOfRangeException (argumentName, actualValue, message);
    }

    public static object CheckNotNullAndType (
        [InvokerParameterName] string argumentName, 
        [AssertionCondition (AssertionConditionType.IS_NOT_NULL)] object actualValue, 
        Type expectedType)
    {
      if (actualValue == null)
        throw new ArgumentNullException (argumentName);

      if (!expectedType.IsInstanceOfType (actualValue))
        throw new ArgumentTypeException (argumentName, expectedType, actualValue.GetType());
      return actualValue;
    }

    ///// <summary>Returns the value itself if it is not <see langword="null"/> and of the specified value type.</summary>
    ///// <typeparam name="TExpected"> The type that <paramref name="actualValue"/> must have. </typeparam>
    ///// <exception cref="ArgumentNullException"> <paramref name="actualValue"/> is <see langword="null"/>.</exception>
    ///// <exception cref="ArgumentTypeException"> <paramref name="actualValue"/> is an instance of another type (which is not a subclass of <typeparamref name="TExpected"/>).</exception>
    //public static TExpected CheckNotNullAndType<TExpected> (string argumentName, object actualValue)
    //  where TExpected: class
    //{
    //  if (actualValue == null)
    //    throw new ArgumentNullException (argumentName);
    //  return CheckType<TExpected> (argumentName, actualValue);
    //}

    /// <summary>Returns the value itself if it is not <see langword="null"/> and of the specified value type.</summary>
    /// <typeparam name="TExpected"> The type that <paramref name="actualValue"/> must have. </typeparam>
    /// <exception cref="ArgumentNullException">The <paramref name="actualValue"/> is a <see langword="null"/>.</exception>
    /// <exception cref="ArgumentTypeException">The <paramref name="actualValue"/> is an instance of another type.</exception>
    // Duplicated in Remotion.Linq.Utilities.ArgumentUtility
    public static TExpected CheckNotNullAndType<TExpected> (
        [InvokerParameterName] string argumentName, 
        [AssertionCondition (AssertionConditionType.IS_NOT_NULL)] object actualValue)
        // where TExpected: struct
    {
      if (actualValue == null)
        throw new ArgumentNullException (argumentName);

      if (! (actualValue is TExpected))
        throw new ArgumentTypeException (argumentName, typeof (TExpected), actualValue.GetType());
      return (TExpected) actualValue;
    }

    public static object CheckType ([InvokerParameterName] string argumentName, object actualValue, Type expectedType)
    {
      if (actualValue == null)
      {
        if (NullableTypeUtility.IsNullableType_NoArgumentCheck (expectedType))
          return null;
        else
          throw new ArgumentTypeException (argumentName, expectedType, null);
      }

      if (!expectedType.IsInstanceOfType (actualValue))
        throw new ArgumentTypeException (argumentName, expectedType, actualValue.GetType());
      return actualValue;
    }

    /// <summary>Returns the value itself if it is of the specified type.</summary>
    /// <typeparam name="TExpected"> The type that <paramref name="actualValue"/> must have. </typeparam>
    /// <exception cref="ArgumentTypeException"> 
    ///     <paramref name="actualValue"/> is an instance of another type (which is not a subtype of <typeparamref name="TExpected"/>).</exception>
    /// <exception cref="ArgumentNullException"> 
    ///     <paramref name="actualValue" /> is null and <typeparamref name="TExpected"/> cannot be null. </exception>
    /// <remarks>
    ///   For non-nullable value types, you should use either <see cref="CheckNotNullAndType{TExpected}"/> or pass the type 
    ///   <see cref="Nullable{T}" /> instead.
    /// </remarks>
    public static TExpected CheckType<TExpected> ([InvokerParameterName] string argumentName, object actualValue)
    {
      if (actualValue == null)
      {
        try
        {
          return (TExpected) actualValue;
        }
        catch (NullReferenceException)
        {
          throw new ArgumentNullException (argumentName);
        }
      }

      if (!(actualValue is TExpected))
        throw new ArgumentTypeException (argumentName, typeof (TExpected), actualValue.GetType());

      return (TExpected) actualValue;
    }


    /// <summary>Checks whether <paramref name="actualType"/> is not <see langword="null"/> and can be assigned to <paramref name="expectedType"/>.</summary>
    /// <exception cref="ArgumentNullException">The <paramref name="actualType"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentTypeException">The <paramref name="actualType"/> cannot be assigned to <paramref name="expectedType"/>.</exception>
    public static Type CheckNotNullAndTypeIsAssignableFrom (
        [InvokerParameterName] string argumentName,
        [AssertionCondition (AssertionConditionType.IS_NOT_NULL)] Type actualType,
        [AssertionCondition (AssertionConditionType.IS_NOT_NULL)] Type expectedType)
    {
      if (actualType == null)
        throw new ArgumentNullException (argumentName);
      return CheckTypeIsAssignableFrom (argumentName, actualType, expectedType);
    }

    /// <summary>Checks whether <paramref name="actualType"/> can be assigned to <paramref name="expectedType"/>.</summary>
    /// <exception cref="ArgumentTypeException">The <paramref name="actualType"/> cannot be assigned to <paramref name="expectedType"/>.</exception>
    // Duplicated in Remotion.Linq.Utilities.ArgumentUtility
    public static Type CheckTypeIsAssignableFrom (
        [InvokerParameterName] string argumentName, 
        Type actualType, 
        [AssertionCondition (AssertionConditionType.IS_NOT_NULL)] Type expectedType)
    {
      CheckNotNull ("expectedType", expectedType);
      if (actualType != null)
      {
        if (!expectedType.IsAssignableFrom (actualType))
        {
          string message = string.Format ("Argument {0} is a {2}, which cannot be assigned to type {1}.", argumentName, expectedType, actualType);
          throw new ArgumentTypeException (message, argumentName, expectedType, actualType);
        }
      }

      return actualType;
    }

    /// <summary>Checks whether all items in <paramref name="collection"/> are of type <paramref name="itemType"/> or a null reference.</summary>
    /// <exception cref="ArgumentItemTypeException"> If at least one element is not of the specified type or a derived type. </exception>
    public static T CheckItemsType<T> ([InvokerParameterName] string argumentName, T collection, Type itemType)
        where T: ICollection
    {
      // ReSharper disable CompareNonConstrainedGenericWithNull
      if (collection != null)
          // ReSharper restore CompareNonConstrainedGenericWithNull
      {
        int index = 0;
        foreach (object item in collection)
        {
          if (item != null && !itemType.IsInstanceOfType (item))
            throw new ArgumentItemTypeException (argumentName, index, itemType, item.GetType());
          ++index;
        }
      }

      return collection;
    }

    /// <summary>Checks whether all items in <paramref name="collection"/> are of type <paramref name="itemType"/> and not null references.</summary>
    /// <exception cref="ArgumentItemTypeException"> If at least one element is not of the specified type or a derived type. </exception>
    /// <exception cref="ArgumentItemNullException"> If at least one element is a null reference. </exception>
    public static T CheckItemsNotNullAndType<T> ([InvokerParameterName] string argumentName, T collection, Type itemType)
        where T: ICollection
    {
      // ReSharper disable CompareNonConstrainedGenericWithNull
      if (collection != null)
          // ReSharper restore CompareNonConstrainedGenericWithNull
      {
        int index = 0;
        foreach (object item in collection)
        {
          if (item == null)
            throw new ArgumentItemNullException (argumentName, index);
          if (!itemType.IsInstanceOfType (item))
            throw new ArgumentItemTypeException (argumentName, index, itemType, item.GetType());
          ++index;
        }
      }

      return collection;
    }

    /// <summary>Checks whether <paramref name="enumValue"/> is defined within its enumeration type.</summary>
    /// <exception cref="ArgumentNullException"> If <paramref name="enumValue"/> is a null reference. </exception>
    /// <exception cref="ArgumentOutOfRangeException"> If <paramref name="enumValue"/> has a numeric value that is not completely defined within its 
    /// enumeration type. For flag types, every bit must correspond to at least one enumeration value. </exception>
    public static Enum CheckValidEnumValue (
        [InvokerParameterName] string argumentName, 
        [AssertionCondition (AssertionConditionType.IS_NOT_NULL)] Enum enumValue)
    {
      if (enumValue == null)
        throw new ArgumentNullException (argumentName);

      if (! EnumUtility.IsValidEnumValue (enumValue))
        throw new ArgumentOutOfRangeException (argumentName);

      return enumValue;
    }

    /// <summary>Checks whether <paramref name="enumValue"/> is of the enumeration type <typeparamref name="TEnum"/> and defined within this type.</summary>
    /// <remarks>
    /// When successful, the value is returned as a <c>Nullable</c> of the specified type for direct assignment. 
    /// </remarks>
    /// <exception cref="ArgumentTypeException"> If <paramref name="enumValue"/> is not of the specified type. </exception>
    /// <exception cref="ArgumentOutOfRangeException"> If <paramref name="enumValue"/> has a numeric value that is not completely defined within its 
    /// enumeration type. For flag types, every bit must correspond to at least one enumeration value. </exception>
    public static TEnum? CheckValidEnumValueAndType<TEnum> ([InvokerParameterName] string argumentName, object enumValue)
        where TEnum: struct
    {
      if (enumValue == null)
        return default (TEnum?);

      if (! (enumValue is TEnum))
        throw new ArgumentTypeException (argumentName, typeof (TEnum), enumValue.GetType());

      if (! EnumUtility.IsValidEnumValue (enumValue))
        throw new ArgumentOutOfRangeException (argumentName);

      return (TEnum?) enumValue;
    }

    /// <summary>Checks whether <paramref name="enumValue"/> is of the enumeration type <typeparamref name="TEnum"/>, is defined within this 
    /// type, and is not a null reference.</summary>
    /// <remarks>
    /// When successful, the value is returned as the specified type for direct assignment. 
    /// </remarks>
    /// <exception cref="ArgumentNullException"> If <paramref name="enumValue"/> is a null reference. </exception>
    /// <exception cref="ArgumentTypeException"> If <paramref name="enumValue"/> is not of the specified type. </exception>
    /// <exception cref="ArgumentOutOfRangeException"> If <paramref name="enumValue"/> has a numeric value that is not completely defined within its 
    /// enumeration type. For flag types, every bit must correspond to at least one enumeration value. </exception>
    public static TEnum CheckValidEnumValueAndTypeAndNotNull<TEnum> (
        [InvokerParameterName] string argumentName, 
        [AssertionCondition (AssertionConditionType.IS_NOT_NULL)] object enumValue)
        where TEnum: struct
    {
      if (enumValue == null)
        throw new ArgumentNullException (argumentName);

      if (! (enumValue is TEnum))
        throw new ArgumentTypeException (argumentName, typeof (TEnum), enumValue.GetType());

      if (!EnumUtility.IsValidEnumValue (enumValue))
        throw new ArgumentOutOfRangeException (argumentName);

      return (TEnum) enumValue;
    }
  }
}
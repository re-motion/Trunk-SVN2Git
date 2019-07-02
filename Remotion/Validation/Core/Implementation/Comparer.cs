// Decompiled with JetBrains decompiler
// Type: FluentValidation.Internal.Comparer
// Assembly: FluentValidation, Version=5.0.0.1, Culture=neutral, PublicKeyToken=a82054b837897c66
// MVID: 30628A95-CE3F-41E4-BA2A-29882CBD79CE
// Assembly location: C:\Development\re-motion_svn2git\Remotion\ObjectBinding\Web.Development.WebTesting.TestSite\Bin\FluentValidation.dll

using System;

namespace Remotion.Validation.Implementation
{
  public static class Comparer
  {
    /// <summary>Tries to compare the two objects.</summary>
    public static bool TryCompare(IComparable value, IComparable valueToCompare, out int result)
    {
      try
      {
        Comparer.Compare(value, valueToCompare, out result);
        return true;
      }
      catch
      {
        result = 0;
      }
      return false;
    }

    /// <summary>
    /// Tries to do a proper comparison but may fail.
    /// First it tries the default comparison, if this fails, it will see
    /// if the values are fractions. If they are, then it does a double
    /// comparison, otherwise it does a long comparison.
    /// </summary>
    private static void Compare(IComparable value, IComparable valueToCompare, out int result)
    {
      try
      {
        result = value.CompareTo((object)valueToCompare);
      }
      catch (ArgumentException)
      {
        if (value is Decimal || valueToCompare is Decimal || (value is double || valueToCompare is double) || (value is float || valueToCompare is float))
          result = Convert.ToDouble((object)value).CompareTo(Convert.ToDouble((object)valueToCompare));
        else
          result = ((long)value).CompareTo((long)valueToCompare);
      }
    }

    /// <summary>
    /// Tries to compare the two objects, but will throw an exception if it fails.
    /// </summary>
    /// <returns>True on success, otherwise False.</returns>
    public static int GetComparisonResult(IComparable value, IComparable valueToCompare)
    {
      int result;
      if (Comparer.TryCompare(value, valueToCompare, out result))
        return result;
      return value.CompareTo((object)valueToCompare);
    }

    /// <summary>
    /// Tries to compare the two objects, but will throw an exception if it fails.
    /// </summary>
    /// <returns>True on success, otherwise False.</returns>
    public static bool GetEqualsResult(IComparable value, IComparable valueToCompare)
    {
      int result;
      if (Comparer.TryCompare(value, valueToCompare, out result))
        return result == 0;
      return value.Equals((object)valueToCompare);
    }
  }
}
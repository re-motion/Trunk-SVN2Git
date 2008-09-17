/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using System.Diagnostics;
using Remotion.Utilities;

namespace Remotion.Development.UnitTesting
{
  public struct MemoryUsage
  {
    public struct MemoryValue
    {
      private readonly long _bytes;

      public MemoryValue (long bytes)
        : this ()
      {
        _bytes = bytes;
      }

      public long Bytes
      {
        get { return _bytes; }
      }

      public decimal MegaBytes { get { return Bytes / 1024.0m / 1024.0m; } }

      public override string ToString ()
      {
        return MegaBytes.ToString ("N2") + " MB";
      }

      public string ToDifferenceString ()
      {
        if (Bytes > 0)
          return "+" + ToString ();
        else
          return ToString ();
      }

      public static MemoryValue operator - (MemoryValue left, MemoryValue right)
      {
        return new MemoryValue (left.Bytes - right.Bytes);
      }
    }

    public static MemoryUsage GetCurrent (string description)
    {
      using (var process = Process.GetCurrentProcess ())
      {
        return new MemoryUsage (
            description,
            new MemoryValue (process.WorkingSet64),
            new MemoryValue (GC.GetTotalMemory (false)),
            new MemoryValue (GC.GetTotalMemory (true)));
      }
    }

    private readonly string _description;
    private readonly MemoryValue _workingSet;
    private readonly MemoryValue _managedMemoryBeforeCollect;
    private readonly MemoryValue _managedMemoryAfterCollect;

    public MemoryUsage (string description, MemoryValue workingSet, MemoryValue managedMemoryBeforeCollect, MemoryValue managedMemoryAfterCollect)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("description", description);

      _description = description;
      _managedMemoryAfterCollect = managedMemoryAfterCollect;
      _managedMemoryBeforeCollect = managedMemoryBeforeCollect;
      _workingSet = workingSet;
    }

    public string Description
    {
      get { return _description; }
    }

    public MemoryValue WorkingSet
    {
      get { return _workingSet; }
    }

    public MemoryValue ManagedMemoryBeforeCollect
    {
      get { return _managedMemoryBeforeCollect; }
    }

    public MemoryValue ManagedMemoryAfterCollect
    {
      get { return _managedMemoryAfterCollect; }
    }

    public void DumpToConsole ()
    {
      Console.WriteLine (GetDumpString());
    }

    public string GetDumpString ()
    {
      return string.Format ("{0}:\n\tWorking set: {1}\n\tManaged memory before collect: {2}\n\tAfter collect: {3}",
                            Description,
                            WorkingSet,
                            ManagedMemoryBeforeCollect,
                            ManagedMemoryAfterCollect);
    }

    public void DumpComparison (MemoryUsage comparison)
    {
      Console.WriteLine (GetComparisonDumpString(comparison));
    }

    public string GetComparisonDumpString (MemoryUsage comparison)
    {
      return string.Format (
          "Compared to {0}:\n\tWorking set: {1}\n\tManaged memory before collect: {2}\n\tAfter collect: {3}",
          comparison.Description,
          (WorkingSet - comparison.WorkingSet).ToDifferenceString (),
          (ManagedMemoryBeforeCollect - comparison.ManagedMemoryBeforeCollect).ToDifferenceString (),
          (ManagedMemoryAfterCollect - comparison.ManagedMemoryAfterCollect).ToDifferenceString ());
    }
  }
}
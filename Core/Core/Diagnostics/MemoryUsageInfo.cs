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

namespace Remotion.Diagnostics
{
  public struct MemoryUsageInfo
  {
    public static MemoryUsageInfo GetCurrent (string description)
    {
      using (var process = Process.GetCurrentProcess ())
      {
        return new MemoryUsageInfo (
            description,
            new ByteValue (process.WorkingSet64),
            new ByteValue (GC.GetTotalMemory (false)),
            new ByteValue (GC.GetTotalMemory (true)));
      }
    }

    private readonly string _description;
    private readonly ByteValue _workingSet;
    private readonly ByteValue _managedMemoryBeforeCollect;
    private readonly ByteValue _managedMemoryAfterCollect;

    public MemoryUsageInfo (string description, ByteValue workingSet, ByteValue managedMemoryBeforeCollect, ByteValue managedMemoryAfterCollect)
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

    public ByteValue WorkingSet
    {
      get { return _workingSet; }
    }

    public ByteValue ManagedMemoryBeforeCollect
    {
      get { return _managedMemoryBeforeCollect; }
    }

    public ByteValue ManagedMemoryAfterCollect
    {
      get { return _managedMemoryAfterCollect; }
    }

    public void DumpToConsole ()
    {
      Console.WriteLine (ToString());
    }

    public override string ToString ()
    {
      return string.Format ("{0}:{1}\tWorking set: {2}{1}\tManaged memory before collect: {3}{1}\tAfter collect: {4}",
                            Description,
                            Environment.NewLine,
                            WorkingSet,
                            ManagedMemoryBeforeCollect,
                            ManagedMemoryAfterCollect);
    }

    public void DumpComparisonToConsole (MemoryUsageInfo comparison)
    {
      Console.WriteLine (ToDifferenceString (comparison));
    }

    public string ToDifferenceString (MemoryUsageInfo comparison)
    {
      return string.Format (
          "Compared to {0}:{1}\tWorking set: {2}{1}\tManaged memory before collect: {3}{1}\tAfter collect: {4}",
          comparison.Description,
          Environment.NewLine,
          (WorkingSet - comparison.WorkingSet).ToDifferenceString (),
          (ManagedMemoryBeforeCollect - comparison.ManagedMemoryBeforeCollect).ToDifferenceString (),
          (ManagedMemoryAfterCollect - comparison.ManagedMemoryAfterCollect).ToDifferenceString ());
    }

    public string ToCSVString ()
    {
      return string.Format ("{0};{1};{2}", WorkingSet.Bytes, ManagedMemoryBeforeCollect.Bytes, ManagedMemoryAfterCollect.Bytes);
    }
  }
}
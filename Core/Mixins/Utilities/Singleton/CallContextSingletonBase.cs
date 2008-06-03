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

namespace Remotion.Mixins.Utilities.Singleton
{
  public class CallContextSingletonBase<TSelf, TCreator> where TCreator : IInstanceCreator<TSelf>, new()
  {
    private static CallContextSingleton<TSelf> s_instance = new CallContextSingleton<TSelf> (typeof (TSelf).FullName + "_Singleton",
      new TCreator().CreateInstance);

    public static TSelf Current
    {
      get { return s_instance.Current; }
    }

    public static bool HasCurrent
    {
      get { return s_instance.HasCurrent; }
    }

    public static void SetCurrent(TSelf value)
    {
      s_instance.SetCurrent (value);
    }
  }
}

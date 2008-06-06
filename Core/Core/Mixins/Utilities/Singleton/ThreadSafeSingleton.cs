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
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using System.Text;
using Remotion;
using Remotion.Utilities;

namespace Remotion.Mixins.Utilities.Singleton
{
  public class ThreadSafeSingleton<T>
      where T : class
  {
    private DoubleCheckedLockingContainer<T> _instanceHolder;

    public ThreadSafeSingleton (Func<T> creator)
    {
      ArgumentUtility.CheckNotNull ("creator", creator);

      _instanceHolder = new DoubleCheckedLockingContainer<T> (creator);
    }

    public bool HasCurrent
    {
      get { return _instanceHolder.HasValue; }
    }

    public T Current
    {
      get { return _instanceHolder.Value; }
    }

    public void SetCurrent (T value)
    {
      _instanceHolder.Value = value;
    }
  }
}

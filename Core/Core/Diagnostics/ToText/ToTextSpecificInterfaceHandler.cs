/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

namespace Remotion.Diagnostics.ToText
{
  public abstract class ToTextSpecificInterfaceHandler<T> : IToTextSpecificInterfaceHandler
  {
    public ToTextSpecificInterfaceHandler ()
    {
      Priority = 0;
    }

    public ToTextSpecificInterfaceHandler (int priority)
    {
      Priority = priority;
    }

    public abstract void ToText (T t, ToTextBuilder toTextBuilder);

    public virtual void ToText (object obj, ToTextBuilder toTextBuilder)
    {
      ToText ((T) obj, toTextBuilder);
    }

    public int Priority { get; set; }
  }
}
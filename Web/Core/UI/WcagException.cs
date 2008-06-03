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
using System.Runtime.Serialization;
using System.Web.UI;

namespace Remotion.Web.UI
{

public class WcagException: Exception
{
  public WcagException()
    : this ("An element on the page is not WCAG conform.", null)
  {
  }

  public WcagException (int priority)
    : base (string.Format ("An element on the page does comply with a priority {0} checkpoint.", priority))
  {
  }

  public WcagException (int priority, Control control)
    : base (string.Format (
       "{0} '{1}' does not comply with a priority {2} checkpoint.", 
        control.GetType().Name, control.ID, priority))
  {
  }

  public WcagException (int priority, Control control, string property)
    : base (string.Format (
        "The value of property '{0}' for {1} '{2}' does not comply with a priority {3} checkpoint.", 
        property, control.GetType().Name, control.ID, priority))
  {
  }

  public WcagException (string message, Exception innerException)
    : base (message, innerException)
  {
  }

  protected WcagException (SerializationInfo info, StreamingContext context)
    : base (info, context)
  {
  }
}

}

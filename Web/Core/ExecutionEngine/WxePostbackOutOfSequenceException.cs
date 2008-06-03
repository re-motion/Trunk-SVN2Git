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

namespace Remotion.Web.ExecutionEngine
{

/// <summary> This exception indicates an attempt to resubmit a page cached in the browser's history. </summary>
[Serializable]
public class WxePostbackOutOfSequenceException: WxeException
{
  public WxePostbackOutOfSequenceException()
    : base ("The server has received a post back from a page that has already been submitted. "
        + "The page's state is no longer valid. Please navigate to the start page to restart the web application.")
  {
  }

  protected WxePostbackOutOfSequenceException (SerializationInfo info, StreamingContext context)
    : base (info, context)
  {
  }
}

}

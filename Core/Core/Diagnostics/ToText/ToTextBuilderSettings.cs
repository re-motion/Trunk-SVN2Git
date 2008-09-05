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
  public class ToTextBuilderSettings
  {
    public ToTextBuilderSettings ()
    {
      EnumerablePostfix = "}";
      EnumerableElementPostfix = "";
      EnumerableOtherElementPrefix = ",";
      EnumerableFirstElementPrefix = "";
      EnumerablePrefix = "{";

      ArrayPostfix = "}";
      ArrayElementPostfix = "";
      ArrayOtherElementPrefix = ",";
      ArrayFirstElementPrefix = "";
      ArrayPrefix = "{";
    }

    public string ArrayPrefix { get; set; }
    public string ArrayFirstElementPrefix { get; set; }
    public string ArrayOtherElementPrefix { get; set; }
    public string ArrayElementPostfix { get; set; }
    public string ArrayPostfix { get; set; }

    public string EnumerablePrefix { get; set; }
    public string EnumerableFirstElementPrefix { get; set; }
    public string EnumerableOtherElementPrefix { get; set; }
    public string EnumerableElementPostfix { get; set; }
    public string EnumerablePostfix { get; set; }
  }
}
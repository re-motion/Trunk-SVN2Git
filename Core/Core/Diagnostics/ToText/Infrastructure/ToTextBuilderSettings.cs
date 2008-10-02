// 
//  Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
// 
//  This program is free software: you can redistribute it and/or modify it under 
//  the terms of the re:motion license agreement in license.txt. If you did not 
//  receive it, please visit http://www.re-motion.org/licensing.
//  
//  Unless otherwise provided, this software is distributed on an "AS IS" basis, 
//  WITHOUT WARRANTY OF ANY KIND, either express or implied. 
// 
// 
namespace Remotion.Diagnostics.ToText.Infrastructure
{
  public class ToTextBuilderSettings
  {
    public ToTextBuilderSettings ()
    {
      EnumerablePostfix = "}";
      EnumerableElementPostfix = "";
      EnumerableElementPrefix = "";
      EnumerableSeparator = ",";
      EnumerablePrefix = "{";

      ArrayPostfix = "}";
      ArrayElementPostfix = "";
      ArrayElementPrefix = "";
      ArraySeparator = ",";
      ArrayPrefix = "{";
    }

    public string ArrayPrefix { get; set; }
    public string ArrayElementPrefix { get; set; }
    public string ArrayElementPostfix { get; set; }
    public string ArraySeparator { get; set; }
    public string ArrayPostfix { get; set; }

    public string EnumerablePrefix { get; set; }
    public string EnumerableElementPrefix { get; set; }
    public string EnumerableElementPostfix { get; set; }
    public string EnumerableSeparator { get; set; }
    public string EnumerablePostfix { get; set; }
  }
}
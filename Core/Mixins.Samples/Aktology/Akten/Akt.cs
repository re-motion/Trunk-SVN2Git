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

namespace Remotion.Mixins.Samples.Aktology.Akten
{
  public interface IAkt
  {
    string ID { get; set; }
    string Inhalt { get; set; }
  }

  public class Akt : IAkt
  {
    private string _id;
    private string _inhalt;

    public string ID
    {
      get { return _id; }
      set { _id = value; }
    }

    public string Inhalt
    {
      get { return _inhalt; }
      set { _inhalt = value; }
    }
  }
}

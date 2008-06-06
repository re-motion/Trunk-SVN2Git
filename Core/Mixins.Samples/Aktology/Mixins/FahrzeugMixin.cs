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

namespace Remotion.Mixins.Samples.Aktology.Mixins
{
  public interface IFahrzeugMixin
  {
    string Type { get; set; }
    string Kennzeichen { get; set; }
  }

  public class FahrzeugMixin : IFahrzeugMixin // Wir könnten von "Mixin<Akt>" oder "Mixin<Akt, IAkt>" ableiten, ist aber für dieses Mixin nicht nötig
  {
    private string _type;
    private string _kennzeichen;

    public string Type
    {
      get { return _type; }
      set { _type = value; }
    }

    public string Kennzeichen
    {
      get { return _kennzeichen; }
      set { _kennzeichen = value; }
    }
  }
}

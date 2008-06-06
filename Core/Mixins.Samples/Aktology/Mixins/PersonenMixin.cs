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
using Remotion.Mixins;
using Remotion.Mixins.Samples.Aktology.Akten;

namespace Remotion.Mixins.Samples.Aktology.Mixins
{
  public interface IPersonenMixin
  {
    string Vorname { get; set; }
    string Nachname { get; set; }
  }

  public class PersonenMixin : IPersonenMixin // Wir könnten von "Mixin<Akt>" oder "Mixin<Akt, IAkt>" ableiten, ist aber für dieses Mixin nicht nötig
  {
    private string _vorname;
    private string _nachname;

    public string Vorname
    {
      get { return _vorname; }
      set { _vorname = value; }
    }

    public string Nachname
    {
      get { return _nachname; }
      set { _nachname = value; }
    }
  }
}

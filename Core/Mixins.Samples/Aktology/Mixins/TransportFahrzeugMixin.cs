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
  public interface ITransportFahrzeugMixin : IFahrzeugMixin
  {
    string MaximaleLadung { get; set; }
  }

  public class TransportFahrzeugMixin : FahrzeugMixin, ITransportFahrzeugMixin
  {
    private string _maximaleLadung;

    public string MaximaleLadung
    {
      get { return _maximaleLadung; }
      set { _maximaleLadung = value; }
    }
  }
}

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
using Remotion.Mixins.Samples.Aktology.Akten;
using Remotion.Mixins.Samples.Aktology.Mixins;
using Remotion.Mixins;
using Remotion.Mixins.Context;

namespace Remotion.Mixins.Samples.Aktology
{
  public interface IFahrzeugAkt : IAkt, IFahrzeugMixin {}
  public interface IFahrzeugSachAkt : ISachAkt, IFahrzeugMixin {}

  public interface ITransportFahrzeugAkt : IAkt, ITransportFahrzeugMixin {}
  public interface ITransportFahrzeugSachAkt : ISachAkt, ITransportFahrzeugMixin {}

  public interface IPersonenAkt : IAkt, IPersonenMixin {}
  public interface IPersonenSachAkt : ISachAkt, IPersonenMixin {}

  public interface IAngestelltenAkt : IAkt, IAngestelltenMixin {}
  public interface IAngestelltenSachAkt : ISachAkt, IAngestelltenMixin {}

  public static class AktFactory
  {
    public static IFahrzeugAkt CreateFahrzeugAkt ()
    {
      return Create<IFahrzeugAkt, Akt, FahrzeugMixin> ();
    }

    public static IFahrzeugSachAkt CreateFahrzeugSachAkt ()
    {
      return Create<IFahrzeugSachAkt, SachAkt, FahrzeugMixin> ();
    }

    public static ITransportFahrzeugAkt CreateTransportFahrzeugAkt ()
    {
      return Create<ITransportFahrzeugAkt, Akt, TransportFahrzeugMixin> ();
    }

    public static ITransportFahrzeugSachAkt CreateTransportFahrzeugSachAkt ()
    {
      return Create<ITransportFahrzeugSachAkt, SachAkt, TransportFahrzeugMixin> ();
    }

    public static IPersonenAkt CreatePersonenAkt ()
    {
      return Create<IPersonenAkt, Akt, PersonenMixin> ();
    }

    public static IPersonenSachAkt CreatePersonenSachAkt ()
    {
      return Create<IPersonenSachAkt, SachAkt, PersonenMixin> ();
    }

    public static IAngestelltenAkt CreateAngestelltenAkt ()
    {
      return Create<IAngestelltenAkt, Akt, AngestelltenMixin> ();
    }

    public static IAngestelltenSachAkt CreateAngestelltenSachAkt ()
    {
      return Create<IAngestelltenSachAkt, SachAkt, AngestelltenMixin> ();
    }

    private static TInterface Create<TInterface, TBaseType, TMixin> ()
    {
      using (MixinConfiguration.BuildNew().ForClass<TBaseType>().AddMixin<TMixin>().AddCompleteInterface<TInterface>().EnterScope())
      {
        return ObjectFactory.Create<TInterface>().With();
      }
    }
  }
}

// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;
using Remotion.Mixins.Samples.Aktology.Akten;
using Remotion.Mixins.Samples.Aktology.Mixins;

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

// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) rubicon IT GmbH, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
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
using System.Collections.Generic;
using Remotion.Implementation;
using Remotion.ServiceLocation;
using Remotion.Web.Infrastructure;
using Remotion.Web.Legacy.Factories;
using Remotion.Web.Legacy.Infrastructure;
using Remotion.Web.Legacy.Infrastructure.Factories;
using Remotion.Web.Legacy.UI.Controls.Rendering;
using Remotion.Web.UI.Controls;
using Remotion.Web.UI.Controls.DatePickerButtonImplementation.Rendering;
using Remotion.Web.UI.Controls.DropDownMenuImplementation.Rendering;
using Remotion.Web.UI.Controls.ListMenuImplementation.Rendering;
using Remotion.Web.UI.Controls.SingleViewImplementation.Rendering;
using Remotion.Web.UI.Controls.TabbedMenuImplementation.Rendering;
using Remotion.Web.UI.Controls.TabbedMultiViewImplementation.Rendering;
using Remotion.Web.UI.Controls.WebButtonImplementation.Rendering;
using Remotion.Web.UI.Controls.WebTabStripImplementation.Rendering;
using Remotion.Web.UI.Controls.WebTreeViewImplementation.Rendering;

namespace Remotion.Web.Legacy
{
  /// <summary>
  /// Provides the service configuration for the legacy rendering support of <b>Remotion.Web</b>.
  /// </summary>
  public static class LegacyServiceConfigurationService
  {
    public static IEnumerable<ServiceConfigurationEntry> GetConfiguration ()
    {
      yield return new ServiceConfigurationEntry (
          typeof (IClientScriptBehavior), typeof (QuirksModeClientScriptBehavior), LifetimeKind.Singleton);

      yield return new ServiceConfigurationEntry (
          typeof (IResourceUrlFactory), typeof (QuirksModeResourceUrlFactory), LifetimeKind.Singleton);

      yield return new ServiceConfigurationEntry (
          typeof (IDatePickerButtonRenderer), typeof (DatePickerButtonQuirksModeRenderer), LifetimeKind.Singleton);

      yield return new ServiceConfigurationEntry (
          typeof (IDatePickerPageRenderer), typeof (DatePickerPageQuirksModeRenderer), LifetimeKind.Singleton);

      yield return new ServiceConfigurationEntry (
          typeof (IDropDownMenuRenderer), typeof (DropDownMenuQuirksModeRenderer), LifetimeKind.Singleton);

      yield return new ServiceConfigurationEntry (
          typeof (IListMenuRenderer), typeof (ListMenuQuirksModeRenderer), LifetimeKind.Singleton);

      yield return new ServiceConfigurationEntry (
          typeof (ISingleViewRenderer), typeof (SingleViewQuirksModeRenderer), LifetimeKind.Singleton);

      yield return new ServiceConfigurationEntry (
          typeof (ITabbedMenuRenderer), typeof (TabbedMenuQuirksModeRenderer), LifetimeKind.Singleton);

      yield return new ServiceConfigurationEntry (
          typeof (ITabbedMultiViewRenderer), typeof (TabbedMultiViewQuirksModeRenderer), LifetimeKind.Singleton);

      yield return new ServiceConfigurationEntry (
          typeof (IWebButtonRenderer), typeof (WebButtonQuirksModeRenderer), LifetimeKind.Singleton);

      yield return new ServiceConfigurationEntry (
          typeof (IWebTabStripRenderer), typeof (WebTabStripQuirksModeRenderer), LifetimeKind.Singleton);

      yield return new ServiceConfigurationEntry (
          typeof (IWebTreeViewRenderer), typeof (WebTreeViewQuirksModeRenderer), LifetimeKind.Singleton);

      yield return new ServiceConfigurationEntry (
          typeof (IThemedResourceUrlResolver), typeof (QuirksModeResourceUrlResolver), LifetimeKind.Singleton);

      yield return new ServiceConfigurationEntry (
          typeof (IThemedResourceUrlResolverFactory), typeof (QuirksModeResourceUrlResolverFactory), LifetimeKind.Singleton);
    }
  }
}
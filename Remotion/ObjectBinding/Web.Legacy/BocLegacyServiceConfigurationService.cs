// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using Remotion.ObjectBinding.Web.Legacy.UI.Controls.BocBooleanValueImplementation.Rendering;
using Remotion.ObjectBinding.Web.Legacy.UI.Controls.BocDateTimeValueImplementation.Rendering;
using Remotion.ObjectBinding.Web.Legacy.UI.Controls.BocEnumValueImplementation.Rendering;
using Remotion.ObjectBinding.Web.Legacy.UI.Controls.BocListImplementation.Rendering;
using Remotion.ObjectBinding.Web.Legacy.UI.Controls.BocReferenceValueImplementation.Rendering;
using Remotion.ObjectBinding.Web.Legacy.UI.Controls.BocTextValueImplementation.Rendering;
using Remotion.ObjectBinding.Web.Legacy.UI.Controls.Factories;
using Remotion.ObjectBinding.Web.UI.Controls.BocBooleanValueImplementation.Rendering;
using Remotion.ObjectBinding.Web.UI.Controls.BocDateTimeValueImplementation.Rendering;
using Remotion.ObjectBinding.Web.UI.Controls.BocEnumValueImplementation.Rendering;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Rendering;
using Remotion.ObjectBinding.Web.UI.Controls.BocReferenceValueImplementation.Rendering;
using Remotion.ObjectBinding.Web.UI.Controls.BocTextValueImplementation.Rendering;
using Remotion.ServiceLocation;

namespace Remotion.ObjectBinding.Web.Legacy
{
  public static class BocLegacyServiceConfigurationService
  {
    public static IEnumerable<ServiceConfigurationEntry> GetConfiguration ()
    {
      yield return
          new ServiceConfigurationEntry (
              typeof (BocListQuirksModeCssClassDefinition), typeof (BocListQuirksModeCssClassDefinition), LifetimeKind.Singleton);
      yield return new ServiceConfigurationEntry (typeof (IBocIndexColumnRenderer), typeof (BocIndexColumnQuirksModeRenderer), LifetimeKind.Singleton);
      yield return
          new ServiceConfigurationEntry (typeof (IBocSelectorColumnRenderer), typeof (BocSelectorColumnQuirksModeRenderer), LifetimeKind.Singleton);
      yield return new ServiceConfigurationEntry (typeof (IBocRowRenderer), typeof (BocRowQuirksModeRenderer), LifetimeKind.Singleton);
      yield return
          new ServiceConfigurationEntry (typeof (IBocListTableBlockRenderer), typeof (BocListTableBlockQuirksModeRenderer), LifetimeKind.Singleton);
      yield return new ServiceConfigurationEntry (typeof (IBocCommandColumnRenderer), typeof (BocCommandColumnQuirksModeRenderer), LifetimeKind.Singleton);
      yield return
          new ServiceConfigurationEntry (typeof (IBocCompoundColumnRenderer), typeof (BocCompoundColumnQuirksModeRenderer), LifetimeKind.Singleton);
      yield return
          new ServiceConfigurationEntry (typeof (IBocCustomColumnRenderer), typeof (BocCustomColumnQuirksModeRenderer), LifetimeKind.Singleton);
      yield return
          new ServiceConfigurationEntry (
              typeof (IBocDropDownMenuColumnRenderer), typeof (BocDropDownMenuColumnQuirksModeRenderer), LifetimeKind.Singleton);
      yield return
          new ServiceConfigurationEntry (typeof (IBocListMenuBlockRenderer), typeof (BocListMenuBlockQuirksModeRenderer), LifetimeKind.Singleton);
      yield return
          new ServiceConfigurationEntry (
              typeof (IBocListNavigationBlockRenderer), typeof (BocListNavigationBlockQuirksModeRenderer), LifetimeKind.Singleton);
      yield return new ServiceConfigurationEntry (typeof (IBocListRenderer), typeof (BocListQuirksModeRenderer), LifetimeKind.Singleton);
      yield return
          new ServiceConfigurationEntry (
              typeof (IBocRowEditModeColumnRenderer), typeof (BocRowEditModeColumnQuirksModeRenderer), LifetimeKind.Singleton);
      yield return
          new ServiceConfigurationEntry (typeof (IBocSimpleColumnRenderer), typeof (BocSimpleColumnQuirksModeRenderer), LifetimeKind.Singleton);
      yield return
          new ServiceConfigurationEntry (
              typeof (IBocBooleanValueResourceSetFactory), typeof (BocBooleanValueQuirksModeResourceSetFactory), LifetimeKind.Singleton);
      yield return new ServiceConfigurationEntry (typeof (IBocEnumValueRenderer), typeof (BocEnumValueQuirksModeRenderer), LifetimeKind.Singleton);
      yield return
          new ServiceConfigurationEntry (typeof (IBocDateTimeValueRenderer), typeof (BocDateTimeValueQuirksModeRenderer), LifetimeKind.Singleton);
      yield return
          new ServiceConfigurationEntry (typeof (IBocBooleanValueRenderer), typeof (BocBooleanValueQuirksModeRenderer), LifetimeKind.Singleton);
      yield return new ServiceConfigurationEntry (typeof (IBocCheckBoxRenderer), typeof (BocCheckBoxQuirksModeRenderer), LifetimeKind.Singleton);
      yield return new ServiceConfigurationEntry (typeof (IBocTextValueRenderer), typeof (BocTextValueQuirksModeRenderer), LifetimeKind.Singleton);
      yield return
          new ServiceConfigurationEntry (
              typeof (IBocMultilineTextValueRenderer), typeof (BocMultilineTextValueQuirksModeRenderer), LifetimeKind.Singleton);
      yield return
          new ServiceConfigurationEntry (typeof (IBocReferenceValueRenderer), typeof (BocReferenceValueQuirksModeRenderer), LifetimeKind.Singleton);
      yield return
          new ServiceConfigurationEntry (
              typeof (IBocAutoCompleteReferenceValueRenderer), typeof (BocAutoCompleteReferenceValueQuirksModeRenderer), LifetimeKind.Singleton);
    }
  }
}
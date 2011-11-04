// This file is part of re-strict (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License version 3.0 
// as published by the Free Software Foundation.
// 
// This program is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program; if not, see http://www.gnu.org/licenses.
// 
// Additional permissions are listed in the file re-motion_exceptions.txt.
// 
using System;
using System.Collections.Specialized;
using System.Linq;
using System.Web.UI.HtmlControls;
using Remotion.Utilities;
using Remotion.Web.UI.Controls;

namespace Remotion.SecurityManager.Clients.Web.Classes.OrganizationalStructure
{
  public abstract class BaseEditControl<TSelf> : BaseControl, IFormGridRowProvider
      where TSelf: BaseEditControl<TSelf>
  {
    protected abstract FormGridManager GetFormGridManager ();

    public override bool Validate ()
    {
      bool isValid = base.Validate();

      isValid &= GetFormGridManager().Validate();

      return isValid;
    }

    StringCollection IFormGridRowProvider.GetHiddenRows (HtmlTable table)
    {
      ArgumentUtility.CheckNotNull ("table", table);

      var provider = GetFormGridRowProvider();
      if (provider == null)
        return new StringCollection();

      return provider.GetHiddenRows ((TSelf) this, table, GetFormGridManager());
    }

    FormGridRowInfoCollection IFormGridRowProvider.GetAdditionalRows (HtmlTable table)
    {
      ArgumentUtility.CheckNotNull ("table", table);

      var provider = GetFormGridRowProvider();
      if (provider == null)
        return new FormGridRowInfoCollection();

      return provider.GetAdditionalRows ((TSelf) this, table, GetFormGridManager());
    }

    private IOrganizationalStructureEditControlFormGridRowProvider<TSelf> GetFormGridRowProvider ()
    {
      var providers = ServiceLocator.GetAllInstances<IOrganizationalStructureEditControlFormGridRowProvider<TSelf>>().ToArray();
      if (providers.Length > 1)
      {
        throw new InvalidOperationException (
            string.Format (
                "Registering multiple implementations of '{0}' is not supported for providing FormGridRow-information.",
                typeof (IOrganizationalStructureEditControlFormGridRowProvider<TSelf>).FullName));
      }

      return providers.SingleOrDefault();
    }
  }
}
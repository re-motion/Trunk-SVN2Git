// This file is part of re-strict (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// re-strict is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License version 3.0 as
// published by the Free Software Foundation.
// 
// re-strict is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with re-strict; if not, see http://www.gnu.org/licenses.
// 
// Additional permissions are listed in the file re-motion_exceptions.txt.
// 
using System;
using Remotion.Diagnostics.ToText;
using Remotion.Diagnostics.ToText.Infrastructure;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.Utilities;

namespace Remotion.SecurityManager.Domain.ToTextSpecificTypeHandlers
{
  [ToTextSpecificHandler]
  public class Role_ToTextSpecificTypeHandler : ToTextSpecificTypeHandler<Role>
  {
    public override void ToText (Role role, IToTextBuilder toTextBuilder)
    {
      ArgumentUtility.CheckNotNull ("role", role);
      ArgumentUtility.CheckNotNull ("toTextBuilder", toTextBuilder);
      toTextBuilder.ib<Role> ("");
      if (toTextBuilder.OutputComplexity >= ToTextBuilderBase.ToTextBuilderOutputComplexityLevel.Complex)
      {
        //toTextBuilder.e ("user", x.User.UserName).e ("group", x.Group.Name).e ("pos", x.Position.Name);
        toTextBuilder.e ("user", role.User.UserName).e ("group", role.Group.DisplayName).e ("pos", role.Position.DisplayName);
      }
      else
      {
        //toTextBuilder.e (x.User.UserName).e (x.Group.Name).e (x.Position.Name);
        toTextBuilder.e (role.User.UserName).e (role.Group.DisplayName).e (role.Position.DisplayName);
      }
      toTextBuilder.ie ();
    }
  }
}

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
using Remotion.Diagnostics.ToText;
using Remotion.Diagnostics.ToText.Infrastructure;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.Utilities;

namespace Remotion.SecurityManager.Domain.ToTextSpecificTypeHandlers
{
  [ToTextSpecificHandler]
  public class User_ToTextSpecificTypeHandler : ToTextSpecificTypeHandler<User>
  {
    public override void ToText (User user, IToTextBuilder toTextBuilder)
    {
      ArgumentUtility.CheckNotNull ("user", user);
      ArgumentUtility.CheckNotNull ("toTextBuilder", toTextBuilder);
      if (toTextBuilder.OutputComplexity >= ToTextBuilderBase.ToTextBuilderOutputComplexityLevel.Complex)
      {
        toTextBuilder.ib<User> ().e ("user", user.UserName).e ("title", user.Title).e ("first", user.FirstName).e ("last", user.LastName);
        toTextBuilder.e ("display", user.DisplayName).e ("group", user.OwningGroup).e ("tenant", user.Tenant);
        toTextBuilder.nl ().e ("roles", user.Roles);
        toTextBuilder.ie ();
      }
      else
      {
        toTextBuilder.ib<User> ("").e (user.UserName).ie();
      }
    }

  }




}
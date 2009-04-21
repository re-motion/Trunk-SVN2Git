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
using Remotion.Diagnostics.ToText;
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.Utilities;

namespace Remotion.SecurityManager.Domain.ToTextSpecificTypeHandlers
{
  [ToTextSpecificHandler]
  public class AbstractRoleDefinition_ToTextSpecificTypeHandler : ToTextSpecificTypeHandler<AbstractRoleDefinition>
  {
    public override void ToText (AbstractRoleDefinition abstractRoleDefinition, IToTextBuilder toTextBuilder)
    {

      ArgumentUtility.CheckNotNull ("abstractRoleDefinition", abstractRoleDefinition);
      ArgumentUtility.CheckNotNull ("toTextBuilder", toTextBuilder);
      toTextBuilder.ib<AbstractRoleDefinition> ("").e (abstractRoleDefinition.DisplayName).ie ();
    }
  }
}

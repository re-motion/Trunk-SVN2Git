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

namespace Remotion.Data.DomainObjects.DataManagement.Commands
{
  /// <summary>
  /// Provides the possibility to implement <see cref="IDataManagementCommand"/> ad-hoc using delegates and lambdas, without having to create a 
  /// new class.
  /// </summary>
  public class AdHocCommand : IDataManagementCommand
  {
    public Action NotifyClientTransactionOfBeginHandler { get; set; }
    public Action BeginHandler { get; set; }
    public Action PerformHandler { get; set; }
    public Action EndHandler { get; set; }
    public Action NotifyClientTransactionOfEndHandler { get; set; }
    
    public Func<AdHocCommand, ExpandedCommand> Expander { get; set; }

    public void NotifyClientTransactionOfBegin ()
    {
      if (NotifyClientTransactionOfBeginHandler != null)
        NotifyClientTransactionOfBeginHandler ();
    }

    public void Begin ()
    {
      if (BeginHandler != null)
        BeginHandler ();
    }

    public void Perform ()
    {
      if (PerformHandler != null)
        PerformHandler ();
    }

    public void End ()
    {
      if (EndHandler != null)
        EndHandler ();
    }

    public void NotifyClientTransactionOfEnd ()
    {
      if (NotifyClientTransactionOfEndHandler != null)
        NotifyClientTransactionOfEndHandler ();
    }

    public ExpandedCommand ExpandToAllRelatedObjects ()
    {
      if (Expander != null)
        return Expander (this);
      else
        return new ExpandedCommand (this);
    }
  }
}
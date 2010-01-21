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
using System.Collections.ObjectModel;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement.Commands
{
  /// <summary>
  /// Composes several <see cref="IDataManagementCommand"/> instances into a single command.
  /// </summary>
  /// <remarks>
  /// This can, for example, be used to model bidirectional relation modifications. Such modifications always comprise multiple steps: they need to 
  /// be performed on either side of the relation being changed, and usually they also invole one "previous" or "new" related object. (Eg. an insert 
  /// modificaton has a previous related object (possibly <see langword="null" />), a remove modification has an old related object.)
  /// <see cref="CompositeDataManagementCommand"/> aggregates these modification steps and allows executing and raising events for them all at once.
  /// </remarks>
  public class CompositeDataManagementCommand : IDataManagementCommand
  {
    private readonly List<IDataManagementCommand> _commands;

    public CompositeDataManagementCommand (IEnumerable<IDataManagementCommand> commands)
    {
      ArgumentUtility.CheckNotNull ("commands", commands);
      _commands = new List<IDataManagementCommand> (commands);
    }

    public CompositeDataManagementCommand (params IDataManagementCommand[] commands)
        : this ((IEnumerable<IDataManagementCommand>) commands)
    {
    }

    public ReadOnlyCollection<IDataManagementCommand> GetCommands ()
    {
      return _commands.AsReadOnly ();
    }

    public void AddCommand (IDataManagementCommand command)
    {
      ArgumentUtility.CheckNotNull ("command", command);
      _commands.Add (command);
    }

    public void Begin ()
    {
      foreach (var command in _commands)
        command.Begin ();
    }

    public void Perform ()
    {
      foreach (var command in _commands)
        command.Perform ();
    }

    public void End ()
    {
      for (int i = _commands.Count - 1; i >= 0; i--)
        _commands[i].End();
    }

    public void NotifyClientTransactionOfBegin ()
    {
      foreach (var command in _commands)
        command.NotifyClientTransactionOfBegin ();
    }

    public void NotifyClientTransactionOfEnd ()
    {
      for (int i = _commands.Count - 1; i >= 0; i--)
        _commands[i].NotifyClientTransactionOfEnd ();
    }

    public IDataManagementCommand ExtendToAllRelatedObjects ()
    {
      return this;
    }


  }
}
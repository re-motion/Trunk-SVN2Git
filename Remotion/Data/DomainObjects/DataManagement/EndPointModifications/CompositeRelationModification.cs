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

namespace Remotion.Data.DomainObjects.DataManagement.EndPointModifications
{
  /// <summary>
  /// Composes several <see cref="IDataManagementCommand"/> instances into a single command.
  /// </summary>
  /// <remarks>
  /// This can, for example, be used to model bidirectional relation modifications. Such modifications always comprise multiple steps: they need to 
  /// be performed on either side of the relation being changed, and usually they also invole one "previous" or "new" related object. (Eg. an insert 
  /// modificaton has a previous related object (possibly <see langword="null" />), a remove modification has an old related object.)
  /// <see cref="CompositeRelationModification"/> aggregates these modification steps and allows executing and raising events for them all at once.
  /// </remarks>
  // TODO 1914: Refactor to CompositeDataManagementCommand, move to outer namespace
  // TODO 1914: should implement IDataManagementCommand; should get two versions of ExecuteAllSteps (WithEvents/WithoutEvents)
  // TODO 1914: in CollectionEndPointReplaceSameModification and ObjectEndPointSetSameModification, there is no need to have a WithoutEvents object, those modifications don't send any notifications anyway
  public abstract class CompositeRelationModification
  {
    private readonly List<IDataManagementCommand> _commands;

    protected CompositeRelationModification (IEnumerable<IDataManagementCommand> commands)
    {
      ArgumentUtility.CheckNotNull ("commands", commands);
      _commands = new List<IDataManagementCommand> (commands);
    }

    public ReadOnlyCollection<IDataManagementCommand> GetModificationSteps ()
    {
      return _commands.AsReadOnly ();
    }

    public void AddModificationStep (IDataManagementCommand command)
    {
      ArgumentUtility.CheckNotNull ("command", command);
      _commands.Add (command);
    }

    public abstract void ExecuteAllSteps ();
  }
}

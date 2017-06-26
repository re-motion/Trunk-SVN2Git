﻿// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.ControlSelection;

namespace Remotion.Web.Development.WebTesting.FluentControlSelection
{
  /// <summary>
  /// Selection command builder, preparing a <see cref="FirstControlSelectionCommand{TControlObject}"/>.
  /// </summary>
  /// <typeparam name="TControlSelector">The <see cref="IFirstControlSelector{TControlObject}"/> to use.</typeparam>
  /// <typeparam name="TControlObject">The specific <see cref="ControlObject"/> type to select.</typeparam>
  public class FirstControlSelectionCommandBuilder<TControlSelector, TControlObject>
      : IControlSelectionCommandBuilder<TControlSelector, TControlObject>,
        IControlOptionalSelectionCommandBuilder<TControlSelector, TControlObject>
      where TControlSelector : IFirstControlSelector<TControlObject>
      where TControlObject : ControlObject
  {
    /// <inheritdoc/>
    IControlSelectionCommand<TControlObject> IControlSelectionCommandBuilder<TControlSelector, TControlObject>.Using (TControlSelector controlSelector)
    {
      ArgumentUtility.CheckNotNull ("controlSelector", controlSelector);

      return new FirstControlSelectionCommand<TControlObject> (controlSelector);
    }

    /// <inheritdoc/>
    IControlOptionalSelectionCommand<TControlObject> IControlOptionalSelectionCommandBuilder<TControlSelector, TControlObject>.Using (TControlSelector controlSelector)
    {
      ArgumentUtility.CheckNotNull ("controlSelector", controlSelector);

      return new FirstControlSelectionCommand<TControlObject> (controlSelector);
    }
  }
}
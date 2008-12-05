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

namespace Remotion.Web.UI.Controls
{

/// <summary>
///   This interface contains all public members of System.Web.UI.Control. It is used to derive interfaces that will be
///   implemented by deriving from System.Web.UI.Control.
/// </summary>
/// <remarks>
///   The reason for providing this interface is that derived interfaces do not need to be casted to System.Web.UI.Control.
/// </remarks>
public interface IControlWithDesignTimeSupport
{
  void PreRenderForDesignMode();
}

}

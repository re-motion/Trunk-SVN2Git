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
namespace Remotion.ObjectBinding
{
  /// <summary>
  ///   The <see cref="IBusinessObjectWithIdentity"/> interface provides functionality to uniquely identify a business object 
  ///   within its business object domain.
  /// </summary>
  /// <remarks>
  ///   With the help of the <see cref="IBusinessObjectWithIdentity"/> interface it is possible to persist and later restore 
  ///   a reference to the business object. 
  /// </remarks>
  public interface IBusinessObjectWithIdentity : IBusinessObject
  {
    /// <summary> Gets the programmatic <b>ID</b> of this <see cref="IBusinessObjectWithIdentity"/> </summary>
    /// <value> A <see cref="string"/> uniquely identifying this object. </value>
    /// <remarks> This value must be be unqiue within its business object domain. </remarks>
    string UniqueIdentifier { get; }
  }
}

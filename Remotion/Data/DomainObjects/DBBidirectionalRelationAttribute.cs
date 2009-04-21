// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects
{
  //TODO: Tests
  /// <summary>
  /// Declares a relation as bidirectional. Use <see cref="ContainsForeignKey"/> to indicate the the foreign key side in a one-to-one relation
  /// and the <see cref="SortExpression"/> to specify the <b>Order By</b>-clause.
  /// </summary>
  public class DBBidirectionalRelationAttribute: BidirectionalRelationAttribute
  {
    private bool _containsForeignKey = false;
    private string _sortExpression;

    /// <summary>
    /// Initializes a new instance of the <see cref="DBBidirectionalRelationAttribute"/> class with the name of the oppsite property
    /// and the <see cref="ContainsForeignKey"/> value.
    /// </summary>
    /// <param name="oppositeProperty">The name of the opposite property. Must not be <see langword="null" /> or empty.</param>
    public DBBidirectionalRelationAttribute (string oppositeProperty)
        : base (oppositeProperty)
    {
    }

    /// <summary>Gets or sets a flag that indicates the foreign key side in a one-to-one relation.</summary>
    /// <remarks>The <see cref="ContainsForeignKey"/> property may only be specified on one side of a one-to-one-relaiton.</remarks>
    public bool ContainsForeignKey
    {
      get { return _containsForeignKey; }
      set { _containsForeignKey = value; }
    }

    /// <summary>
    /// Gets or sets the <b>Order By</b>-clause of the select statement used to retrieve the collection side of a one-to-many-relation.
    /// </summary>
    /// <remarks>The <see cref="SortExpression"/> property may only be specified on collection properties.</remarks>
    public string SortExpression
    {
      get { return _sortExpression; }
      set
      {
        ArgumentUtility.CheckNotNull ("value", value);
        value = value.Trim();
        ArgumentUtility.CheckNotNullOrEmpty ("value", value);
        _sortExpression = StringUtility.EmptyToNull (value);
      }
    }
  }
}

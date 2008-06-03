/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

namespace Remotion.ObjectBinding
{
  /// <summary>
  ///   Extends an <see cref="IBusinessObjectBoundControl"/> with the option of writing the 
  ///   <see cref="IBusinessObjectBoundControl.Value"/> back into the bound <see cref="IBusinessObject"/>.
  /// </summary>
  /// <remarks>
  ///   See <see cref="SaveValue"/> for a description of the data binding process.
  /// </remarks>
  /// <seealso cref="IBusinessObjectBoundControl"/>
  /// <seealso cref="IBusinessObjectDataSource"/>
  public interface IBusinessObjectBoundEditableControl: IBusinessObjectBoundControl
  {
    /// <summary>
    ///   Saves the <see cref="IBusinessObjectBoundControl.Value"/> back into the bound <see cref="IBusinessObject"/>.
    /// </summary>
    /// <remarks>
    ///   <para>
    ///     When <see cref="SaveValue"/> is executed, the object held by <see cref="IBusinessObjectBoundControl.Value"/>
    ///     is written back into the <see cref="IBusinessObject"/> provided by the 
    ///     <see cref="IBusinessObjectBoundControl.DataSource"/>.
    ///   </para><para>
    ///     This method is usually called by 
    ///     <see cref="IBusinessObjectDataSource.SaveValues">IBusinessObjectDataSource.SaveValues</see>.
    ///   </para>
    /// </remarks>
    /// <param name="interim"> Specifies whether this is the final saving, or an interim saving. </param>
    void SaveValue (bool interim);
  
    /// <summary>
    ///   Gets a flag that determines whether the control is to be displayed in read-only mode.
    /// </summary>
    bool IsReadOnly { get; }
  }
}

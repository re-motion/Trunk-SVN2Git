/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using Remotion.Data.DomainObjects;
using Remotion.Utilities;

namespace Remotion.Data.UnitTests.DomainObjects.Core.EventReceiver
{
  public abstract class DataContainerMockEventReceiver
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    public DataContainerMockEventReceiver (DataContainer dataContainer)
    {
      ArgumentUtility.CheckNotNull ("dataContainer", dataContainer);

      dataContainer.PropertyChanged += new PropertyChangeEventHandler (PropertyChanged);
      dataContainer.PropertyChanging += new PropertyChangeEventHandler (PropertyChanging);
    }

    // abstract methods and properties

    public abstract void PropertyChanging (object sender, PropertyChangeEventArgs args);
    public abstract void PropertyChanged (object sender, PropertyChangeEventArgs args);

  }
}

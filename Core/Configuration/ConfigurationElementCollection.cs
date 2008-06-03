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
using System.Configuration;
using Remotion.Utilities;

namespace Remotion.Configuration
{
  public interface INamedConfigurationElement
  {
    string Name { get; }
  }

  //TODO: Test
  public class ConfigurationElementCollection<TElement>: ConfigurationElementCollection
      where TElement: ConfigurationElement, INamedConfigurationElement, new()
  {
    public ConfigurationElementCollection ()
    {
    }

    public TElement this [int index]
    {
      get { return (TElement) BaseGet (index); }
      set
      {
        if (BaseGet (index) != null)
          BaseRemoveAt (index);
        BaseAdd (index, value);
      }
    }

    public new TElement this [string Name]
    {
      get { return (TElement) BaseGet (Name); }
    }

    public int IndexOf (TElement element)
    {
      return BaseIndexOf (element);
    }

    public void Add (TElement element)
    {
      ArgumentUtility.CheckNotNull ("element", element);

      BaseAdd (element);
    }

    public void Remove (TElement element)
    {
      ArgumentUtility.CheckNotNull ("element", element);

      if (BaseIndexOf (element) >= 0)
        BaseRemove (element.Name);
    }

    /// <summary>When overridden in a derived class, creates a new <see cref="ConfigurationElement"/>.</summary>
    /// <returns>A new <see cref="ConfigurationElement"/>.</returns>
    protected override ConfigurationElement CreateNewElement()
    {
      return new TElement();

    }

    /// <summary>Gets the element key for a specified configuration element when overridden in a derived class.</summary>
    /// <param name="element">The <see cref="ConfigurationElement"/> to return the key for. </param>
    /// <returns>An <see cref="Object"/> that acts as the key for the specified <see cref="ConfigurationElement"/>.</returns>
    protected override object GetElementKey (ConfigurationElement element)
    {
      return ((TElement) element).Name;
    }

    public override ConfigurationElementCollectionType CollectionType
    {
      get { return ConfigurationElementCollectionType.AddRemoveClearMap; }
    }

    protected override string ElementName
    {
      get { return "add"; }
    }

    protected override bool ThrowOnDuplicate
    {
      get { return true; }
    }
  }
}

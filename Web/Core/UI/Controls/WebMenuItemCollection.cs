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
using System.Collections;
using System.ComponentModel;
using System.Drawing.Design;
using System.Web.UI;
using Remotion.Collections;
using Remotion.Utilities;
using Remotion.Web.UI.Design;

namespace Remotion.Web.UI.Controls
{

/// <summary> A collection of <see cref="WebMenuItem"/> objects. </summary>
[Editor (typeof (WebMenuItemCollectionEditor), typeof (UITypeEditor))]
public class WebMenuItemCollection: ControlItemCollection
{
  /// <summary> Sorts the <paramref name="menuItems"/> by their categories." </summary>
  /// <param name="menuItems"> Must not be <see langword="null"/> or contain items that are <see langword="null"/>. </param>
  /// <param name="generateSeparators"> <see langword="true"/> to generate a separator before starting a new category. </param>
  /// <returns> The <paramref name="menuItems"/>, sorted by their categories. </returns>
  public static WebMenuItem[] GroupMenuItems (WebMenuItem[] menuItems, bool generateSeparators)
  {
    ArgumentUtility.CheckNotNullOrItemsNull ("menuItems", menuItems);

    //  <string category, ArrayList menuItems>
    NameObjectCollection groupedMenuItems = new NameObjectCollection();
    ArrayList categories = new ArrayList();
    
    for (int i = 0; i < menuItems.Length; i++)
    {
      WebMenuItem menuItem = (WebMenuItem) menuItems[i];

      string category = StringUtility.NullToEmpty (menuItem.Category);
      ArrayList menuItemsForCategory;
      if (groupedMenuItems.Contains (category))
      {
        menuItemsForCategory = (ArrayList) groupedMenuItems[category];
      }
      else
      {
        menuItemsForCategory = new ArrayList();
        groupedMenuItems.Add (category, menuItemsForCategory);
        categories.Add (category);
      }
      menuItemsForCategory.Add (menuItem);
    }
      
    ArrayList arrayList = new ArrayList();
    bool isFirst = true;
    for (int i = 0; i < categories.Count; i++)
    {
      string category = (string) categories[i];
      if (generateSeparators)
      {
        if (isFirst)
          isFirst = false;
        else
          arrayList.Add (WebMenuItem.GetSeparator());
      }
      arrayList.AddRange ((ArrayList) groupedMenuItems[category]);
    }
    return (WebMenuItem[]) arrayList.ToArray (typeof (WebMenuItem));
  }

  /// <summary> Initializes a new instance. </summary>
  public WebMenuItemCollection (Control ownerControl, Type[] supportedTypes)
    : base (ownerControl, supportedTypes)
  {
  }

  /// <summary> Initializes a new instance. </summary>
  public WebMenuItemCollection (Control ownerControl)
    : this (ownerControl, new Type[] {typeof (WebMenuItem)})
  {
  }

  public new WebMenuItem[] ToArray()
  {
    return (WebMenuItem[]) InnerList.ToArray (typeof (WebMenuItem));
  }

  //  Do NOT make this indexer public. Ever. Or ASP.net won't be able to de-serialize this property.
  protected internal new WebMenuItem this[int index]
  {
    get { return (WebMenuItem) List[index]; }
    set { List[index] = value; }
  }

  /// <summary> Sorts the <see cref="WebMenuItem"/> objects by their categories." </summary>
  /// <param name="generateSeparators"> <see langword="true"/> to generate a separator before starting a new category. </param>
  /// <returns> The <see cref="WebMenuItem"/> objects, sorted by their categories. </returns>
  public WebMenuItem[] GroupMenuItems (bool generateSeparators)
  {
    return WebMenuItemCollection.GroupMenuItems (ToArray(), generateSeparators);
  }
}

}

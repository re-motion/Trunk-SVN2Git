using System;
using System.Web.UI;
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.Web.UI.Controls;

namespace Remotion.ObjectBinding.Sample
{
  public class PersonListItemCommandState: IBocListItemCommandState
  {
    public PersonListItemCommandState ()
    {
    }

    public bool IsEnabled(
        BocList list, 
        IBusinessObject businessObject, 
        BocCommandEnabledColumnDefinition columnDefinition)
    {
      return true;
    }
  }
}
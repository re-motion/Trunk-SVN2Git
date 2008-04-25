using System;
using Remotion.ObjectBinding;
using Remotion.Data.DomainObjects.Queries;

namespace Remotion.Data.DomainObjects.Web.Test.Domain
{
  [BindableObject]
  public class SearchObjectWithUndefinedEnum
  {
    // types

    // static members and constants

    // member fields

    private UndefinedEnum _undefinedEnum;

    // construction and disposing

    public SearchObjectWithUndefinedEnum ()
    {
      _undefinedEnum = UndefinedEnum.Undefined;
    }

    // methods and properties

    public UndefinedEnum UndefinedEnum
    {
      get { return _undefinedEnum; }
      set { _undefinedEnum = value; }
    }
  }
}
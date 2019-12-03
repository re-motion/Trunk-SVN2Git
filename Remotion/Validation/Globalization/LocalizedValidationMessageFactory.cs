// This file is part of the re-motion Core Framework (www.re-motion.org)
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
using Remotion.Globalization;
using Remotion.Reflection;
using Remotion.ServiceLocation;
using Remotion.Utilities;
using Remotion.Validation.Implementation;

namespace Remotion.Validation.Globalization
{
  [ImplementationFor (typeof (IValidationMessageFactory), Position = Position, Lifetime = LifetimeKind.Singleton, RegistrationType = RegistrationType.Multiple)]
  public class LocalizedValidationMessageFactory : IValidationMessageFactory
  {
    public const int Position = 0;

    [MultiLingualResources ("Remotion.Validation.Globalization.Globalization.LocalizedValidationMessageFactory")]
    public static class Resources { }

    private readonly IMemberInformationGlobalizationService _memberInformationGlobalizationService;
    private readonly IGlobalizationService _globalizationService;

    public LocalizedValidationMessageFactory (
        IMemberInformationGlobalizationService memberInformationGlobalizationService,
        IGlobalizationService globalizationService)
    {
      ArgumentUtility.CheckNotNull ("memberInformationGlobalizationService", memberInformationGlobalizationService);
      ArgumentUtility.CheckNotNull ("globalizationService", globalizationService);

      _memberInformationGlobalizationService = memberInformationGlobalizationService;
      _globalizationService = globalizationService;
    }

    public IValidationMessage CreateValidationMessageForPropertyValidator (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);

      var typeInformation = TypeAdapter.Create (type);
      var typeInformationForResourceResolution = TypeAdapter.Create (typeof (LocalizedValidationMessageFactory));

      if (!_memberInformationGlobalizationService.ContainsTypeDisplayName (typeInformation, typeInformationForResourceResolution))
        return null;

      return new DelegateBasedValidationMessage (
          () => _memberInformationGlobalizationService.GetTypeDisplayNameOrDefault (typeInformation, typeInformationForResourceResolution));
    }
  }
}
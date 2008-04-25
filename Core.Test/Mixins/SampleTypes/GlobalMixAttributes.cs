using System;
using Remotion.UnitTests.Mixins.SampleTypes;
using Remotion.Mixins;

[assembly: Mix (typeof (TargetClassForGlobalMix), typeof (MixinForGlobalMix),
    AdditionalDependencies = new Type[] {typeof (AdditionalDependencyForGlobalMix)},
    SuppressedMixins = new Type[] {typeof (SuppressedMixinForGlobalMix)})]

[assembly: Mix (typeof (TargetClassForGlobalMix), typeof (SuppressedMixinForGlobalMix))]
[assembly: Mix (typeof (TargetClassForGlobalMix), typeof (AdditionalDependencyForGlobalMix))]
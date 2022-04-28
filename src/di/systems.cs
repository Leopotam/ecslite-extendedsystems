// ----------------------------------------------------------------------------
// The Proprietary or MIT-Red License
// Copyright (c) 2012-2022 Leopotam <leopotam@yandex.ru>
// ----------------------------------------------------------------------------

using Leopotam.EcsLite.Di;
#if ENABLE_IL2CPP
using System;
using Unity.IL2CPP.CompilerServices;
#endif

namespace Leopotam.EcsLite.ExtendedSystems {
#if ENABLE_IL2CPP
    [Il2CppSetOption (Option.NullChecks, false)]
    [Il2CppSetOption (Option.ArrayBoundsChecks, false)]
#endif
    public static class Extensions {
        public static EcsSystems AddGroup (this EcsSystems systems, string groupName, bool defaultState, string eventWorldName, params IEcsSystem[] nestedSystems) {
            return systems.Add (new EcsGroupSystemWithDi (groupName, defaultState, eventWorldName, nestedSystems));
        }
    }

#if ENABLE_IL2CPP
    [Il2CppSetOption (Option.NullChecks, false)]
    [Il2CppSetOption (Option.ArrayBoundsChecks, false)]
#endif
    public class EcsGroupSystemWithDi : EcsGroupSystem, Di.IEcsInjectSystem {
        public EcsGroupSystemWithDi (string name, bool defaultState, string eventsWorldName, params IEcsSystem[] systems) : base (name, defaultState, eventsWorldName, systems) { }

        public void Inject (EcsSystems systems, params object[] injects) {
            for (int i = 0, iMax = _allSystems.Length; i < iMax; i++) {
                var system = _allSystems[i];
                if (system is IEcsInjectSystem injectSystem) {
                    injectSystem.Inject (systems, injects);
                    continue;
                }
                Di.Extensions.InjectToSystem (system, systems, injects);
            }
        }
    }
}

#if ENABLE_IL2CPP
// Unity IL2CPP performance optimization attribute.
namespace Unity.IL2CPP.CompilerServices {
    enum Option {
        NullChecks = 1,
        ArrayBoundsChecks = 2
    }

    [AttributeUsage (AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
    class Il2CppSetOptionAttribute : Attribute {
        public Option Option { get; private set; }
        public object Value { get; private set; }

        public Il2CppSetOptionAttribute (Option option, object value) { Option = option; Value = value; }
    }
}
#endif
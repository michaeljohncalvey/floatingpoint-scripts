﻿using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using CloudCity;

namespace Autelia.Serialization.Mementos.Unity
{
    [JsonObject(MemberSerialization.Fields)]
    [System.Serializable]
    public sealed class FoliageTrackerMemento : BehaviourMemento<FoliageTracker, FoliageTrackerMemento>
    {

        bool _active;
        int _buyCost;
        int _radius;
        Material _sphereMaterial;
        Sphere _sphereScript;

        protected override bool Serialize(FoliageTracker originator)
        {
            base.Serialize(originator);
            _active = originator.active;
            _buyCost = originator.buyCost;
            _radius = originator.radius;
            _sphereMaterial = originator.sphereMaterial;
            _sphereScript = originator.sphereScript;
            return true;
        }

        protected override void Deserialize(ref FoliageTracker r)
        {
            base.Deserialize(ref r);

            r.active = _active;
            r.buyCost = _buyCost;
            r.radius = _radius;
            r.sphereMaterial = _sphereMaterial;
            r.sphereScript = _sphereScript;
        }

        public static implicit operator FoliageTrackerMemento(FoliageTracker foliageTracker)
        { return Create(foliageTracker); }

        public static implicit operator FoliageTracker(FoliageTrackerMemento foliageTrackerMemento)
        {
            if (foliageTrackerMemento == null)
                return null;
            return foliageTrackerMemento.Originator;
        }
    }
}
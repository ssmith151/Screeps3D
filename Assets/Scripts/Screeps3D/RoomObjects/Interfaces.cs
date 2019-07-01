﻿using System.Collections.Generic;
using Screeps3D.Rooms;
using Screeps_API;

namespace Screeps3D.RoomObjects
{

    internal interface IObjectViewComponent
    {
        void Init();
        void Load(RoomObject roomObject);
        void Delta(JSONObject data);
        void Unload(RoomObject roomObject);
    }

    internal interface IRoomObject
    {
        Room Room { get; }
    }

    internal interface IEnergyObject
    {
        float Energy { get; set; }
        float EnergyCapacity { get; set; }
    }

    internal interface IPowerObject
    {
        float Power { get; set; }
        float PowerCapacity { get; set; }
    }

    internal interface IResourceObject
    {
        float ResourceAmount { get; set; }
        float ResourceCapacity { get; set; }
        string ResourceType { get; set; }
    }

    internal interface IRegenerationObject: IRoomObject
    {
        float NextRegenerationTime { get; set; }
    }

    internal interface INamedObject
    {
        string Name { get; set; }
    }

    internal interface IOwnedObject
    {
        string UserId { get; set; }
        ScreepsUser Owner { get; set; }
    }

    internal interface ICooldownObject
    {
        float Cooldown { get; set; }
    }

    internal interface IHitpointsObject
    {
        float Hits { get; set; }
        float HitsMax { get; set; }
    }

    internal interface IDecay : IRoomObject
    {
        float NextDecayTime { get; set; }
    }

    internal interface IStoreObject : IRoomObject
    {
        Dictionary<string, float> Store { get; }
        float StoreCapacity { get; set; }
        float TotalResources { get; set; }
    }

    internal interface IProgress
    {
        float Progress { get; set; }
        float ProgressMax { get; set; }
    }

    internal interface IActionObject
    {
        Dictionary<string, JSONObject> Actions { get; }
    }
}
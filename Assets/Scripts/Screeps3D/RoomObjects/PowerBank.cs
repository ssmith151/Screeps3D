using System.Collections.Generic;
using System.Diagnostics;

namespace Screeps3D.RoomObjects
{
    /*
       {
	        "_id": "5d1944d618769d42ddabaaf2",
	        "type": "powerBank",
	        "x": 25,
	        "y": 35,
	        "room": "E0S25",
	        "power": 1962,
	        "hits": 2000000,
	        "hitsMax": 2000000,
	        "decayTime": 8206678
        }

        // https://docs.screeps.com/api/#StructurePowerBank
        Hits	2,000,000
        Return damage	50%
        Capacity	500 — 10,000
        Decay	5,000 ticks
    */

    public class PowerBank : Structure
    {

        internal PowerBank()
        {
        }

        internal override void Unpack(JSONObject data, bool initial)
        {
            base.Unpack(data, initial);
        }
    }
}
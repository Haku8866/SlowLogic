using System.ComponentModel;
using Terraria.ModLoader.Config;
using Newtonsoft.Json;
using System.Diagnostics;
using Steamworks;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace SlowLogic
{
    public class SlowConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ServerSide;
        [Label("Propagation delay")]
        [Tooltip("This is the number of ticks a wiring component will wait before sending an output")]
        [Range(0f, 100f)]
        [Slider]
        [Increment(1f)]
        [DefaultValue(1)]
        public int PropagationDelay;
        [Label("Return to vanilla wiring")]
        [Tooltip("This will disable any wiring effects")]
        [DefaultValue(false)]
        public bool AlwaysAllow;
    }
}
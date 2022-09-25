using MonoMod.Cil;
using System;
using System.Diagnostics;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using static Terraria.ModLoader.ModContent;
using static Mono.Cecil.Cil.OpCodes;
using System.Timers;
using IL.Terraria;
using IL.Terraria.Chat.Commands;
using System.Collections.Generic;
using System.Linq;

namespace SlowLogic
{
	public class SlowLogic : Mod
    {
        public static bool AllowTripWire = false;
        public static int TripWireCount = 0;
        public static Stopwatch sw = new Stopwatch();
        public static Stopwatch sw2 = new Stopwatch();
        public static ulong AverageWire = 0;
        public static ulong AverageAll = 0;
        public static ulong Count = 0;
        public static Vector2 LastPoint = new Vector2(0f, 0f);
        public static long WireCount = 0;
        public override void Load()
        {
            LastPoint = new Vector2(0, 0);
            sw2.Start();
            IL.Terraria.Wiring.TripWire += HookDelay;
            IL.Terraria.Wiring.HitWire += NewHookDelay;
            base.Load();
        }
        private void NewHookDelay(ILContext il)
        {
            var c = new ILCursor(il);
            if (!c.TryGotoNext(i => i.MatchLdfld("Terraria.DataStructures.Point16", "X")))
            {
                return;
            }
            c.EmitDelegate<Action>(() =>
            {
                WireCount++;
            });
        }
        private void HookDelay(ILContext il)
        {
            var c = new ILCursor(il);
            //find the first line in the TripWire() function's code
            if (!c.TryGotoNext(i => i.MatchLdsfld("Terraria.Main", "netMode")))
            {
                return;
            }
            //create a label to use later
            var label = c.DefineLabel();
            //check if the mod is enabled or not
            c.EmitDelegate<Func<bool>>(() => GetInstance<SlowConfig>().AlwaysAllow);
            //if it is then allow the signal through (aka skip to the bottom two lines of code here)
            c.Emit(Brtrue, label);
            //check if a signal is allowed to be sent
            c.EmitDelegate<Func<bool>>(() => AllowTripWire);
            //if it is then allow the signal through (aka skip to the bottom two lines of code here)
            c.Emit(Brtrue, label);
            //Ldarg_0 and Ldarg_1 are the coordinates of where the signal is coming from, we will use these to spawn the projectile in the right place
            c.Emit(Ldarg_0);
            c.Emit(Ldarg_1);
            c.EmitDelegate<Action<int, int>>((x, y) => {
                //adjust the coordinates to be ontop of the current tile (where we'll spawn the projectile)
                x += 1;
                y += 1;
                //as we've determined that the signal isn't allowed to be sent, we spawn an invisible projectile that waits for a few ticks before despawning
                Terraria.Projectile.NewProjectile(null, new Vector2(x, y), new Vector2(0, 0), ModContent.ProjectileType<TimerProjectile>(), 0, 0);
                //when this projectile despawns it sends a signal and sets the "AllowTripWire" flag to true so it doesn't get blocked
            });
            //we still need to block the original signal, so we return here before any of the original function gets processed
            c.Emit(Ret);
            //this code won't be executed if the signal isn't allowed through
            c.MarkLabel(label);
            //we set the "AllowTripWire" flag to false again, which means a signal can only be sent from one of our invisible projectiles which sets it to true beforehand
            c.EmitDelegate<Action>(() => {AllowTripWire = false;});
            c.EmitDelegate<Action>(() => {TripWireCount += 1;});
            c.EmitDelegate<Action>(() => {sw.Start();});
            if (!c.TryGotoNext(i => i.MatchCall("Terraria.Wiring", "LogicGatePass()")))
            {
                return;
            }
            c.EmitDelegate<Action>(() => { sw.Stop(); });
        }
    }
}
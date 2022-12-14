using Terraria.ModLoader;
using Terraria;
using static Terraria.ModLoader.ModContent;
namespace SlowLogic
{
    class TimerProjectile : ModProjectile
    {
        public override void SetDefaults()
        {
            //make the projectile invisible and stop it from being affected by anything
            int delay = GetInstance<SlowConfig>().PropagationDelay;
            Projectile.height = 1;
            Projectile.width = 1;
            Projectile.timeLeft = delay;
            Projectile.alpha = 0;
            Projectile.penetrate = -1;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }
        public override void Kill(int timeLeft)
        {
            //when the projectile dies, call TripWire() with permission on the tile it was spawned on
            int x = (int)Projectile.position.X;
            int y = (int)Projectile.position.Y;
            SlowLogic.AllowTripWire = true;
            Wiring.TripWire(x, y, 1, 1);
            base.Kill(timeLeft);
        }
    }
}
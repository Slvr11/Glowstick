using System;
using System.Collections;
using InfinityScript;

namespace Glowstick
{
    public class Glowstick : BaseScript
    {
        public static int fx_glowstickGlow;

        public Glowstick()
        {
            GSCFunctions.PreCacheItem("lightstick_mp");
            fx_glowstickGlow = GSCFunctions.LoadFX("misc/glow_stick_glow_green");
            PlayerConnected += OnPlayerConnected;
        }
        void OnPlayerConnected(Entity player)
        {
            player.SetField("glowstickOut", false);
            player.SpawnedPlayer += () => OnPlayerSpawned(player);
            player.OnNotify("grenade_fire", (p, g, w) => onGrenadeFire(p, (Entity)g, (string)w));
        }
        public static void OnPlayerSpawned(Entity player)
        {
            giveGlowstick(player);
            spawnPlayerAtGlowstick(player);
        }
        private static void onGrenadeFire(Entity player, Entity grenade, string weaponName)
        {
            if (weaponName != "lightstick_mp") return;
            if (!player.IsAlive) return;

            //StartAsync(waitForGlowstickDrop(player, grenade));
            dropGlowstick(player, player.Origin);
        }
        private static void dropGlowstick(Entity player, Vector3 position)
        {
            Entity glowstick = GSCFunctions.Spawn("script_model", position);
            glowstick.SetModel("viewmodel_light_stick");
            glowstick.Angles = player.Angles;
            Entity fx = GSCFunctions.SpawnFX(fx_glowstickGlow, glowstick.Origin);
            fx.Angles = glowstick.Angles;
            GSCFunctions.TriggerFX(fx);
            glowstick.SetField("fx", fx);

            player.SetField("glowstick", glowstick);
        }
        private static IEnumerator waitForGlowstickDrop(Entity player, Entity grenade)
        {
            Parameter[] returns = null;

            yield return grenade.WaitTill_return("missile_stuck", new Action<Parameter[]>((p) => returns = p));

            if (returns == null) yield break;

            Vector3 position = returns[0].As<Vector3>();

            dropGlowstick(player, position);
        }
        public static void spawnPlayerAtGlowstick(Entity player)
        {
            if (!player.HasField("glowstick")) return;

            Entity glowstick = player.GetField<Entity>("glowstick");
            player.SetOrigin(glowstick.Origin);
            player.SetPlayerAngles(glowstick.Angles);
            glowstick.GetField<Entity>("fx").Delete();
            glowstick.ClearField("fx");
            glowstick.Delete();
            player.ClearField("glowstick");
        }
        public static void giveGlowstick(Entity player)
        {
            if (player.HasWeapon("flare_mp"))
            {
                player.TakeWeapon("flare_mp");
                player.GiveWeapon("lightstick_mp");
            }
        }
    }
}


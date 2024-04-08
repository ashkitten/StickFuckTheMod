using System.IO;
using HarmonyLib;

namespace StickFuckTheMod;

class ControllerPatches
{
	public static void Patch(Harmony harmony)
	{
        var onTakeDamageMethod = AccessTools.Method(typeof(Controller), "OnTakeDamage");
        var onTakeDamageMethodPrefix = new HarmonyMethod(typeof(ControllerPatches).GetMethod(nameof(OnTakeDamageMethodPrefix)));
        harmony.Patch(onTakeDamageMethod, prefix: onTakeDamageMethodPrefix);
	}

	public static void OnTakeDamageMethodPrefix(Controller __instance, float damageTaken)
	{
		var localPlayerID = GameManager.Instance.mMultiplayerManager.LocalPlayerIndex;
		if (__instance.playerID == localPlayerID)
		{
			HealthHandler healthHandler = __instance.GetComponent<HealthHandler>();
			float vibrationAmount = damageTaken / (healthHandler.health + damageTaken);

			Plugin.Log.LogInfo($"vibrate {vibrationAmount}");
			File.WriteAllText("buttplug.commands", $"{vibrationAmount}");
		}
	}
}

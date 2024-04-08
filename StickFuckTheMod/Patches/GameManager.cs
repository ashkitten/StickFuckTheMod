// using System.IO;
using System.IO;
using HarmonyLib;

namespace StickFuckTheMod;

class GameManagerPatches
{
	public static void Patch(Harmony harmony)
	{
		var revivePlayerMethod = AccessTools.Method(typeof(GameManager), "RevivePlayer");
		var revivePlayerMethodPrefix = new HarmonyMethod(typeof(GameManagerPatches).GetMethod(nameof(RevivePlayerMethodPrefix)));
		harmony.Patch(revivePlayerMethod, postfix: revivePlayerMethodPrefix);
	}

	public static void RevivePlayerMethodPrefix(GameManager __instance, Controller playerToRevive, bool newMap)
	{
		var localPlayerID = GameManager.Instance.mMultiplayerManager.LocalPlayerIndex;
		if (playerToRevive.playerID == localPlayerID)
		{
			Plugin.Log.LogInfo($"revived");
			File.WriteAllText(".buttplug.commands", "s");
			File.Move(".buttplug.commands", "buttplug.commands");
		}
	}
}

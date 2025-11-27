using System.Collections;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;
using UnityEngine.Audio;

namespace PEAKJumpscares;

[BepInAutoPlugin]
public partial class Plugin : BaseUnityPlugin
{
    internal static ManualLogSource Log { get; private set; } = null!;
    internal static AssetBundle jumpscareAssetBundle = null!;
    internal static GameObject jumpscarePrefab = null!;
    internal static AudioMixer jumpscareAudioMixer = null!;
    private ConfigEntry<bool> configFoxyBool = null!;
    private ConfigEntry<bool> configBadBoneBool = null!;
    private ConfigEntry<float> configFoxyVolume = null!;
    private ConfigEntry<float> configBadBoneVolume = null!;
    private ConfigEntry<int> configFoxyOdds = null!;
    private ConfigEntry<int> configBadBoneOdds = null!;


    private class Patcher
    {
        // spawn the jumpscare prefab on run start (but not airport because that's technically a run)
        [HarmonyPatch(typeof(RunManager), "StartRun")]
        [HarmonyPostfix]
        private static void RunManagerStartRunPostfix(RunManager __instance)
        {
            if (!Character.localCharacter.inAirport)
            {
                __instance.StartCoroutine(SpawnJumpscareAfterWait());
                
            }
        }

        // the run actually starts before the loading screen destroys itself, so this wait lets it do that before we jumpscare
        // i could also do some crazy stuff to add an OnDestroy to the LoadingScreen but that does not sound fun
        static IEnumerator SpawnJumpscareAfterWait()
        {
            yield return new WaitForSeconds(10);
            Instantiate(jumpscarePrefab);
        }
    }

    private void Awake()
    {
        Log = Logger;
        Harmony.CreateAndPatchAll(typeof(Patcher));

        configFoxyBool = Config.Bind("Toggles", "FoxyToggle", true, "Enables the Foxy jumpscare if true, disables if false");
        configBadBoneBool = Config.Bind("Toggles", "BadToTheBoneToggle", true, "Enables the Bad To The Bone riff if true, disables if false");
        configFoxyVolume = Config.Bind("Volume", "FoxyVolumeAdjust", 0.0f, "Adjusts the volume of the Foxy jumpscare by decibels (max +10dB because this is loud by default, min -80dB)");
        configBadBoneVolume = Config.Bind("Volume", "BadToTheBoneVolumeAdjust", 0.0f, "Adjusts the volume of the Bad To The Bone riff by decibels (max +20dB, min -80dB)");
        configFoxyOdds = Config.Bind("Jumpscare Odds", "FoxyOdds", 10000, "If this is X, there is a 1 in X chance of the Foxy jumpscare every single second");
        configBadBoneOdds = Config.Bind("Jumpscare Odds", "BadToTheBoneOdds", 2500, "If this is X, there is a 1 in X chance of the Bad To The Bone riff every single second");

        string jumpscareAssetBundlePath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "jumpscare");
        jumpscareAssetBundle = AssetBundle.LoadFromFile(jumpscareAssetBundlePath);
        jumpscarePrefab = jumpscareAssetBundle.LoadAsset<GameObject>("Jumpscare.prefab");
        jumpscareAudioMixer = jumpscareAssetBundle.LoadAsset<AudioMixer>("JumpscareAudioMixer.mixer");
        Jumpscare jumpscareComponent = jumpscarePrefab.AddComponent<Jumpscare>();
        jumpscareComponent.mixer = jumpscareAudioMixer;
        jumpscareComponent.configFoxyBool = configFoxyBool.Value;
        jumpscareComponent.configBadBoneBool = configBadBoneBool.Value;
        jumpscareComponent.configFoxyVolume = configFoxyVolume.Value;
        jumpscareComponent.configBadBoneVolume = configBadBoneVolume.Value;
        jumpscareComponent.configFoxyOdds = configFoxyOdds.Value;
        jumpscareComponent.configBadBoneOdds = configBadBoneOdds.Value;

        // Log our awake here so we can see it in LogOutput.log file
        Log.LogInfo($"Plugin {Name} is loaded!");
    }
}

using HarmonyLib;
using Il2CppAssets.Scripts.Unity.UI_New.InGame;
using MelonLoader;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using XmasMod2025.Assets;

namespace XmasMod2025;

[RegisterTypeInIl2Cpp]
public class PulseVignette : MonoBehaviour
{
    public AnimationCurve curve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.5f, 0.25f), new Keyframe(1, 0));

    public int index = 2;

    private Vignette vignette;

    public float speed = 1;

    void Start()
    {
        vignette = GetComponent<Volume>().profile.components[index].Cast<Vignette>();
    }
    
    private void Update()
    {
        vignette.intensity.value = curve.Evaluate(Time.time % speed / speed); // so doing % speed / speed (lets say speed = 2) would be 0-1.9999 / 2 or 0 - 0.99995, just getting there at half the rate
    }
}


internal static class PostProcessing
{
    public static GameObject VolumePrefab;
    public static Volume XmasVolume;
    public static VolumeProfile XmasVolumeProfile;
    public static PulseVignette VignettePulse;

    public static bool isNight = false;
    public static bool possibleNullReference = false;
    
    public static void Load()
    {
        VolumePrefab = AssetHelper.GetObject("XmasVolume");
        VignettePulse = VolumePrefab.AddComponent<PulseVignette>();
        VignettePulse.speed = 4;
        Camera.main.gameObject.GetComponent<UniversalAdditionalCameraData>().renderPostProcessing = true;
    }

    [HarmonyPatch(typeof(InGame), nameof(InGame.StartMatch))]
    private static class InGame_StartMatch
    {
        public static void Postfix()
        {
            PrepareVolume();
            DisableNight();
        }
    }
    
    public static void PrepareVolume()
    {
        if (!XmasVolume)
        {
            XmasVolume = GameObject.Instantiate(VolumePrefab).GetComponent<Volume>();
            XmasVolumeProfile = XmasVolume.profile;
        }
        else if (!XmasVolumeProfile)
        {
            XmasVolumeProfile = XmasVolume.profile;
        }
    }

    public static void EnableNight()
    {
        if(isNight) return;
        if(possibleNullReference) PrepareVolume();
        
        isNight = true;
        XmasVolumeProfile.components[0].active = true;
        XmasVolumeProfile.components[2].active = true;
        XmasVolumeProfile.components[3].active = true;
    }
    public static void DisableNight()
    {
        if(!isNight) return;
        if(possibleNullReference) PrepareVolume();
        
        isNight = false;
        
        XmasVolumeProfile.components[0].active = false;
        XmasVolumeProfile.components[2].active = false;
        XmasVolumeProfile.components[3].active = false;
    }

    public static void SetPulseSpeed(float totalSpeedMultiplier)
    {
        VignettePulse.speed = 4 * totalSpeedMultiplier;
    }
}
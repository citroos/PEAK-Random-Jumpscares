using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.Video;

namespace PEAKJumpscares;

public class Jumpscare : MonoBehaviour
{
    VideoPlayer videoPlayer = null!;
    AudioSource audioSource = null!;
    RawImage rawImage = null!;
    Color chromaKeyColor;
    public AudioMixer mixer = null!;

    public bool configFoxyBool;
    public bool configBadBoneBool;
    public float configFoxyVolume;
    public float configBadBoneVolume;
    public int configFoxyOdds;
    public int configBadBoneOdds;

    float MAX_FOXY_VOLUME = 10f;
    float MAX_BONE_VOLUME = 20f;

    void Start()
    {
        videoPlayer = transform.GetComponentInChildren<VideoPlayer>();
        audioSource = transform.Find("BadToTheBoneAudioSource").GetComponent<AudioSource>();
        rawImage = transform.GetComponentInChildren<RawImage>();
        ColorUtility.TryParseHtmlString("#00D600", out chromaKeyColor);


        StartCoroutine(TryJumpscare());
        videoPlayer.loopPointReached += OnVideoEnd;

        RenderTexture rt = (RenderTexture) videoPlayer.targetTexture;
        RenderTexture.active = rt;
        GL.Clear(true, true, chromaKeyColor);
        RenderTexture.active = null;
    }

    IEnumerator TryJumpscare()
    {
        while (true)
        {
            // IS THAT THE BITE OF '87??
            if (UnityEngine.Random.Range(1, configFoxyOdds) == 1 && configFoxyBool)
            {
                rawImage.enabled = true;
                mixer.SetFloat("FoxyVolume", Mathf.Min(configFoxyVolume, MAX_FOXY_VOLUME));
                videoPlayer.Play();
            }

            // lmao i'm dead
            if (UnityEngine.Random.Range(1, configBadBoneOdds) == 1 && configBadBoneBool)
            {
                mixer.SetFloat("BadToTheBoneVolume", Mathf.Min(configBadBoneVolume, MAX_BONE_VOLUME));
                audioSource.Play();
            }
            yield return new WaitForSeconds(1);
        }
    }

    // set texture to the chromaKeyColor so that we're not stuck on the last foxy frame
    void OnVideoEnd(VideoPlayer vp)
    {
        vp.Stop();

        RenderTexture rt = (RenderTexture) vp.targetTexture;
        RenderTexture.active = rt;
        GL.Clear(true, true, chromaKeyColor);
        RenderTexture.active = null;

        if (rawImage != null)
        {
            rawImage.enabled = false;
        }
    }
}

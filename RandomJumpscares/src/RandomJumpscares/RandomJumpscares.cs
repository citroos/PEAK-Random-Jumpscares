using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

namespace RandomJumpscares;

public class RandomJumpscares : MonoBehaviour
{
    AudioSource badBoneAudio = null!;
    GameObject foxy = null!;
    AudioSource foxyAudio = null!;
    Animator foxyAnimator = null!;
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
        badBoneAudio = transform.Find("BadToTheBone").GetComponent<AudioSource>();
        foxy = transform.Find("Foxy").gameObject;
        foxyAudio = foxy.GetComponent<AudioSource>();
        foxyAnimator = foxy.GetComponent<Animator>();

        StartCoroutine(TryJumpscare());
    }

    IEnumerator TryJumpscare()
    {
        while (true)
        {
            // IS THAT THE BITE OF '87??
            if (UnityEngine.Random.Range(1, configFoxyOdds) == 1 && configFoxyBool)
            {
                mixer.SetFloat("FoxyVolume", Mathf.Min(configFoxyVolume, MAX_FOXY_VOLUME));
                StartCoroutine(PlayFoxyJumpscare());
            }

            // lmao i'm dead
            if (UnityEngine.Random.Range(1, configBadBoneOdds) == 1 && configBadBoneBool)
            {
                mixer.SetFloat("BadToTheBoneVolume", Mathf.Min(configBadBoneVolume, MAX_BONE_VOLUME));
                badBoneAudio.Play();
            }
            yield return new WaitForSeconds(5);
        }
    }

    IEnumerator PlayFoxyJumpscare()
    {
        foxyAnimator.enabled = true;
        foxyAudio.Play();
        foxyAnimator.Play("FoxyJumpscare", 0, 0f);

        AnimatorStateInfo animatorState = foxyAnimator.GetCurrentAnimatorStateInfo(0);
        yield return new WaitForSeconds(animatorState.length / foxyAnimator.speed);

        foxyAnimator.enabled = false;
    }
}

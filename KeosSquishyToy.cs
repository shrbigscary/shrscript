using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using easyInputs;
using Photon.Pun;

public class KeosSquishyToy : MonoBehaviourPunCallbacks, IPunObservable
{
    [Header("Made By Keo.CS no need for Credits")]
    [Header("Photon Sync")]
    public PhotonView PTView;
    [Header("Animation")]
    public Animator Animator;
    public string FloatName;
    [Header("Sound")]
    public float SoundTrigger;
    public float ResetTrigger;
    public AudioSource AudioSource;
    [Header("Trigger")]
    public EasyHand hand;
    public bool UseGrip;
    public bool UseTrigger;
    [Header("Testing")]
    public bool Test;
    public float TestValue;

    private float currentValue;
    private bool canPlaySound = true;

    private void Update()
    {
        if (PTView.IsMine)
        {
            if (UseGrip)
            {
                currentValue = EasyInputs.GetGripButtonFloat(hand);
                HandleAnimationAndSound();
            }
            if (UseTrigger)
            {
                currentValue = EasyInputs.GetTriggerButtonFloat(hand);
                HandleAnimationAndSound();
            }

            if (Test)
            {
                currentValue = TestValue;
                HandleAnimationAndSound();
            }
        }
    }

    private void HandleAnimationAndSound()
    {
        Animator.SetFloat(FloatName, currentValue);

        if (currentValue >= SoundTrigger && canPlaySound)
        {
            PlaySound();
            canPlaySound = false;
        }
        else if (currentValue >= ResetTrigger)
        {
            canPlaySound = true;
        }
    }

    private void PlaySound()
    {
        AudioSource.Play();
        photonView.RPC("RPC_PlaySound", RpcTarget.Others);
    }

    [PunRPC]
    private void RPC_PlaySound()
    {
        AudioSource.Play();
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(currentValue);
        }
        else
        {
            currentValue = (float)stream.ReceiveNext();
            Animator.SetFloat(FloatName, currentValue);
        }
    }
}

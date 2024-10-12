using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(PhotonView))]
public class KeosVents : MonoBehaviourPunCallbacks
{
    [Header("This script was made by Keo.cs")]
    [Header("You do not have to give credits")]
    public GameObject targetGameObject;
    public string parameterName;
    public Animator targetAnimator;
    public string Tag;

    private void Start()
    {
        if (targetGameObject != null)
        {
            targetAnimator = targetGameObject.GetComponent<Animator>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(tag))
        {
            photonView.RPC("SetAnimatorParameter", RpcTarget.All, true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(Tag))
        {
            photonView.RPC("SetAnimatorParameter", RpcTarget.All, false);
        }
    }

    [PunRPC]
    void SetAnimatorParameter(bool state)
    {
        targetAnimator.SetBool(parameterName, state);
    }
}
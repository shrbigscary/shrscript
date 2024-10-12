using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(PhotonView))]
public class KeosDoorButton : MonoBehaviourPun
{
    [Header("This script was made by Keo.cs")]
    [Header("You do not have to give credits")]
    public string targetTag = "HandTag";
    public Transform position1;
    public Transform position2;
    public float moveDuration = 1.0f;
    public float cooldown = 2.0f;
    public bool useEaseInOut = true;
    public Material pressedMaterial;
    public float materialChangeDuration = 1.0f;
    public List<GameObject> linkedButtons;
    public GameObject objectToMove;

    private bool isMoving = false;
    private bool isAtPosition1 = true;
    private float lastPressedTime;
    private Material originalMaterial;
    private Renderer buttonRenderer;

    private static bool globalIsMoving = false;

    private void Start()
    {
        if (objectToMove == null)
        {
            objectToMove = gameObject;
        }
        buttonRenderer = GetComponent<Renderer>();
        if (buttonRenderer != null)
        {
            originalMaterial = buttonRenderer.material;
        }

        if (position1 == null || position2 == null)
        {
            Debug.LogError("Position1 and Position2 must be assigned in the inspector.");
        }

        if (!TryGetComponent(out Collider collider))
        {
            Debug.LogError("This GameObject must have a Collider component.");
        }
        else
        {
            collider.isTrigger = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(targetTag) && !isMoving && !globalIsMoving && Time.time > lastPressedTime + cooldown)
        {
            lastPressedTime = Time.time;
            photonView.RPC("MoveObject", RpcTarget.All);
        }
    }

    [PunRPC]
    private void MoveObject()
    {
        if (globalIsMoving)
        {
            return;
        }
        globalIsMoving = true;
        Vector3 targetPosition = isAtPosition1 ? position2.position : position1.position;
        isAtPosition1 = !isAtPosition1;
        StartCoroutine(MoveToPosition(targetPosition));
        StartCoroutine(ChangeMaterial());

        foreach (GameObject button in linkedButtons)
        {
            PhotonView linkedPhotonView = button.GetComponent<PhotonView>();
            if (linkedPhotonView != null)
            {
                linkedPhotonView.RPC("SetIsAtPosition1", RpcTarget.All, isAtPosition1);
            }
        }
    }

    [PunRPC]
    private void SetIsAtPosition1(bool position)
    {
        isAtPosition1 = position;
    }

    private System.Collections.IEnumerator MoveToPosition(Vector3 target)
    {
        isMoving = true;
        Vector3 start = objectToMove.transform.position;
        float elapsedTime = 0;

        while (elapsedTime < moveDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / moveDuration;

            if (useEaseInOut)
            {
                t = t * t * (3f - 2f * t);
            }

            objectToMove.transform.position = Vector3.Lerp(start, target, t);
            yield return null;
        }

        objectToMove.transform.position = target;
        isMoving = false;
        globalIsMoving = false;
    }

    private System.Collections.IEnumerator ChangeMaterial()
    {
        if (pressedMaterial != null && buttonRenderer != null)
        {
            buttonRenderer.material = pressedMaterial;
            yield return new WaitForSeconds(materialChangeDuration);
            buttonRenderer.material = originalMaterial;
        }
    }
}
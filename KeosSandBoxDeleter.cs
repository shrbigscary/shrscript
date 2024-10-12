using Photon.Pun;
using UnityEngine;
using easyInputs;

public class KeosSandBoxDeleter : MonoBehaviourPunCallbacks
{
    [Header("This script was made by Keo.cs")]
    public LayerMask deletableLayer;
    public Material previewMaterial;
    public EasyHand Ehand;

    private GameObject previewObject;
    private Material originalMaterial;
    private bool canDelete = false;

    void Update()
    {
        if (!photonView.IsMine && PhotonNetwork.IsConnected)
        {
            return;
        }

        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, deletableLayer))
        {
            GameObject hitObject = hit.collider.gameObject;
            if (previewObject != hitObject)
            {
                ResetPreviewObject();
                previewObject = hitObject;
                Renderer renderer = previewObject.GetComponent<Renderer>();
                if (renderer != null)
                {
                    originalMaterial = renderer.material;
                    renderer.material = previewMaterial;
                }
            }
            canDelete = true;
        }
        else
        {
            ResetPreviewObject();
            canDelete = false;
        }

        if ((Input.GetMouseButtonDown(0) || EasyInputs.GetTriggerButtonTouched(Ehand)) && canDelete)
        {
            photonView.RPC("DeleteObject", RpcTarget.All, previewObject.GetComponent<PhotonView>().ViewID);
        }
    }

    void ResetPreviewObject()
    {
        if (previewObject != null)
        {
            Renderer renderer = previewObject.GetComponent<Renderer>();
            if (renderer != null && originalMaterial != null)
            {
                renderer.material = originalMaterial;
            }
            previewObject = null;
        }
    }

    [PunRPC]
    void DeleteObject(int viewID)
    {
        GameObject obj = PhotonView.Find(viewID).gameObject;
        if (obj != null)
        {
            PhotonNetwork.Destroy(obj);
        }
    }
}

using GorillaLocomotion;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class KeosTeleport : MonoBehaviour
{
    [Header("This was made by Keo.CS")]
    public Player gorillaPlayer;
    public Transform teleportPoint;
    public string handTag = "HandTag";
    public float disableDuration = 0.5f;
    public float TPDuration = 1f;
    [Header("Enable")]
    [Tooltip("After TP it going to Enable those")]
    public List<GameObject> Enable;
    [Header("Disable")]
    [Tooltip("After TP it going to Disable those")]
    public List<GameObject> Disable;

    Collider[] allColliders;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(handTag))
        {
            TP();
        }
    }

    public void TP()
    {
        StartCoroutine(Teleport());
    }

    IEnumerator Teleport()
    {
        allColliders = FindObjectsOfType<Collider>();
        gorillaPlayer.GetComponent<Rigidbody>().isKinematic = true;
        foreach (Collider collider in allColliders)
        {
            collider.enabled = false;
        }
        gorillaPlayer.transform.position = teleportPoint.transform.position;
        yield return new WaitForSeconds(TPDuration);
        foreach (Collider collider in allColliders)
        {
            collider.enabled = true;
        }
        gorillaPlayer.GetComponent<Rigidbody>().isKinematic = false;
        foreach (GameObject obj in Enable)
        {
            obj.SetActive(true);
        }
        foreach (GameObject obj in Disable)
        {
            obj.SetActive(false);
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(KeosTeleport))]
    public class KeosTeleportEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            KeosTeleport script = (KeosTeleport)target;
            if (GUILayout.Button("TP"))
            {
                if (Application.isPlaying)
                {
                    script.TP();
                }
                else
                {
                    Debug.LogWarning("You have to be in playmode to click it yk");
                }
            }
        }
    }
#endif
}
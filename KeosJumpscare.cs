using System.Collections;
using UnityEngine;

namespace GorillaLocomotion
{
    public class KeosJumpscare : MonoBehaviour
    {
        [Header("This was made by Keo.CS")]
        public Player player;
        public GameObject gorillaPlayer;
        public Transform teleportPoint;
        public string handTag = "HandTag";
        public string PlayerTag = "Player";
        public float disableDuration = 0.5f;
        public float gravityDisableTime = 1f;
        public float jumpscareDuration = 2f;
        public GameObject Jumpscare;

        private Rigidbody gorillaRigidbody;
        private Collider[] allColliders;

        void Start()
        {
            gorillaRigidbody = gorillaPlayer.GetComponent<Rigidbody>();
            allColliders = FindObjectsOfType<Collider>();
            Jumpscare.SetActive(false);
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(handTag) || other.CompareTag(PlayerTag))
            {
                foreach (var collider in allColliders)
                {
                    collider.enabled = false;
                }

                TeleportGorilla();
            }
        }

        void TeleportGorilla()
        {
            player.disableMovement = true;
            gorillaRigidbody.useGravity = false;
            Invoke(nameof(EnableGravity), gravityDisableTime);

            gorillaPlayer.transform.position = teleportPoint.position;

            Invoke(nameof(EnableColliders), disableDuration);

            Jumpscare.SetActive(true);
            StartCoroutine(JumpscareEnd(jumpscareDuration));
        }

        void EnableGravity()
        {
            gorillaRigidbody.useGravity = true;
        }

        void EnableColliders()
        {
            foreach (var collider in allColliders)
            {
                collider.enabled = true;
            }
        }

        IEnumerator JumpscareEnd(float duration)
        {
            yield return new WaitForSeconds(duration);
            Jumpscare.SetActive(false);
            player.disableMovement = false;
        }
    }

}
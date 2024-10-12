using UnityEngine;

public class KeosAntiAlias : MonoBehaviour
{
    [Header("This script was made by Keo.cs")]
    [Header("You do not have to give credits")]
    [SerializeField] private int antiAliasingLevel = 2;
    [SerializeField] private int textureQualityLevel = 1;
    [SerializeField] private float minRotationX = 70f;
    [SerializeField] private float maxRotationX = 90f;

    public void UpdateTextures()
    {
        ApplyQualitySettings();
        DisableShadowsIfRotationInRange();
    }

    private void ApplyQualitySettings()
    {
        QualitySettings.antiAliasing = antiAliasingLevel;
        QualitySettings.masterTextureLimit = textureQualityLevel;
    }

    private void DisableShadowsIfRotationInRange()
    {
        float rotationX = transform.rotation.eulerAngles.x;
        if (rotationX > minRotationX && rotationX < maxRotationX)
        {
            foreach (var renderer in FindObjectsOfType<Renderer>())
            {
                renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            }
        }
    }
}

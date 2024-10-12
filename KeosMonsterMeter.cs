using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class KeosMonsterMeter : MonoBehaviour
{
    [System.Serializable]
    public class NamedGameObject
    {
        public string Name;
        public GameObject Object;
    }
    [Header("This script was made by Keo.cs")]
    [Header("You do not have to give credits")]
    public GameObject GorillaPlayer;
    public List<NamedGameObject> GameObjectList;
    public TMP_Text DistanceText;
    public TMP_Text NameDisplay;
    public int DecimalPlaces = 0;

    void Update()
    {
        if (GorillaPlayer == null || GameObjectList == null || GameObjectList.Count == 0)
        {
            return;
        }

        NamedGameObject closestObject = null;
        float closestDistance = float.MaxValue;

        foreach (var namedObject in GameObjectList)
        {
            float distance = GetXZDistance(GorillaPlayer.transform.position, namedObject.Object.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestObject = namedObject;
            }
        }

        if (closestObject != null)
        {
            DistanceText.text = closestDistance.ToString("F" + DecimalPlaces);
            NameDisplay.text = closestObject.Name;
        }
    }

    float GetXZDistance(Vector3 a, Vector3 b)
    {
        return Vector2.Distance(new Vector2(a.x, a.z), new Vector2(b.x, b.z));
    }
}

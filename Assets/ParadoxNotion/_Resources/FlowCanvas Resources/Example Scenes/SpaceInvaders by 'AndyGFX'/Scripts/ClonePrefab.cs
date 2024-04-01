using System.Collections;
using UnityEngine;

public class ClonePrefab : MonoBehaviour
{
    public void Clone(GameObject prefab, Vector3 position) {
        Instantiate(prefab, position, Quaternion.identity);
    }
}
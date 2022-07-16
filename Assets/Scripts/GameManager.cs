using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject prefab;
    public void OnClickNinjaSpawn()
    {
       
            Instantiate(prefab, new Vector3(-6, -3, 0), Quaternion.identity);
    }
}

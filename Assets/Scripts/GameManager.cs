using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using Photon.Pun;

public class GameManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private int maxMana;
    [SerializeField] private int currentMana;
    [SerializeField] private float cooldownTime;
    [SerializeField] Text manaText;
    [SerializeField] Image manaImage;

    [SerializeField] private int ninjaCost;
    [SerializeField] private int wolfCost;
    [SerializeField] private int bunnyCost;

    [SerializeField] private Text ninjaText;
    [SerializeField] private Text wolfText;
    [SerializeField] private Text bunnyText;


    [HideInInspector] public float gravityScale;
    public override void OnEnable()
    {
        cooldownTime = 1.0f;
        maxMana = 10;
        currentMana = 0;
        StartCoroutine("RestoreMana", cooldownTime);
        gravityScale = 1.0f;
        UpdateManaUI();
        ninjaText.text = $"({ninjaCost.ToString()})";
        wolfText.text = $"({wolfCost.ToString()})";
        bunnyText.text = $"({bunnyCost.ToString()})";
    }
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Q))
        {
            SpawnNinja();
        }
        if(Input.GetKeyDown(KeyCode.W))
        {
            SpawnWolf();
        }
        if(Input.GetKeyDown(KeyCode.E))
        {
            SpawnBunny();
        }
    }
    public void SpawnNinja()

    {
        if(currentMana >= ninjaCost)
        {
            bool isHost = GameObject.Find("NetworkManager").GetComponent<NetworkManager>().isHost;
            Vector3 spawnPos = isHost ? new Vector3(-7, -4.5f, 0) : new Vector3(7, -4.5f, 0);
            PhotonNetwork.Instantiate("Ninja", spawnPos, Quaternion.identity);
            currentMana -= ninjaCost;
            UpdateManaUI();
        }
    }

    public void SpawnWolf()
    {
        if(currentMana >= wolfCost)
        {
            bool isHost = GameObject.Find("NetworkManager").GetComponent<NetworkManager>().isHost;
            Vector3 spawnPos = isHost ? new Vector3(-7, -4.5f, 0) : new Vector3(7, -4.5f, 0);
            PhotonNetwork.Instantiate("Wolf", spawnPos, Quaternion.identity);
            currentMana -= wolfCost;
            UpdateManaUI();
        }
        
    }

    public void SpawnBunny()
    {
        if(currentMana >= bunnyCost)
        {
            bool isHost = GameObject.Find("NetworkManager").GetComponent<NetworkManager>().isHost;
            Vector3 spawnPos = isHost ? new Vector3(-7, -4.5f, 0) : new Vector3(7, -4.5f, 0);
            PhotonNetwork.Instantiate("Bunny", spawnPos, Quaternion.identity);
            currentMana -= bunnyCost;
            UpdateManaUI();
        }
        
    }
    IEnumerator RestoreMana(float cooldownTime)
    {
        // Debug.Log("Time = "+Time.time);
        if(currentMana < maxMana && gravityScale < 0)
        {
            currentMana += 1;
            UpdateManaUI();
        }
        yield return new WaitForSeconds(cooldownTime);
        StartCoroutine("RestoreMana", cooldownTime);
    }

    void UpdateManaUI()
    {
        manaImage.fillAmount = ((float)currentMana) / maxMana;
        manaText.text = $"{currentMana} / {maxMana}";
    }
}

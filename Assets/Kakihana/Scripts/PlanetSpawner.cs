using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetSpawner : MonoBehaviour
{
    // 惑星自動生成スクリプト

    [SerializeField] private int planetMaxnum;
    [SerializeField] private int count;

    public GameObject[] planetsPre; // 生成したい惑星


    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("PlanetCreate", 3, 3);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void PlanetCreate()
    {
        if (count == planetMaxnum) return;
        int spawnPlanetNum = Random.Range(0, planetsPre.Length);
        float x = Random.Range(-20.0f, 20.0f);
        float y = 0.0f;
        float z = Random.Range(-20.0f, 20.0f);
        Vector3 spawnPos = new Vector3(x, y, z);
        Instantiate(planetsPre[spawnPlanetNum], spawnPos, Quaternion.identity);
        count++;
    }
}

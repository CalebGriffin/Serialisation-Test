using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BallManager : MonoBehaviour
{
    [SerializeField] private Transform playerTrans;
    [SerializeField] private GameObject ballPrefab, goalObj;

    public static BallManager instance;
    private Vector3[] wallsPosArr;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GetWallPos()
    {
        GameObject[] wallObjArr = GameObject.FindGameObjectsWithTag("Wall");
        wallsPosArr = wallObjArr.Select(go => go.transform.position).ToArray();
    }

    public void SpawnBall()
    {
        Vector3 position = RandomPosition();

        bool positionFound = false;
        int i = 0;
        while (!positionFound)
        {
            i++;
            if (i > 999)
            {
                Debug.Log("Couldn't find a position");
                break;
            }

            if (wallsPosArr.Contains(new Vector3(position.x, -0.15f, position.z)))
            {
                Debug.Log("Wall List");
                position = RandomPosition();
                continue;
            }

            if (position.x == goalObj.transform.position.x && position.z == goalObj.transform.position.z)
            {
                Debug.Log("Goal");
                position = RandomPosition();
                continue;
            }

            ////if (Physics.OverlapSphere(position, 0.01f).Length > 1)
            ////{
                ////Debug.Log("OverlapSphere");
                ////position = RandomPosition();
                ////continue;
            ////}

            if (Vector3.Distance(position, playerTrans.position) <= 2f)
            {
                Debug.Log("Player");
                position = RandomPosition();
                continue;
            }

            positionFound = true;
            Instantiate(ballPrefab, position, Quaternion.identity);
        }
    }

    Vector3 RandomPosition()
    {
        int x = Random.Range(-5, 6);
        int y = Random.Range(-5, 6);
        return new Vector3(x, 0, y);
    }
}

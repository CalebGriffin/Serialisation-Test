using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class JSONCreator : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [ContextMenu("CreateJSON")]
    public void CreateJSON()
    {
        GameObject[] wallObjArr = GameObject.FindGameObjectsWithTag("Wall");
        SaveObject saveObject = new SaveObject
        {
            playerPos = GameObject.FindGameObjectWithTag("Player").transform.position,
            goalPos = GameObject.FindGameObjectWithTag("Goal").transform.position,
            wallPosArr = wallObjArr.Select(go => go.transform.position).ToArray()
        };
        string json = JsonUtility.ToJson(saveObject);
        Debug.Log(json);
        GUIUtility.systemCopyBuffer = json;
    }

}

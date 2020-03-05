using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPoint : MonoBehaviour
{
    public static CameraPoint Instance = null;

    void Awake() {
        Instance = this;
    }

    // 在编辑器中显示一个图标
    void OnDrawGizmos() {
        Gizmos.DrawIcon(transform.position, "CameraPoint.tif");
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

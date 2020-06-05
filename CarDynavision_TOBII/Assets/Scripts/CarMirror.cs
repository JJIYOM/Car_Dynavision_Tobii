using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarMirror : MonoBehaviour
{
    private Camera CameraMirror;

    void Awake()
    {
        CameraMirror = GetComponent<Camera>();        
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPreCull()
    {
        CameraMirror.ResetProjectionMatrix();
        CameraMirror.projectionMatrix = CameraMirror.projectionMatrix * Matrix4x4.Scale(new Vector3(-1, 1, 1));
    }

    public void OnPreRender()
    {
        GL.invertCulling = true;
    }

    public void OnPostRender()
    {
        GL.invertCulling = false;
    }
}

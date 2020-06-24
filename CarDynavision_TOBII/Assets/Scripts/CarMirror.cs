using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * 사이드미러&룸미러 설정 스크립트 (카메라 좌우반전)
 * https://midason.tistory.com/139 참고
 */

public class CarMirror : MonoBehaviour
{
    private Camera CameraMirror; 

    void Awake()
    {
        CameraMirror = GetComponent<Camera>();        
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

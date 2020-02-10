using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DepthRenderer : MonoBehaviour, Voxar.IImageReceiver<Texture2D>
{
    private Renderer renderer;
    public Vector2Int resolution;

    // Start is called before the first frame update
    void Start()
    {
        renderer = GetComponent<Renderer>();
    }

    //ReceiveData is called once per frame
    public void ReceiveData(Texture2D data)
    {
        renderer.material.mainTexture = data;
        data.Apply();
    }

    public void SetDataResolution(int x, int y)
    {
        resolution.x = x;
        resolution.y = y;
    }
}

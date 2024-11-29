using UnityEngine;

public class InfiniteVerticalBackground : MonoBehaviour
{
    private Material mat;
    private float distance;

    public float speed = 0.2f;
    private void Start()
    {
        mat = GetComponent<Renderer>().material;
    }
    private void Update()
    {
        distance += Time.deltaTime * speed;
        mat.SetTextureOffset("_MainTex", Vector2.up * distance);
    }
}

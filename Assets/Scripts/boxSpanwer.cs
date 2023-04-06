using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class boxSpanwer : NetworkBehaviour
{
    public List<Transform> platforms;
    public NetworkObject boxPb;

    [Header("Properties")]
    public float spacing;
    public int height;

    void Start()
    {
        Spawn();
    }
    private void Spawn()
    {
        if(IsServer)
        {
            var boxSize = boxPb.GetComponent<BoxCollider2D>().bounds.size.x;
            foreach (Transform platform in platforms)
            {
                for (int i = 0; i < height; i++)
                {
                    var pos = platform.position + new Vector3(0, (platform.GetComponent<Collider2D>().bounds.size.y / 2) + i, 0);
                    Instantiate(boxPb, pos, Quaternion.identity).Spawn(true);
                }
            }
        }
    }
}

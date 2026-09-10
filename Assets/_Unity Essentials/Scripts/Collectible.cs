using UnityEngine;

public class Collectible : MonoBehaviour
{
    public float rotationsSpeed;
    public GameObject onCollectEffect;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }
    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0,rotationsSpeed,0);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (onCollectEffect != null)
            {
                Instantiate(onCollectEffect, transform.position, Quaternion.identity);
            }
            else
            {
                Debug.LogWarning("onCollectEffect Empty Value", this);
            }
            Destroy(gameObject);
        }
    }
}

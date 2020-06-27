using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutOfBoundsRespawner : MonoBehaviour
{
    Vector3 mRespawnPosition;
    // Start is called before the first frame update
    void Start()
    {
        mRespawnPosition = GameObject.FindWithTag("Player").transform.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            other.GetComponent<Rigidbody>().velocity = Vector3.zero;
            other.transform.position = mRespawnPosition;
        }
    }
}

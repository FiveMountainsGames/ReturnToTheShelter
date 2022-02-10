using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public float speedObstacle;
    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        rb.velocity = Vector2.left * speedObstacle * GameManager.Instance.generalSpeed;

        if (transform.position.x <= -15)
        {
            ObjectPool.Instance.DeleteObject(this.gameObject);
        }
    }
}

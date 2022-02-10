using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdjustSortingLayer : MonoBehaviour
{
    private int orderNum;
    private SpriteRenderer spRenderer;
    private GameObject player;
    private Collider2D col;
    private PolygonCollider2D polCol;

    private void Start()
    {
        spRenderer = GetComponent<SpriteRenderer>();
        col = this.GetComponent<Collider2D>();
        player = GameObject.FindGameObjectWithTag("Player");
        orderNum = spRenderer.sortingOrder;
    }

    private void OnEnable()
    {
        if (this.gameObject.name == "tubeSimpleLeft")
        {
            spRenderer = GetComponent<SpriteRenderer>();
            polCol = GetComponent<PolygonCollider2D>();
            polCol.isTrigger = false;
            spRenderer.sortingOrder *= -1;
        }
    }

    private void Update()
    {
        if (this.gameObject.name == "ObstacleWarning" || this.gameObject.name == "RedBarrel" || this.gameObject.name == "Boxes")
        {
            if (transform.position.y > player.transform.position.y + 0.1f)
            {
                spRenderer.sortingOrder = -orderNum;
            }
            else
            {
                spRenderer.sortingOrder = orderNum;
            }
        }
    }
}

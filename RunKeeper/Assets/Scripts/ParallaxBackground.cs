using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{

    [SerializeField] GameObject[] layersParallax;

    [SerializeField] float speedLayer0 = .5f;
    [SerializeField] float speedLayer1 = 5.0f;
    [SerializeField] float speedLayer2 = 3.0f;
    [SerializeField] float speedLayer3 = 1.0f;
    private Vector2[] startPos = new Vector2[5];
    private float repeatWidth;

    private void Start()
    {
        startPos[0] = layersParallax[0].transform.position;
        startPos[1] = layersParallax[1].transform.position;
        startPos[2] = layersParallax[2].transform.position;
        startPos[3] = layersParallax[3].transform.position;
        repeatWidth = gameObject.GetComponent<BoxCollider2D>().size.x / 2;
    }

    private void Update()
    {
        if (GameManager.Instance != null)
        {
            layersParallax[0].transform.Translate(Vector2.left * Time.deltaTime * speedLayer0 * GameManager.Instance.generalSpeed);
            layersParallax[1].transform.Translate(Vector2.left * Time.deltaTime * speedLayer1 * GameManager.Instance.generalSpeed);
            layersParallax[2].transform.Translate(Vector2.left * Time.deltaTime * speedLayer2 * GameManager.Instance.generalSpeed);
            layersParallax[3].transform.Translate(Vector2.left * Time.deltaTime * speedLayer3 * GameManager.Instance.generalSpeed);
        }
        else
        {
            layersParallax[0].transform.Translate(Vector2.left * Time.deltaTime * speedLayer0);
            layersParallax[1].transform.Translate(Vector2.left * Time.deltaTime * speedLayer1);
            layersParallax[2].transform.Translate(Vector2.left * Time.deltaTime * speedLayer2);
            layersParallax[3].transform.Translate(Vector2.left * Time.deltaTime * speedLayer3);
        }
        RepeatLayers();
    }

    private void RepeatLayers()
    {
        if (layersParallax[0].transform.position.x < startPos[0].x - repeatWidth)
        {
            layersParallax[0].transform.position = startPos[0];
        }

        if (layersParallax[1].transform.position.x < startPos[1].x - repeatWidth)
        {
            layersParallax[1].transform.position = startPos[1];
        }

        if (layersParallax[2].transform.position.x < startPos[2].x - repeatWidth)
        {
            layersParallax[2].transform.position = startPos[2];
        }

        if (layersParallax[3].transform.position.x < startPos[3].x - repeatWidth)
        {
            layersParallax[3].transform.position = startPos[3];
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardHighlights : MonoBehaviour
{
    public static BoardHighlights Instance { set; get; }
    public GameObject highlightPrefab;
    private List<GameObject> highlights;

    private void Start()
    {
        Instance = this;
        highlights = new List<GameObject>();
    }

    //Get highlight object
    private GameObject getHighlightObject()
    {
        GameObject go = highlights.Find(g => !g.activeSelf);
        if(go == null)
        {
            go = Instantiate(highlightPrefab);
            highlights.Add(go);
        }
        return go;
    }

    //Highlights available moves for piece 
    public void highlightAllowedMoves(bool[,] moves)
    {
        for(int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if(moves[i,j])
                {
                    GameObject go = getHighlightObject();
                    go.SetActive(true);

                    Vector3 startPoint = Vector3.zero;
                    startPoint.x += 5f;
                    startPoint.y += 7.25f;
                    startPoint.z += (10f * 7) + 5f;
                    Vector3 origin = startPoint;
                    origin.x += (10f * i);
                    //origin.y += boardHeight;
                    origin.z -= (10f * j);

                    go.transform.position = origin;
                }
            }
        }
    }

    //Hide move highlights
    public void hideHighlights()
    {
        foreach(GameObject go in highlights)
        {
            go.SetActive(false);
        }
    }
}

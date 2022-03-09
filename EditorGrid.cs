using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class EditorGrid : MonoBehaviour {

    public int cell_size_in_pixels = 30;
    public Vector3 offset = Vector3.zero;
    private float x, y, z;
    private float cell_size;
    private Vector3 prev, movePos, tempPos;
    private Event e;

    public float getCellSize()
    {
        return cell_size;
    }

	// Use this for initialization
	void Start () {
        x = 0f;
        y = 0f;
        z = 0f;
        cell_size = cell_size_in_pixels * ((Camera.main.orthographicSize) / (Screen.height / 2.0f));
        prev.x = transform.position.x;
        prev.y = transform.position.y;
        prev.z = transform.position.z;
        ViewLevels.Instance.addGameObject(gameObject);

    }
	
	// Update is called once per frame
	void Update () {
        if (Application.isEditor && !Application.isPlaying)
        {
            if (transform.position.x != prev.x)
            {
                x = (Mathf.Round((transform.position.x ) / cell_size) * cell_size) + (cell_size / 2) + (offset.x * ((Camera.main.orthographicSize) / (Screen.height / 2.0f)));
                tempPos = transform.position;
                tempPos.x = x;
                transform.position = tempPos;
            }
            if (transform.position.y != prev.y)
            {
                y = (Mathf.Round((transform.position.y) / cell_size) * cell_size) + (cell_size / 2) + (offset.y * ((Camera.main.orthographicSize) / (Screen.height / 2.0f)));
                tempPos = transform.position;
                tempPos.y = y;
                transform.position = tempPos;
            }
            if(transform.position.z != prev.z)
            {
                z = (Mathf.Round((transform.position.z) / cell_size) * cell_size) + (offset.z * ((Camera.main.orthographicSize) / (Screen.height / 2.0f)));
                tempPos = transform.position;
                tempPos.z = z;
                transform.position = tempPos;
            }
            if(prev != transform.position)
            {
                ViewLevels.Instance.removeGameObject(gameObject, prev.z);
                ViewLevels.Instance.addGameObject(gameObject);
            }
            prev = transform.position;
        }
    }

    void OnDestroy()
    {
        ViewLevels.Instance.removeGameObject(gameObject);
    }
}

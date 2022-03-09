using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelController : MonoBehaviour {

    private bool paused;
    public GameObject enemy;
    public GameObject wall;
    private Resetable[] startingObjects;
    private List<Resetable> resetableObjects;
    private List<Placeable> placedObjects;
    public string nextLevelName;

    public void nextLevel()
    {
        SceneManager.LoadScene(nextLevelName, LoadSceneMode.Single);
    }

	// Use this for initialization
	void Start () {
        setPaused(true);
        startingObjects = Object.FindObjectsOfType<Resetable>();
        resetableObjects = new List<Resetable>();
        placedObjects = new List<Placeable>();
	}
	
	// Update is called once per frame
	void Update () {
		if(paused)
        {
            if(Input.GetKeyDown(KeyCode.P))
            {
                setPaused(false);
            }
            else if(Input.GetKeyDown(KeyCode.E))
            {
                GameObject _enemy = Instantiate(enemy, transform);
                _enemy.GetComponent<Placeable>().enabled = true;
                resetableObjects.Add(_enemy.GetComponent<Resetable>());
                placedObjects.Add(_enemy.GetComponent<Placeable>());
            }
            else if (Input.GetKeyDown(KeyCode.W))
            {
                GameObject _wall = Instantiate(wall, transform);
                _wall.GetComponent<Placeable>().enabled = true;
                placedObjects.Add(_wall.GetComponent<Placeable>());
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                softReset();
            }
            else if (Input.GetKeyDown(KeyCode.R))
            {
                hardReset();
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                setPaused(true);
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                softReset();
            }
            else if (Input.GetKeyDown(KeyCode.R))
            {
                hardReset();
            }
        }
	}

    public bool getPaused()
    {
        return paused;
    }

    public void setPaused(bool _paused)
    {
        paused = _paused;
        Pauseable[] pauseables = GameObject.FindObjectsOfType<Pauseable>();
        if(paused)
        {
            foreach (Pauseable pauseable in pauseables)
            {
                pauseable.setPaused(true);
            }
        }
        else
        {
            foreach (Pauseable pauseable in pauseables)
            {
                pauseable.setPaused(false);
            }
        }
    }

    public void softReset()
    {
        foreach (Resetable resetableObject in resetableObjects)
        {
            resetableObject.reset();
        }
        foreach (Resetable startingObject in startingObjects)
        {
            startingObject.reset();
        }
    }

    public void hardReset()
    {
        foreach (Placeable placedObject in placedObjects)
        {
            Destroy(placedObject.gameObject);
        }
        placedObjects.Clear();
        resetableObjects.Clear();
        foreach (Resetable startingObject in startingObjects)
        {
            startingObject.reset();
        }
    }

    public void goalReached()
    {
        softReset();
        setPaused(true);
    }
}

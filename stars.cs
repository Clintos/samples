using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class stars : Singleton<stars>
{
    List<GameObject> stars_list = new List<GameObject> { };

    public void addStar(GameObject star)
    {
        stars_list.Add(star);
    }

    public void removeStar(GameObject star)
    {
        stars_list.Remove(star);
        Destroy(star);
    }

    public void clear()
    {
        foreach (GameObject s in stars_list)
        {
            Destroy(s);
        }
        stars_list.Clear();
    }

    public void startRecord()
    {
        foreach (GameObject s in stars_list)
        {
            s.GetComponent<star_controller>().startRecord();
        }
    }

    public void rewindRecord()
    {
        foreach (GameObject s in stars_list)
        {
            s.GetComponent<star_controller>().rewindRecord();
        }
    }

    public void forwardRecord()
    {
        foreach (GameObject s in stars_list)
        {
            s.GetComponent<star_controller>().forwardRecord();
        }
    }

    public void pauseRecord()
    {
        foreach (GameObject s in stars_list)
        {
            s.GetComponent<star_controller>().pauseRecord();
        }
    }

    public void pauseRecordStop()
    {
        foreach (GameObject s in stars_list)
        {
            s.GetComponent<star_controller>().pauseRecordStop();
        }
    }

    public void stopRecord()
    {
        foreach (GameObject s in stars_list)
        {
            s.GetComponent<star_controller>().stopRecord();
        }
    }

    public void startPlay()
    {
        foreach (GameObject s in stars_list)
        {
            s.GetComponent<star_controller>().startPlay();
        }
    }

    public void rewindPlay()
    {
        foreach (GameObject s in stars_list)
        {
            s.GetComponent<star_controller>().rewindPlay();
        }
    }

    public void pausePlay()
    {
        foreach (GameObject s in stars_list)
        {
            s.GetComponent<star_controller>().pausePlay();
        }
    }

    public void pausePlayStop()
    {
        foreach (GameObject s in stars_list)
        {
            s.GetComponent<star_controller>().pausePlayStop();
        }
    }

    public void stopPlay()
    {
        foreach (GameObject s in stars_list)
        {
            s.GetComponent<star_controller>().stopPlay();
        }
    }

    public void startMove()
    {
        foreach (GameObject s in stars_list)
        {
            s.GetComponent<star_controller>().startMove();
        }
    }

    public void stopMove()
    {
        foreach (GameObject s in stars_list)
        {
            s.GetComponent<star_controller>().stopMove();
        }
    }

    public void resetRecord()
    {
        foreach (GameObject s in stars_list)
        {
            s.GetComponent<star_controller>().resetRecord();
        }
    }

    public void showNotes()
    {
        foreach (GameObject s in stars_list)
        {
            s.GetComponent<star_controller>().showNote();
        }
    }

    public void hideNotes()
    {
        foreach (GameObject s in stars_list)
        {
            s.GetComponent<star_controller>().hideNote();
        }
    }

    public void save(StreamWriter w)
    {
        foreach (GameObject s in stars_list)
        {
            w.WriteLine(s.GetComponent<star_controller>().save());
        }
    }

    public void load(StreamReader r, GameObject star_prefab)
    {
        string text = r.ReadLine();
        clear();
        while(text != null)
        {
            GameObject temp = Instantiate(star_prefab, new Vector3(0,0,0), Quaternion.identity) as GameObject;
            temp.GetComponent<star_controller>().load(text);
            addStar(temp);
            text = r.ReadLine();
        }
    }

}

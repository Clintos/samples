using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class star_controller : MonoBehaviour
{
    public GameObject star_model;
    public GameObject star_light;
    public GameObject click_effect, star_idle_effect, star_glow;
    public float scale;
    public float light_range;
    public AudioClip[] note;
    public int note_int,octave_int;
    AudioSource audio_source;
    Light star_light_light;
    SphereCollider col;
    RaycastHit hit;
    Ray ray;
    MeshRenderer mesh;
    ParticleSystem click_part, star_idle_part, star_glow_part;
    ParticleSystem.MainModule click_part_settings, star_idle_part_settings, star_glow_part_settings;
    string[] note_string = new string[12] { "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B" };
    public Color color;
    LinkedList<int> timeline = new LinkedList<int> { };
    LinkedListNode<int> timeline_node;
    bool record = false;
    bool play = false;
    bool move = false;
    bool follow = false;
    bool display = false;
    bool selected = false;
    int record_count = 0;
    int play_count = 0;
    Vector3 pos, UI_pos, off_pos, t_pos;
    Vector2 offset;
    GUIStyle centeredStyle;
    int label_size;

    public string save()
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        string text = transform.position.x + " " + transform.position.y + " " + transform.position.z + " " 
            + UI_pos.x + " " + UI_pos.y + " " + UI_pos.z + " "
            + offset.x + " " + offset.y + " "
            + color.r + " " + color.g + " " + color.b + " " 
            + note_int + " " 
            + octave_int;
        sb.Append(text);
        LinkedListNode<int> t_node = timeline.First;
        while(t_node != null)
        {
            sb.Append(" " + t_node.Value);
            t_node = t_node.Next;
        }
        return sb.ToString();
    }

    public void load(string text)
    {
        string[] strArr = text.Split(" "[0]);
        transform.position = new Vector3(float.Parse(strArr[0]), float.Parse(strArr[1]), float.Parse(strArr[2]));
        UI_pos = new Vector3(float.Parse(strArr[3]), float.Parse(strArr[4]), float.Parse(strArr[5]));
        offset = new Vector2(float.Parse(strArr[6]), float.Parse(strArr[7]));
        setColor(new Color(float.Parse(strArr[8]), float.Parse(strArr[9]), float.Parse(strArr[10])));
        setNote(int.Parse(strArr[11]), int.Parse(strArr[12]));
        LinkedListNode<int> t_node;
        t_node = timeline.First;
        for(int i = 13; i  < strArr.Length; i++)
        {
            if(i == 13)
            {
                timeline.AddFirst(int.Parse(strArr[i]));
                t_node = timeline.First;
            }
            else
            {
                timeline.AddAfter(t_node,int.Parse(strArr[i]));
                t_node = t_node.Next;
            }
        }
    }

    public void toggleSelected()
    {
        selected = !selected;
    }

    public void startMove()
    {
        move = true;
    }

    public void stopMove()
    {
        move = false;
    }

    public void startRecord()
    {
        record_count = 0;
        record = true;
        startPlay();
    }

    public void pauseRecord()
    {
        pausePlay();
        record = false;
    }

    public void pauseRecordStop()
    {
        pausePlayStop();
        record = true;
    }

    public void rewindRecord()
    {
        record_count = 0;
        rewindPlay();
    }

    public void forwardRecord()
    {
        forwardPlay();
        record_count = play_count;
    }

    public void stopRecord()
    {
        record_count = 0;
        record = false;
        stopPlay();
    }

    public void startPlay()
    {
        timeline_node = timeline.First;
        play_count = 0;
        play = true;
    }

    public void pausePlay()
    {
        play = false;
    }

    public void pausePlayStop()
    {
        play = true;
    }

    public void rewindPlay()
    {
        play_count = 0;
        timeline_node = timeline.First;
    }

    public void forwardPlay()
    {
        play_count = timeline.Last.Value + 1;
        timeline_node = null;
    }

    public void stopPlay()
    {
        audio_source.Stop();
        play_count = 0;
        play = false;
    }

    public void showNote()
    {
        display = true;
    }

    public void hideNote()
    {
        display = false;
    }

    public void resetRecord()
    {
        timeline.Clear();
    }
    // Use this for initialization
    void Awake()
    {
        UI_pos = Camera.main.WorldToScreenPoint(transform.position);
        offset.y = (Screen.height - UI_pos.y) / Screen.height;
        offset.x = (Screen.width - UI_pos.x) / Screen.width;
        note_int = 0;
        octave_int = 0;
        click_part = click_effect.GetComponent<ParticleSystem>();
        star_idle_part = star_idle_effect.GetComponent<ParticleSystem>();
        star_glow_part = star_glow.GetComponent<ParticleSystem>();
        click_part_settings = click_part.main;
        star_idle_part_settings = star_idle_part.main;
        star_glow_part_settings = star_glow_part.main;
        mesh = star_model.GetComponent<MeshRenderer>();
        star_model.transform.localScale = new Vector3(scale, scale, scale);
        col = GetComponent<SphereCollider>();
        col.radius = (3*scale)/4;
        star_light_light = star_light.GetComponent<Light>();
        setColor(color);
        star_light_light.range = light_range;
        audio_source = star_model.GetComponent<AudioSource>();
        setNote(note_int,octave_int);
    }

    // Update is called once per frame
    void Update()
    {
        off_pos = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width * offset.x, Screen.height * offset.y, 0));
        t_pos = transform.position;
        t_pos.x = off_pos.x;
        t_pos.y = off_pos.y;
        transform.position = t_pos;
        if (play || record)
        {
            play_count++;
            if (timeline_node != null)
            {
                if (timeline_node.Value == play_count)
                {
                    timeline_node = timeline_node.Next;
                    doClick();
                }
            }
        }
        if (record && !move)
        {
            record_count++;
            if(Clicked())
            {
                if (timeline_node == null)
                {
                    if (timeline.First == null)
                    {
                        timeline.AddFirst(record_count);
                        timeline_node = timeline.First;
                    }
                    else
                    {
                        timeline.AddLast(record_count);
                        timeline_node = timeline.Last;
                    }
                }
                else
                {
                    if (timeline_node.Value < record_count)
                    {
                        timeline.AddAfter(timeline_node, record_count);
                        timeline_node = timeline_node.Next;
                    }
                    else if (timeline_node.Value > record_count)
                    {
                        timeline.AddBefore(timeline_node, record_count);
                    }
                }
                doClick();
            }
        }
        else
        {
            if (move)
            {
                if(Clicked())
                {
                    follow = !follow;
                }
            }
            else
            {
                if (Clicked())
                {
                    doClick();
                }
            }
            if(follow)
            {
                pos = transform.position;
                transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                pos.x = transform.position.x;
                pos.y = transform.position.y;
                transform.position = pos;
                UI_pos = Camera.main.WorldToScreenPoint(transform.position);
                offset.y = (UI_pos.y) / Screen.height;
                offset.x = (UI_pos.x) / Screen.width;
                if (Input.GetMouseButtonUp(0))
                {
                    follow = !follow;
                }
            }
        }
    }

    void OnGUI()
    {
        if(display)
        {
            centeredStyle = GUI.skin.GetStyle("Label");
            centeredStyle.alignment = TextAnchor.MiddleCenter;
            UI_pos = Camera.main.WorldToScreenPoint(transform.position);
            GUI.skin.label.fontSize = Mathf.Min(Screen.width, Screen.height) / 16;
            label_size = Mathf.Min(Screen.width, Screen.height) / 8 ;
            GUI.Label(new Rect(UI_pos.x-(label_size/2), Screen.height - UI_pos.y - label_size, label_size, label_size), note_string[note_int] + octave_int, centeredStyle);
        }
    }

    bool Clicked()
    {
        if (Input.GetMouseButtonDown(0))
        {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform == transform)
                {
                    return true;
                }
            }
        }
        else
        {
            for (int i = 0; i < Input.touchCount; ++i)
            {
                if (Input.GetTouch(i).phase == TouchPhase.Began)
                {
                    ray = Camera.main.ScreenPointToRay(Input.GetTouch(i).position);

                    if (Physics.Raycast(ray, out hit))
                    {
                        if (hit.transform == transform)
                        {
                            return true;
                        }
                    }
                }
            }
        }
        return false;
    }

    void doClick()
    {
        click_part.Emit(360);
        audio_source.Stop();
        audio_source.Play();
    }

    public void setNote(int n, int o)
    {
        note_int = n;
        octave_int = o;
        audio_source.clip = note[octave_int];
        audio_source.pitch = Mathf.Pow(2f, ((float)note_int) / 12.0f);
    }

    public int getNote()
    {
        return note_int;
    }

    public int getOctave()
    {
        return octave_int;
    }

    public void setColor(Color c)
    {
        color = c;
        star_light_light.color = c;
        click_part_settings.startColor = c;
        star_idle_part_settings.startColor = c;
        star_glow_part_settings.startColor = c;
        mesh.material.color = c;
    }

    public Color getColor()
    {
        return color;
    }
}

using SFB;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VisController : MonoBehaviour
{

    public Transform cube1;

    //#public Text output;

    public enum DataTypes { None, MT, };

    public DataTypes dataType = DataTypes.None;
    public bool dataReady = false;
    public bool visStarted = false;
    public int currentTick = 0;
    public float timePlaying = 0.0f;

    public List<float[]> MTlist;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("q"))
        {
            OpenFile();
        }

        if (dataType != DataTypes.None && dataReady && !visStarted) {
            currentTick = 0;
            timePlaying = 0.0f;
            visStarted = true;
        }

        if (visStarted) {
            if (dataType == DataTypes.MT) {
                if (currentTick <= MTlist.Count - 1) {

                    while (timePlaying > 10.0f) { 
                        var v = MTlist[currentTick];
                        Quaternion quat = new Quaternion(v[11], v[12], v[13], v[14]);
                        cube1.rotation = quat;
                        currentTick++;
                        timePlaying -= 10.0f;
                    }
                }
            }
        }

        timePlaying += Time.deltaTime*1000;
    }

    void OpenFile() {
        DataUnready();
        var paths = StandaloneFileBrowser.OpenFilePanel("Open File", "", "", false);
        var loader = new WWW(new System.Uri(paths[0]).AbsoluteUri);
        ParseData(loader.text);
    }

    void DataUnready() {
        dataReady = false;
        dataType = DataTypes.None;
    }

    void ParseData(string data) {
        var lines = data.Split('\n');

        

        for (int i = 0; i < lines.Length; i++)
        {
            var line = lines[i];

            if (dataType == DataTypes.None)
            {
                if (line.Contains("MT Manager")) {
                    dataType = DataTypes.MT;
                    MTlist = new List<float[]>();
                }
            }

            if (line.StartsWith("//") || line.StartsWith("PacketCounter")) continue;

            if (dataType == DataTypes.MT) {
                var v = line.Split('	');
                if (v.Length == 18) { 
                    float[] vars = {
                        float.Parse(v[0]),
                        float.Parse(v[1]),
                        float.Parse(v[2]),
                        float.Parse(v[3]),
                        float.Parse(v[4]),
                        float.Parse(v[5]),
                        float.Parse(v[6]),
                        float.Parse(v[7]),
                        float.Parse(v[8]),
                        float.Parse(v[9]),
                        float.Parse(v[10]),
                        float.Parse(v[11]),
                        float.Parse(v[12]),
                        float.Parse(v[13]),
                        float.Parse(v[14]),
                        float.Parse(v[15]),
                        float.Parse(v[16]),
                        float.Parse(v[17])
                        };
                    MTlist.Add(vars);
                }
            }
        }

        dataReady = true;
    }
}

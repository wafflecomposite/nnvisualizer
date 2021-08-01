using SFB;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VisController : MonoBehaviour
{

    public Transform cube1;
    public Transform cube2;

    //#public Text output;

    public enum DataTypes { None, MT, RRN0 };

    public DataTypes dataType = DataTypes.None;
    public bool dataReady = false;
    public bool visStarted = false;
    public bool pause = false;
    public int forceTick = 0;
    public int currentTick = 0;
    public float timePlaying = 0.0f;
    public Text progressLabel;
    public Text filetypeLabel;

    public List<float[]> genericFloatList;

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
                filetypeLabel.text = "Type: Raw MT, Objects: 1";
                if (currentTick <= genericFloatList.Count - 1) {

                    while (timePlaying > 10.0f) {
                        progressLabel.text = string.Format("{0}/{1}", currentTick, genericFloatList.Count);
                        var v = genericFloatList[currentTick];
                        Quaternion quat = new Quaternion(v[11], v[12], v[13], v[14]);
                        cube1.rotation = quat;
                        //cube2.rotation = Quaternion.Euler(v[15], v[16], v[17]);
                        Quaternion quat2 = Quaternion.identity;
                        quat2.eulerAngles = new Vector3(-v[16], -v[17], -v[15]);
                        cube2.localRotation = quat2;
                        currentTick++;
                        forceTick = currentTick;
                        timePlaying -= 10.0f;
                    }
                    if (forceTick != currentTick)
                    {
                        progressLabel.text = string.Format("{0}/{1}", forceTick, genericFloatList.Count);
                        var v = genericFloatList[forceTick];
                        Quaternion quat = new Quaternion(v[11], v[12], v[13], v[14]);
                        cube1.rotation = quat;
                        Quaternion quat2 = Quaternion.identity;
                        quat2.eulerAngles = new Vector3(-v[16], -v[17], -v[15]);
                        cube2.localRotation = quat2;
                    }
                }
            }
            else if (dataType == DataTypes.RRN0)
            {
                filetypeLabel.text = "Type: RNN0, Objects: 2";
                if (currentTick <= genericFloatList.Count - 1)
                {
                    while (timePlaying > 10.0f)
                    {
                        progressLabel.text = string.Format("{0}/{1}", currentTick, genericFloatList.Count);
                        var v = genericFloatList[currentTick];
                        //Quaternion quat = new Quaternion(v[11], v[12], v[13], v[14]);
                        cube1.rotation = Quaternion.Euler(v[0], 0, v[1]);
                        cube2.rotation = Quaternion.Euler(v[2], 0, v[3]);
                        currentTick++;
                        forceTick = currentTick;
                        timePlaying -= 10.0f;
                    }
                    if (forceTick != currentTick) {
                        progressLabel.text = string.Format("{0}/{1}", forceTick, genericFloatList.Count);
                        var v = genericFloatList[forceTick];
                        cube1.rotation = Quaternion.Euler(v[0], 0, v[1]);
                        cube2.rotation = Quaternion.Euler(v[2], 0, v[3]);
                    }
                }
            }
        }

        if (!pause) timePlaying += Time.deltaTime*1000;
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
        dataType = DataTypes.None;
        currentTick = 0;
        timePlaying = 0.0f;


        for (int i = 0; i < lines.Length; i++)
        {
            var line = lines[i];

            if (dataType == DataTypes.None)
            {
                if (line.Contains("MT Manager")) {
                    dataType = DataTypes.MT;
                    genericFloatList = new List<float[]>();
                }
                else if (line.Contains("#rnn_mt_0"))
                {
                    dataType = DataTypes.RRN0;
                    genericFloatList = new List<float[]>();
                }
            }

            if (line.StartsWith("//") || line.StartsWith("PacketCounter") || line.StartsWith("#")) continue;

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
                    genericFloatList.Add(vars);
                }
            }
            else if (dataType == DataTypes.RRN0)
            {
                var v = line.Split('	');
                if (v.Length == 4)
                {
                    float[] vars = {
                        float.Parse(v[0]),
                        float.Parse(v[1]),
                        float.Parse(v[2]),
                        float.Parse(v[3])
                        };
                    genericFloatList.Add(vars);
                }
            }
        }

        dataReady = true;
    }
}

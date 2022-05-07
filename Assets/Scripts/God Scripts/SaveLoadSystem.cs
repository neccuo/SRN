using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class SaveLoadSystem : MonoBehaviour
{
    private class MiniNpc
    {
        public int id;
        public float x_axis;
        public float y_axis;

        public MiniNpc(int id, float x, float y)
        {
            this.id = id;
            this.x_axis = x;
            this.y_axis = y;
        }
    }

    float TwoFRound(float num) // YOU LEFT HERE
    {
        return (float) Math.Round(num, 2);
        //return (float) Math.Round(num * 100f) / 100f;
    }

    public string TraverseNpcs()
    {
        string jsonString = "[";
        int idRegister;
        int i = 0;
        foreach(Transform child in GameObject.Find("NPCs").transform)
        {
            if(i != 0)
                jsonString += ",";
            idRegister = child.gameObject.GetComponent<NPC>().GetNPCID();
            jsonString += JsonUtility.ToJson(new MiniNpc(idRegister, child.position.x, child.position.y));
            i++;
        }
        jsonString += "]"; 
        return jsonString;
    }

    public IEnumerator SavePos()
    {
        WWWForm form = new WWWForm();
        form.AddField("npcpos_json", TraverseNpcs());
        Debug.Log(TraverseNpcs());
        UnityWebRequest www = UnityWebRequest.Post("http://localhost/sqlconnect/savepos.php", form);
        yield return www.SendWebRequest();
        if(www.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("register: success");
            Debug.Log(www.downloadHandler.text);
        }
        else
        {
            Debug.Log("register: fail");
            Debug.Log(www.error);
            Debug.Log(www.result);
            Debug.Log(www.downloadHandler.error);
            Debug.Log(www.downloadHandler.text);
        }
    }


    public IEnumerator Load()
    {
        UnityWebRequest www = UnityWebRequest.Get("http://localhost/sqlconnect/load.php");
        yield return www.SendWebRequest();
        if( www.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("load: success");
            Debug.Log(www.downloadHandler.text);
        }
        else
        {
            Debug.Log("load: fail");
            Debug.Log(www.error);
            Debug.Log(www.result);

        }
    }

    public IEnumerator Register(NPC newNpc)
    {
        WWWForm form = new WWWForm();
        form.AddField("id", newNpc.GetNPCID().ToString());
        form.AddField("name", newNpc.gameObject.name);
        form.AddField("credits", "1500");
        form.AddField("race", "human");
        form.AddField("hull_id", "1");
        form.AddField("x_axis", newNpc.transform.position.x.ToString());
        form.AddField("y_axis", newNpc.transform.position.y.ToString());
        UnityWebRequest www = UnityWebRequest.Post("http://localhost/sqlconnect/register.php", form);
        yield return www.SendWebRequest();
        if(www.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("register: success");
        }
        else
        {
            Debug.Log("register: fail");
            Debug.Log(www.error);
            Debug.Log(www.result);
        }
    }

    public IEnumerator EraseNPCS()
    {
        UnityWebRequest www = UnityWebRequest.Get("http://localhost/sqlconnect/erase_npcs.php");
        yield return www.SendWebRequest();
        if(www.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("register: success");
            Debug.Log(www.downloadHandler.text);
        }
        else
        {
            Debug.LogError("register: fail");
            Debug.Log(www.error);
            Debug.Log(www.result);
        }
    }
}

    

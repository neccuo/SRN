using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SaveLoadSystem : MonoBehaviour
{
    private ShipSpawner _shipSpawner;

    [SerializeField] private GameObject _npcManager; // Needs update, check here



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

    public class MedNpc
    {
        public int id;
        public string name;
        public int credits;
        public string race;
        public int ship_id;
        public float x_axis;
        public float y_axis;

        public void SetByIndex(int num, string val)
        {
            switch(num)
            {
                case 0:
                    id = Int32.Parse(val);
                    break;
                case 1:
                    name = val;
                    break;
                case 2:
                    credits = Int32.Parse(val);
                    break;
                case 3:
                    race = val;
                    break;
                case 4:
                    ship_id = Int32.Parse(val);
                    break;
                case 5:
                    x_axis = float.Parse(val);
                    break;
                case 6:
                    y_axis = float.Parse(val);
                    break;
                default:
                    throw new Exception("Invalid index value");
            }
        }

        public override string ToString()
        {
            return "(" + id + ", " + name + ", " + x_axis + ", " + y_axis + ")";
        }
    }

    // TODO: USE something like OnGameStartUp() 
    void Start()
    {
        _shipSpawner = gameObject.GetComponent<ShipSpawner>();

    }

    void Update()
    {
        // BEWARE :D
    }

    public void LoadAllNpcs()
    {
        StartCoroutine(LoadDB((retVal) => 
        {
            // Debug.Log(retVal);
            List<MedNpc> MedNpcList = ParseStrToList(retVal, 7 ); // 7 for now, maybe later something like ===>>> typeof(MedNpc).GetProperties().Length
            foreach(MedNpc npc in MedNpcList)
            {
                _shipSpawner.LoadExistingNpc(npc.id, npc.name, npc.ship_id ,npc.x_axis, npc.y_axis);
            }
        }));
    }

    List<MedNpc> ParseStrToList(string json, int elemPer) // elemPer is field size of MedNpc
    {
        List<string> list = new List<string>(json.Split(','));
        for(int i = 0; i < list.Count; i++) {list[i] = list[i].Trim('[', ']', '"');}
        if(elemPer > 0 && list.Count % elemPer != 0){throw new Exception("It is not possible to create equal elements");}

        List<MedNpc> retVal = new List<MedNpc>();
        MedNpc medNpcPointer = new MedNpc();
        for(int i = 0; i < list.Count; i++)
        {
            medNpcPointer.SetByIndex(i%elemPer, list[i]);
            if(i % elemPer == elemPer-1) // 4 % 5 == 4
            {
                retVal.Add(medNpcPointer);
                medNpcPointer = new MedNpc();
            }
        }
        return retVal;
    }

    // bok
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
        foreach(Transform child in _npcManager.transform)
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

    public IEnumerator NpcItemBuy(int npcID, int itemID, int quantity)
    {
        WWWForm form = new WWWForm();
        form.AddField("pilot_id", npcID.ToString());
        form.AddField("item_id", itemID.ToString());
        form.AddField("quantity", quantity.ToString());
        UnityWebRequest www = UnityWebRequest.Post("http://localhost/sqlconnect/npc_item_buy.php", form);
        yield return www.SendWebRequest();
        if(www.result == UnityWebRequest.Result.Success)
        {
            // Debug.Log("BUY: success");
            Debug.Log(www.downloadHandler.text);
        }
        else
        {
            Debug.Log("BUY: fail");
            Debug.Log(www.error);
            Debug.Log(www.result);
        }
    }
    public IEnumerator NpcItemBuy(int npcID, int itemID, int quantity, System.Action<string> callback)
    {
        WWWForm form = new WWWForm();
        form.AddField("pilot_id", npcID.ToString());
        form.AddField("item_id", itemID.ToString());
        form.AddField("quantity", quantity.ToString());
        UnityWebRequest www = UnityWebRequest.Post("http://localhost/sqlconnect/npc_item_buy.php", form);
        yield return www.SendWebRequest();
        if(www.result == UnityWebRequest.Result.Success)
        {
            callback(www.downloadHandler.text);
            Debug.Log(www.downloadHandler.text);
        }
        else
        {
            callback("-");
            Debug.Log(www.error);
            Debug.Log(www.result);
        }
    }

    public IEnumerator NpcItemSell(int npcID, int itemID, int quantity)
    {
        WWWForm form = new WWWForm();
        form.AddField("pilot_id", npcID.ToString());
        form.AddField("item_id", itemID.ToString());
        form.AddField("quantity", quantity.ToString());
        UnityWebRequest www = UnityWebRequest.Post("http://localhost/sqlconnect/npc_item_sell.php", form);
        yield return www.SendWebRequest();
        if(www.result == UnityWebRequest.Result.Success)
        {
            Debug.Log(www.downloadHandler.text);
        }
        else
        {
            Debug.Log("SELL: fail");
            Debug.Log(www.error);
            Debug.Log(www.result);
        }
    }

    public IEnumerator NpcItemSell(int npcID, int itemID, int quantity, System.Action<string> callback)
    {
        WWWForm form = new WWWForm();
        form.AddField("pilot_id", npcID.ToString());
        form.AddField("item_id", itemID.ToString());
        form.AddField("quantity", quantity.ToString());
        UnityWebRequest www = UnityWebRequest.Post("http://localhost/sqlconnect/npc_item_sell.php", form);
        yield return www.SendWebRequest();
        if(www.result == UnityWebRequest.Result.Success)
        {
            callback(www.downloadHandler.text);
            Debug.Log(www.downloadHandler.text);
        }
        else
        {
            callback("-");
            Debug.Log(www.error);
            Debug.Log(www.result);
        }
    }

    public IEnumerator SavePos()
    {
        WWWForm form = new WWWForm();
        form.AddField("npcpos_json", TraverseNpcs());
        // Debug.Log(TraverseNpcs());
        UnityWebRequest www = UnityWebRequest.Post("http://localhost/sqlconnect/savepos.php", form);
        yield return www.SendWebRequest();
        if(www.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("SavePos: success");
            // Debug.Log(www.downloadHandler.text);
        }
        else
        {
            Debug.Log("SavePos: fail");
            Debug.Log(www.error);
            Debug.Log(www.result);
        }
    }

    public IEnumerator LoadDB(System.Action<string> callback)
    {
        UnityWebRequest www = UnityWebRequest.Get("http://localhost/sqlconnect/load.php");
        yield return www.SendWebRequest();
        if( www.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("LoadDB(string): success");
            callback(www.downloadHandler.text);
            // Debug.Log(www.downloadHandler.text);
        }
        else
        {
            Debug.Log("LoadDB(string): fail");
            Debug.Log(www.error);
            Debug.Log(www.result);
            callback("-");
        }
    }

    public IEnumerator LoadDB()
    {
        UnityWebRequest www = UnityWebRequest.Get("http://localhost/sqlconnect/load.php");
        yield return www.SendWebRequest();
        if( www.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("LoadDB(): success");
            Debug.Log(www.downloadHandler.text);
        }
        else
        {
            Debug.Log("LoadDB(): fail");
            Debug.Log(www.error);
            Debug.Log(www.result);
        }
    }

    public IEnumerator Register(NPC newNpc, System.Action<int> callback=null)
    {
        WWWForm form = new WWWForm();
        form.AddField("id", newNpc.GetNPCID().ToString());
        form.AddField("name", newNpc.gameObject.name);
        form.AddField("credits", "15000");
        form.AddField("race", "human");
        form.AddField("hull_id", newNpc.ship.GetHullID().ToString());
        form.AddField("x_axis", newNpc.transform.position.x.ToString());
        form.AddField("y_axis", newNpc.transform.position.y.ToString());
        UnityWebRequest www = UnityWebRequest.Post("http://localhost/sqlconnect/register.php", form);
        yield return www.SendWebRequest();
        if(www.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Register: success");
            callback(1);
        }
        else
        {
            Debug.Log("Register: fail");
            Debug.Log(www.error);
            Debug.Log(www.result);
            callback(-1);
        }
    }

    /* TODO: REMEMBER UNITY EVENTS FOR THIS */
    public IEnumerator UpdatePrices()
    {
        UnityWebRequest www = UnityWebRequest.Get("http://localhost/sqlconnect/item_price_update.php");
        yield return www.SendWebRequest();
        if(www.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("UpdatePrices: success");
            // Debug.Log(www.downloadHandler.text);
        }
        else
        {
            Debug.LogError("UpdatePrices: fail");
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
            Debug.Log("EraseNPCS: success");
            Debug.Log(www.downloadHandler.text);
        }
        else
        {
            Debug.LogError("EraseNPCS: fail");
            Debug.Log(www.error);
            Debug.Log(www.result);
        }
    }
}

    

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

public class ShopConfigInfo
{
    public int id;
    public string goods;
    public float attack;
    public float[] attackUp;
    public int[] levelUp;
    public string[] descript;
}

public class ShopConfig : MonoBehaviour
{
    private static ShopConfig m_ShopConfig;
    public static ShopConfig singleton{ get{ return m_ShopConfig;}}
    public Dictionary<int,ShopConfigInfo> m_ShopConfigInfo = new Dictionary<int,ShopConfigInfo>();
    
    void Awake()
    {
        m_ShopConfig = this;
        LoadXml();
    }
    void LoadXml()
    {
        TextAsset t;
        string data = Resources.Load("Xml/ShopConfig").ToString();
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(data);
        XmlNodeList nodeList = xmlDoc.SelectNodes("root / node");
        ShopConfigInfo info = null;
        foreach (XmlNode xmlNode in nodeList)
        {
            info = new ShopConfigInfo();
            foreach (XmlAttribute s in xmlNode.Attributes)
            {
                switch (s.Name)
                {
                    case "int_id":
                        info.id = int.Parse(s.Value);
                        break;
                    case "string_goods":
                        info.goods = s.Value;
                        break;
                    case "float_attack":
                        info.attack = float.Parse(s.Value);
                        break;
                    case "arrayFloat_attackUp":
                        info.attackUp = System.Array.ConvertAll(s.Value.Split('_'),float.Parse);
                        break;
                    case "arrayInt_levelUp":
                        info.levelUp = System.Array.ConvertAll(s.Value.Split('_'),int.Parse);
                        break;
                    case "arrayString_descript":
                        info.descript = s.Value.Split('_');
                        break;
                }
            }
            m_ShopConfigInfo[info.id] = info;
        }
    }
}

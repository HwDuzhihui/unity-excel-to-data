using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Text;
using System.IO;
using System.Xml;

/*
 *  一键导表
 *  
 *  使用方法：配合tools,将xml导入Resources
 *            生成Mono挂到场景
 */
public class EasyCodeAutoXml : Editor {


    static string m_XmlFile = Application.dataPath + "/Resources/Xml";
    static string m_ScriptsFile = Application.dataPath + "/Scripts/";
    static string m_Tab = "    ";
    static string split = "_";


    [MenuItem("EasyCodeEditor/Create Xml Code")]
    public static void CreateCode()
    {
        DirectoryInfo direction = new DirectoryInfo(m_XmlFile);
        FileInfo[] files = direction.GetFiles("*", SearchOption.AllDirectories);

        for(int i=0;i<files.Length;i++)
        {
            string scriptName = files[i].Name;
            if (scriptName.EndsWith(".meta")) continue;
            scriptName = scriptName.Split('.')[0];

            CodeFormat(scriptName);
        }

    }


    static void CodeFormat(string scriptName)
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("using UnityEngine;");
        sb.AppendLine("using System.Collections;");
        sb.AppendLine("using System.Collections.Generic;");
        sb.AppendLine("using System.Xml;");
        sb.AppendLine();

        sb.AppendLine("public class " + scriptName + "Info");
        sb.AppendLine("{");
        List<string> nodeNames = NodeList(scriptName);
        for(int i=0;i<nodeNames.Count;i++)
        {
            sb.Append(m_Tab);
            string[] s = nodeNames[i].Split(split.ToCharArray()[0]);
            if(s[0].ToLower() == "arrayint")
                sb.AppendLine("public " + "int[]" + " " + s[1] + ";");
            else if (s[0].ToLower() == "arrayfloat")
                sb.AppendLine("public " + "float[]" + " " + s[1] + ";");
            else if (s[0].ToLower() == "arraystring")
                sb.AppendLine("public " + "string[]" + " " + s[1] + ";");
            else
                sb.AppendLine("public " + s[0].ToLower() + " " + s[1] + ";");
        }
        sb.AppendLine("}");
        sb.AppendLine();

        sb.AppendLine("public class " + scriptName + " : MonoBehaviour");
        sb.AppendLine("{");
        sb.Append(m_Tab);
        sb.AppendLine("private static "+ scriptName + " m_" + scriptName + ";");
        sb.Append(m_Tab);
        sb.AppendLine("public static " + scriptName + " singleton" + "{ get{ return" + " m_" +scriptName + ";}}");
        sb.Append(m_Tab);
        sb.AppendLine("public Dictionary<int," + scriptName + "Info>" + " m_" + scriptName + "Info = new Dictionary<int," + scriptName + "Info>();");
        sb.Append(m_Tab);
        sb.AppendLine();
        sb.Append(m_Tab);
        sb.AppendLine("void Awake()");
        sb.Append(m_Tab);
        sb.AppendLine("{");
        sb.Append(m_Tab + m_Tab);
        sb.AppendLine("m_" + scriptName + " = this;");
        sb.Append(m_Tab + m_Tab);
        sb.AppendLine("LoadXml();");
        sb.Append(m_Tab);
        sb.AppendLine("}");
        sb.Append(m_Tab);
        sb.AppendLine("void LoadXml()");
        sb.Append(m_Tab);
        sb.AppendLine("{");
        sb.Append(m_Tab + m_Tab);
        sb.AppendLine("TextAsset t;");
        sb.Append(m_Tab + m_Tab);
        sb.AppendLine("string data = Resources.Load(" + "\"Xml/" + scriptName + "\").ToString();");
        sb.Append(m_Tab + m_Tab);
        sb.AppendLine("XmlDocument xmlDoc = new XmlDocument();");
        sb.Append(m_Tab + m_Tab);
        sb.AppendLine("xmlDoc.LoadXml(data);");
        sb.Append(m_Tab + m_Tab);
        sb.AppendLine("XmlNodeList nodeList = xmlDoc.SelectNodes(" + "\"root / node\"" + ");");
        sb.Append(m_Tab + m_Tab);
        sb.AppendLine(scriptName + "Info" + " info = null;");
        sb.Append(m_Tab + m_Tab);
        sb.AppendLine("foreach (XmlNode xmlNode in nodeList)");
        sb.Append(m_Tab + m_Tab);
        sb.AppendLine("{");
        sb.Append(m_Tab + m_Tab + m_Tab);
        sb.AppendLine("info = new " + scriptName +"Info();");
        sb.Append(m_Tab + m_Tab + m_Tab);
        sb.AppendLine("foreach (XmlAttribute s in xmlNode.Attributes)");
        sb.Append(m_Tab + m_Tab + m_Tab);
        sb.AppendLine("{");
        sb.Append(m_Tab + m_Tab + m_Tab + m_Tab);
        sb.AppendLine("switch (s.Name)");
        sb.Append(m_Tab + m_Tab + m_Tab + m_Tab);
        sb.AppendLine("{");
        for (int i=0;i<nodeNames.Count;i++)
        {
            string[] s = nodeNames[i].Split(split.ToCharArray()[0]);
            sb.Append(m_Tab + m_Tab + m_Tab + m_Tab + m_Tab);
            sb.AppendLine("case \"" + nodeNames[i] + "\":");

            sb.Append(m_Tab + m_Tab + m_Tab + m_Tab + m_Tab + m_Tab);
            if (s[0].ToLower() == "int")
                sb.AppendLine("info." + s[1] + " = int.Parse(s.Value);");
            if (s[0].ToLower() == "float")
                sb.AppendLine("info." + s[1] + " = float.Parse(s.Value);");
            if (s[0].ToLower() == "string")
                sb.AppendLine("info." + s[1] + " = s.Value;");
            if (s[0].ToLower() == "bool")
                sb.AppendLine("info." + s[1] + " = bool.Parse(s.Value);");
            if (s[0].ToLower() == "arrayint")
                sb.AppendLine("info." + s[1] + " = System.Array.ConvertAll(" + "s.Value.Split('" + split + "')," + "int.Parse);");
            if (s[0].ToLower() == "arrayfloat")
                sb.AppendLine("info." + s[1] + " = System.Array.ConvertAll(" + "s.Value.Split('" + split + "')," + "float.Parse);");
            if (s[0].ToLower() == "arraystring")
                sb.AppendLine("info." + s[1] + " = s.Value.Split('" + split + "');");
            sb.Append(m_Tab + m_Tab + m_Tab + m_Tab + m_Tab + m_Tab);
            sb.AppendLine("break;");
        }
        sb.Append(m_Tab + m_Tab + m_Tab + m_Tab);
        sb.AppendLine("}");
        sb.Append(m_Tab + m_Tab + m_Tab);
        sb.AppendLine("}");
        sb.Append(m_Tab + m_Tab + m_Tab);
        sb.AppendLine("m_" + scriptName + "Info[info.id] = info;" );
        sb.Append(m_Tab + m_Tab);
        sb.AppendLine("}");
        sb.Append(m_Tab);
        sb.AppendLine("}");
        sb.AppendLine("}");

        if (!Directory.Exists(m_ScriptsFile))
            Directory.CreateDirectory(m_ScriptsFile);
        File.WriteAllText(m_ScriptsFile + scriptName + ".cs", sb.ToString(),Encoding.UTF8);

        AssetDatabase.Refresh();
    }

    static List<string> NodeList(string scriptName)
    {
        string data = Resources.Load("Xml/" + scriptName).ToString(); ;
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(data);
        XmlNodeList nodeList = xmlDoc.SelectNodes("root/node");

        List<string> nodeNames = new List<string>();
        foreach (XmlNode xmlNode in nodeList)
            foreach (XmlAttribute s in xmlNode.Attributes)
                if (!nodeNames.Contains(s.Name))
                    nodeNames.Add(s.Name);

        return nodeNames;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Text;
using System.IO;
using System.Xml;

public class AutoXml : Editor {


    static string m_XmlFile = Application.dataPath + "/Resources/Xml";
    static string m_ScriptsFile = Application.dataPath + "/Scripts/";
    static string m_Tab = "    ";


    [MenuItem("工具/生成Xml代码")]
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
            string[] s = nodeNames[i].Split('_');
            sb.AppendLine("public " + s[0].ToLower() + " " + s[1] +";");
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
        sb.AppendLine("public List<" + scriptName + "Info>" + " m_" + scriptName + "Info = new List<" + scriptName + "Info>();");
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
            string[] s = nodeNames[i].Split('_');
            sb.Append(m_Tab + m_Tab + m_Tab + m_Tab + m_Tab);
            sb.AppendLine("case \"" + s[1] +"\":");

            sb.Append(m_Tab + m_Tab + m_Tab + m_Tab + m_Tab + m_Tab);
            if (s[0].ToLower() == "int")
                sb.AppendLine("info." + s[1] + " = int.Parse(s.Value);");
            if (s[0].ToLower() == "float")
                sb.AppendLine("info." + s[1] + " = float.Parse(s.Value);");
            if (s[0].ToLower() == "string")
                sb.AppendLine("info." + s[1] + " = s.Value;");
            if (s[0].ToLower() == "bool")
                sb.AppendLine("info." + s[1] + " = bool.Parse(s.Value);");
            sb.Append(m_Tab + m_Tab + m_Tab + m_Tab + m_Tab + m_Tab);
            sb.AppendLine("break;");
        }
        sb.Append(m_Tab + m_Tab + m_Tab + m_Tab);
        sb.AppendLine("}");
        sb.Append(m_Tab + m_Tab + m_Tab);
        sb.AppendLine("}");
        sb.Append(m_Tab + m_Tab + m_Tab);
        sb.AppendLine("m_" + scriptName + "Info.Add(info);" );
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

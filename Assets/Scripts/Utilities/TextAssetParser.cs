using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class TextAssetParser : MonoBehaviour
{
    public static List<string> Parse(TextAsset asset)
    {
        string content = asset.text;

        return content.Split(new char[] { '\n','\r' }, System.StringSplitOptions.RemoveEmptyEntries).ToList();
    }
}

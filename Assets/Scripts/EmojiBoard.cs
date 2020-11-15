using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;

public class EmojiBoard : MonoBehaviour
{
    public GameObject buttonPrefab;
    public List<Sprite> emojiSprites = new List<Sprite>();
    public GridLayoutGroup rightboard;
    void Start()
    {
        float uppos = Mathf.NegativeInfinity;
        float dwpos = Mathf.Infinity;
        foreach (var e in emojiSprites)
        {
            GameObject bp = Instantiate(buttonPrefab, rightboard.transform, false);
            Image img = bp.GetComponent<Image>();
            img.sprite = e;
            Button b = bp.GetComponent<Button>();
            b.onClick.AddListener(() => EmojiClicked(bp));
            Vector3[] v = new Vector3[4];
            img.rectTransform.GetWorldCorners(v);

            Debug.LogFormat("{0} :: {1}", v[1], v[0]);
            if (uppos < v[1].y)
            {
                uppos = v[1].y;
            }
            if (dwpos > v[0].y)
            {
                dwpos = v[0].y;
            }
        }
        
        RectTransform rt = rightboard.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(rt.sizeDelta.x, (uppos - dwpos));
    }

    private void EmojiClicked(GameObject go)
    {
        Debug.Log("Got a emoji clicked");
    }
}

[CustomEditor(typeof(EmojiBoard))]
public class EmojiBoardEditor : Editor
{
    private EmojiBoard m_target;
    private DefaultAsset targetFolder;

    void OnEnable()
    {
        m_target = (EmojiBoard)target;
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.LabelField("Emoji Board Contols");

        DefaultAsset prevtarg = targetFolder;
        targetFolder = (DefaultAsset)EditorGUILayout.ObjectField(
            "Update Emoji Sprite Folder",
            prevtarg,
            typeof(DefaultAsset),
            false
        );

        if (prevtarg != targetFolder)
        {
            m_target.emojiSprites.Clear();
            if (targetFolder != null)
            {
                string path = AssetDatabase.GetAssetPath(targetFolder) + '/';
                int midlen = path.Length;
                path = path.Substring(6); // move past "Assets"
                path = Application.dataPath + path;
                int prelen = path.Length - midlen;
                Debug.LogFormat("Gathering emoji from {0}", path);
                string[] paths = Directory.GetFiles(path);
                foreach (var f in paths)
                {
                    if (f.EndsWith(".png"))
                    {
                        var sf = f.Substring(prelen);
                        var s = (Sprite)AssetDatabase.LoadAssetAtPath(sf, typeof(Sprite));
                        m_target.emojiSprites.Add(s);
                    }
                }
            }
        }

        DrawDefaultInspector();
    }
}

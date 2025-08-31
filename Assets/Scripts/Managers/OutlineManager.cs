using System.Collections.Generic;
using UnityEngine;

public class OutlineManager : Singleton<OutlineManager>
{
    [SerializeField] private OutlineManagerData outlineManagerData;

    private Dictionary<GameObject, Outline> outlines = new();


    void Awake()
    {
        CreateSingleton(true);
    }


    public void Register(GameObject obj)
    {
        if (!outlines.ContainsKey(obj))
        {
            Outline outline = obj.AddComponent<Outline>();
            outline.OutlineColor = outlineManagerData.DefaultOutlineColor;
            outline.OutlineWidth = 0;
            outlines.Add(obj, outline);
        }
    }

    public void Unregister(GameObject obj)
    {
        if (outlines.ContainsKey(obj))
        {
            outlines.Remove(obj);
        }
    }

    public void Show(GameObject obj)
    {
        if (outlines.TryGetValue(obj, out Outline outline))
        {
            outline.OutlineWidth = outlineManagerData.ActiveWidth;
        }
    }

    public void Hide(GameObject obj)
    {
        if (outlines.TryGetValue(obj, out Outline outline))
        {
            outline.OutlineWidth = 0;
        }
    }
}

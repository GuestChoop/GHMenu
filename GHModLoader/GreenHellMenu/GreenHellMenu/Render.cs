using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Render
{
    public static void DrawName(Vector3 pos, Color color, string name)
    {
        Vector3 vec = Camera.main.WorldToScreenPoint(pos);
        double distance = Math.Round(Vector3.Distance(Player.Get().transform.position, pos));
        if (vec.z > 0f && distance <= 200f)
        {
            GUIStyle style = new GUIStyle()
            {
                alignment = TextAnchor.MiddleCenter,
            };
            style.normal.textColor = color;
            GUI.Label(new Rect(new Vector2(vec.x - 3f, (float)Screen.height - vec.y + 5f), new Vector2(0f, 0f)), name + " " + (int)distance + "m", style);
            GUI.color = color;
        }
    }
}

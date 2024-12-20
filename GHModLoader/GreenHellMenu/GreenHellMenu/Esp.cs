using AIs;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Esp
{
    //public static AI[] allAI;
    public static FlockChild[] birds;
    public static Item[] items;
    public static bool isTracking = false;
    public static bool stop = false;

    public static void AIEsp()
    {
        if (GHAPI.IsCurrentSceneGame())
        {
            foreach (AI ai in AIManager.Get().m_ActiveAIs)
            {
                if(ai != null)
                {
                    Render.DrawName(ai.transform.position, Color.green, ai.GetAIID().ToString());
                }
            }
        }
    }

    public static IEnumerator BirdEsp()
    {

        while (true)
        {
            birds = Object.FindObjectsOfType<FlockChild>();
            yield return new WaitForSeconds(5);
        }
    }

    public static IEnumerator ItemEsp()
    {
        while (true)
        {
            items = Item.s_AllItems.ToArray();
            yield return new WaitForSeconds(5);
        }
    }
}

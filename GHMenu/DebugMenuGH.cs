using UnityEngine;
using UnityEngine.UI;

public class DebugMenuGH
{
    public static MenuScreen menudebugwound;
    public static MenuScreen menudebugAI;
    public static MenuScreen menudebugachievement;
    public static MenuScreen menudebugselectmode;
    public static MenuScreen menudebugskill;
    public static MenuScreen menudebugspawner;
    public static MenuScreen menudebugtele;
    public static MenuScreen menudebugconstruction;
    public static MenuScreen menudebugP2P;
    public static MenuScreen menudebugcamera;

    public static void GetAllDebug()
    {
        menudebugwound = MenuInGameManager.Get().GetMenu(typeof(MenuDebugWounds));
        //menudebugAI = MenuInGameManager.Get().GetMenu(typeof(MenuDebugAI));
        //menudebugachievement = MenuInGameManager.Get().GetMenu(typeof(MenuDebugAchievements));
        //menudebugselectmode = MenuInGameManager.Get().GetMenu(typeof(MenuDebugSelectMode));
        //menudebugskill = MenuInGameManager.Get().GetMenu(typeof(MenuDebugSkills));
        //menudebugspawner = MenuInGameManager.Get().GetMenu(typeof(MenuDebugSpawners));
        //menudebugtele = MenuInGameManager.Get().GetMenu(typeof(MenuDebugTeleport));
        //menudebugconstruction = MenuInGameManager.Get().GetMenu(typeof(MenuDebugConstructionGhost));
        //menudebugP2P = MenuInGameManager.Get().GetMenu(typeof(MenuDebugP2P));
        //menudebugcamera = MenuInGameManager.Get().GetMenu(typeof(MenuDebugCamera));
    }
    public static void ShowDebugWound()
    {
        if (Input.GetKeyDown(KeyCode.F6))
        {
            var GHmain = GreenHellMenu.Instance;

            GHmain.isdebugshow = !GHmain.isdebugshow;
            Button[] btninchild = menudebugwound.GetComponentsInChildren<Button>(true);

            if (GHmain.isdebugshow && menudebugwound != null)
            {
                foreach (Button btn in btninchild)
                {
                    btn.enabled = true;
                    btn.interactable = true;
                }
                menudebugwound.Show();
                Player.Get().BlockMoves();
                Player.Get().LockNotepad();
                Player.Get().LockMap();
                Player.Get().BlockRotation();
                Player.Get().m_ActiveFightController.BlockFight();
                Player.Get().LockWatch();
                if (!CursorManager.Get().IsCursorVisible())
                {
                    CursorManager.Get().ShowCursor(true);
                    MenuInGameManager.Get().SetCursorVisible(true);
                }
            }
            else
            {
                if (CursorManager.Get().IsCursorVisible())
                {
                    CursorManager.Get().ShowCursor(false);
                    MenuInGameManager.Get().SetCursorVisible(false);
                }
                menudebugwound.Hide();
                Player.Get().UnblockMoves();
                Player.Get().UnlockNotepad();
                Player.Get().UnlockMap();
                Player.Get().UnblockRotation();
                Player.Get().m_ActiveFightController.UnblockFight();
                Player.Get().UnlockWatch();
            }
        }
    }

}

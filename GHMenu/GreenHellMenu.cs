using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;
using BepInEx;
using System.IO;
using System.Reflection;
using BepInEx.Logging;
using UnityEngine.SceneManagement;


[BepInPlugin(GUID, NAME, VERSION)]
public class GreenHellMenu : BaseUnityPlugin
{
    private const string GUID = "GHMenu_By_Guest_Choop";
    private const string NAME = "GH Menu";
    private const string VERSION = "1.0.0";
    private Harmony harmony;
    public static ManualLogSource log;
    public static GreenHellMenu Instance;
    AssetBundle asset;
    GameObject menu;
    public bool check;
    GameObject prefab;
    public bool isSkipAcceDefault;
    Dictionary<ItemInfo, GameObject> buttoninfo = new Dictionary<ItemInfo, GameObject>();
    //Vector2 mouseposition = new Vector2();
    //Vector2 startposition = new Vector2();
    //Vector2 offset = new Vector2();
    public Vector3 AccelerationInAir = new Vector3(0, 5, 0);
    string CurrentTab = "Food";
    string nameitem;
    bool itemsInitialized = false;
    InputField itemsearch;
    bool aiespenable;
    bool humanespenable;
    bool itemespenable;
    public bool flyenable;
    public bool isdebugshow;
    public CanvasGroup canvas;
    GameObject saveNameBG;
    GameObject posView;
    bool isNewName;
    bool isChangeName;
    GameObject posSave;

    public void Start()
    {
        //AssetBundle.GetAllLoadedAssetBundles().ToList().ForEach(bundle =>
        //{
        //    if (bundle.name == "greenhell.assets")
        //    {
        //        bundle.Unload(true);
        //    }
        //});

        Instance = this;
        log = Logger;
        harmony = new Harmony("Patch_Rigid_To_Player");
        harmony.PatchAll();
        Debug.Log($"{harmony.Id} Patch Success !!!");
        LoadAssets();
        menu = Instantiate(asset.LoadAsset<GameObject>("GreenHellCanvas"));
        prefab = asset.LoadAsset<GameObject>("ItemSlot");
        saveNameBG = menu.transform.Find("Background").Find("TeleTab").Find("ScrollView").Find("SaveBG").gameObject;
        posView = asset.LoadAsset<GameObject>("PosView");
        menu.transform.Find("Background").gameObject.AddComponent<UIDragable>();
        canvas = menu.transform.Find("Background").gameObject.AddComponent<CanvasGroup>();
        UIDragable.canvas = menu.gameObject.GetComponent<Canvas>();
        UIDragable.rect = menu.transform.Find("Background").GetComponent<RectTransform>();
        //DragUI(menu.transform.Find("Background").gameObject);
        //netObj = new GameObject("Health Network");
        //netObj.AddComponent<AddHealthReplicator>();
        menu.transform.Find("Background").Find("ItemBtn").GetComponent<Button>().onClick.AddListener(() =>
        {
            MenuChange("ItemTab");
        });

        menu.transform.Find("Background").Find("PlayerBtn").GetComponent<Button>().onClick.AddListener(() =>
        {
            MenuChange("PlayerTab");
        });

        menu.transform.Find("Background").Find("TeleBtn").GetComponent<Button>().onClick.AddListener(() =>
        {
            MenuChange("TeleTab");
        });
        menu.transform.Find("Background").Find("EspBtn").GetComponent<Button>().onClick.AddListener(() =>
        {
            MenuChange("EspTab");
        });
        menu.transform.Find("Background").Find("ItemTab").Find("FoodGory").GetComponent<Button>().onClick.AddListener(() =>
        {
            Tab("Food");
        });
        menu.transform.Find("Background").Find("ItemTab").Find("WeaponGory").GetComponent<Button>().onClick.AddListener(() =>
        {
            Tab("Weapon");
        });
        menu.transform.Find("Background").Find("ItemTab").Find("ArmorGory").GetComponent<Button>().onClick.AddListener(() =>
        {
            Tab("Armor");
        });
        menu.transform.Find("Background").Find("ItemTab").Find("ItemGory").GetComponent<Button>().onClick.AddListener(() =>
        {
            Tab("Item");
        });

        foreach (Transform tr in menu.transform.Find("Background").Find("PlayerTab").Find("PlayerStatsView").Find("PlayerStats").gameObject.transform)
        {
            tr.GetComponent<Button>().onClick.AddListener(() =>
            {
                var module = PlayerConditionModule.Get();
                if (!module)
                {
                    return;
                }
                //if (tr.Find("Text").GetComponent<Text>().text == "Full Health")
                //{
                //    Log("Current HP: "+module.GetMaxHP());
                //    module.m_HP += module.GetMaxHP() - module.GetHP();
                //    Log("HP Increased: "+module.GetMaxHP());
                //}
                if (tr.Find("Text").GetComponent<Text>().text == "Full Stamina")
                {
                    module.m_Stamina += module.GetMaxStamina() - module.GetStamina();
                }
                else if (tr.Find("Text").GetComponent<Text>().text == "Full Dirtiness")
                {
                    module.m_Dirtiness -= module.m_MaxDirtiness - module.m_Dirtiness;
                }
                else if (tr.Find("Text").GetComponent<Text>().text == "Full Energy")
                {
                    module.m_Energy += module.GetMaxEnergy() - module.GetEnergy();
                }
                else if (tr.Find("Text").GetComponent<Text>().text == "Full Sanity")
                {
                    module.m_Sanity = 100;
                }
                else if (tr.Find("Text").GetComponent<Text>().text == "Full Hydration")
                {
                    module.m_Hydration += module.GetMaxHydration() - module.GetHydration();
                }
                else if (tr.Find("Text").GetComponent<Text>().text == "Full NutritionCarbo")
                {
                    module.m_NutritionCarbo += module.GetMaxNutritionCarbo() - module.GetNutritionCarbo();
                }
                else if (tr.Find("Text").GetComponent<Text>().text == "Full NutritionFat")
                {
                    module.m_NutritionFat += module.GetMaxNutritionFat() - module.GetNutritionFat();
                }
                else
                    module.m_NutritionProteins += module.GetMaxNutritionProtein() - module.GetNutritionProtein();
            });
        }

        itemsearch = menu.transform.Find("Background").Find("ItemTab").Find("InputField").GetComponent<InputField>();
        menu.transform.Find("Background").Find("ItemTab").Find("InputField").Find("Placeholder").GetComponent<Text>().text = "Type name item...";
        itemsearch.onValueChanged.AddListener((t) => { nameitem = t; SearchItem(); });

        menu.transform.Find("Background").Find("TeleTab").Find("TeleAccept").GetComponent<Button>().onClick.AddListener(() =>
        {
            if (menu.transform.Find("Background").Find("TeleTab").Find("InputFieldX").GetComponent<InputField>().text == string.Empty || menu.transform.Find("Background").Find("TeleTab").Find("InputFieldY").GetComponent<InputField>().text == string.Empty || menu.transform.Find("Background").Find("TeleTab").Find("InputFieldZ").GetComponent<InputField>().text == string.Empty)
            {
                Debug.LogError("Coordinates cannot be empty");
                //HNotify.Get().AddNotification(HNotify.NotificationType.normal, "Coordinates cannot be empty", 3, HNotify.ErrorSprite);
                return;
            }
            float xAxis = float.Parse(menu.transform.Find("Background").Find("TeleTab").Find("InputFieldX").GetComponent<InputField>().text);
            float yAxis = float.Parse(menu.transform.Find("Background").Find("TeleTab").Find("InputFieldY").GetComponent<InputField>().text);
            float zAxis = float.Parse(menu.transform.Find("Background").Find("TeleTab").Find("InputFieldZ").GetComponent<InputField>().text);
            if (Player.Get() != null)
            {
                Player.Get().TeleportTo(new Vector3(xAxis, yAxis, zAxis), Player.Get().transform.rotation, false);
            }
        });

        menu.transform.Find("Background").Find("EspTab").Find("TimeBtn").GetComponent<Button>().onClick.AddListener(() =>
        {
            if (menu.transform.Find("Background").Find("EspTab").Find("InputFieldHour").GetComponent<InputField>().text == string.Empty)
            {
                Debug.LogError("Hour cannot be empty");
                return;
            }
            int minute = menu.transform.Find("Background").Find("EspTab").Find("InputFieldMinute").GetComponent<InputField>().text == string.Empty ? 0 : int.Parse(menu.transform.Find("Background").Find("EspTab").Find("InputFieldMinute").GetComponent<InputField>().text);
            int hour = int.Parse(menu.transform.Find("Background").Find("EspTab").Find("InputFieldHour").GetComponent<InputField>().text);
            if (MainLevel.Instance != null)
            {
                MainLevel.Instance.SetDayTime(hour, minute);
            }           
        });

        menu.transform.Find("Background").Find("TeleTab").Find("SaveBtn").GetComponent<Button>().onClick.AddListener(() =>
        {
            saveNameBG.SetActive(true);
            saveNameBG.transform.Find("InputField").GetComponent<InputField>().text = "";
            isNewName = true;
        });
        saveNameBG.transform.Find("Button").GetComponent<Button>().onClick.AddListener(() =>
        {
            if (Player.Get() != null)
            {
                var posCurrent = Player.Get().GetWorldPosition();
                NewOrChangeName(posCurrent);
            }
        });

        menu.transform.Find("Background").Find("PlayerTab").Find("PlayerStatsView").Find("Misc").Find("FlyBtn").GetComponent<Button>().onClick.AddListener(() =>
        {
            flyenable = !flyenable;
            var player = Player.Get();
            if (!player)
            {
                return;
            }
            if (flyenable)
            {
                player.BlockMoves();
            }
            else
            {
                player.UnblockMoves();
            }
        });
        menu.transform.Find("Background").Find("EspTab").Find("AIEsp").GetComponent<Button>().onClick.AddListener(() =>
        {
            aiespenable = !aiespenable;
        });
        menu.transform.Find("Background").Find("EspTab").Find("BirdAIEsp").GetComponent<Button>().onClick.AddListener(() =>
        {
            humanespenable = !humanespenable;
        });
        menu.transform.Find("Background").Find("EspTab").Find("ItemEsp").GetComponent<Button>().onClick.AddListener(() =>
        {
            itemespenable = !itemespenable;
        });

        menu.SetActive(false);
        saveNameBG.SetActive(false);
        //BG = GameObject.Find("MainMenu");
        //bool open = false;
        //var obj = Instantiate(BG.Find("Buttons").Find("Continue").gameObject, BG.Find("Buttons"));
        //obj.name = "DM";
        //obj.gameObject.SetActive(true);
        //obj.gameObject.GetComponent<UIButtonEx>().interactable = true;
        //BG.GetComponent<MainMenu>().AddMenuButton(obj.gameObject.GetComponent<UIButtonEx>(), "MODS");
        //obj.GetComponentInChildren<Text>().text = "MODS";
        //obj.gameObject.GetComponent<UIButtonEx>().onClick.AddListener(() => { open = !open; menu.SetActive(open);});
        DontDestroyOnLoad(menu);
        StartCoroutine(Esp.ItemEsp());
        StartCoroutine(Esp.BirdEsp());
        MenuChange("ItemTab");
        Debug.Log("Mod GreenHell Menu has been loaded!");
    }

    public void LoadAssets()
    {
        Assembly assembly = Assembly.GetExecutingAssembly();
        string name = assembly.GetManifestResourceNames().FirstOrDefault(str => str.EndsWith("greenhell.assets"));
        Stream stream = assembly.GetManifestResourceStream(name);
        asset = AssetBundle.LoadFromStream(stream);
    }

    private void NewOrChangeName(Vector3 pos)
    {
        if (isNewName)
        {
            var newSave = Instantiate(posView, menu.transform.Find("Background").Find("TeleTab").Find("ScrollView").Find("Viewport").Find("Content"));
            newSave.transform.Find("PosBtn").Find("Text").GetComponent<Text>().text = saveNameBG.transform.Find("InputField").GetComponent<InputField>().text;
            isNewName = false;
            var linkObject = newSave;
            linkObject.transform.Find("NameBtn").GetComponent<Button>().onClick.AddListener(() =>
            {
                saveNameBG.SetActive(true);
                saveNameBG.transform.Find("InputField").GetComponent<InputField>().text = "";
                isChangeName = true;
                posSave = linkObject.gameObject;
            });
            linkObject.transform.Find("DeleteBtn").GetComponent<Button>().onClick.AddListener(() =>
            {
                Destroy(linkObject.gameObject);
            });
            linkObject.transform.Find("PosBtn").GetComponent<Button>().onClick.AddListener(() =>
            {
                Player.Get().TeleportTo(pos, Player.Get().transform.rotation, false);
            });
        }
        if(isChangeName)
        {
            posSave.transform.Find("PosBtn").Find("Text").GetComponent<Text>().text = saveNameBG.transform.Find("InputField").GetComponent<InputField>().text;
            posSave = null;
            isChangeName = false;
        }
        saveNameBG.SetActive(false);
        Logger.LogMessage($"{NAME} is loaded");
    }

    //private void DragHandler()
    //{
    //    Vector2 position;
    //    RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)menu.GetComponent<Canvas>().transform, mouseposition, menu.GetComponent<Canvas>().worldCamera, out position);
    //    rect.anchoredPosition = menu.GetComponent<Canvas>().transform.TransformPoint(position);
    //}

    //private void ChangePositionUI()
    //{
    //    rect.position = mouseposition - offset;
    //}

    private void MenuChange(string name)
    {
        foreach (Transform t in menu.transform.Find("Background").gameObject.transform)
        {
            var hascomp = t.gameObject.GetComponent<CanvasGroup>();
            if (hascomp)
            {
                hascomp.alpha = t.transform.name == name ? 1 : 0;
                hascomp.interactable = t.transform.name == name ? true : false;
                hascomp.blocksRaycasts = t.transform.name == name ? true : false;
                t.gameObject.SetActive(true);
            }
        }
    }

    //private void UpdateMousePosition()
    //{
    //    mouseposition.x = Input.mousePosition.x;
    //    mouseposition.y = Input.mousePosition.y;
    //}

    //private void UpdateStartPosition()
    //{
    //    startposition.x = rect.anchoredPosition.x;
    //    startposition.y = rect.anchoredPosition.y;
    //}

    //private void UpdateDiffPosition()
    //{
    //    offset = mouseposition - startposition;
    //}

    //private void DragUI(GameObject obj)
    //{
    //    if (obj.GetComponent<UnityEngine.EventSystems.EventTrigger>() == null)
    //    {
    //        obj.AddComponent<UnityEngine.EventSystems.EventTrigger>();
    //    }
    //    var eventtrigger = obj.GetComponent<UnityEngine.EventSystems.EventTrigger>();
    //    UnityEngine.EventSystems.EventTrigger.Entry entry = new UnityEngine.EventSystems.EventTrigger.Entry();
    //    entry.eventID = EventTriggerType.Drag;
    //    entry.callback.AddListener((func) => DragHandler());
    //    eventtrigger.triggers.Add(entry);
    //}

    private void SearchItem()
    {
        var nametolower = nameitem.ToLower();
        foreach (GameObject spawnbtn in buttoninfo.Values)
        {
            if (menu.transform.Find("Background").Find("ItemTab").Find("InputField").GetComponent<InputField>().text == "")
            {
                spawnbtn.SetActive(true);
            }
            else
            {
                spawnbtn.SetActive(spawnbtn.transform.Find("Text").GetComponent<Text>().text.ToLower().Contains(nametolower));
            }
        }
    }

    private void Tab(string tabname)
    {
        CurrentTab = tabname;
        buttoninfo.ToList().ForEach(x =>
        {
            if (CurrentTab == "Weapon")
            {
                x.Value.SetActive(x.Key.IsWeapon());
            }
            else if (CurrentTab == "Armor")
            {
                x.Value.SetActive(x.Key.IsArmor());
            }
            else if (CurrentTab == "Food")
            {
                x.Value.SetActive(x.Key.IsFood());
            }
            else
            {
                x.Value.SetActive(!x.Key.IsWeapon() && !x.Key.IsArmor() && !x.Key.IsFood());
            }
        });
    }


    private IEnumerator InitGameItemsTab()
    {
        GameObject content = menu.transform.Find("Background").Find("ItemTab").Find("ScrollView").Find("Viewport").Find("Content").gameObject;
        foreach (Transform transform in content.transform)
        {
            Destroy(transform.gameObject);
        }

        if (ItemsManager.Get() != null)
        {
            var itemids = ItemsManager.Get().GetAllInfos().Values;
            foreach (ItemInfo itemname in itemids)
            {

                if (itemname != null && itemname.m_ID.ToString() != "None" && !itemname.m_ID.ToString().ToLower().Contains("spoiled") && !itemname.m_ID.ToString().ToLower().Contains("burned") && itemname.m_ID.ToString() != "ArmadilloThreeBanded_Body" && itemname.m_ID.ToString() != "Anthill_powder" && itemname.m_ID.ToString() != "River_silt")
                {
                    GameObject btnspawn = Instantiate(prefab, content.transform);
                    buttoninfo.Add(ItemsManager.Get().GetInfo(itemname.m_ID.ToString()), btnspawn);

                    btnspawn.transform.Find("Text").GetComponent<Text>().text = itemname.m_ID.ToString();
                    btnspawn.transform.Find("Image").GetComponent<Image>().sprite = ItemsManager.Get().m_ItemIconsSprites.Values.Where(x => x.name == itemname.m_ID.ToString()).FirstOrDefault();
                    btnspawn.GetComponent<Button>().onClick.AddListener(() =>
                    {
                        Item itemspawned = ItemsManager.Get().CreateItem(itemname.m_ID.ToString(), true);
                        itemspawned.transform.LookAt(Player.Get().transform.position);
                        itemspawned.transform.position = Player.Get().transform.position + Player.Get().transform.forward * 2 + new Vector3(0f, 2f, 0f);
                        //Debug.Log(Vector3.Distance(itemspawned.transform.position, Player.Get().transform.position));
                        //Debug.Log(itemspawned.transform.position);
                        //HNotify.Get().AddNotification(HNotify.NotificationType.scaling, itemname.m_ID.ToString() + " " + "Spawned", 2, HNotify.CheckSprite);
                    });
                }
                yield return new WaitForEndOfFrame();
            }
        }
        Tab("Food");
    }

    public void Update()
    {
        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            if (check)
            {
                check = false;
            }
            if (menu && menu.activeSelf)
            {
                menu.SetActive(false);
            }
            if (Input.GetKeyDown(KeyCode.F2))
            {
                return;
                //HNotify.Get().AddNotification(HNotify.NotificationType.spinning, "Calm down, game haven't started yet", 2, HNotify.LoadingSprite);
            }
        }
        if (Input.GetKeyDown(KeyCode.F2))
        {
            check = !check;
            if (check)
            {
                menu.SetActive(true);
                CursorManager.Get().SetCursor(CursorManager.TYPE.Normal);
                CursorManager.Get().ShowCursor(true);
                Player.Get().BlockMoves();
                Player.Get().LockNotepad();
                Player.Get().LockMap();
                Player.Get().BlockRotation();
                Player.Get().m_ActiveFightController.BlockFight();
                Player.Get().LockWatch();
            }
            else
            {
                menu.SetActive(false);
                CursorManager.Get().ShowCursor(false);
                Player.Get().UnblockMoves();
                Player.Get().UnlockNotepad();
                Player.Get().UnlockMap();
                Player.Get().UnblockRotation();
                Player.Get().m_ActiveFightController.UnblockFight();
                Player.Get().UnlockWatch();
            }
            if (!itemsInitialized && SceneManager.GetActiveScene().name == "Level")
            {
                StartCoroutine(InitGameItemsTab());
                foreach (Transform child in menu.transform.Find("Background").Find("TeleTab").Find("ScrollView").Find("Viewport").Find("Content"))
                {
                    Destroy(child.gameObject);
                }
                itemsInitialized = true;
            }
        }
        //if (Input.GetKeyDown(KeyCode.F3) && ReplTools.AmIMaster())
        //{
        //    ReplicatedLogicalPlayer.s_AllLogicalPlayers.ForEach(peer =>
        //    {
        //        if (!peer.IsMaster())
        //        {
        //            AddHealthReplicator.Get().OnAddHealth(peer.gameObject.GetComponent<ReplicatedPlayerParams>().m_Health);
        //        }
        //    });
        //}
        //if (Input.GetKeyDown(KeyCode.F4) && ReplTools.AmIMaster())
        //{
        //    TeleportReplicator.Get().OnTeleportTarget(Player.Get().transform.position + Vector3.back * 2, Player.Get().transform.rotation);
        //}

        //if (Input.GetMouseButton(0))
        //{
        //    UpdateMousePosition();
        //}
        //if (Input.GetMouseButtonDown(0))
        //{
        //    UpdateStartPosition();
        //    UpdateDiffPosition();
        //}
        var menuInitialized = MenuInGameManager.Get();
        if (SceneManager.GetActiveScene().name == "Level" && menuInitialized != null)
        {
            DebugMenuGH.GetAllDebug();
            DebugMenuGH.ShowDebugWound();
        }
        var playerInitialized = Player.Get();
        if(!playerInitialized)
        {
            return;
        }
        if (SceneManager.GetActiveScene().name == "Level")
        {
            if (flyenable)
            {
                isSkipAcceDefault = true;
                playerInitialized.SetUseGravity(false);
                playerInitialized.m_CharacterController.enabled = false;
                Fly.PlayerFly();
                Cheats.m_GodMode = true;
            }
            else
            {
                isSkipAcceDefault = false;
                playerInitialized.SetUseGravity(true);
                playerInitialized.m_CharacterController.enabled = true;
                Invoke("ShouldDisableGodMode", 2);
            }
            menu.transform.Find("Background").Find("PlayerTab").Find("PlayerStatsView").Find("PlayerStats").Find("HealthBtn").GetComponent<Button>().onClick.AddListener(() =>
            {
                playerInitialized.m_ConditionModule.m_HP += playerInitialized.m_ConditionModule.GetMaxHP() - playerInitialized.m_ConditionModule.GetHP();
            });
        }
        //if (Input.GetKeyDown(KeyCode.F3) && GHAPI.IsCurrentSceneGame())
        //{
        //    P2PSession p2psession = P2PSession.Instance;
        //    if (p2psession.AmIMaster())
        //    {
        //        //P2PConnection connect = p2psession.m_Connections.FirstOrDefault();
        //        var logicalPlayer = ReplicatedLogicalPlayer.s_AllLogicalPlayers;
        //        foreach (var peer in logicalPlayer)
        //        {
        //            if (peer != null && !peer.IsMaster())
        //            {
        //                HNotify.Get().AddNotification(HNotify.NotificationType.scaling, $"{(peer.ReplGetOwner() != null ? peer.ReplGetOwner().GetDisplayName() : "hasn't connect")}", 2, HNotify.CheckSprite);
        //            }
        //        }
        //    }
        //}
    }
    void ShouldDisableGodMode()
    {
        var playerInitialized = Player.Get();
        if (!playerInitialized)
        {
            return;
        }
        if (!playerInitialized.m_IsInAir && Cheats.m_GodMode)
        {
            Cheats.m_GodMode = false;
        }
    }

    private void OnGUI()
    {
        if (aiespenable)
        {
            Esp.AIEsp();
        }

        if (humanespenable)
        {
            for (int i = 0; i < Esp.birds.Length; i++)
            {
                var bird = Esp.birds[i];
                if (bird != null && (int)VectorExtention.Distance(bird.transform.position, Player.Get().GetWorldPosition()) <= 100)
                {
                    if (!bird.name.Contains("Butterfly_Orange") && !bird.name.Contains("Butterfly_Blue") && !bird.name.Contains("Butterfly_LOD0") && bird.name != null)
                    {
                        Render.DrawName(bird.transform.position, Color.green, bird.name);
                    }
                }
            }
        }

        if (itemespenable)
        {
            for (int i = 0; i < Esp.items.Length; i++)
            {
                var item = Esp.items[i];
                if (item != null && (int)VectorExtention.Distance(item.transform.position, Player.Get().GetWorldPosition()) <= 30)
                {
                    if (item.IsFood() || item.GetComponentInChildren<HeavyObject>() || item.GetInfoID() == Enums.ItemID.Stick || item.GetInfoID() == Enums.ItemID.Small_Stick
                    || item.GetComponentInChildren<Weapon>() || item.GetComponentInChildren<LiquidContainer>() || item.GetComponentInChildren<ItemHold>()
                    || item.GetComponentInChildren<ItemTool>() || item.GetInfoID() == Enums.ItemID.Obsidian_Stone)
                    {
                        Render.DrawName(item.transform.position, Color.cyan, item.GetInfoID().ToString());
                    }
                }
            }
        }
    }
}
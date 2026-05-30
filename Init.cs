using Melio.Classes;
using Melio.Patches;
using Melio.Patches.Menu;
using MelonLoader;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using Melio.Hooks;

[assembly: MelonInfo(typeof(MelioMod), "Melio", "1.0.0", "Newwer")]
[assembly: MelonGame(null, null)]

public class MelioMod : MelonMod
{
    private Rect windowRect = new Rect(20, 20, 380, 320);

    private bool showMenu = true;
    private bool stylesInitialized = false;

    private int selectedTab = 0;

    private readonly string[] tabs =
    {
        "Server",
        "Photon",
        "GorillaTagger",
        "Anti-Exploits"
    };

    private GUIStyle windowStyle;
    private GUIStyle buttonStyle;
    private GUIStyle labelStyle;
    private GUIStyle tabStyle;

    private float reconnectTimeout = -1f;
    private bool waitingForReconnect = false;
    private bool antitag = false;

    private Vector2 tabScroll;
    private Vector2 contentScroll;

    private float currentContentHeight = 0f;

    private const float minTabWidth = 90f;
    private const float maxTabWidth = 160f;
    private const float tabHeight = 28f;
    private const float tabSpacing = 4f;

    public override void OnInitializeMelon()
    {
        MelonLogger.Msg("Melio initialized");
        HarmonyHandler.LoadHarmony();
    }

    public override void OnUpdate()
    {
        
        if (waitingForReconnect)
        {
            if (PhotonNetwork.IsConnected)
            {
                MelonLogger.Msg("Connected to Photon.");
                waitingForReconnect = false;

                
                if (antitag)
                {
                    if (!PhotonNetwork.IsMasterClient)
                        {
                        Methods.InvisForU(RpcTarget.MasterClient);
                    }
                    {
                        if ((Hooks.InfectedList().Contains(NetworkSystem.Instance.LocalPlayer))) 
                        {
                            Hooks.RemoveInfected(NetworkSystem.Instance.LocalPlayer);
                        }
                            
                    }

                        
                }
            }
            else if (Time.time > reconnectTimeout)
            {
                MelonLogger.Error("Failed to connect to Photon.");
                waitingForReconnect = false;
            }

            return; 
        }

        
    }

    public override void OnGUI()
    {
        HandleInput();

        if (!showMenu)
            return;

        if (!stylesInitialized)
        {
            InitStyles();
            stylesInitialized = true;
        }

        DrawTheme();

        windowRect = GUI.Window(
            0,
            windowRect,
            DrawWindow,
            "Melio",
            windowStyle
        );
    }

    private void InitStyles()
    {
        windowStyle = new GUIStyle(GUI.skin.window);

        buttonStyle = new GUIStyle(GUI.skin.button)
        {
            fontSize = 13,
            fixedHeight = 30
        };

        labelStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 13
        };

        labelStyle.normal.textColor = Color.white;

        tabStyle = new GUIStyle(GUI.skin.button)
        {
            fontSize = 12,
            fixedHeight = tabHeight
        };
    }

    private void HandleInput()
    {
        Event e = Event.current;

        if (e != null &&
            e.type == EventType.KeyDown &&
            e.keyCode == KeyCode.Insert)
        {
            showMenu = !showMenu;
        }
    }

    private void DrawTheme()
    {
        GUI.backgroundColor = new Color(
            0.15f,
            0.35f,
            0.75f
        );

        GUI.contentColor = Color.white;
    }

    private void DrawWindow(int id)
    {
        GUILayout.Space(10);

        DrawScrollableTabs();

        GUILayout.Space(10);

        contentScroll = GUILayout.BeginScrollView(
            contentScroll,
            false,
            true
        );

        GUILayout.BeginVertical();

        switch (selectedTab)
        {
            case 0:
                DrawServerTab();
                break;

            case 1:
                DrawPhotonTab();
                break;

            case 2:
                DrawGorillaTaggerTab();
                break;

            case 3:
                DrawAntiExploitsTab();
                break;
        }

        GUILayout.EndVertical();

        if (Event.current.type == EventType.Repaint)
        {
            currentContentHeight =
                GUILayoutUtility.GetLastRect().yMax;
        }

        GUILayout.EndScrollView();

        AutoResizeWindow();

        GUI.DragWindow(
            new Rect(
                0,
                0,
                9999,
                25
            )
        );
    }

    private void AutoResizeWindow()
    {
        float padding = 130f;

        float targetHeight =
            Mathf.Clamp(
                currentContentHeight + padding,
                200f,
                600f
            );

        windowRect.height = Mathf.Lerp(
            windowRect.height,
            targetHeight,
            Time.deltaTime * 10f
        );
    }

    private void DrawScrollableTabs()
    {
        float padding = 20f;

        float availableWidth =
            windowRect.width - padding;

        float calculatedWidth =
            (availableWidth -
            ((tabs.Length - 1) * tabSpacing))
            / tabs.Length;

        float buttonWidth = Mathf.Clamp(
            calculatedWidth,
            minTabWidth,
            maxTabWidth
        );

        tabScroll = GUILayout.BeginScrollView(
            tabScroll,
            false,
            false,
            GUIStyle.none,
            GUI.skin.horizontalScrollbar,
            GUILayout.Height(tabHeight + 18)
        );

        GUILayout.BeginHorizontal();

        for (int i = 0; i < tabs.Length; i++)
        {
            bool selected = i == selectedTab;

            GUI.backgroundColor =
                selected
                ? new Color(0.2f, 0.5f, 1f)
                : new Color(0.2f, 0.2f, 0.2f);

            if (GUILayout.Button(
                tabs[i],
                tabStyle,
                GUILayout.Width(buttonWidth),
                GUILayout.Height(tabHeight)
            ))
            {
                selectedTab = i;
            }

            GUILayout.Space(tabSpacing);
        }

        GUI.backgroundColor = Color.white;

        GUILayout.EndHorizontal();

        GUILayout.EndScrollView();
    }
    
    private void DrawServerTab()
    {
        GUILayout.Label(
            "Server",
            labelStyle
        );

        GUILayout.Space(15);

        if (GUILayout.Button(
            "Leave",
            buttonStyle
        ))
        {
            NetworkSystem.Instance.ReturnToSinglePlayer();
        }
    }
    private void DrawAntiExploitsTab()
    {
        GUILayout.Label(
            "Anti-Exploits: " + antitag,
            labelStyle
        );

        GUILayout.Space(15);

        if (GUILayout.Button(
            "Anti-Tag",
            buttonStyle
        ))
        {
            antitag = !antitag;
        }
    }

    private void DrawPhotonTab()
    {
        GUILayout.Label(
            "Photon",
            labelStyle
        );

        GUILayout.Space(15);

        if (GUILayout.Button(
            "Disconnect from Server Websocket",
            buttonStyle
        ))
        {
            if (PhotonNetwork.InRoom)
            {
                PhotonNetwork.Disconnect();

                MelonLogger.Warning(
                    "Disconnected from Photon websocket."
                );
            }
        }

        GUILayout.Space(15);

        if (GUILayout.Button(
            "Disconnect",
            buttonStyle
        ))
        {
            if (PhotonNetwork.InRoom)
            {
                PhotonNetwork.LeaveRoom();
            }
        }

        GUILayout.Space(15);

        if (GUILayout.Button(
            "Join Random",
            buttonStyle
        ))
        {
            if (!PhotonNetwork.InRoom)
            {
                PhotonNetwork.JoinRandomRoom();
            }
        }

        GUILayout.Space(15);

        if (GUILayout.Button(
            "Reconnect to Server Websocket",
            buttonStyle
        ))
        {
            PhotonNetwork.Reconnect();

            reconnectTimeout =
                Time.time + 3f;

            waitingForReconnect = true;
        }

        GUILayout.Space(15);

        if (GUILayout.Button(
            "Toggle Queue",
            buttonStyle
        ))
        {
            PhotonNetwork.IsMessageQueueRunning =
                !PhotonNetwork.IsMessageQueueRunning;
        }
        GUILayout.Space(15);

        if (GUILayout.Button(
            "Lock Room",
            buttonStyle
        ))
        {
           Methods.SetRoomStatus(false);
        }
        GUILayout.Space(15);

        if (GUILayout.Button(
            "Unlock Room",
            buttonStyle
        ))
        {
            Methods.SetRoomStatus(true);
        }
        GUILayout.Space(15);

        if (GUILayout.Button(
            "Lag All",
            buttonStyle
        ))
        {
            Methods.LagTarget(ReceiverGroup.Others, (float)0.1, 3200);
            Methods.RPCProtection();
        }
    }

    private void DrawGorillaTaggerTab()
    {
        GUILayout.Label(
            "GorillaTagger",
            labelStyle
        );

        GUILayout.Space(15);

        if (GUILayout.Button(
            "Reauthenticate",
            buttonStyle
        ))
        {
            MothershipAuthenticator.Instance.BeginLoginFlow();
        }
    }
}

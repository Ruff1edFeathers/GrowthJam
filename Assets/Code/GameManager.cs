using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager s_Instance { get; private set; }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Init()
    {
        //Load GameManager Prefab, Initialize and place it in DontDestoryOnLoad
        GameObject GameManager_Prefab = Resources.Load<GameObject>("PF_GameManager");
        if(GameManager_Prefab == null)
        {
            Debug.LogError("Failed to load GameManager Prefab, something has gone wrong?!");
            return;
        }

        GameObject GameManager_Instance = Instantiate(GameManager_Prefab);
        DontDestroyOnLoad(GameManager_Instance);
        s_Instance = GameManager_Instance.GetComponent<GameManager>();

        if(s_Instance == null)
        {
            Debug.LogError("Initalized GameManager without a GameManager Component?!");
        }
    }

    //Inspector Varibles
    public InputActionAsset m_GameInputAsset;
    public MainMenuUI m_MainMenuUI;
    public PauseUI m_PauseUI;

    //Cached Values
    public InputWrapper m_InputWrapper;

    private bool m_Setup;

    private void OnSetup()
    {
        m_InputWrapper = new InputWrapper(m_GameInputAsset);

        m_PauseUI.OnSetup();
        m_MainMenuUI.SetState(true);
    }

    //Master Game Loop
    private void Update()
    {
        if(!m_Setup)
        {
            OnSetup();
            m_Setup = true;
        }

        //Grab Inputs before anything else
        m_InputWrapper.OnUpdate();

        //Only update pause UI when we aren't show the Main Menu UI
        if(!m_MainMenuUI.m_State)
        {
            m_PauseUI.OnUpdate();

            if (m_PauseUI.m_State)
                return;
        }

        //Run Main Game Loop
        PlatformManager.Instance.OnUpdate(!m_MainMenuUI.m_State);
    }
}

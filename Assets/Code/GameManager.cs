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

        s_Instance.OnSetup();
    }

    //Inspector Varibles
    public InputActionAsset m_GameInputAsset;

    //Cached Values
    public InputWrapper m_InputWrapper;

    private void OnSetup()
    {
        m_InputWrapper = new InputWrapper(m_GameInputAsset);
    }

    //Master Game Loop
    private void Update()
    {
        //Grab Inputs before anything else
        m_InputWrapper.OnUpdate();

        //Run Main Game Loop
        PlatformManager.Instance?.OnUpdate(this);
    }
}

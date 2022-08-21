using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager s_Instance { get; private set; }

    //Inspector Varibles
    public InputActionAsset m_GameInputAsset;
    public GameObject m_MainMenuUI;
    public GameObject m_EndUI;
    public HUDUI      m_HUDUI;
    public PlatformManager m_PlatformManager;
    public CameraController m_CameraController;
    public PlayerController m_PlayerController;
    public AnimateEnv m_AnimateEnv;
    public AnimatePlants m_AnimatePlants;

    //Cached Values
    public InputWrapper m_InputWrapper;

    private bool  m_Setup;
    private bool  m_PlayingGame;
    private float m_PlayerSpawnTimer;

    public void StartGame()
    {
        if(m_PlayingGame)
        {
            Debug.LogError("Game should already be running?");
            return;
        }

        m_MainMenuUI.SetActive(false);
        m_EndUI.SetActive(false);
        m_PlayingGame = true;
    }

    private void Awake()
    {
        s_Instance = this;
    }

    private void Start()
    {
        s_Instance     = this;
        m_InputWrapper = new InputWrapper(m_GameInputAsset);

        m_PlatformManager.OnSetup();

        m_HUDUI.OnSetup();
        m_PlayerController.OnSetup();
        m_CameraController.OnSetup();
        m_AnimateEnv.OnSetup();
        m_AnimatePlants.OnSetup();

        m_MainMenuUI.SetActive(true);
        m_EndUI.SetActive(false);
        m_PlayerSpawnTimer = 5f;
    }

    //Master Game Loop
    private void Update()
    {
        if(!m_Setup)
        {
            m_Setup = true;
        }

        //Grab Inputs before anything else
        m_InputWrapper.OnUpdate();

        if (!m_PlayingGame)
        {
            m_CameraController.OnUpdate();
            return;
        }

        if (m_PlayerSpawnTimer > 0.0f)
        {
            m_PlayerSpawnTimer -= Time.deltaTime;

            if (m_PlayerSpawnTimer <= 0.0f)
            {
                //Show Player
                m_PlayerController.gameObject.SetActive(true);
                m_HUDUI.Show();
                m_CameraController.m_State = eCameraState.TrackTarget;
            }
        }
        else
        {
            m_PlayerController.OnUpdate(m_PlatformManager);
        }

        m_CameraController.OnUpdate();

        SpikeTrap.UpdateTraps();
        SlidingPlatform.UpdatePlatforms();

        m_AnimateEnv.OnUpdate();
        m_AnimatePlants.OnUpdate(m_PlayerController);
    }
}

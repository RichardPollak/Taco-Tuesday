using UnityEngine;
using UnityEngine.SceneManagement;

public class CollisionHandler : MonoBehaviour
{
    [SerializeField] float loadDelay = 1f;
    [SerializeField] AudioClip crashAudio;
    [SerializeField] AudioClip successAudio;
    [SerializeField] ParticleSystem crashParticles;
    [SerializeField] ParticleSystem successParticles;

    AudioSource audioSource;

    bool isTransitioning = false;
    bool enableCollisions = true;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        RespondToDebugKeys();
    }

    void RespondToDebugKeys()
    {
        if (Input.GetKey(KeyCode.L))
        {
            LoadNextLevel();
        }
        else if (Input.GetKey(KeyCode.C))
        {
            enableCollisions = !enableCollisions;
        }
    }

    void OnCollisionEnter(Collision other)
    {
        if (isTransitioning || !enableCollisions) { return; }

        switch(other.gameObject.tag)
        {
            case "Friendly":
                Debug.Log("This thing is friendly");
                break;
            case "Finish":
                StartSuccessSequence();
                break;
            case "Fuel":
                //todo: add fuel mechanic
                break;
            default:
                StartCrashSequence();
                break;
        }
    }

    void StartSuccessSequence()
    {
        audioSource.Stop();
        audioSource.PlayOneShot(successAudio);
        successParticles.Play();
        InitializeTransition();
        Invoke("LoadNextLevel", loadDelay);
    }

    void StartCrashSequence()
    {
        audioSource.Stop();
        audioSource.PlayOneShot(crashAudio);
        crashParticles.Play();
        InitializeTransition();
        Invoke("ReloadLevel", loadDelay);
    }
    
    void LoadNextLevel()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = IsFinalScene(currentSceneIndex) ? 0 : currentSceneIndex + 1;

        SceneManager.LoadScene(nextSceneIndex);
    }

    void ReloadLevel()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
    }

    void InitializeTransition()
    {
        isTransitioning = true;
        DisableMovement();
    }

    void DisableMovement()
    {
        gameObject.GetComponent<Movement>().enabled = false;
    }

    bool IsFinalScene(int input)
    {
        return input == SceneManager.sceneCountInBuildSettings - 1;
    }
}

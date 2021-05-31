using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{
    [SerializeField] float rcsThrust;
    [SerializeField] float mainThrust;

    [SerializeField] AudioClip mainEngine;
    [SerializeField] AudioClip deathSound;
    [SerializeField] AudioClip successSound;

    [SerializeField] ParticleSystem mainEngineParticles;
    [SerializeField] ParticleSystem deathParticles;
    [SerializeField] ParticleSystem successParticles;

    private Rigidbody rigidbody;
    AudioSource mainSound;
    bool collisionsDisabled = true;

    enum State { Alive, Dying, Transcending };
    State state = State.Alive;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        mainSound = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (state == State.Alive)
        {
            RespondToThrustInput();
            RespondToRotateInput();
        }
        if(Debug.isDebugBuild)
        {
            DebugModeInputs();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (state != State.Alive || !collisionsDisabled)    return;
        

        switch(collision.gameObject.tag)
        {
            case "Friendly":
                //Do nothing
                break;
            case "Finish":
                StartSuccessSequence();
                break;
            default:
                StartDeathSequence();
                break;
        }
    }
    private void DebugModeInputs()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            LoadNextLevel();
        }
        else if(Input.GetKeyDown(KeyCode.C))
        {
            collisionsDisabled = !collisionsDisabled;
        }
    }

    private void StartDeathSequence()
    {
        state = State.Dying;
        mainSound.Stop();
        mainSound.PlayOneShot(deathSound);
        if (mainEngineParticles.isPlaying)
            mainEngineParticles.Stop();
        deathParticles.Play();
        Invoke("LoadFirstLevel", deathSound.length);
    }

    private void RespondToRotateInput()
    {
        rigidbody.freezeRotation = true;
        float rotationThisFrame = rcsThrust * Time.deltaTime;

        if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(Vector3.back * rotationThisFrame);
        }

        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(Vector3.forward * rotationThisFrame);
        }
        rigidbody.freezeRotation = false;
    }

    private void StartSuccessSequence()
    {
        state = State.Transcending;
        mainSound.Stop();
        mainSound.PlayOneShot(successSound);
        if (mainEngineParticles.isPlaying)
            mainEngineParticles.Stop();
        successParticles.Play();
        Invoke("LoadNextLevel", successSound.length); //parameterise time
    }

    private void LoadFirstLevel()
    {
        SceneManager.LoadScene(0);
    }

    private void LoadNextLevel()
    {
        int currentLevelIndex = SceneManager.GetActiveScene().buildIndex;
        int nextLevelIndex = currentLevelIndex + 1;
        if (currentLevelIndex == SceneManager.sceneCountInBuildSettings)
        {
            nextLevelIndex = 0;
        }
        SceneManager.LoadScene(nextLevelIndex);
    }

    private void RespondToThrustInput()
    {
        if (Input.GetKey(KeyCode.Space))       
            ApplyThrust();       
        else
        {
            mainSound.Stop();
            mainEngineParticles.Stop();
        }

    }

    private void ApplyThrust()
    {
        rigidbody.AddRelativeForce(Vector3.up * mainThrust * Time.deltaTime);
        if (!mainSound.isPlaying)
        {
            mainSound.PlayOneShot(mainEngine);
        }
        if(!mainEngineParticles.isPlaying)
        {
            mainEngineParticles.Play();
        }
    }
}

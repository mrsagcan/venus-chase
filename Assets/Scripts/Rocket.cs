using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{
    [SerializeField] float rcsThrust;
    [SerializeField] float mainThrust;
    [SerializeField] AudioClip mainEngine;
    [SerializeField] AudioClip deathSound;
    [SerializeField] AudioClip successSound;
    private Rigidbody rigidbody;
    AudioSource mainSound;

    enum State { Alive, Dying, Transcending };
    State state = State.Alive;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        mainSound = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        //somewhere stop sound on death
        if(state == State.Alive)
        {
            RespondToThrustInput();
            RespondToRotateInput();
        }
    }


    private void OnCollisionEnter(Collision collision)
    {
        if(state != State.Alive)    return;
        

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

    private void StartDeathSequence()
    {
        state = State.Dying;
        mainSound.Stop();
        mainSound.PlayOneShot(deathSound);
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
        Invoke("LoadNextLevel", successSound.length); //parameterise time
    }

    private void LoadFirstLevel()
    {
        SceneManager.LoadScene(0);
    }

    private void LoadNextLevel()
    {
        SceneManager.LoadScene(1); // todo allow for more than two levels
    }

    private void RespondToThrustInput()
    {
        if (Input.GetKey(KeyCode.Space))       
            ApplyThrust();       
        else
            mainSound.Stop();
    }

    private void ApplyThrust()
    {
        rigidbody.AddRelativeForce(Vector3.up * mainThrust);
        if (!mainSound.isPlaying)
            mainSound.PlayOneShot(mainEngine);
    }
}

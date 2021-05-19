using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{
    [SerializeField] float rcsThrust;
    [SerializeField] float mainThrust;
    private Rigidbody rigidbody;
    AudioSource thrustSound;

    enum State { Alive, Dying, Transcending };
    State state = State.Alive;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        thrustSound = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        //somewhere stop sound on death
        if(state == State.Alive)
        {
            Thrust();
            Rotate();
        }
    }

    private void Rotate()
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

    private void OnCollisionEnter(Collision collision)
    {
        if(state != State.Alive)    return;
        

        switch(collision.gameObject.tag)
        {
            case "Friendly":
                //Do nothing
                break;
            case "Finish":
                state = State.Transcending;
                Invoke("LoadNextLevel",1f); //parameterise time
                break;
            default:
                state = State.Dying;
                Invoke("LoadFirstLevel", 1f);
                break;
        }
    }

    private void LoadFirsLevel()
    {
        SceneManager.LoadScene(0);
    }

    private void LoadNextLevel()
    {
        SceneManager.LoadScene(1); // todo allow for more than two levels
    }

    private void Thrust()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            rigidbody.AddRelativeForce(Vector3.up * mainThrust);
            if (!thrustSound.isPlaying)
                thrustSound.Play();
        }
        else
            thrustSound.Stop();
    }
}

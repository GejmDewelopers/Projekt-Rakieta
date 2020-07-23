using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{
    // ŻEBY BYŁO WIDOCZNE W INSPEKTORZE: PUBLIC ALBO [SERIALIZEFIELD] (Public gorsze)
    [SerializeField] float rcsThrust = 100f;
    [SerializeField] float mainThrust = 1000f;
    [SerializeField] float levelLoadDelay = 2f;

    [SerializeField] ParticleSystem mainEngineParticles;
    [SerializeField] ParticleSystem successParticles;
    [SerializeField] ParticleSystem deathParticles;
    Rigidbody rigidBody;
    AudioSource[] sounds;
    AudioSource audioSource;
    AudioSource audioSourceDead;
    AudioSource audioSourceWin;
    AudioSource audioSourceLoad;
    bool areCollisions = true;
    int nextSceneToLoad;

    enum State { Alive, Dying, Transcending}
    State state = State.Alive;
    // Start is called before the first frame update
    void Start()
    {
        nextSceneToLoad = SceneManager.GetActiveScene().buildIndex + 1;
        sounds = GetComponents<AudioSource>();
        audioSource = sounds[0];
        audioSourceDead = sounds[1];
        audioSourceWin = sounds[2];
        audioSourceLoad = sounds[3];
        rigidBody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (state == State.Alive || state == State.Transcending)
        {
            Thrust();
            Rotate();
        }
        if (Debug.isDebugBuild){
            RespondToDebugKeys();
        }
        
    }

    private void RespondToDebugKeys()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            LoadNextLevel();
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            areCollisions = !areCollisions;
        }
    }

    private void Rotate()
    {
        rigidBody.freezeRotation = true; //wyłącza fizyke

        float rotationSpeedThisFrame = rcsThrust * Time.deltaTime;

        if (Input.GetKey(KeyCode.RightArrow) && Input.GetKey(KeyCode.LeftArrow) == false)
        {
            transform.Rotate(Vector3.back * rotationSpeedThisFrame);
        }
        if (Input.GetKey(KeyCode.LeftArrow) && Input.GetKey(KeyCode.RightArrow) == false)
        {
            transform.Rotate(Vector3.forward * rotationSpeedThisFrame);
        }
        rigidBody.freezeRotation = false; //włącza fizyke
    }

    private void Thrust()
    {
        float mainSpeedThisFrame = mainThrust * Time.deltaTime;
        audioSource.Pause();
        mainEngineParticles.Stop();
        if (Input.GetKey(KeyCode.Space))
        {
            mainEngineParticles.Play();
            audioSource.Play();
            rigidBody.AddRelativeForce(Vector3.up * mainSpeedThisFrame);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (state != State.Alive) { return; }
        switch (collision.gameObject.tag)
        {
            case "Friendly":
                break;
            case "Finish":
                state = State.Transcending;
                successParticles.Play();
                audioSourceWin.Play();
                Invoke("LoadNextLevel", levelLoadDelay); // parameterize time
                break;
            default:
                if (areCollisions)
                {
                    state = State.Dying;
                    audioSource.Stop();
                    deathParticles.Play();
                    audioSourceDead.Play();
                    Invoke("Death", levelLoadDelay);

                }
                break;
        }
    }

    private void LoadNextLevel()
    {
        if(nextSceneToLoad == 5) SceneManager.LoadScene(0);
        else SceneManager.LoadScene(nextSceneToLoad);
    }
    private void Death()
    {
        audioSourceLoad.Play();
        SceneManager.LoadScene(nextSceneToLoad-1);
    }
}

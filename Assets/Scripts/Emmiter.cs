using UnityEngine;
using UnityEngine.Audio;

public class Emmiter : MonoBehaviour
{
    public Collider Area;   // Drag your zone/room collider here
    public GameObject Goal;
    private string currentTagName = "Goal";

    public bool isActive = true;
    public AudioSource audioSource;
    public AudioClip beepClip;
    public float beepInterval = 0.6f;

    private float _timer;


    void Start()
    {
        FindGoalObject();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (Goal == null) return;

        
        Vector3 closestPoint = Area.ClosestPoint(Goal.transform.position);
        transform.position = closestPoint;


        if (!isActive || audioSource == null || beepClip == null) return;

        _timer -= Time.deltaTime;
        if (_timer <= 0f)
        {
            audioSource.PlayOneShot(beepClip);
            _timer = beepInterval;
        }
    }

    public void UpdateTargetGoal(string newTag)
    {
        

        currentTagName = newTag;
        if (currentTagName == "Goal5")
        {
            Debug.Log("Still tracking Goal4...");
            currentTagName = "Goal4";
        }
        FindGoalObject();
    }

    private void FindGoalObject()
    {
        GameObject foundObject = GameObject.FindGameObjectWithTag(currentTagName);

        if (foundObject != null)
        {
            Goal = foundObject;
            Debug.Log($"Ambient sound is now tracking: {Goal.name}");
        }
        else
        {
            Debug.LogWarning($"Could not find any GameObject with the tag: {currentTagName}");
        }
    }


}
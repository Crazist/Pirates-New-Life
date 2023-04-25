using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour
{
    [SerializeField] private Transform playerTransform; // reference to the character's transform
    [SerializeField] private float cameraSpeed = 1.0f; // the speed at which the camera moves

    [SerializeField] private Animator _animator;

    [SerializeField] private float zoneSize = 2.0f; // the size of the zone in which the camera moves

    private float minX; // the minimum x position of the camera
    private float maxX; // the maximum x position of the camera

    void Start()
    {
        // calculate the minimum and maximum x positions based on the zone size
        minX = transform.position.x - zoneSize;
        maxX = transform.position.x + zoneSize;
    }

    void Update()
    {
        // get the current x position of the camera
        float currentX = transform.position.x;

        // get the current x position of the player
        float playerX = playerTransform.position.x;

        // check if the player is moving within the zone
        if (playerX < minX || playerX > maxX)
        {
            // calculate the new x position of the camera
            float newX = Mathf.Lerp(currentX, playerX, cameraSpeed * Time.deltaTime);

            // update the camera position
            transform.position = new Vector3(newX, transform.position.y, transform.position.z);
        }

        if(transform.position.x < currentX + 1 || transform.position.x < currentX - 1) 
        {
            minX = transform.position.x - zoneSize;
            maxX = transform.position.x + zoneSize;
        }
    }

    public void ShakeCameraAnimation()
    {
        var _rand = Random.Range(0, 3);
        switch (_rand)
        {
            case 0:
                _animator.SetTrigger("shake");
                break;
            case 1:
                _animator.SetTrigger("shake2");
                break;
            case 2:
                _animator.SetTrigger("shake3");
                break;
        }
    }
}


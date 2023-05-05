using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour
{
    [SerializeField] private Transform playerTransform; // reference to the character's transform
    [SerializeField] private float cameraSpeed = 1.0f; // the speed at which the camera moves
    [SerializeField] private float cameraDistance = 10.0f; // the distance between the camera and the player
    [SerializeField] private float cameraHeight = 3.0f; // the height of the camera above the player
    [SerializeField] private float cameraOffset = 2.0f; // the horizontal offset of the camera from the player

    [SerializeField] private Animator _animator;

    [SerializeField] private float zoneSize = 2.0f; // the size of the zone in which the camera moves

    private float minX; // the minimum x position of the camera
    private float maxX; // the maximum x position of the camera
    private float minZ; // the minimum z position of the camera
    private float maxZ; // the maximum z position of the camera

    private bool isMoving = false;
    private Vector3 targetPosition;
    private void Start()
    {
        CalculateMinMaxPositions();
    }

    private void Update()
    {
        float playerX = playerTransform.position.x;
        float playerZ = playerTransform.position.z;

        if (playerX < minX || playerX > maxX || playerZ < minZ || playerZ > maxZ)
        {
            targetPosition = new Vector3(playerTransform.position.x + cameraOffset, transform.position.y, playerTransform.position.z - cameraDistance);
            isMoving = true;
        }

        if (isMoving)
        {
            // calculate the target x and z positions based on the player position and camera offset/distance
            float targetX = playerTransform.position.x + cameraOffset;
            float targetZ = playerTransform.position.z - cameraDistance;

            // use Lerp to smoothly move the camera towards the target position
            Vector3 targetPosition = new Vector3(targetX, transform.position.y, targetZ);
            transform.position = Vector3.Lerp(transform.position, targetPosition, cameraSpeed * Time.deltaTime);

            if (transform.position == targetPosition)
            {
                isMoving = false;
            }
        }

        CalculateMinMaxPositions();
    }

    private void CalculateMinMaxPositions()
    {
        minX = transform.position.x - cameraOffset - zoneSize;
        maxX = transform.position.x - cameraOffset + zoneSize;
        minZ = transform.position.z + cameraDistance - zoneSize;
        maxZ = transform.position.z + cameraDistance + zoneSize;
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


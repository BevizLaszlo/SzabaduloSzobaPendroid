using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float delay = 1.5f;
    [SerializeField] private float viewDistance = 14f;

    private Vector3 offset;
    private float lowY;
    private bool isFacingRight;

    public static CameraFollow Instance;

    private void Start()
    {
        isFacingRight = true;
        offset = transform.position - target.position;
        offset.x -= viewDistance / 2;
        lowY = transform.position.y;

        if (Instance != null)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        GameObject.DontDestroyOnLoad(this.gameObject);
    }

    private void FixedUpdate()
    {
        if (isFacingRight != target.localScale.x < 0)
        {
            isFacingRight = target.localScale.x < 0;
            offset.x += isFacingRight ? -viewDistance : viewDistance;
        }

        Vector3 targetPosition = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, targetPosition, delay * Time.deltaTime);
        if (transform.position.y < lowY)
        {
            transform.position = new(transform.position.x, lowY, transform.position.z);
        }
    }

    public void ResetPosition()
    {
        transform.position = new(0, 0, 0);
    }
}

using UnityEngine;

public class GlacierSpawner : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float distanceToRespawn = 10f;
    [SerializeField] private float scrollSpeed = -2f;
    [SerializeField] private float totalLength = 50f;

    private float scrollLocation;
    private bool isScrolling;

    public float IsScrolling => IsScrolling;
    private Transform playerTransform;

    private void Start()
    {
        playerTransform = Player.Instance.transform;
    }

    private void Update()
    {
        isScrolling = GameManager.Instance.GameState == GameState.Playing;

        if (!isScrolling)
            return;

        // parallax movement effect on glaciers
        scrollLocation += (scrollSpeed * GameManager.Instance.DifficultyModifier) * Time.deltaTime;
        Vector3 newLocation = (playerTransform.position.z + scrollLocation) * Vector3.forward;
        transform.position = newLocation;

        // when reaching respawn distance, relocate the furthest glacier
        // note: the furthest glacier is considered as the first child
        var leftFurthestGlacier = transform.GetChild(0);
        if (leftFurthestGlacier.transform.position.z < playerTransform.position.z - distanceToRespawn)
        {
            // teleport the furthest glacier physically and make it as last child in the hierarchy
            // left side glacier
            leftFurthestGlacier.localPosition += Vector3.forward * totalLength;
            leftFurthestGlacier.SetSiblingIndex(transform.childCount);

            // do the same for the furthest right side glacier
            // right side glacier
            var rightFurthestGlacier = transform.GetChild(0);
            rightFurthestGlacier.localPosition += Vector3.forward * totalLength;
            rightFurthestGlacier.SetSiblingIndex(transform.childCount);
        }
    }
}

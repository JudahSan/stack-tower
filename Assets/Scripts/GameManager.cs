using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Transform blockPrefab;
    [SerializeField] private Transform blockHolder;

    [Header("Drop Settings")]
    [SerializeField] private float dropCooldown = 0.5f; // Time between drops

    private Transform currentBlock = null;
    private Rigidbody2D currentRigidbody;

    private Vector2 blockStartPosition = new Vector2(0f, 4f);

    private float blockSpeed = 8f;
    private float blockSpeedIncrement = 0.5f;
    private int blockDirection = 1;
    private float xLimit = 5;

    // Track if we can drop another block
    private bool canDrop = true;

    void Start()
    {
        SpawnNewBlock();
    }

    private void SpawnNewBlock()
    {
        currentBlock = Instantiate(blockPrefab, blockHolder);
        currentBlock.position = blockStartPosition;
        currentBlock.GetComponent<SpriteRenderer>().color = Random.ColorHSV();
        currentRigidbody = currentBlock.GetComponent<Rigidbody2D>();
        currentRigidbody.simulated = false; // Ensure physics is disabled initially

        blockSpeed += blockSpeedIncrement;
    }

    void Update()
    {
        if (currentBlock && canDrop)
        {
            float moveAmount = Time.deltaTime * blockSpeed * blockDirection;
            currentBlock.position += new Vector3(moveAmount, 0, 0);

            if (Mathf.Abs(currentBlock.position.x) > xLimit)
            {
                currentBlock.position = new Vector3(blockDirection * xLimit, currentBlock.position.y, 0);
                blockDirection = -blockDirection;
            }

            // Check input only if we can drop
            if (Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                DropCurrentBlock();
            }
        }
    }

    private void DropCurrentBlock()
    {
        if (currentBlock != null && canDrop)
        {
            // Disable dropping temporarily
            canDrop = false;

            // Drop the current block
            currentBlock = null;
            currentRigidbody.simulated = true;

            // Start cooldown before spawning next block
            StartCoroutine(DropCooldownRoutine());
        }
    }

    // Coroutine that handles the delay between drops
    private IEnumerator DropCooldownRoutine()
    {
        Debug.Log("Starting drop cooldown for " + dropCooldown + " seconds");

        // Wait for the cooldown period
        yield return new WaitForSeconds(dropCooldown);

        Debug.Log("Cooldown complete, spawning new block");

        // Re-enable dropping
        canDrop = true;

        // Spawn the next block
        SpawnNewBlock();
    }
}

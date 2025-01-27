using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerControl : MonoBehaviour
{
    public float moveSpeed;
    public LayerMask solidObjectsLayer;
    public LayerMask interactableLayer;
    public LayerMask grassLayer;

    public event Action OnEncountered;

    private bool isMoving;
    private Vector2 input;

    private Animator animator;

    public static PlayerControl Instance { get; private set; }

    private bool canMove = true;


    private void Awake()
    {
        animator = GetComponent<Animator>();
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Destroy duplicate players
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void HandleUpdate()
    {
        if (!canMove) return; // Prevent movement

        if (!isMoving){ 
            // Get the input from the player
            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");

            Debug.Log($"Player Input: {input}");

            if (input != Vector2.zero)
            {   
                animator.SetFloat("moveX", input.x);
                animator.SetFloat("moveY", input.y);
                var targetPos = transform.position; // current position of the player
                targetPos.x += input.x;
                targetPos.y += input.y;
                
                if (IsWalkable(targetPos))
                    StartCoroutine(Move(targetPos));
                
            }
            animator.SetBool("isMoving", isMoving);

            if(Input.GetKeyDown(KeyCode.Z))
            {
                Interact();

            }
        }
    }

    void Interact()
    {
        var facingDir = new Vector3(animator.GetFloat("moveX"), animator.GetFloat("moveY"));
        var interactPos = transform.position + facingDir; // the direction of the player facing

        // Debug.DrawLine(transform.position, interactPos, Color.red, 0.5f);

        var collider = Physics2D.OverlapCircle(interactPos, 0.3f, interactableLayer);
        if (collider != null)
        {
            collider.GetComponent<Interactable>()?.Interact();
        }
    }

    IEnumerator Move(Vector3 targetPos)
    {
        Debug.Log($"Starting Move: TargetPos = {targetPos}");
        isMoving = true;
        while ((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon) // while the distance between the player and the target position is greater than 0
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = targetPos;
        isMoving = false;
        Debug.Log($"Finished Move: CurrentPos = {transform.position}");

        CheckForEncounters();
    }

 

    // private void  OnTriggerEnter2D(Collider2D other)
    // {
    //     if (other.CompareTag("EncounterTile"))
    //     {
    //         if (UnityEngine.Random.Range(1, 101) <= 10)
    //         {
    //             animator.SetBool("isMoving", false);
    //             OnEncountered();
    //         }
    //     }
    // }

    

    private bool IsWalkable(Vector3 targetPos)
    {
        if (Physics2D.OverlapCircle(targetPos, 0.2f, solidObjectsLayer | interactableLayer) != null)
        {
            return false;
        }
        return true;
    }

    private void CheckForEncounters()
    {
        if (Physics2D.OverlapCircle(transform.position, 0.2f, grassLayer) != null)
        {
            if (UnityEngine.Random.Range(1, 101) <= 50)
            {
                animator.SetBool("isMoving", false);
                OnEncountered();
            }
        }
    }

    private IEnumerator PreventMovementOnSpawn()
    {
        canMove = false; // Disable movement
        Debug.Log("Movement disabled temporarily.");
        yield return new WaitForSeconds(0.5f); // Wait for half a second
        canMove = true; // Re-enable movement
        Debug.Log("Movement re-enabled.");
    }

    // Call this in the Portal script after setting the player's position:
    public void DisableMovementTemporarily()
    {
        StartCoroutine(PreventMovementOnSpawn());
    }


    public void ResetInput()
    {
        input = Vector2.zero;
        animator.SetBool("isMoving", false);
        animator.SetFloat("moveX", 0);
        animator.SetFloat("moveY", 0);
        Debug.Log("Input and animator reset.");
    }

}
using UnityEngine;
using DG.Tweening;

public class ShootingPositionMovement : MonoBehaviour
{
    [SerializeField] Vector3 start;
    [SerializeField] Vector3 end;
    [SerializeField] float duration = 2f;
    [SerializeField] Ease easeType = Ease.InOutQuad;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        // Set initial position
        transform.localPosition = start;
        
        // Create the movement sequence
        Sequence movementSequence = DOTween.Sequence();
        
        // Add movement to end position
        movementSequence.Append(transform.DOLocalMove(end, duration).SetEase(easeType));
        
        // Add movement back to start position
        movementSequence.Append(transform.DOLocalMove(start, duration).SetEase(easeType));
        
        // Set the sequence to loop infinitely
        movementSequence.SetLoops(-1);
    }
}

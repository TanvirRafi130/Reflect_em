using UnityEngine;
using DG.Tweening;
using TMPro;
public class WorldCanvas : MonoBehaviour
{

    private static WorldCanvas instance;
    public static WorldCanvas Instance => instance;

    [SerializeField] GameObject textObject;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void ShowDamageText(Vector2 pos, float damageValue)
    {
        var obj = Instantiate(textObject, this.transform.position, Quaternion.identity);
        obj.transform.parent = this.transform;
        obj.transform.localScale = Vector3.zero;
        obj.transform.position = pos;
        var textComp = obj.GetComponent<TextMeshProUGUI>();
        textComp.text = $"-{damageValue}";
        textComp.color = Color.red;
        // Create sequence for floating up and scaling
        Sequence sequence = DOTween.Sequence();
        sequence.Append(obj.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack));
        sequence.Join(obj.transform.DOLocalMoveY(pos.y+2f, 0.3f).SetEase(Ease.OutQuad));
        sequence.OnComplete(() => Destroy(obj, 0.1f));
    }
}

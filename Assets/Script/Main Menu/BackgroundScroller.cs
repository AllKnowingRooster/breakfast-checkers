using UnityEngine;
using UnityEngine.UI;

public class BackgroundScroller : MonoBehaviour
{
    [SerializeField] private RawImage bgImage;
    [SerializeField] private float xOffset;
    [SerializeField] private float yOffset;
    private void Update()
    {
        bgImage.uvRect = new Rect(new Vector2(bgImage.uvRect.x + (xOffset * Time.deltaTime), bgImage.uvRect.y + (yOffset * Time.deltaTime)), bgImage.uvRect.size);
    }
}

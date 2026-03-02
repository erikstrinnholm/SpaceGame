using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Slider))]
public class UISliderAudio : MonoBehaviour,
    ISelectHandler, IPointerDownHandler, IPointerUpHandler
{
    [Header("Sounds")]
    [SerializeField] private string selectSound = "UI_SliderSelect";
    [SerializeField] private string moveSound   = "UI_SliderMove";
    [SerializeField] private string dragSound   = "UI_SliderDrag";

    private Slider slider;
    private bool isDragging;

    private void Awake() {
        slider = GetComponent<Slider>();
        slider.onValueChanged.AddListener(OnSliderValueChanged);
    }

    public void OnSelect(BaseEventData eventData) {
        CoreRoot.Instance.Audio.Play(selectSound);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.pointerId == PointerInputModule.kMouseLeftId) {
            isDragging = true;
            CoreRoot.Instance.Audio.Play(dragSound);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (isDragging){
            isDragging = false;
            CoreRoot.Instance.Audio.Stop(dragSound);
        }
    }

    private void OnSliderValueChanged(float value) {
        if (!isDragging)
            CoreRoot.Instance.Audio.Play(moveSound);
    }

    private void OnDestroy() {
        slider.onValueChanged.RemoveListener(OnSliderValueChanged);
        if (isDragging)
            CoreRoot.Instance.Audio.Stop(dragSound);
    }
}

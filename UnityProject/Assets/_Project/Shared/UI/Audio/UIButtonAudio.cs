using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIButtonAudio : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, ISelectHandler, ISubmitHandler
{
    [SerializeField] private string selectSound = "UI_ButtonHighlight";
    [SerializeField] private string clickSound = "UI_ButtonSelect";


    public void OnPointerEnter(PointerEventData eventData) {
        CoreRoot.Instance.Audio.Play(selectSound);
    }

    public void OnSelect(BaseEventData eventData) {
        CoreRoot.Instance.Audio.Play(selectSound);
    }

    // Mouse Click
    public void OnPointerClick(PointerEventData eventData) {
        CoreRoot.Instance.Audio.Play(clickSound);
    }
    
    //Controller/Keyboard Confirm
    public void OnSubmit(BaseEventData eventData) {
        CoreRoot.Instance.Audio.Play(clickSound);
    }    
}

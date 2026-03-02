using UnityEngine;

public interface IInventoryTab {
    void OnTabOpened();                     //
    void OnTabClosed();                     //
    void OnNavigate(Vector2 direction);     //cargoTabUI ok
    void OnMouseMove(Vector2 pos);          //cargoTabUI ok
    void OnConfirm();
    void OnCancel();
}

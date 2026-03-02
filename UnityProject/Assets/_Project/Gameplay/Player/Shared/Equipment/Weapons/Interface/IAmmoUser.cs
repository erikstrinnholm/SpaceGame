public interface IAmmoUser {
    int CurrentAmmo { get; }
    int MagazineSize { get; }
    int ReserveAmmo { get; }   // -1 = infinite
    bool IsReloading { get; }
    float ReloadProgress { get; }   //0..1, valid only when reloading
}

public interface IHeatUser {
    float CurrentHeat { get; }
    float MaxHeat { get; }
    bool IsOverheated { get; }
    float OverheatProgress { get; }   //0..1, valid only when overheated
}

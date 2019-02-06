using ThrowAwayData;
public class TrainViewModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Cargo { get; set; }
    public int Cars { get; set; }
    public TrainViewModel() : this(new Train())
    {
    }
    public TrainViewModel(Train _train)
    {
        Id = _train.Id ?? 0;
        Name = _train.Name;
        Cargo = _train.Cargo;
        Cars = _train.Cars;
    }
}
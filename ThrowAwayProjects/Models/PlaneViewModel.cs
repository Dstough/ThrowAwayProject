using ThrowAwayDb;
class PlaneViewModel
{
    public int Id{get;set;}
    public string Name {get;set;}
    public string Cabin {get;set;}
    public int MaxPassengerCount {get;set;}
    public PlaneViewModel() :this(new Plane())
    {
    }
    public PlaneViewModel(Plane _Plane)
    {
        Id = _Plane.Id ?? 0;
        Name = _Plane.Name;
        Cabin = _Plane.Cabin;
        MaxPassengerCount = _Plane.MaxPassengerCount;
    }
}
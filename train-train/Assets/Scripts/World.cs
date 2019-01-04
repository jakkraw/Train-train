using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.SceneManagement;

public class Level
{
    public static Profile profile;

    static List<StationSymbol> symbols;
    static List<Texture2D> passengers;
    static Texture2D driver;
    static bool limitPassengers;
    static Train train;

    public static void set(Profile p, Train train)
    {
        Level.train = train;
        profile = p;
        symbols = new List<StationSymbol>(p.selectedSymbols);
        passengers = new List<Texture2D>(p.selectedPassengers);
        driver = p.selectedDriver;
        limitPassengers = profile.limitPassengers;

        train.driver.texture = driver;
        train.SpeedLimit = Data.Profile.trainSpeed;
    }

    public static StationSymbol getNextStationSymbol()
    {
        if(symbols.Count == 0) { return null; }

        var s = symbols[0];
        symbols.RemoveAt(0);
        if (!profile.doesEnd)
        {
            symbols.Add(s);
        }

        return s;
    }

    public static StationSymbol getRandomPossibleDestination()
    {
        if (symbols.Count == 0) { return null; }
        var destinations = symbols.GetRange(0,symbols.Count);
        var next_index = System.Math.Min(Random.Range(0, destinations.Count), Random.Range(0, destinations.Count));
        return destinations[next_index];
    }

    public static Texture2D getRandomPassengerTexture()
    {
        return passengers[Random.Range(0, passengers.Count)];
    }

    public static Passenger getNextPassenger()
    {
        var t = getRandomPassengerTexture();
        if (t == null) return null;

        var d = getRandomPossibleDestination();
        if (d == null) return null;

        return Passenger.GetPassenger(d, t);
    }

    public static Station spawnNextStation()
    {
        var nextSymbol = getNextStationSymbol();
        if (nextSymbol == null) return null;
      
        var newstation = Station.Spawn(nextSymbol);

        var toSpawn = train.seats.Count(s => (s.isEmpty() || s.passenger.GetComponent<Passenger>().symbol.symbol == nextSymbol));

        foreach (var seat in newstation.seats)
        {
            if(Random.value < (profile.limitPassengers ? 0.9 : 0.4))
            {
                if (profile.limitPassengers && toSpawn-- <= 0) {
                    continue; }

                var p = getNextPassenger();
                if (p != null)
                {
                    seat.Place(p);
                }
            }
           
        }

        return newstation;
    }

}


public class World : MonoBehaviour {
    public Station firstStation;
    public Train train;
    public Environment environment;
    public Text scoreText;
    int score = 0;
    bool quit = false;
    float _newStationDistance = 100;
    bool enablePassengerMove = false;

    private void Start()
    {
        Level.set(Data.Profile, train);

        scoreText.gameObject.SetActive(Level.profile.allowScore);
        

        var toSpawn = train.seats.Count;
        foreach (var seat in firstStation.seats)
        {
            if (Random.value < (Level.profile.limitPassengers ? 0.9 : 0.4))
            {
                if (Level.profile.limitPassengers && toSpawn-- <= 0)
                {
                    continue;
                }

                var p = Level.getNextPassenger();
                if (p != null)
                {
                    seat.Place(p);
                }
            }

        }

    }

    void Update () {
        SpawnStations(); 
        HandleInput();
        MoveWorld();
        train.Decelerate();
        if (quit)
        {
            quitGame();
        }

        var rect = ClosestStation().GetComponent<BoxCollider2D>().bounds;
        enablePassengerMove = train.seats.All(seat => rect.Contains(seat.transform.position));

        foreach (Seat seat in FindObjectsOfType<Seat>()) {
            seat.setActive(train.Speed == 0 && enablePassengerMove);
        }


        var passengersToLeave = ClosestStation().seats.FindAll(s => s.passenger && s.passenger.symbol.symbol == ClosestStation().symbol.symbol);
        foreach (var seat in passengersToLeave)
        {
            seat.leaveSeat();
            score+=3;
            scoreText.text = score.ToString();
        }

    }

    void MoveWorld()
    {
        environment.SetMoveSpeed(-train.Speed);
        foreach (GameObject station in GameObject.FindGameObjectsWithTag("Station"))
        {
            station.transform.Translate(Time.deltaTime * -train.Speed, 0.0f, 0.0f);
        }
    }

    public void quitGame()
    {
         StartCoroutine(removeTrain());
    }


    private IEnumerator removeTrain()
    {
        train.playLeave();
        yield return new WaitForSeconds(5);
        SceneManager.LoadScene("Menu");

    }

    void SpawnStations()
    {
        var trainPos = train.transform.position;
        var station = ClosestStation();
        var stationPos = station.transform.position;
        if (Vector3.Distance(trainPos, stationPos) > _newStationDistance && trainPos.x > stationPos.x)
        {
            Destroy(station.gameObject);
            var newStation = Level.spawnNextStation();
            if(newStation == null) {
                quit = true;
            }
        }
            _newStationDistance = Random.Range(30, 70);
    }

    void SwapSeat(Seat a, Seat b)
    {
        if (!a || !b) return;
        var p1 = a.Remove();
        var p2 = b.Remove();
        if(p2) a.Place(p2);
        if(p1) b.Place(p1);
    }

    Station ClosestStation()
    {
        var a = GameObject.FindGameObjectsWithTag("Station");
        return a[0].GetComponent<Station>();
    }

    void HandleInput()
    {


        if (Input.touchCount < 1)
        {
            if(train.Speed != 0)
            {
                var trainPos = train.middle.position;
                var station = ClosestStation();
                var stationPos = station.middle.position;
                if (trainPos.x < stationPos.x)
                {
                    var k = Vector3.Distance(train.middle.position, station.middle.position) / 10.0f;
                    if (k < 2.5)
                    {
                        train.Break();
                        if (train.Speed < 4) train.Speed = 4;
                        if (k < 0.1)
                        {
                            if (enablePassengerMove)
                            {
                                train.Speed = 0;
                            }
                        }
                    }
                }
            }   
            return;
        }


        RaycastHit hit;
        if (!Physics.Raycast(Camera.main.ScreenPointToRay(Input.touches[0].position), out hit, 500)) {
            train.Break();
            return;
        }

        var gameObject = hit.collider.gameObject;


        if (gameObject.CompareTag("Accelerate")) {
            train.Accelerate();
        }

        if (gameObject.CompareTag("Train Seat"))
        {
            var seat = gameObject.GetComponent<Seat>();
            var passenger = seat.passenger;
            if (passenger)
            {
                var station_seat = ClosestStation().FreeSeat();
                if (station_seat)
                {
                    passenger.image.canvas.sortingOrder = 2000;
                    SwapSeat(station_seat, seat);
                    if (passenger.symbol.symbol != ClosestStation().symbol.symbol) {
                        passenger.playSad();
                        scoreText.text = (--score).ToString();
                    }
                }
            }
            
        }

        if (gameObject.CompareTag("Station Seat"))
        {
            var seat = gameObject.GetComponent<Seat>();
            var passenger = seat.passenger;
            if (passenger)
            {
                var train_seat = train.FreeSeat();
                if (train_seat)
                {
                    passenger.image.canvas.sortingOrder = 19;
                    SwapSeat(train_seat, seat);
                }
            }
            
        }

    }

}

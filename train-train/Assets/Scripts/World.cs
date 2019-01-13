using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Level
{
    public static Profile profile;
    private static List<Symbol> symbols;
    private static List<Texture2D> passengers;
    private static Texture2D driver;
    private static bool limitPassengers;
    private static Train train;

    public static void set(Profile p, Train train)
    {
        Level.train = train;
        profile = p;
        symbols = new List<Symbol>(p.selectedSymbols);
        passengers = new List<Texture2D>(p.selectedPassengers);
        driver = p.selectedDriver;
        limitPassengers = profile.limitPassengers;

        train.driver = driver;
        train.SpeedLimit = Data.Profile.trainSpeed;
    }

    public static Symbol getNextStationSymbol()
    {
        if (symbols.Count == 0) { return null; }

        var s = symbols[0];
        symbols.RemoveAt(0);
        if (!profile.doesEnd) {
            symbols.Add(s);
        }

        return s;
    }

    public static Symbol getRandomPossibleDestination()
    {
        if (symbols.Count == 0) { return null; }
        var destinations = symbols.GetRange(0, symbols.Count);
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
        if (t == null) {
            return null;
        }

        var d = getRandomPossibleDestination();
        if (d == null) {
            return null;
        }

        return Passenger.GetPassenger(d, t);
    }

    public static Station spawnNextStation()
    {
        var nextSymbol = getNextStationSymbol();
        if (nextSymbol == null) {
            return null;
        }

        var newstation = Station.Spawn(nextSymbol);

        var toSpawn = train.seats.Count(s => (s.isEmpty() || s.passenger.symbolRepresentation.symbol == nextSymbol));

        foreach (var seat in newstation.seats) {
            if (Random.value < (profile.limitPassengers ? 0.9 : 0.4)) {
                if (profile.limitPassengers && toSpawn-- <= 0) {
                    continue;
                }

                var p = getNextPassenger();
                if (p != null) {
                    seat.Place(p);
                }
            }

        }

        return newstation;
    }

}

public class World : MonoBehaviour
{
    public Station firstStation;
    public Train train;
    public Environment environment;
    public Text scoreText;
    private int score = 0;
    private bool quit = false;
    private float _newStationDistance = 100;
    private bool enablePassengerMove = false;

    private void Start()
    {
        Level.set(Data.Profile, train);

        scoreText.gameObject.SetActive(Level.profile.allowScore);


        var toSpawn = train.seats.Count;
        foreach (var seat in firstStation.seats) {
            if (Random.value < (Level.profile.limitPassengers ? 0.9 : 0.4)) {
                if (Level.profile.limitPassengers && toSpawn-- <= 0) {
                    continue;
                }

                var p = Level.getNextPassenger();
                if (p != null) {
                    seat.Place(p);
                }
            }

        }

    }

    private void Update()
    {
        SpawnStations();
        HandleInput();
        MoveWorld();
        train.Decelerate();
        if (quit) {
            quitGame();
        }
        var station = ClosestStation();
        var rect = station.GetComponent<BoxCollider2D>().bounds;
        enablePassengerMove = train.seats.All(seat => rect.Contains(seat.transform.position));

        foreach (Seat seat in FindObjectsOfType<Seat>()) {
            seat.setActive(train.Speed == 0 && enablePassengerMove);
        }


        var passengersToLeave = station.seats.FindAll(s => s.passenger && s.passenger.symbol == station.symbol);
        foreach (var seat in passengersToLeave) {
            seat.leaveSeat();
            score += 3;
            scoreText.text = score.ToString();
        }

    }

    private void MoveWorld()
    {
        environment.SetMoveSpeed(-train.Speed);
        foreach (GameObject station in GameObject.FindGameObjectsWithTag("Station")) {
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

    private void SpawnStations()
    {
        var trainPos = train.transform.position;
        var station = ClosestStation();
        var stationPos = station.transform.position;
        if (Vector3.Distance(trainPos, stationPos) > _newStationDistance && trainPos.x > stationPos.x) {
            Destroy(station.gameObject);
            var newStation = Level.spawnNextStation();
            if (newStation == null) {
                quit = true;
            }
        }
        _newStationDistance = Random.Range(30, 70);
    }

    private Station ClosestStation()
    {
        var a = GameObject.FindGameObjectsWithTag("Station");
        if(a.Count()>=1) { return a[0].GetComponent<Station>(); }
        return null;
    }

    private void stopOnStation(Train train, Station station)
    {
        if (train.Speed != 0) {
            var trainPos = train.middle.position;
            var stationPos = station.middle.position;
            var k = Vector3.Distance(train.middle.position, station.middle.position) / (float)10;
            var trainBehind = train.middle.position.x < station.middle.position.x;
            if (k < 2.5) {
                train.Break();
                if (train.Speed < 4 && trainBehind) {
                    train.Speed = 4;
                }

                if (k < 0.1 || (k < 0.3 && !trainBehind)) {
                    train.Speed = 0;
                }
            }
        }
    }

    private void movePassengerToStation(Seat seat, Station station)
    {
        var passenger = seat.passenger;
        if (!passenger) { return; }
        var station_seat = station.FreeSeat();
        if (!station_seat) { return; }

        passenger.image.canvas.sortingOrder = 2000;
        station_seat.Place(seat.Remove());

        if (passenger.symbolRepresentation.symbol != station.symbolRepresentation.symbol) {
            passenger.playSad();
            scoreText.text = (--score).ToString();
        }
    }

    private void movePassengerToTrain(Seat seat, Train train)
    {
        var passenger = seat.passenger;
        if (!passenger) { return; }
        var train_seat = train.FreeSeat();
        if (!train_seat) { return; }

        passenger.image.canvas.sortingOrder = 19;
        train_seat.Place(seat.Remove());
    }

    private void handleHit(GameObject gameObject)
    {

        if (gameObject.CompareTag("Accelerate")) {
            train.Accelerate();
        }

        if (gameObject.CompareTag("Train Seat")) {
            var station = ClosestStation();
            var seat = gameObject.GetComponent<Seat>();
            movePassengerToStation(seat, station);
        }

        if (gameObject.CompareTag("Station Seat")) {
            var seat = gameObject.GetComponent<Seat>();
            movePassengerToTrain(seat, train);
        }
    }

    private void HandleInput()
    {
        bool anyTouch = false;

        if (Input.touchCount > 0 || Input.GetMouseButton(0)) { anyTouch = true; }

        Vector2 position = new Vector2();

        if (Input.touchCount > 0) {
            position = Input.touches[0].position;
        }

        if (Input.GetMouseButton(0)) {
            position = Input.mousePosition;
        }

        var station = ClosestStation();

        if (!anyTouch) {
            stopOnStation(train, station);
            return;
        }

        RaycastHit hit;
        if (!Physics.Raycast(Camera.main.ScreenPointToRay(position), out hit, 500)) {
            train.Break();
            return;
        }

        var gameObject = hit.collider.gameObject;

        handleHit(gameObject);

    }

}

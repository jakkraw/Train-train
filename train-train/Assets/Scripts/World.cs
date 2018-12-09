using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class Level
{
    public static Profile profile;

    static int currentIndex;
    static List<Symbol_> symbols;

    public static void set(Profile p)
    {
        profile = p;
        currentIndex = 0;
        symbols = p.symbols;
    }

    public static Symbol_ getNextStationSymbol()
    {
        if (currentIndex >= symbols.Count) { return null; }

        var symbol = symbols[currentIndex++];

        return symbol;
    }

    public static Symbol_ getRandomPossibleDestination()
    {
        if (currentIndex >= symbols.Count) { return null; }
        var destinations = symbols.GetRange(currentIndex, symbols.Count - currentIndex);
        return destinations[Random.Range(0, destinations.Count)];
    }

    public static Texture2D getRandomPassengerTexture()
    {
        return profile.passengers[Random.Range(0, profile.passengers.Count)];
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

        foreach (var seat in newstation.seats)
        {
            if(Random.value < 0.4)
            {
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
    bool quit = false;
    float _newStationDistance = 100;
    bool enablePassengerMove = false;

    private void Start()
    {
        Level.set(Data.currentProfile);

        train.driver.texture = Level.profile.driver;

        foreach (var seat in firstStation.seats)
        {
            var p = Level.getNextPassenger();
            if(p != null)
              seat.Place(p);
        }
    }

    void Update () {
        SpawnStations(); 
        HandleInput();
        MoveWorld();
        train.Decelerate();

        var rect = ClosestStation().GetComponent<BoxCollider2D>().bounds;
        enablePassengerMove = train.seats.All(seat => rect.Contains(seat.transform.position));

        foreach (Seat seat in FindObjectsOfType<Seat>()) {
            seat.setActive(train.Speed == 0 && enablePassengerMove);
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
        var anim = train.GetComponent<Animator>();
        anim.Play("train_leave");
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
            if(newStation == null && !quit)
            {
                quit = true;
                quitGame();
                train.GetComponent<Animator>().Play("train_leave");
            }
        }
            _newStationDistance = Random.Range(30, 40);
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
            var sseat = ClosestStation().FreeSeat();
            if (!seat.isEmpty() && !sseat.blocked) SwapSeat(sseat, seat);
        }

        if (gameObject.CompareTag("Station Seat"))
        {
            var seat = gameObject.GetComponent<Seat>();
            var train_seat = train.FreeSeat();
            if(train_seat && !seat.blocked) { SwapSeat(train_seat, seat); }
        }

    }

}

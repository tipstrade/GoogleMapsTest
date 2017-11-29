using GoogleMapsApi;
using GoogleMapsApi.Entities.Common;
using GoogleMapsApi.Entities.Directions.Request;
using GoogleMapsApi.Entities.Geocoding.Request;
using GoogleMapsApi.Entities.Geocoding.Response;
using GoogleMapsApi.StaticMaps;
using GoogleMapsApi.StaticMaps.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1 {
  public partial class Form1 : Form {
    public Form1() {
      InitializeComponent();
    }

    private void Form1_Load(object sender, EventArgs e) {

      //var geocoded = await TestGeocodeAsync();
      //TestMap(geocoded);
      //TestDirections(geocoded);

      //Console.WriteLine();
      //Console.WriteLine("Press any key to continue...");
    }

    protected override void OnLoad(EventArgs e) {
      base.OnLoad(e);
    }

    private const string ApiKey = "AIzaSyCgq4xZNTN-9e5OePiHGIUvOwFojlq0kro";

    private static List<string> Locations { get; set; } = new List<string>() { "N16 5DU", "SO50 4NQ" };

    private const string Region = "GBR";

    private  void TestDirections(IEnumerable<Location> locations) {
      Console.WriteLine($"{nameof(TestMap)}:");

      var request = new DirectionsRequest() {
        ApiKey = ApiKey,
        Destination = locations.Last().LocationString,
        Region = Region,
        TravelMode = TravelMode.Driving,
        Origin = locations.First().LocationString
      };

      Console.Write("\tGetting directions ...");
      var response = GoogleMaps.Directions.Query(request);
      Console.Write($"\t{response.Status}");

      Console.Write("\t" + string.Join("; ", (from r in response.Routes select $"{r.Summary} - {r.Legs.First().Distance.Text}")));

      Console.WriteLine();
    }

    public  List<Location> TestGeocode() {
      Console.WriteLine($"{nameof(TestGeocode)}:");

      var list = new List<Location>();

      foreach (var location in Locations) {
        Console.Write($"\t{location} ...");

        var request = new GeocodingRequest() {
          ApiKey = ApiKey,
          Address = location,
          Region = Region,
        };

        var response =  GoogleMaps.Geocode.Query(request);
        Console.Write($"\t{response.Status}");

        if (response.Status == Status.OK) {
          list.AddRange(from r in response.Results select r.Geometry.Location);
          Console.Write("\t" + string.Join("; ", (from r in response.Results select r.Geometry.Location.LocationString).ToArray()));
        }

        Console.WriteLine();
      }

      return list;
    }

    public  void TestMap(IEnumerable<Location> locations) {
      Console.WriteLine($"{nameof(TestMap)}:");

      var markers = new List<Marker>() {
        new Marker() {
          Locations = locations.Cast<ILocationString>().ToList(),
          Style = new MarkerStyle() {
            Color = "red",
          }
        }
      };

      var request = new StaticMapRequest(markers, new ImageSize(512, 512)) {
        ApiKey = ApiKey,
      };

      var engine = new StaticMapsEngine() {
      };

      Console.Write("\tGenerating Map URL ...");
      try {
        var url = engine.GenerateStaticMapURL(request);
        Console.Write(url);
      } catch (Exception ex) {
        Console.Write($"\t{ex.GetType()} {ex.Message}");
      }

      Console.WriteLine();
    }

    private void button1_Click(object sender, EventArgs e) {
      var geocoded = TestGeocode();
      TestMap(geocoded);
      TestDirections(geocoded);

      //Console.WriteLine();
      //Console.WriteLine("Press any key to continue...");
      return;

      Foo().ContinueWith(delegate {

      });
    }

    private async void button2_Click(object sender, EventArgs e) {
      await Foo();
    }

    public async Task Foo() {
      await Task.Delay(1000);

      var text = textBox1.Text;

      textBox2.Text = string.Join("", text.Reverse().ToArray());
    }
  }
}

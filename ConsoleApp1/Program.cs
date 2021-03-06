﻿using GoogleMapsApi;
using GoogleMapsApi.Entities.Common;
using GoogleMapsApi.Entities.Directions.Request;
using GoogleMapsApi.Entities.Geocoding.Request;
using GoogleMapsApi.Entities.Geocoding.Response;
using GoogleMapsApi.StaticMaps;
using GoogleMapsApi.StaticMaps.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleApp1 {
  public class Program {
    private const string ApiKey = "AIzaSyCgq4xZNTN-9e5OePiHGIUvOwFojlq0kro";

    private static List<string> Locations { get; set; } = new List<string>() { "N16 5DU", "SO50 4NQ" };

    private const string Region = "GBR";

    public static void Main(string[] args) {
      var geocoded = TestGeocode();
      TestMap(geocoded);
      TestDirections(geocoded);

      Console.WriteLine();
      Console.WriteLine("Press any key to continue...");
      Console.Read();
    }

    private static void TestDirections(IEnumerable<Location> locations) {
      Console.WriteLine($"{nameof(TestMap)}:");

      var request = new DirectionsRequest() {
        ApiKey = ApiKey,
        Destination =  locations.Last().LocationString,
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

    public static List<Location> TestGeocode() {
      Console.WriteLine($"{nameof(TestGeocode)}:");

      var list = new List<Location>();

      foreach (var location in Locations) {
        Console.Write($"\t{location} ...");

        var request = new GeocodingRequest() {
          ApiKey = ApiKey,
          Address = location,
          Region = Region,
        };

        var response = GoogleMaps.Geocode.Query(request);
        Console.Write($"\t{response.Status}");

        if (response.Status == Status.OK) {
          list.AddRange(from r in response.Results select r.Geometry.Location);
          Console.Write("\t" + string.Join("; ", (from r in response.Results select r.Geometry.Location.LocationString).ToArray()));
        }

        Console.WriteLine();
      }

      return list;
    }

    public static void TestMap(IEnumerable<Location> locations) {
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
  }
}

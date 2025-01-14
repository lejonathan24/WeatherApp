using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Weather_Monitoring_Application.Data;
using Weather_Monitoring_Application.Models;

namespace Weather_Monitoring_Application.Controllers
{
    public class LocationsController : Controller
    {
        private readonly Weather_Monitoring_ApplicationContext _context;
        private readonly IHttpClientFactory _httpClientFactory;
        public LocationsController(Weather_Monitoring_ApplicationContext context, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
        }

        // GET: Locations
        public async Task<IActionResult> Index()
        {
            return View(await _context.Location.ToListAsync());
        }
        
        
        // GET: Locations/Details/5

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var location = await _context.Location
                .FirstOrDefaultAsync(m => m.Id == id);

            if (location == null)
            {
                return NotFound();
            }
            var debug = true;
            double latitude = -9999;
            double longitude = -9999;
            HttpClient client = _httpClientFactory.CreateClient();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true }; // Set options for JSON serializer

            if (!debug)
            {
                // Use IHttpClientFactory or reuse HttpClient

                // 1. Calculate lat and long based on zip code
                var google_api_key = Environment.GetEnvironmentVariable("google_api_key");
                var googleBaseAddress = "https://maps.googleapis.com";
                var googleRequestString = $"/maps/api/geocode/json?address={location.Zipcode}&key={google_api_key}";

                // Create a new HttpRequestMessage for Google API request
                var googleRequest = new HttpRequestMessage(HttpMethod.Get, googleBaseAddress + googleRequestString);

                var googlerequest = await client.SendAsync(googleRequest);
                string googleresult = await googlerequest.Content.ReadAsStringAsync(); // Get the JSON response as a string

                
                var geocodeResponse = JsonSerializer.Deserialize<GeocodeResponse>(googleresult, options);
                Console.WriteLine(geocodeResponse);


                if (geocodeResponse != null)
                {
                    //var results = geocodeResponse.Results[0];
                    // Access the first result
                    LocationCoords geocodeCoords = geocodeResponse.Results[0].Geometry.Location;

                    latitude = geocodeCoords.Lat;
                    longitude = geocodeCoords.Lng;

                    // Use latitude and longitude as needed
                    Console.WriteLine($"Latitude: {latitude}, Longitude: {longitude}");
                }
                else
                {
                    Console.WriteLine("No results found or error in the response.");
                }
                //Console.WriteLine(googleresult);

                // 2. Create API request for weather

                latitude = Math.Round(latitude, 6);
                longitude = Math.Round(longitude, 6);
            }
            else
            {
                Console.WriteLine("Debug Mode - Using default coords");
                longitude = Math.Round(-95.26, 6);
                latitude = Math.Round(29.69, 6);
            }
            

            var weatherBaseAddress = "https://api.open-meteo.com";
            var weatherUrlRequestString = $"/v1/forecast?latitude={latitude}&longitude={longitude}&hourly=temperature_2m";

            // Create a new HttpRequestMessage for weather request
            var weatherRequest = new HttpRequestMessage(HttpMethod.Get, weatherBaseAddress + weatherUrlRequestString);

            // Send the weather request
            var weatherResponse = await client.SendAsync(weatherRequest);
            string result = await weatherResponse.Content.ReadAsStringAsync(); // Get the JSON response as a string

            
            var weatherData = JsonSerializer.Deserialize<WeatherResponse>(result, options); // Deserialize the response into an object

            weatherData.Location = location; // Add location to weather response

            // Return the weather data in the view
            return View(weatherData);
        }

        // GET: Locations/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Locations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,City,State,Zipcode")] Location location)
        {
            if (ModelState.IsValid)
            {
                _context.Add(location);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(location);
        }

        // GET: Locations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var location = await _context.Location.FindAsync(id);
            if (location == null)
            {
                return NotFound();
            }
            return View(location);
        }

        // POST: Locations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,City,State,Zipcode")] Location location)
        {
            if (id != location.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(location);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LocationExists(location.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(location);
        }

        // GET: Locations/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var location = await _context.Location
                .FirstOrDefaultAsync(m => m.Id == id);
            if (location == null)
            {
                return NotFound();
            }

            return View(location);
        }

        // POST: Locations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var location = await _context.Location.FindAsync(id);
            if (location != null)
            {
                _context.Location.Remove(location);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LocationExists(int id)
        {
            return _context.Location.Any(e => e.Id == id);
        }
    }
}

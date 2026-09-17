using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Test_Taste_Console_Application.Domain.DataTransferObjects;

namespace Test_Taste_Console_Application.Domain.Objects
{
    public class Planet
    {
        public string Id { get; set; }
        public float SemiMajorAxis { get; set; }
        public float AvgTemp { get; set; }
        public ICollection<Moon> Moons { get; set; }

        public float AverageMoonGravity
        {
            get => HasMoons() ? Moons.Average(m => m.Gravity) : 0.0f;
        }

        public float AverageMoonTemperature
        {
            get
            {
                if (!HasMoons()) return 0.0f;

                // Check if any moons have individual non-zero temperatures
                var moonsWithTemp = Moons.Where(m => m.AvgTemp != 0).ToArray();
                if (moonsWithTemp.Any())
                {
                    return moonsWithTemp.Average(m => m.AvgTemp > 0 ? m.AvgTemp - 273.15f : m.AvgTemp);
                }

                // Fallback to planet's average temperature converted from Kelvin to Celsius (supporting negative Celsius values)
                if (AvgTemp > 0)
                {
                    return AvgTemp - 273.15f;
                }

                return AvgTemp;
            }
        }

        public Planet(PlanetDto planetDto)
        {
            Id = planetDto.Id;
            SemiMajorAxis = planetDto.SemiMajorAxis;
            AvgTemp = planetDto.AvgTemp;
            Moons = new Collection<Moon>();
            if(planetDto.Moons != null)
            {
                foreach (MoonDto moonDto in planetDto.Moons)
                {
                    Moons.Add(new Moon(moonDto));
                }
            }
        }

        public Boolean HasMoons()
        {
            return (Moons != null && Moons.Count > 0);
        }
    }
}

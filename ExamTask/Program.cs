using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace ExamTask
{
    class Program
    {
        public class Employee
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public int CityId { get; set; }
            public City City { get; set; }
        }
        public class Doctor : Employee
        {
            public string Specialization { get; set; }
        }
        public class Engineer : Employee
        {
            public string FavoriteVideogame { get; set; }
        }
        public class City
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public int Latitude { get; set; }
            public int Longitude { get; set; }
            public List<Employee> Employees { get; set; }
        }
        public class CityContext : DbContext
        {
            public CityContext() : base("ExamTask")
            { }
            public DbSet<City> Cities { get; set; }
            public DbSet<Employee> Employees { get; set; }
            public DbSet<Doctor> Doctors { get; set; }
            public DbSet<Engineer> Engineers { get; set; }

            protected override void OnModelCreating(DbModelBuilder modelBuilder)
            {
                modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
                modelBuilder.Entity<Employee>().HasRequired(x => x.City).WithMany(x => x.Employees).HasForeignKey(x => x.CityId);

                modelBuilder.Entity<Doctor>().Property(d => d.Specialization).IsRequired();
                modelBuilder.Entity<Employee>().Property(e => e.Name).HasMaxLength(128);
            }
        }

        private static void SeedDB()
        {
            var ctx = new CityContext();
            ctx.Cities.Add(new City { Name = "Kiev", Latitude = 1, Longitude = 1 });
            ctx.Cities.Add(new City { Name = "Lviv", Latitude = 1, Longitude = 1 });

            ctx.Doctors.Add(new Doctor { CityId = 1, Name = "name1", Specialization = "spec1" });
            ctx.Doctors.Add(new Doctor { CityId = 1, Name = "name2", Specialization = "spec2" });
            ctx.Doctors.Add(new Doctor { CityId = 1, Name = "name3", Specialization = "spec3" });
            ctx.Doctors.Add(new Doctor { CityId = 1, Name = "name4", Specialization = "spec4" });

            ctx.Engineers.Add(new Engineer { CityId = 1, Name = "name4", FavoriteVideogame = "game1" });

            ctx.SaveChanges();
        }
        public static List<string> GetSpecialization(string cityName)
        {
            using (var ctx = new CityContext())
            {
                var responseDB = (from d in ctx.Doctors 
                                  join c in ctx.Cities 
                                  on d.CityId equals c.Id
                                  where c.Name == cityName 
                                  select d.Specialization);
                return responseDB.ToList();
            }
        }
        static void Main(string[] args)
        {
            foreach (var b in GetSpecialization("Kiev"))
            {
                Console.WriteLine(b);
            }
        }
    }
}

using CharacterManagerApiTutorial.Models;
using CharacterManagerApiTutorial.Models.Auth;
using Microsoft.EntityFrameworkCore;

namespace CharacterManagerApiTutorial.Data
{
    public class CharacterManagerDbContext(DbContextOptions<CharacterManagerDbContext> options) : DbContext(options)
    {
        public DbSet<Character> Characters => Set<Character>();
        public DbSet<FactionType> FactionTypes => Set<FactionType>();
        public DbSet<RaceType> RaceTypes => Set<RaceType>();
        public DbSet<ClassType> ClassTypes => Set<ClassType>();
        public DbSet<Realm> Realms => Set<Realm>();
        public DbSet<CharacterMappings> CharacterMappings => Set<CharacterMappings>();
        public DbSet<User> Users => Set<User>();


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Seed data
            modelBuilder.Entity<FactionType>().HasData(
                new FactionType { Id = 1, Name = "Light Side" },
                new FactionType { Id = 2, Name = "Dark Side" }
            );
            modelBuilder.Entity<RaceType>().HasData(
                new RaceType { Id = 1, Name = "Human" },
                new RaceType { Id = 2, Name = "Dwarf" },
                new RaceType { Id = 3, Name = "Elf" },
                new RaceType { Id = 4, Name = "Orc" },
                new RaceType { Id = 5, Name = "Zombie" },
                new RaceType { Id = 6, Name = "Alien" }
            );
            modelBuilder.Entity<ClassType>().HasData(
                new ClassType { Id = 1, Name = "Warrior" },
                new ClassType { Id = 2, Name = "Archer" },
                new ClassType { Id = 3, Name = "Wizard" },
                new ClassType { Id = 4, Name = "Ninja" },
                new ClassType { Id = 5, Name = "Medic" },
                new ClassType { Id = 6, Name = "Paladin" }
            );
            modelBuilder.Entity<Realm>().HasData(
                new Realm { Id = 1, Name = "Frostgard", Type = "Neutral" },
                new Realm { Id = 2, Name = "Barren Land", Type = "Neutral" },
                new Realm { Id = 3, Name = "Wraithwind", Type = "PVP" },
                new Realm { Id = 4, Name = "Amber Expanse", Type = "Neutral" },
                new Realm { Id = 5, Name = "Boiling Isle", Type = "PVP" }
            );
            modelBuilder.Entity<CharacterMappings>().HasData(
                // Human (Light Side)
                new CharacterMappings { Id = 1, FactionId = 1, RaceId = 1, ClassId = 1 },
                new CharacterMappings { Id = 2, FactionId = 1, RaceId = 1, ClassId = 2 },
                new CharacterMappings { Id = 3, FactionId = 1, RaceId = 1, ClassId = 3 },
                new CharacterMappings { Id = 4, FactionId = 1, RaceId = 1, ClassId = 4 },
                new CharacterMappings { Id = 5, FactionId = 1, RaceId = 1, ClassId = 5 },
                new CharacterMappings { Id = 6, FactionId = 1, RaceId = 1, ClassId = 6 },

                // Dwarf (Light Side)
                new CharacterMappings { Id = 7, FactionId = 1, RaceId = 2, ClassId = 1 },
                new CharacterMappings { Id = 8, FactionId = 1, RaceId = 2, ClassId = 2 },
                new CharacterMappings { Id = 9, FactionId = 1, RaceId = 2, ClassId = 5 },
                new CharacterMappings { Id = 10, FactionId = 1, RaceId = 2, ClassId = 6 },

                // Elf (Light Side)
                new CharacterMappings { Id = 11, FactionId = 1, RaceId = 3, ClassId = 2 },
                new CharacterMappings { Id = 12, FactionId = 1, RaceId = 3, ClassId = 3 },
                new CharacterMappings { Id = 13, FactionId = 1, RaceId = 3, ClassId = 4 },
                new CharacterMappings { Id = 14, FactionId = 1, RaceId = 3, ClassId = 5 },

                // Orc (Dark Side)
                new CharacterMappings { Id = 15, FactionId = 2, RaceId = 4, ClassId = 1 },
                new CharacterMappings { Id = 16, FactionId = 2, RaceId = 4, ClassId = 2 },
                new CharacterMappings { Id = 17, FactionId = 2, RaceId = 4, ClassId = 4 },

                // Zombie (Dark Side)
                new CharacterMappings { Id = 18, FactionId = 2, RaceId = 5, ClassId = 1 },
                new CharacterMappings { Id = 19, FactionId = 2, RaceId = 5, ClassId = 2 },
                new CharacterMappings { Id = 20, FactionId = 2, RaceId = 5, ClassId = 3 },
                new CharacterMappings { Id = 21, FactionId = 2, RaceId = 5, ClassId = 4 },
                new CharacterMappings { Id = 22, FactionId = 2, RaceId = 5, ClassId = 5 },

                // Alien (Dark Side)
                new CharacterMappings { Id = 23, FactionId = 2, RaceId = 6, ClassId = 1 },
                new CharacterMappings { Id = 24, FactionId = 2, RaceId = 6, ClassId = 2 },
                new CharacterMappings { Id = 25, FactionId = 2, RaceId = 6, ClassId = 3 },
                new CharacterMappings { Id = 26, FactionId = 2, RaceId = 6, ClassId = 4 },
                new CharacterMappings { Id = 27, FactionId = 2, RaceId = 6, ClassId = 5 },
                new CharacterMappings { Id = 28, FactionId = 2, RaceId = 6, ClassId = 6 }
            );
        }
    }
}

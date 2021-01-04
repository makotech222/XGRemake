using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Xenogears.Database;

namespace Xenogears.Database
{
    public class XenogearsDBContext : DbContext
    {
        public DbSet<XGCharacter> Characters { get; set; }
        public DbSet<XGCharacterAnimation> CharacterAnimations { get; set; }
        public DbSet<XGGameState> GameStates { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseSqlServer(DatabaseTools.SQLConnection);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<XGCharacterAnimation>()
                .HasKey(x => x.Id);

            var enumConverter = new EnumToStringConverter<EActionTypes>();
            modelBuilder.Entity<XGCharacterAnimation>()
                .Property(e => e.Name)
                .HasConversion(enumConverter);

            modelBuilder.Entity<XGCharacter>()
                .HasMany(a => a.RawAnimationsData)
                .WithOne()
                .HasForeignKey(x => x.CharacterName);

            Seed(modelBuilder);
        }

        private void Seed(ModelBuilder modelBuilder)
        {

            int id = 1;
            modelBuilder.Entity<XGGameState>().HasData(new XGGameState() { Id = id++, Name = "GameStart" });
            modelBuilder.Entity<XGGameState>().HasData(new XGGameState() { Id = id++, Name = "Lahan_TalkedToDanOutside" });
            modelBuilder.Entity<XGGameState>().HasData(new XGGameState() { Id = id++, Name = "Lahan_TalkedToAlice" });
            modelBuilder.Entity<XGGameState>().HasData(new XGGameState() { Id = id++, Name = "UzukiHouse_TalkedToCitan" });

            modelBuilder.Entity<XGCharacter>().HasData(new XGCharacter() { Name = "Fei" });
            modelBuilder.Entity<XGCharacter>().HasData(new XGCharacter() { Name = "Elly" });
            modelBuilder.Entity<XGCharacter>().HasData(new XGCharacter() { Name = "Citan" });
            modelBuilder.Entity<XGCharacter>().HasData(new XGCharacter() { Name = "Bart" });
            modelBuilder.Entity<XGCharacter>().HasData(new XGCharacter() { Name = "Billy" });
            modelBuilder.Entity<XGCharacter>().HasData(new XGCharacter() { Name = "Rico" });
            modelBuilder.Entity<XGCharacter>().HasData(new XGCharacter() { Name = "Esmeralda" });
            modelBuilder.Entity<XGCharacter>().HasData(new XGCharacter() { Name = "Chu-Chu" });
            modelBuilder.Entity<XGCharacter>().HasData(new XGCharacter() { Name = "Maria" });
            modelBuilder.Entity<XGCharacter>().HasData(new XGCharacter() { Name = "Adult Esmeralda" });
            id = 1;
            modelBuilder.Entity<XGCharacterAnimation>().HasData(new XGCharacterAnimation() { Id = id++, CharacterName = "Fei", Name = EActionTypes.Idle, Count = 1, StartSprite = @"FieldPCSprites2\003922_bin\0000.png", SkipBetweenFiles = 0 });
            modelBuilder.Entity<XGCharacterAnimation>().HasData(new XGCharacterAnimation() { Id = id++, CharacterName = "Fei", Name = EActionTypes.Walk, Count = 8, StartSprite = @"FieldPCSprites1\000427_bin\0025.png", SkipBetweenFiles = 0 });
            modelBuilder.Entity<XGCharacterAnimation>().HasData(new XGCharacterAnimation() { Id = id++, CharacterName = "Fei", Name = EActionTypes.Run, Count = 8, StartSprite = @"FieldPCSprites1\000427_bin\0065.png", SkipBetweenFiles = 0 });
            modelBuilder.Entity<XGCharacterAnimation>().HasData(new XGCharacterAnimation() { Id = id++, CharacterName = "Fei", Name = EActionTypes.Jump, Count = 8, StartSprite = @"FieldPCSprites1\000427_bin\0105.png", SkipBetweenFiles = 0 });
            modelBuilder.Entity<XGCharacterAnimation>().HasData(new XGCharacterAnimation() { Id = id++, CharacterName = "Fei", Name = EActionTypes.Climb, Count = 4, StartSprite = @"FieldPCSprites1\000427_bin\0005.png", SkipBetweenFiles = 0 });
            modelBuilder.Entity<XGCharacterAnimation>().HasData(new XGCharacterAnimation() { Id = id++, CharacterName = "Fei", Name = EActionTypes.Action1, Count = 4, StartSprite = @"FieldPCSprites1\000427_bin\0145.png", SkipBetweenFiles = 7 });
            modelBuilder.Entity<XGCharacterAnimation>().HasData(new XGCharacterAnimation() { Id = id++, CharacterName = "Fei", Name = EActionTypes.Action2, Count = 3, StartSprite = @"FieldPCSprites1\000427_bin\0149.png", SkipBetweenFiles = 8 });
            modelBuilder.Entity<XGCharacterAnimation>().HasData(new XGCharacterAnimation() { Id = id++, CharacterName = "Fei", Name = EActionTypes.Action3, Count = 4, StartSprite = @"FieldPCSprites1\000427_bin\0152.png", SkipBetweenFiles = 7 });

            modelBuilder.Entity<XGCharacterAnimation>().HasData(new XGCharacterAnimation() { Id = id++, CharacterName = "Elly", Name = EActionTypes.Idle, Count = 1, StartSprite = @"FieldPCSprites2\003923_bin\0003.png", SkipBetweenFiles = 3 });
            modelBuilder.Entity<XGCharacterAnimation>().HasData(new XGCharacterAnimation() { Id = id++, CharacterName = "Elly", Name = EActionTypes.Walk, Count = 8, StartSprite = @"FieldPCSprites1\000428_bin\0055.png", SkipBetweenFiles = 0 });
            modelBuilder.Entity<XGCharacterAnimation>().HasData(new XGCharacterAnimation() { Id = id++, CharacterName = "Elly", Name = EActionTypes.Run, Count = 8, StartSprite = @"FieldPCSprites1\000428_bin\0150.png", SkipBetweenFiles = 0 });
            modelBuilder.Entity<XGCharacterAnimation>().HasData(new XGCharacterAnimation() { Id = id++, CharacterName = "Elly", Name = EActionTypes.Jump, Count = 6, StartSprite = @"FieldPCSprites1\000428_bin\0000.png", SkipBetweenFiles = 0 });
            modelBuilder.Entity<XGCharacterAnimation>().HasData(new XGCharacterAnimation() { Id = id++, CharacterName = "Elly", Name = EActionTypes.Action1, Count = 3, StartSprite = @"FieldPCSprites2\003923_bin\0000.png", SkipBetweenFiles = 1 });

            modelBuilder.Entity<XGCharacterAnimation>().HasData(new XGCharacterAnimation() { Id = id++, CharacterName = "Citan", Name = EActionTypes.Idle, Count = 1, StartSprite = @"FieldPCSprites2\003923_bin\0040.png", SkipBetweenFiles = 0 });
            modelBuilder.Entity<XGCharacterAnimation>().HasData(new XGCharacterAnimation() { Id = id++, CharacterName = "Citan", Name = EActionTypes.Walk, Count = 8, StartSprite = @"FieldPCSprites1\000429_bin\0120.png", SkipBetweenFiles = 6 });
            modelBuilder.Entity<XGCharacterAnimation>().HasData(new XGCharacterAnimation() { Id = id++, CharacterName = "Citan", Name = EActionTypes.Run, Count = 8, StartSprite = @"FieldPCSprites1\000429_bin\0060.png", SkipBetweenFiles = 0 });
            modelBuilder.Entity<XGCharacterAnimation>().HasData(new XGCharacterAnimation() { Id = id++, CharacterName = "Citan", Name = EActionTypes.Jump, Count = 6, StartSprite = @"FieldPCSprites1\000429_bin\0000.png", SkipBetweenFiles = 0 });
            modelBuilder.Entity<XGCharacterAnimation>().HasData(new XGCharacterAnimation() { Id = id++, CharacterName = "Citan", Name = EActionTypes.Climb, Count = 4, StartSprite = @"FieldPCSprites1\000429_bin\0100.png", SkipBetweenFiles = 0 });
            modelBuilder.Entity<XGCharacterAnimation>().HasData(new XGCharacterAnimation() { Id = id++, CharacterName = "Citan", Name = EActionTypes.Action1, Count = 4, StartSprite = @"FieldPCSprites1\000429_bin\0040.png", SkipBetweenFiles = 0 });
            modelBuilder.Entity<XGCharacterAnimation>().HasData(new XGCharacterAnimation() { Id = id++, CharacterName = "Citan", Name = EActionTypes.Action2, Count = 6, StartSprite = @"FieldPCSprites1\000429_bin\0128.png", SkipBetweenFiles = 8 });
        }
    }
}
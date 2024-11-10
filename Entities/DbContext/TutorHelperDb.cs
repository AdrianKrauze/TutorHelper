using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TutorHelper.Entities.ToSeed;


namespace TutorHelper.Entities.DbContext
{
    public class TutorHelperDb : IdentityDbContext<User>
    {
        public TutorHelperDb(DbContextOptions<TutorHelperDb> options)
            : base(options)
        {
        }

        public DbSet<Lesson> Lessons { get; set; }
        public DbSet<LessonPlace> LessonPlaces { get; set; }
        public DbSet<Note> Notes { get; set; }

        public DbSet<EduStage> EduStages { get; set; }

        public DbSet<Student> Students { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<StudentCondition> StudentConditions { get; set; }
        public DbSet<UserSubTaught> UserSubTaughts { get; set; }

        // Optional: Override OnModelCreating to configure entity relationships or other settings
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Student>()
                 .OwnsOne(e => e.PlaceholderCourseData);

            //User to UserSubTaught

            modelBuilder.Entity<UserSubTaught>()
            .HasKey(ust => new { ust.UserId, ust.SubjectId });

            modelBuilder.Entity<UserSubTaught>()
                .HasOne(ust => ust.User)
                .WithMany(u => u.UserSubTaughts)
                .HasForeignKey(ust => ust.UserId)
                .OnDelete(DeleteBehavior.Cascade); // Cascade delete for User

            modelBuilder.Entity<UserSubTaught>()
                .HasOne(ust => ust.Subject)
                .WithMany(s => s.UserSubTaughts)
                .HasForeignKey(ust => ust.SubjectId)
                .OnDelete(DeleteBehavior.Restrict);
            // User to Student relationship
            modelBuilder.Entity<User>()
                .HasMany(u => u.Students)
                .WithOne(s => s.CreatedBy)
                .HasForeignKey(s => s.CreatedById)
                .OnDelete(DeleteBehavior.Cascade); // Delete all students when user is deleted

            // Student to Lesson relationship
            modelBuilder.Entity<LessonWithStudent>()
            .HasOne(l => l.Student)
            .WithMany(s => s.Lessons)
           .HasForeignKey(l => l.StudentId)
               .OnDelete(DeleteBehavior.Cascade); // Delete all lessons when student is deleted

            // Student to Note relationship
            modelBuilder.Entity<Student>()
                .HasMany(s => s.Notes)
                .WithOne(n => n.Student)
                .HasForeignKey(n => n.StudentId)
                .OnDelete(DeleteBehavior.Cascade); // Delete all notes when student is deleted

            modelBuilder.Entity<User>()
                .HasMany(s => s.Notes)
                .WithOne(n => n.CreatedBy)
                .HasForeignKey(n => n.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Lesson>()
                 .HasDiscriminator<string>("LessonType")
                      .HasValue<Lesson>("BaseLesson")
        .HasValue<LessonWithStudent>("LessonWithStudent");

            // Lesson to EduStage relationship
            modelBuilder.Entity<Lesson>()
                .HasOne(l => l.EduStage)
                .WithMany(e => e.Lessons)
                .HasForeignKey(l => l.EduStageId)
                .OnDelete(DeleteBehavior.Restrict); // Restrict deletion of EduStage when related lessons exist

            // Lesson to Subject relationship
            modelBuilder.Entity<Lesson>()
                .HasOne(l => l.Subject)
                .WithMany(s => s.Lessons)
                .HasForeignKey(l => l.SubjectId)
                .OnDelete(DeleteBehavior.Restrict); // Restrict deletion of Subject when related lessons exist

            // Lesson to LessonPlace relationship
            modelBuilder.Entity<Lesson>()
                .HasOne(l => l.LessonPlace)
                .WithMany(lp => lp.Lessons)
                .HasForeignKey(l => l.LessonPlaceId)
                .OnDelete(DeleteBehavior.Restrict); // Restrict deletion of LessonPlace when related lessons exist

            // Lesson to User (CreatedBy) relationship
            modelBuilder.Entity<Lesson>()
                .HasOne(l => l.CreatedBy)
                .WithMany(u => u.Lessons)
                .HasForeignKey(l => l.CreatedById)
                .OnDelete(DeleteBehavior.Restrict); // Restrict deletion of User when related lessons exist

            // Subject to Student and Lesson relationship
            modelBuilder.Entity<Subject>()
                .HasMany(s => s.Students)
                .WithOne(st => st.Subject)
                .HasForeignKey(st => st.SubjectId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent deletion of Subject when students are associated

            modelBuilder.Entity<Subject>()
                .HasMany(s => s.Lessons)
                .WithOne(l => l.Subject)
                .HasForeignKey(l => l.SubjectId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent deletion of Subject when lessons are associated

            // LessonPlace to Student and Lesson relationship
            modelBuilder.Entity<LessonPlace>()
                .HasMany(lp => lp.Students)
                .WithOne(s => s.LessonPlace)
                .HasForeignKey(s => s.LessonPlaceId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent deletion of LessonPlace when students are associated

            modelBuilder.Entity<LessonPlace>()
                .HasMany(lp => lp.Lessons)
                .WithOne(l => l.LessonPlace)
                .HasForeignKey(l => l.LessonPlaceId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent deletion of LessonPlace when lessons are associated

            // EduStage to Student and Lesson relationship
            modelBuilder.Entity<EduStage>()
                .HasMany(e => e.Students)
                .WithOne(s => s.EduStage)
                .HasForeignKey(s => s.EduStageId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent deletion of EduStage when students are associated

            modelBuilder.Entity<EduStage>()
                .HasMany(e => e.Lessons)
                .WithOne(l => l.EduStage)
                .HasForeignKey(l => l.EduStageId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent deletion of EduStage when lessons are associated

            modelBuilder.Entity<StudentCondition>()
                .HasMany(st => st.Students)
                .WithOne(s => s.StudentCondition)
                .HasForeignKey(s => s.StudentConditionId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<StudentCondition>()
                .HasMany(st => st.Lessons)
                .WithOne(s => s.StudentCondition)
                .HasForeignKey(s => s.StudentConditionId)
                .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<LessonPlace>().HasData(
                new LessonPlace { Id = "1", Type = "U ucznia" },
                new LessonPlace { Id = "2", Type = "U korepetytora" },
                new LessonPlace { Id = "3", Type = "Skype" },
                new LessonPlace { Id = "4", Type = "Zoom" },
                new LessonPlace { Id = "5", Type = "Discord" },
                new LessonPlace { Id = "6", Type = "Google Meet" },
                new LessonPlace { Id = "7", Type = "Microsoft Teams" },
                new LessonPlace { Id = "8", Type = "Inne" }
            );

            modelBuilder.Entity<Subject>().HasData(
                new Subject { Id = "1", Topic = "Matematyka" },
                new Subject { Id = "2", Topic = "Fizyka" },
                new Subject { Id = "3", Topic = "Chemia" },
                new Subject { Id = "4", Topic = "Biologia" },
                new Subject { Id = "5", Topic = "Informatyka" },
                new Subject { Id = "6", Topic = "Geografia" },
                new Subject { Id = "7", Topic = "Historia" },
                new Subject { Id = "8", Topic = "Język polski" },
                new Subject { Id = "9", Topic = "Język angielski" },
                new Subject { Id = "10", Topic = "Język niemiecki" },
                new Subject { Id = "11", Topic = "Język rosyjski" },
                new Subject { Id = "12", Topic = "Wos" },
                new Subject { Id = "13", Topic = "Filozofia" },
                new Subject { Id = "14", Topic = "Język hiszpański" },
                new Subject { Id = "15", Topic = "Język włoski" },
                new Subject { Id = "16", Topic = "Inne" }
            );

            modelBuilder.Entity<EduStage>().HasData(
                new EduStage { Id = "1", Name = "1-3 Szkoły Podstawowej" },
                new EduStage { Id = "2", Name = "4-6 Szkoły Podstawowej" },
                new EduStage { Id = "3", Name = "7 Szkoły Podstawowej" },
                new EduStage { Id = "4", Name = "8 Szkoły Podstawowej E8" },
                new EduStage { Id = "5", Name = "Szkoła Średnia" },
                new EduStage { Id = "6", Name = "Szkoła Średnia Kl. Maturalna" },
                new EduStage { Id = "7", Name = "Inne" }
            );

            modelBuilder.Entity<StudentCondition>().HasData(
                new StudentCondition { Id = "1", Condition = "Private Active" },
                new StudentCondition { Id = "2", Condition = "Private Ended" },
                new StudentCondition { Id = "3", Condition = "Tutoring School Active" },
                new StudentCondition { Id = "4", Condition = "Tutoring School Ended" },
                new StudentCondition { Id = "5", Condition = "One Time" }
                );
        }
    }
}
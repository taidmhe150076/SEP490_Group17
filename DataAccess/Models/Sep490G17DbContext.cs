using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace DataAccess.Models;

public partial class Sep490G17DbContext : DbContext
{
    public Sep490G17DbContext()
    {
    }

    public Sep490G17DbContext(DbContextOptions<Sep490G17DbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AnswerParticipant> AnswerParticipants { get; set; }

    public virtual DbSet<AnswerQuestion> AnswerQuestions { get; set; }

    public virtual DbSet<AnswerSurvey> AnswerSurveys { get; set; }

    public virtual DbSet<Assign> Assigns { get; set; }

    public virtual DbSet<Department> Departments { get; set; }

    public virtual DbSet<Image> Images { get; set; }

    public virtual DbSet<ImageType> ImageTypes { get; set; }

    public virtual DbSet<ImagesWorkShop> ImagesWorkShops { get; set; }

    public virtual DbSet<ParticiPantScore> ParticiPantScores { get; set; }

    public virtual DbSet<Participant> Participants { get; set; }

    public virtual DbSet<ParticipantAnswer> ParticipantAnswers { get; set; }

    public virtual DbSet<Presenter> Presenters { get; set; }

    public virtual DbSet<SentimentAnswerResult> SentimentAnswerResults { get; set; }

    public virtual DbSet<StatusWorkShop> StatusWorkShops { get; set; }

    public virtual DbSet<SurveyAnswerDetail> SurveyAnswerDetails { get; set; }

    public virtual DbSet<SystemRole> SystemRoles { get; set; }

    public virtual DbSet<SystemUser> SystemUsers { get; set; }

    public virtual DbSet<Test> Tests { get; set; }

    public virtual DbSet<TestQuestion> TestQuestions { get; set; }

    public virtual DbSet<TestType> TestTypes { get; set; }

    public virtual DbSet<UrlForm> UrlForms { get; set; }

    public virtual DbSet<WorkShopSurveyQuestion> WorkShopSurveyQuestions { get; set; }

    public virtual DbSet<WorkShopSurveyQuestionTextDetail> WorkShopSurveyQuestionTextDetails { get; set; }

    public virtual DbSet<WorkShopSurveyQuestionType> WorkShopSurveyQuestionTypes { get; set; }

    public virtual DbSet<Workshop> Workshops { get; set; }

    public virtual DbSet<WorkshopQuestion> WorkshopQuestions { get; set; }

    public virtual DbSet<WorkshopSeries> WorkshopSeries { get; set; }

    public virtual DbSet<WorkshopSurveyUrl> WorkshopSurveyUrls { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var builder = new ConfigurationBuilder()
                     .SetBasePath(Directory.GetCurrentDirectory())
                     .AddJsonFile("appsettingss.json", optional: true, reloadOnChange: true);
        IConfigurationRoot configuration = builder.Build();
        optionsBuilder.UseSqlServer(configuration.GetConnectionString("AzureConnection"));
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AnswerParticipant>(entity =>
        {
            entity.HasKey(e => new { e.Id, e.QuestionId, e.TestId, e.AnswerId }).HasName("PK_AnswerParticipants_1");

            entity.Property(e => e.QuestionId).HasColumnName("Question_id");
            entity.Property(e => e.SubmissionTime)
                .HasColumnType("datetime")
                .HasColumnName("submission_time");

            entity.HasOne(d => d.Answer).WithMany(p => p.AnswerParticipants)
                .HasForeignKey(d => d.AnswerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AnswerParticipants_AnswerQuestion");

            entity.HasOne(d => d.Participant).WithMany(p => p.AnswerParticipants)
                .HasForeignKey(d => d.ParticipantId)
                .HasConstraintName("FK_AnswerParticipants_ParticipantAnswers");

            entity.HasOne(d => d.Question).WithMany(p => p.AnswerParticipants)
                .HasForeignKey(d => d.QuestionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AnswerParticipants_WorkshopQuestion");

            entity.HasOne(d => d.Test).WithMany(p => p.AnswerParticipants)
                .HasForeignKey(d => d.TestId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AnswerParticipants_Test");
        });

        modelBuilder.Entity<AnswerQuestion>(entity =>
        {
            entity.ToTable("AnswerQuestion");

            entity.Property(e => e.AnswerText)
                .HasMaxLength(255)
                .HasColumnName("Answer_text");
            entity.Property(e => e.IsCorrectAnswer).HasColumnName("Is_correct_answer");
            entity.Property(e => e.QuestionId).HasColumnName("Question_id");

            entity.HasOne(d => d.Question).WithMany(p => p.AnswerQuestions)
                .HasForeignKey(d => d.QuestionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AnswerQuestion_WorkshopQuestion");
        });

        modelBuilder.Entity<AnswerSurvey>(entity =>
        {
            entity.ToTable("AnswerSurvey");

            entity.HasOne(d => d.WorkShopSurveyQuestion).WithMany(p => p.AnswerSurveys)
                .HasForeignKey(d => d.WorkShopSurveyQuestionId)
                .HasConstraintName("FK_AnswerSurvey_WorkShopSurveyQuestion");
        });

        modelBuilder.Entity<Assign>(entity =>
        {
            entity.HasKey(e => new { e.Id, e.WorkshopSeriesId, e.UserSystemId });

            entity.ToTable("Assign");

            entity.Property(e => e.Id).ValueGeneratedOnAdd();

            entity.HasOne(d => d.UserSystem).WithMany(p => p.Assigns)
                .HasForeignKey(d => d.UserSystemId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Assign_SystemUser");

            entity.HasOne(d => d.WorkshopSeries).WithMany(p => p.Assigns)
                .HasForeignKey(d => d.WorkshopSeriesId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Assign_WorkshopSeries");
        });

        modelBuilder.Entity<Department>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Departme__3213E83F5A2C4CA4");

            entity.ToTable("Department");

            entity.Property(e => e.DepartmentName)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Image>(entity =>
        {
            entity.ToTable("Image");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Image1).HasColumnName("Image");
        });

        modelBuilder.Entity<ImageType>(entity =>
        {
            entity.ToTable("ImageType");

            entity.Property(e => e.ImageType1)
                .HasMaxLength(50)
                .HasColumnName("ImageType");
        });

        modelBuilder.Entity<ImagesWorkShop>(entity =>
        {
            entity.HasKey(e => new { e.Id, e.ImageId, e.WorkshopId, e.ImagesTypeId }).HasName("PK_SlideWorkShop");

            entity.ToTable("ImagesWorkShop");

            entity.Property(e => e.Id).ValueGeneratedOnAdd();

            entity.HasOne(d => d.Image).WithMany(p => p.ImagesWorkShops)
                .HasForeignKey(d => d.ImageId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SlideWorkShop_Image");

            entity.HasOne(d => d.ImagesType).WithMany(p => p.ImagesWorkShops)
                .HasForeignKey(d => d.ImagesTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ImagesWorkShop_ImageType");

            entity.HasOne(d => d.Workshop).WithMany(p => p.ImagesWorkShops)
                .HasForeignKey(d => d.WorkshopId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SlideWorkShop_Workshop");
        });

        modelBuilder.Entity<ParticiPantScore>(entity =>
        {
            entity.HasKey(e => new { e.TestId, e.ParticipantId });

            entity.Property(e => e.SubmissionTime)
                .HasColumnType("datetime")
                .HasColumnName("submission_time");

            entity.HasOne(d => d.Participant).WithMany(p => p.ParticiPantScores)
                .HasForeignKey(d => d.ParticipantId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ParticiPantScores_ParticipantAnswers");

            entity.HasOne(d => d.Test).WithMany(p => p.ParticiPantScores)
                .HasForeignKey(d => d.TestId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ParticiPantScores_Test");
        });

        modelBuilder.Entity<Participant>(entity =>
        {
            entity.Property(e => e.Email).HasMaxLength(50);
            entity.Property(e => e.FavoriteTopics).HasMaxLength(255);
            entity.Property(e => e.FullName).HasMaxLength(50);
            entity.Property(e => e.Major).HasMaxLength(50);
            entity.Property(e => e.TimeStamp).HasColumnType("datetime");

            entity.HasOne(d => d.WorkshopSeries).WithMany(p => p.Participants)
                .HasForeignKey(d => d.WorkshopSeriesId)
                .HasConstraintName("FK_Participants_WorkshopSeries");
        });

        modelBuilder.Entity<ParticipantAnswer>(entity =>
        {
            entity.Property(e => e.ParticipantsEmail).HasMaxLength(50);
        });

        modelBuilder.Entity<Presenter>(entity =>
        {
            entity.ToTable("Presenter");

            entity.Property(e => e.PresenterEmail).HasMaxLength(50);
        });

        modelBuilder.Entity<SentimentAnswerResult>(entity =>
        {
            entity.ToTable("SentimentAnswerResult");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Question)
                .HasMaxLength(200)
                .HasColumnName("question");
            entity.Property(e => e.SentimentAnswer)
                .HasMaxLength(200)
                .HasColumnName("sentiment answer");
            entity.Property(e => e.SurveyId).HasColumnName("surveyId");

            entity.HasOne(d => d.Survey).WithMany(p => p.SentimentAnswerResults)
                .HasForeignKey(d => d.SurveyId)
                .HasConstraintName("FK_SentimentAnswerResult_WorkshopSurveyUrl");
        });

        modelBuilder.Entity<StatusWorkShop>(entity =>
        {
            entity.HasKey(e => e.StatusId);

            entity.ToTable("StatusWorkShop");

            entity.Property(e => e.Status).HasMaxLength(50);
        });

        modelBuilder.Entity<SurveyAnswerDetail>(entity =>
        {
            entity.HasKey(e => new { e.Id, e.WorkShopSurveyQuestionId, e.AnswerSurveyId });

            entity.ToTable("SurveyAnswerDetail");

            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.AnswerText).HasColumnType("text");
            entity.Property(e => e.DateTime).HasColumnType("datetime");
            entity.Property(e => e.ParticipantsEmail).HasMaxLength(50);

            entity.HasOne(d => d.AnswerSurvey).WithMany(p => p.SurveyAnswerDetails)
                .HasForeignKey(d => d.AnswerSurveyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SurveyAnswerDetail_AnswerSurvey");

            entity.HasOne(d => d.WorkShopSurveyQuestion).WithMany(p => p.SurveyAnswerDetails)
                .HasForeignKey(d => d.WorkShopSurveyQuestionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SurveyAnswerDetail_WorkShopSurveyQuestion");
        });

        modelBuilder.Entity<SystemRole>(entity =>
        {
            entity.ToTable("SystemRole");

            entity.Property(e => e.RoleName).HasMaxLength(50);
        });

        modelBuilder.Entity<SystemUser>(entity =>
        {
            entity.ToTable("SystemUser");

            entity.Property(e => e.Dob).HasColumnName("DOB");
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.FirstName).HasMaxLength(250);
            entity.Property(e => e.LastName).HasMaxLength(250);
            entity.Property(e => e.Password).HasMaxLength(255);

            entity.HasOne(d => d.DepartmentldNavigation).WithMany(p => p.SystemUsers)
                .HasForeignKey(d => d.Departmentld)
                .HasConstraintName("FK_SystemUser_Department");

            entity.HasOne(d => d.RoleldNavigation).WithMany(p => p.SystemUsers)
                .HasForeignKey(d => d.Roleld)
                .HasConstraintName("FK_SystemUser_SystemRole");
        });

        modelBuilder.Entity<Test>(entity =>
        {
            entity.ToTable("Test");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.DateStart).HasColumnType("datetime");
            entity.Property(e => e.ExpiredTime).HasColumnType("datetime");
            entity.Property(e => e.TestName).HasMaxLength(50);

            entity.HasOne(d => d.TestType).WithMany(p => p.Tests)
                .HasForeignKey(d => d.TestTypeId)
                .HasConstraintName("FK_Test_TestType");

            entity.HasOne(d => d.Workshop).WithMany(p => p.Tests)
                .HasForeignKey(d => d.WorkshopId)
                .HasConstraintName("FK_Test_Workshop");
        });

        modelBuilder.Entity<TestQuestion>(entity =>
        {
            entity.HasKey(e => new { e.Id, e.QuestionId, e.TestId });

            entity.ToTable("TestQuestion");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.QuestionId).HasColumnName("Question_id");
            entity.Property(e => e.Date).HasColumnType("datetime");

            entity.HasOne(d => d.Question).WithMany(p => p.TestQuestions)
                .HasForeignKey(d => d.QuestionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TestQuestion_WorkshopQuestion");

            entity.HasOne(d => d.Test).WithMany(p => p.TestQuestions)
                .HasForeignKey(d => d.TestId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TestQuestion_Test");
        });

        modelBuilder.Entity<TestType>(entity =>
        {
            entity.ToTable("TestType");

            entity.Property(e => e.TypeName).HasMaxLength(50);
        });

        modelBuilder.Entity<UrlForm>(entity =>
        {
            entity.HasKey(e => new { e.Id, e.WorkshopSurveyUrl, e.WorkshopId });

            entity.ToTable("UrlForm");

            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.UrlForm1)
                .HasMaxLength(1000)
                .HasColumnName("UrlForm");

            entity.HasOne(d => d.Workshop).WithMany(p => p.UrlForms)
                .HasForeignKey(d => d.WorkshopId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UrlForm_Workshop");

            entity.HasOne(d => d.WorkshopSurveyUrlNavigation).WithMany(p => p.UrlForms)
                .HasForeignKey(d => d.WorkshopSurveyUrl)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UrlForm_WorkshopSurveyUrl");
        });

        modelBuilder.Entity<WorkShopSurveyQuestion>(entity =>
        {
            entity.ToTable("WorkShopSurveyQuestion");

            entity.Property(e => e.QuestionText).HasColumnType("text");

            entity.HasOne(d => d.QuestionType).WithMany(p => p.WorkShopSurveyQuestions)
                .HasForeignKey(d => d.QuestionTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_WorkShopSurveyQuestion_WorkShopSurveyQuestionType");

            entity.HasOne(d => d.WorkShop).WithMany(p => p.WorkShopSurveyQuestions)
                .HasForeignKey(d => d.WorkShopId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_WorkShopSurveyQuestion_Workshop");
        });

        modelBuilder.Entity<WorkShopSurveyQuestionTextDetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_WorkShopSurveyQuestionText_Detail_1");

            entity.ToTable("WorkShopSurveyQuestionText_Detail");

            entity.Property(e => e.AnswerText).HasColumnType("text");
            entity.Property(e => e.ParticipantsEmail).HasMaxLength(50);

            entity.HasOne(d => d.Question).WithMany(p => p.WorkShopSurveyQuestionTextDetails)
                .HasForeignKey(d => d.QuestionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_WorkShopSurveyQuestionText_Detail_WorkShopSurveyQuestion");
        });

        modelBuilder.Entity<WorkShopSurveyQuestionType>(entity =>
        {
            entity.ToTable("WorkShopSurveyQuestionType");

            entity.Property(e => e.TypeName).HasMaxLength(50);
        });

        modelBuilder.Entity<Workshop>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Workshop__3213E83F167348E1");

            entity.ToTable("Workshop");

            entity.Property(e => e.DatePresent).HasColumnType("datetime");
            entity.Property(e => e.KeyPresenter).HasMaxLength(50);
            entity.Property(e => e.WorkshopName)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.Presenter).WithMany(p => p.Workshops)
                .HasForeignKey(d => d.PresenterId)
                .HasConstraintName("FK_Workshop_Presenter");

            entity.HasOne(d => d.Status).WithMany(p => p.Workshops)
                .HasForeignKey(d => d.StatusId)
                .HasConstraintName("FK_Workshop_StatusWorkShop");

            entity.HasOne(d => d.WorkshopSeries).WithMany(p => p.Workshops)
                .HasForeignKey(d => d.WorkshopSeriesId)
                .HasConstraintName("FK_Workshop_WorkshopSeries");
        });

        modelBuilder.Entity<WorkshopQuestion>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Workshop__3213E83FF3408217");

            entity.ToTable("WorkshopQuestion");

            entity.HasOne(d => d.Workshop).WithMany(p => p.WorkshopQuestions)
                .HasForeignKey(d => d.WorkshopId)
                .HasConstraintName("FK_WorkshopQuestion_Workshop");
        });

        modelBuilder.Entity<WorkshopSeries>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Workshop__3213E83FAFF7FF5C");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.EndDate).HasColumnType("datetime");
            entity.Property(e => e.StartDate).HasColumnType("datetime");
            entity.Property(e => e.WorkshopSeriesName)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.Department).WithMany(p => p.WorkshopSeries)
                .HasForeignKey(d => d.DepartmentId)
                .HasConstraintName("FK_WorkshopSeries_Department");
        });

        modelBuilder.Entity<WorkshopSurveyUrl>(entity =>
        {
            entity.ToTable("WorkshopSurveyUrl");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.AddedDate).HasColumnType("datetime");
            entity.Property(e => e.FileByte).HasColumnName("fileByte");
            entity.Property(e => e.FileType)
                .HasMaxLength(10)
                .HasColumnName("fileType");
            entity.Property(e => e.SurveyName).HasMaxLength(255);
            entity.Property(e => e.Url)
                .HasMaxLength(1000)
                .HasColumnName("url");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

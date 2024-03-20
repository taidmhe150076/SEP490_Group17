﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

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

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<AnswerQuestion> AnswerQuestions { get; set; }

    public virtual DbSet<AnswerSurvey> AnswerSurveys { get; set; }

    public virtual DbSet<Department> Departments { get; set; }

    public virtual DbSet<Participant> Participants { get; set; }

    public virtual DbSet<QuestionParticipantsDetail> QuestionParticipantsDetails { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<StatusWorkShop> StatusWorkShops { get; set; }

    public virtual DbSet<SurveyAnswerDetail> SurveyAnswerDetails { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<WorkShopSurveyQuestion> WorkShopSurveyQuestions { get; set; }

    public virtual DbSet<WorkShopSurveyQuestionTextDetail> WorkShopSurveyQuestionTextDetails { get; set; }

    public virtual DbSet<WorkShopSurveyQuestionType> WorkShopSurveyQuestionTypes { get; set; }

    public virtual DbSet<Workshop> Workshops { get; set; }

    public virtual DbSet<WorkshopQuestion> WorkshopQuestions { get; set; }

    public virtual DbSet<WorkshopSeries> WorkshopSeries { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)

        => optionsBuilder.UseSqlServer("Server=DESKTOP-LJLCRRI;database=SEP490_G17_DB;uid=sa;pwd=123;TrustServerCertificate=true;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Account__3213E83FB5143254");

            entity.ToTable("Account");

            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.UserName)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.ValidDate).HasColumnType("datetime");
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

        modelBuilder.Entity<Department>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Departme__3213E83F5A2C4CA4");

            entity.ToTable("Department");

            entity.Property(e => e.DepartmentName)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Participant>(entity =>
        {
            entity.Property(e => e.Email).HasMaxLength(50);
            entity.Property(e => e.FullName).HasMaxLength(50);

            entity.HasOne(d => d.Workshop).WithMany(p => p.Participants)
                .HasForeignKey(d => d.WorkshopId)
                .HasConstraintName("FK_Participants_Workshop");
        });

        modelBuilder.Entity<QuestionParticipantsDetail>(entity =>
        {
            entity.HasKey(e => new { e.Id, e.QuestionId, e.AnswerId }).HasName("PK_QuestionParticipants_Details_1");

            entity.ToTable("QuestionParticipants_Details");

            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.Date).HasColumnType("datetime");
            entity.Property(e => e.ParticipantsEmail).HasMaxLength(50);

            entity.HasOne(d => d.Answer).WithMany(p => p.QuestionParticipantsDetails)
                .HasForeignKey(d => d.AnswerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_QuestionParticipants_Details_AnswerQuestion");

            entity.HasOne(d => d.Question).WithMany(p => p.QuestionParticipantsDetails)
                .HasForeignKey(d => d.QuestionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_QuestionParticipants_Details_WorkshopQuestion");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Role__3213E83FF6920F19");

            entity.ToTable("Role");

            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .IsUnicode(false);
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

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__User__3213E83FC9AE2326");

            entity.ToTable("User");

            entity.HasIndex(e => e.AccountId, "UQ__User__F267253F560C4AD2").IsUnique();

            entity.Property(e => e.AccountId).HasColumnName("AccountID");
            entity.Property(e => e.Address)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.DepartmentId).HasColumnName("DepartmentID");
            entity.Property(e => e.Dob)
                .HasColumnType("datetime")
                .HasColumnName("DOB");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.Account).WithOne(p => p.User)
                .HasForeignKey<User>(d => d.AccountId)
                .HasConstraintName("FK_User_Account");

            entity.HasOne(d => d.Department).WithMany(p => p.Users)
                .HasForeignKey(d => d.DepartmentId)
                .HasConstraintName("FK_User_Department");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("FK_User_Role");
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
            entity.Property(e => e.WorkshopName)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.Presenter).WithMany(p => p.Workshops)
                .HasForeignKey(d => d.PresenterId)
                .HasConstraintName("FK_Workshop_User");

            entity.HasOne(d => d.Status).WithMany(p => p.Workshops)
                .HasForeignKey(d => d.StatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Workshop_StatusWorkShop");

            entity.HasOne(d => d.WorkshopSeries).WithMany(p => p.Workshops)
                .HasForeignKey(d => d.WorkshopSeriesId)
                .HasConstraintName("FK_Workshop_WorkshopSeries");
        });

        modelBuilder.Entity<WorkshopQuestion>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Workshop__3213E83FF3408217");

            entity.ToTable("WorkshopQuestion");

            entity.Property(e => e.QuestionText)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.Workshop).WithMany(p => p.WorkshopQuestions)
                .HasForeignKey(d => d.WorkshopId)
                .HasConstraintName("FK_WorkshopQuestion_Workshop");
        });

        modelBuilder.Entity<WorkshopSeries>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Workshop__3213E83FAFF7FF5C");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.EndDate).HasColumnType("datetime");
            entity.Property(e => e.StartDate).HasColumnType("datetime");
            entity.Property(e => e.WorkshopSeriesName)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.Department).WithMany(p => p.WorkshopSeries)
                .HasForeignKey(d => d.DepartmentId)
                .HasConstraintName("FK_WorkshopSeries_Department");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

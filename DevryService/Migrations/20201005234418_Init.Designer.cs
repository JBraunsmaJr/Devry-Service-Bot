﻿// <auto-generated />
using System;
using DevryService;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DevryService.Migrations
{
    [DbContext(typeof(DevryDbContext))]
    [Migration("20201005234418_Init")]
    partial class Init
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.8");

            modelBuilder.Entity("DevryService.Models.MemberStats", b =>
                {
                    b.Property<ulong>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<double>("Points")
                        .HasColumnType("REAL");

                    b.HasKey("Id");

                    b.ToTable("Stats");
                });

            modelBuilder.Entity("DevryService.Models.Questionaire", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("TEXT");

                    b.Property<string>("AnswerDescription")
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .HasColumnType("TEXT");

                    b.Property<double>("Duration")
                        .HasColumnType("REAL");

                    b.Property<string>("JSON")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("LastUsed")
                        .HasColumnType("TEXT");

                    b.Property<string>("Title")
                        .HasColumnType("TEXT");

                    b.Property<int>("Type")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("Questions");

                    b.HasData(
                        new
                        {
                            Id = "1c682017-e13d-4a10-9585-7df8a400f899",
                            AnswerDescription = @"The 3 types are:
`Syntax`:	Not following the rules of the language (how it's written). This prevents you from compiling/running your program

`Runtime`: 	 While running your program an error occurred. These can be mitigated via try/except try/catch (depending on language)

`Logical`: 	 A simple programming mistake. Perhaps using the wrong variable, or wrong mathmatical operator
",
                            Description = "What are the 3 types of errors which can occur during the execution of a programming?",
                            Duration = 5.0,
                            JSON = "{\"CorrectPhrasesCSV\":\"syntax, runtime, logical\",\"AllowPartialCredit\":true,\"PointsForAnswer\":3,\"CorrectPhrases\":[\"syntax\",\" runtime\",\" logical\"]}",
                            Title = "Errors",
                            Type = 1
                        },
                        new
                        {
                            Id = "ec4b87a3-7128-49e3-8375-86dda95d98e6",
                            AnswerDescription = "While running your program an uncaught exception occurred. These can be mitigated via `try/except`, `try/catch` (depending on language)",
                            Description = "When does a runtime error occur?",
                            Duration = 5.0,
                            JSON = "[{\"Text\":\"While the program is running, whenever an un-caught exception is thrown/raised\",\"IsCorrect\":true,\"PointsForAnswer\":1},{\"Text\":\"During compilation\",\"IsCorrect\":false,\"PointsForAnswer\":0},{\"Text\":\"Neither, it's just a bug/mistake in the code.\",\"IsCorrect\":false,\"PointsForAnswer\":0}]",
                            Title = "Errors",
                            Type = 0
                        },
                        new
                        {
                            Id = "b49cfe0c-dfb4-4b35-a675-8518d8019bf7",
                            AnswerDescription = @"`For`: 	 execute something for a finite amount of times

`While`: 	 so long as a condition is true... execute the body of the loop

`Do While`: 	 similar to while, except it runs the body first, THEN checks the condition

`Foreach`: 	 similar to `for`, except it's a short-hand way of iterating over a collection of items",
                            Description = "What are the different kind of `loop` structures in programming?",
                            Duration = 5.0,
                            JSON = "{\"CorrectPhrasesCSV\":\"for,while,do-while,foreach\",\"AllowPartialCredit\":true,\"PointsForAnswer\":8,\"CorrectPhrases\":[\"for\",\"while\",\"do-while\",\"foreach\"]}",
                            Title = "Structures",
                            Type = 1
                        },
                        new
                        {
                            Id = "45f2f0d7-c692-4a4e-ac75-b0d20e3a422b",
                            AnswerDescription = @"`If`: 	 determine if a condition is true, then run the body

`else if` / `elif`: 	 When the if statement (or elif / else if) above is NOT true, if the condition here is true...execute the body

`else`: 	 when the above statement(s) are not true... this shall get run",
                            Description = "What are the different types of `if` statements?",
                            Duration = 5.0,
                            JSON = "{\"CorrectPhrasesCSV\":\"if,else,else if,elif\",\"AllowPartialCredit\":true,\"PointsForAnswer\":8,\"CorrectPhrases\":[\"if\",\"else\",\"else if\",\"elif\"]}",
                            Title = "Structures",
                            Type = 1
                        },
                        new
                        {
                            Id = "2eefcf52-0014-4396-9d43-f050ea1c0341",
                            AnswerDescription = "This is an Object-Oriented Programming (OOP) concept. When you create (gaming folks, \"spawn\") an object (a class) it's known as an `instance`. `Public` members (variables/methods) allow you to `access` them outside the scope of the class.",
                            Description = "In Object-Oriented Programming (OOP) what does the keyword `public` mean?",
                            Duration = 5.0,
                            JSON = "[{\"Text\":\"The method/variable will be externally accessible\",\"IsCorrect\":true,\"PointsForAnswer\":2},{\"Text\":\"Makes a variable accessible to other methods in the class\",\"IsCorrect\":false,\"PointsForAnswer\":1},{\"Text\":\"Only children, and \\\"myself\\\" can access it\",\"IsCorrect\":false,\"PointsForAnswer\":0}]",
                            Title = "Access Types",
                            Type = 0
                        },
                        new
                        {
                            Id = "0557638c-cc1e-4c21-9266-bec28f81292c",
                            AnswerDescription = "This is an Object-Oriented Programm (OOP) concept. Protected means \"only myself, and my children can access this\"",
                            Description = "In Object-Oriented Programming (OOP) what does the keyword `protected` mean?",
                            Duration = 5.0,
                            JSON = "[{\"Text\":\"Only \\\"I\\\" can access\",\"IsCorrect\":false,\"PointsForAnswer\":1},{\"Text\":\"\\\"Anyone\\\" can access\",\"IsCorrect\":false,\"PointsForAnswer\":0},{\"Text\":\"Only \\\"myself and my children\\\" can access\\\"\",\"IsCorrect\":true,\"PointsForAnswer\":2}]",
                            Title = "Access Types",
                            Type = 0
                        },
                        new
                        {
                            Id = "5499aa3d-61a3-4766-98c1-7ba3aa82a11d",
                            AnswerDescription = "In Object-Oriented Programming (OOP) `private` means only \"I\" can access. Meaning, the scope is ONLY within that instance",
                            Description = "In Object-Oriented Programming (OOP) what does the keyword `private` mean?",
                            Duration = 5.0,
                            JSON = "[{\"Text\":\"Only \\\"I\\\" can access\",\"IsCorrect\":true,\"PointsForAnswer\":2},{\"Text\":\"Only \\\"myself and my children\\\" can access\",\"IsCorrect\":false,\"PointsForAnswer\":0},{\"Text\":\"\\\"Anyone\\\" can access\",\"IsCorrect\":false,\"PointsForAnswer\":0}]",
                            Title = "Access Types",
                            Type = 0
                        });
                });

            modelBuilder.Entity("DevryService.Models.Reminder", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("TEXT");

                    b.Property<ulong>("ChannelId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Contents")
                        .HasColumnType("TEXT");

                    b.Property<ulong>("GuildId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("NextRunTime")
                        .HasColumnType("TEXT");

                    b.Property<string>("Schedule")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Reminders");
                });
#pragma warning restore 612, 618
        }
    }
}

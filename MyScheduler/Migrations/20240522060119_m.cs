using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyGP2webapp.Migrations
{
    /// <inheritdoc />
    public partial class m : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Courses",
                columns: table => new
                {
                    IDCRS = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CRS_NO = table.Column<int>(type: "int", maxLength: 7, nullable: false),
                    CRS_CR_HOURS = table.Column<int>(type: "int", maxLength: 2, nullable: false),
                    CRS_A_NAME = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    CRS_SPEC = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: false),
                    CRS_ACTIVE = table.Column<int>(type: "int", maxLength: 1, nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Courses", x => x.IDCRS);
                });

            migrationBuilder.CreateTable(
                name: "degreeProgressPlans",
                columns: table => new
                {
                    IDDegreeProgressPlan = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    College = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Major = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_degreeProgressPlans", x => x.IDDegreeProgressPlan);
                });

            migrationBuilder.CreateTable(
                name: "instractors",
                columns: table => new
                {
                    IdInstructor = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Job_ID = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_instractors", x => x.IdInstructor);
                });

            migrationBuilder.CreateTable(
                name: "studyPlan",
                columns: table => new
                {
                    IdStudyPlan = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    College = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Major = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_studyPlan", x => x.IdStudyPlan);
                });

            migrationBuilder.CreateTable(
                name: "degree_Contents",
                columns: table => new
                {
                    IdDC = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IDCRS = table.Column<int>(type: "int", nullable: false),
                    IDDegreeProgressPlan = table.Column<int>(type: "int", nullable: false),
                    SPEC_CODE = table.Column<int>(type: "int", maxLength: 4, nullable: false),
                    SMST_NO = table.Column<int>(type: "int", maxLength: 1, nullable: false),
                    SPEC_YYT = table.Column<int>(type: "int", maxLength: 5, nullable: false),
                    SPEC_LVL = table.Column<int>(type: "int", maxLength: 1, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_degree_Contents", x => x.IdDC);
                    table.ForeignKey(
                        name: "FK_degree_Contents_Courses_IDCRS",
                        column: x => x.IDCRS,
                        principalTable: "Courses",
                        principalColumn: "IDCRS",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_degree_Contents_degreeProgressPlans_IDDegreeProgressPlan",
                        column: x => x.IDDegreeProgressPlan,
                        principalTable: "degreeProgressPlans",
                        principalColumn: "IDDegreeProgressPlan",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "sections",
                columns: table => new
                {
                    IDSection = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SectionNumber = table.Column<int>(type: "int", nullable: false),
                    Hall = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Start_Sunday = table.Column<DateTime>(type: "datetime2", nullable: true),
                    End_Sunday = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Start_Monday = table.Column<DateTime>(type: "datetime2", nullable: true),
                    End_Monday = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Start_Tuesday = table.Column<DateTime>(type: "datetime2", nullable: true),
                    End_Tuesday = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Start_Wednesday = table.Column<DateTime>(type: "datetime2", nullable: true),
                    End_Wednesday = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Start_Thursday = table.Column<DateTime>(type: "datetime2", nullable: true),
                    End_Thursday = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    courseIDCRS = table.Column<int>(type: "int", nullable: false),
                    InstructorsIdInstructor = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sections", x => x.IDSection);
                    table.ForeignKey(
                        name: "FK_sections_Courses_courseIDCRS",
                        column: x => x.courseIDCRS,
                        principalTable: "Courses",
                        principalColumn: "IDCRS",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_sections_instractors_InstructorsIdInstructor",
                        column: x => x.InstructorsIdInstructor,
                        principalTable: "instractors",
                        principalColumn: "IdInstructor",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "plan_Content",
                columns: table => new
                {
                    IdPlanContent = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IDCRS = table.Column<int>(type: "int", nullable: false),
                    IdStudyPlan = table.Column<int>(type: "int", nullable: false),
                    code = table.Column<int>(type: "int", nullable: false),
                    prerequisite = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_plan_Content", x => x.IdPlanContent);
                    table.ForeignKey(
                        name: "FK_plan_Content_Courses_IDCRS",
                        column: x => x.IDCRS,
                        principalTable: "Courses",
                        principalColumn: "IDCRS",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_plan_Content_studyPlan_IdStudyPlan",
                        column: x => x.IdStudyPlan,
                        principalTable: "studyPlan",
                        principalColumn: "IdStudyPlan",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Students",
                columns: table => new
                {
                    KeyStudent = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ID_Student = table.Column<int>(type: "int", nullable: false),
                    password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    studyPlanIdStudyPlan = table.Column<int>(type: "int", nullable: false),
                    degreeProgressPlanIDDegreeProgressPlan = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Students", x => x.KeyStudent);
                    table.ForeignKey(
                        name: "FK_Students_degreeProgressPlans_degreeProgressPlanIDDegreeProgressPlan",
                        column: x => x.degreeProgressPlanIDDegreeProgressPlan,
                        principalTable: "degreeProgressPlans",
                        principalColumn: "IDDegreeProgressPlan",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Students_studyPlan_studyPlanIdStudyPlan",
                        column: x => x.studyPlanIdStudyPlan,
                        principalTable: "studyPlan",
                        principalColumn: "IdStudyPlan",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "progresses",
                columns: table => new
                {
                    IdProgress = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Mark = table.Column<float>(type: "real", nullable: false),
                    StudentKeyStudent = table.Column<int>(type: "int", nullable: false),
                    courseIDCRS = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_progresses", x => x.IdProgress);
                    table.ForeignKey(
                        name: "FK_progresses_Courses_courseIDCRS",
                        column: x => x.courseIDCRS,
                        principalTable: "Courses",
                        principalColumn: "IDCRS",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_progresses_Students_StudentKeyStudent",
                        column: x => x.StudentKeyStudent,
                        principalTable: "Students",
                        principalColumn: "KeyStudent",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "schedules",
                columns: table => new
                {
                    IDScedule = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    studentsKeyStudent = table.Column<int>(type: "int", nullable: false),
                    Approv_Schedule = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_schedules", x => x.IDScedule);
                    table.ForeignKey(
                        name: "FK_schedules_Students_studentsKeyStudent",
                        column: x => x.studentsKeyStudent,
                        principalTable: "Students",
                        principalColumn: "KeyStudent",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "sectionSchedules",
                columns: table => new
                {
                    IDSS = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IDSection = table.Column<int>(type: "int", nullable: false),
                    IDScedule = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sectionSchedules", x => x.IDSS);
                    table.ForeignKey(
                        name: "FK_sectionSchedules_schedules_IDScedule",
                        column: x => x.IDScedule,
                        principalTable: "schedules",
                        principalColumn: "IDScedule",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_sectionSchedules_sections_IDSection",
                        column: x => x.IDSection,
                        principalTable: "sections",
                        principalColumn: "IDSection",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_degree_Contents_IDCRS",
                table: "degree_Contents",
                column: "IDCRS");

            migrationBuilder.CreateIndex(
                name: "IX_degree_Contents_IDDegreeProgressPlan",
                table: "degree_Contents",
                column: "IDDegreeProgressPlan");

            migrationBuilder.CreateIndex(
                name: "IX_plan_Content_IDCRS",
                table: "plan_Content",
                column: "IDCRS");

            migrationBuilder.CreateIndex(
                name: "IX_plan_Content_IdStudyPlan",
                table: "plan_Content",
                column: "IdStudyPlan");

            migrationBuilder.CreateIndex(
                name: "IX_progresses_courseIDCRS",
                table: "progresses",
                column: "courseIDCRS");

            migrationBuilder.CreateIndex(
                name: "IX_progresses_StudentKeyStudent",
                table: "progresses",
                column: "StudentKeyStudent");

            migrationBuilder.CreateIndex(
                name: "IX_schedules_studentsKeyStudent",
                table: "schedules",
                column: "studentsKeyStudent");

            migrationBuilder.CreateIndex(
                name: "IX_sections_courseIDCRS",
                table: "sections",
                column: "courseIDCRS");

            migrationBuilder.CreateIndex(
                name: "IX_sections_InstructorsIdInstructor",
                table: "sections",
                column: "InstructorsIdInstructor");

            migrationBuilder.CreateIndex(
                name: "IX_sectionSchedules_IDScedule",
                table: "sectionSchedules",
                column: "IDScedule");

            migrationBuilder.CreateIndex(
                name: "IX_sectionSchedules_IDSection",
                table: "sectionSchedules",
                column: "IDSection");

            migrationBuilder.CreateIndex(
                name: "IX_Students_degreeProgressPlanIDDegreeProgressPlan",
                table: "Students",
                column: "degreeProgressPlanIDDegreeProgressPlan");

            migrationBuilder.CreateIndex(
                name: "IX_Students_studyPlanIdStudyPlan",
                table: "Students",
                column: "studyPlanIdStudyPlan");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "degree_Contents");

            migrationBuilder.DropTable(
                name: "plan_Content");

            migrationBuilder.DropTable(
                name: "progresses");

            migrationBuilder.DropTable(
                name: "sectionSchedules");

            migrationBuilder.DropTable(
                name: "schedules");

            migrationBuilder.DropTable(
                name: "sections");

            migrationBuilder.DropTable(
                name: "Students");

            migrationBuilder.DropTable(
                name: "Courses");

            migrationBuilder.DropTable(
                name: "instractors");

            migrationBuilder.DropTable(
                name: "degreeProgressPlans");

            migrationBuilder.DropTable(
                name: "studyPlan");
        }
    }
}

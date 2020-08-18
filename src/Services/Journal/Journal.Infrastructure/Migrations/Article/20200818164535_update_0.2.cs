using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Journal.Infrastructure.Migrations.Article
{
    public partial class update_02 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Affiliation",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Affiliation", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Author",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    OrcidId = table.Column<string>(nullable: true),
                    OrcidLink = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Author", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Discipline",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Discipline", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SubjectCode",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    SubjectGroupCode = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubjectCode", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SubjectGroup",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    DisciplineName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubjectGroup", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AuthorAffiliation",
                columns: table => new
                {
                    AuthorId = table.Column<int>(nullable: false),
                    AffiliationId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthorAffiliation", x => new { x.AuthorId, x.AffiliationId });
                    table.ForeignKey(
                        name: "FK_AuthorAffiliation_Affiliation_AffiliationId",
                        column: x => x.AffiliationId,
                        principalTable: "Affiliation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AuthorAffiliation_Author_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "Author",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Article",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VersionedArxivId = table.Column<string>(nullable: true),
                    PublishedDate = table.Column<DateTime>(nullable: false),
                    UpdatedDate = table.Column<DateTime>(nullable: false),
                    Title = table.Column<string>(nullable: true),
                    Summary = table.Column<string>(nullable: true),
                    Comment = table.Column<string>(nullable: true),
                    JournalReference = table.Column<string>(nullable: true),
                    Doi = table.Column<string>(nullable: true),
                    DoiLinks = table.Column<string>(nullable: true),
                    PrimarySubjectCodeId = table.Column<int>(nullable: true),
                    MscCategory = table.Column<string>(nullable: true),
                    AcmCategory = table.Column<string>(nullable: true),
                    PdfLink = table.Column<string>(nullable: true),
                    ArxivId = table.Column<string>(nullable: true),
                    PrimarySubjectGroupCodeId = table.Column<int>(nullable: true),
                    PrmiaryDisciplineId = table.Column<int>(nullable: true),
                    UpdatedDay = table.Column<int>(nullable: false),
                    UpdatedMonth = table.Column<int>(nullable: false),
                    UpdatedYear = table.Column<int>(nullable: false),
                    VersionNumber = table.Column<int>(nullable: false),
                    IsLatestVersion = table.Column<bool>(nullable: false),
                    IsWithdrawn = table.Column<bool>(nullable: false),
                    JournalProcessedId = table.Column<int>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    LastModified = table.Column<DateTime>(nullable: false),
                    HasProcessingError = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Article", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Article_SubjectCode_PrimarySubjectCodeId",
                        column: x => x.PrimarySubjectCodeId,
                        principalTable: "SubjectCode",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Article_SubjectGroup_PrimarySubjectGroupCodeId",
                        column: x => x.PrimarySubjectGroupCodeId,
                        principalTable: "SubjectGroup",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Article_Discipline_PrmiaryDisciplineId",
                        column: x => x.PrmiaryDisciplineId,
                        principalTable: "Discipline",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Category",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    SubjectCodeId = table.Column<int>(nullable: true),
                    SubjectGroupId = table.Column<int>(nullable: true),
                    DisciplineId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Category", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Category_Discipline_DisciplineId",
                        column: x => x.DisciplineId,
                        principalTable: "Discipline",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Category_SubjectCode_SubjectCodeId",
                        column: x => x.SubjectCodeId,
                        principalTable: "SubjectCode",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Category_SubjectGroup_SubjectGroupId",
                        column: x => x.SubjectGroupId,
                        principalTable: "SubjectGroup",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AuthorArticle",
                columns: table => new
                {
                    AuthorId = table.Column<int>(nullable: false),
                    ArticleId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthorArticle", x => new { x.AuthorId, x.ArticleId });
                    table.ForeignKey(
                        name: "FK_AuthorArticle_Article_ArticleId",
                        column: x => x.ArticleId,
                        principalTable: "Article",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AuthorArticle_Author_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "Author",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Version",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ArxivId = table.Column<string>(nullable: true),
                    SubmissionDate = table.Column<DateTime>(nullable: false),
                    Number = table.Column<int>(nullable: false),
                    ArticleId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Version", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Version_Article_ArticleId",
                        column: x => x.ArticleId,
                        principalTable: "Article",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CategoryArticle",
                columns: table => new
                {
                    CategoryId = table.Column<int>(nullable: false),
                    ArticleId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoryArticle", x => new { x.CategoryId, x.ArticleId });
                    table.ForeignKey(
                        name: "FK_CategoryArticle_Article_ArticleId",
                        column: x => x.ArticleId,
                        principalTable: "Article",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CategoryArticle_Category_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Category",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Article_PrimarySubjectCodeId",
                table: "Article",
                column: "PrimarySubjectCodeId");

            migrationBuilder.CreateIndex(
                name: "IX_Article_PrimarySubjectGroupCodeId",
                table: "Article",
                column: "PrimarySubjectGroupCodeId");

            migrationBuilder.CreateIndex(
                name: "IX_Article_PrmiaryDisciplineId",
                table: "Article",
                column: "PrmiaryDisciplineId");

            migrationBuilder.CreateIndex(
                name: "IX_AuthorAffiliation_AffiliationId",
                table: "AuthorAffiliation",
                column: "AffiliationId");

            migrationBuilder.CreateIndex(
                name: "IX_AuthorArticle_ArticleId",
                table: "AuthorArticle",
                column: "ArticleId");

            migrationBuilder.CreateIndex(
                name: "IX_Category_DisciplineId",
                table: "Category",
                column: "DisciplineId");

            migrationBuilder.CreateIndex(
                name: "IX_Category_SubjectCodeId",
                table: "Category",
                column: "SubjectCodeId");

            migrationBuilder.CreateIndex(
                name: "IX_Category_SubjectGroupId",
                table: "Category",
                column: "SubjectGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_CategoryArticle_ArticleId",
                table: "CategoryArticle",
                column: "ArticleId");

            migrationBuilder.CreateIndex(
                name: "IX_Version_ArticleId",
                table: "Version",
                column: "ArticleId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuthorAffiliation");

            migrationBuilder.DropTable(
                name: "AuthorArticle");

            migrationBuilder.DropTable(
                name: "CategoryArticle");

            migrationBuilder.DropTable(
                name: "Version");

            migrationBuilder.DropTable(
                name: "Affiliation");

            migrationBuilder.DropTable(
                name: "Author");

            migrationBuilder.DropTable(
                name: "Category");

            migrationBuilder.DropTable(
                name: "Article");

            migrationBuilder.DropTable(
                name: "SubjectCode");

            migrationBuilder.DropTable(
                name: "SubjectGroup");

            migrationBuilder.DropTable(
                name: "Discipline");
        }
    }
}

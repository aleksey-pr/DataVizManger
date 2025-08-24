using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DataVizManger.Migrations
{
  /// <inheritdoc />
  public partial class InitDb : Migration
  {
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.CreateTable(
          name: "Users",
          columns: table => new
          {
            Id = table.Column<int>(type: "integer", nullable: false)
                  .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
            Email = table.Column<string>(type: "text", nullable: false),
            UserName = table.Column<String>(type: "text", nullable: false),
            PasswordHash = table.Column<string>(type: "text", nullable: false),
            Role = table.Column<string>(type: "text", nullable: false),
            CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_Users", x => x.Id);
          });
      migrationBuilder.CreateTable(
          name: "Reports",
          columns: table => new
          {
            Id = table.Column<int>(type: "integer", nullable: false)
                  .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
            VoiceCallCount = table.Column<int>(type: "integer", nullable: false),
            VideoCallCount = table.Column<int>(type: "integer", nullable: false),
            MockInterviewCount = table.Column<int>(type: "integer", nullable: false),
            AppliedJobCount = table.Column<int>(type: "integer", nullable: false),
            Date = table.Column<string>(type: "string", nullable: false),
            Standup = table.Column<string>(type: "string", nullable: false),
            Daily = table.Column<string>(type: "string", nullable: false),
            CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_Users", x => x.Id);
          });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.DropTable(
          name: "Users");
    }
  }
}

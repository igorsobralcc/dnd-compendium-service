using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Compendium.Infra.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddAbilityScoreMethods : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ability_score_methods",
                schema: "compendium",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    rule_source_id = table.Column<Guid>(type: "uuid", nullable: false),
                    source_version_id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    name = table.Column<string>(type: "character varying(180)", maxLength: 180, nullable: false),
                    type = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_ability_score_methods", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "ability_score_method_rules",
                schema: "compendium",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    ability_score_method_id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    numeric_value = table.Column<int>(type: "integer", nullable: true),
                    text_value = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_ability_score_method_rules", x => x.id);
                    table.ForeignKey(
                        name: "FK_ability_score_method_rules_ability_score_methods_ability_sc~",
                        column: x => x.ability_score_method_id,
                        principalSchema: "compendium",
                        principalTable: "ability_score_methods",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ability_score_point_buy_costs",
                schema: "compendium",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    ability_score_method_id = table.Column<Guid>(type: "uuid", nullable: false),
                    score = table.Column<int>(type: "integer", nullable: false),
                    cost = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_ability_score_point_buy_costs", x => x.id);
                    table.ForeignKey(
                        name: "FK_ability_score_point_buy_costs_ability_score_methods_ability~",
                        column: x => x.ability_score_method_id,
                        principalSchema: "compendium",
                        principalTable: "ability_score_methods",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ability_score_roll_rules",
                schema: "compendium",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    ability_score_method_id = table.Column<Guid>(type: "uuid", nullable: false),
                    dice_quantity = table.Column<int>(type: "integer", nullable: false),
                    die_size = table.Column<int>(type: "integer", nullable: false),
                    keep_highest_quantity = table.Column<int>(type: "integer", nullable: false),
                    drop_lowest_quantity = table.Column<int>(type: "integer", nullable: true),
                    repetitions = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_ability_score_roll_rules", x => x.id);
                    table.ForeignKey(
                        name: "FK_ability_score_roll_rules_ability_score_methods_ability_scor~",
                        column: x => x.ability_score_method_id,
                        principalSchema: "compendium",
                        principalTable: "ability_score_methods",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ability_score_standard_values",
                schema: "compendium",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    ability_score_method_id = table.Column<Guid>(type: "uuid", nullable: false),
                    position = table.Column<int>(type: "integer", nullable: false),
                    score = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_ability_score_standard_values", x => x.id);
                    table.ForeignKey(
                        name: "FK_ability_score_standard_values_ability_score_methods_ability~",
                        column: x => x.ability_score_method_id,
                        principalSchema: "compendium",
                        principalTable: "ability_score_methods",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ux_ability_score_method_rules_method_code",
                schema: "compendium",
                table: "ability_score_method_rules",
                columns: new[] { "ability_score_method_id", "code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_ability_score_methods_source_version_id",
                schema: "compendium",
                table: "ability_score_methods",
                column: "source_version_id");

            migrationBuilder.CreateIndex(
                name: "ux_ability_score_methods_code",
                schema: "compendium",
                table: "ability_score_methods",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ux_ability_score_point_buy_costs_method_score",
                schema: "compendium",
                table: "ability_score_point_buy_costs",
                columns: new[] { "ability_score_method_id", "score" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ux_ability_score_roll_rules_method",
                schema: "compendium",
                table: "ability_score_roll_rules",
                column: "ability_score_method_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ux_ability_score_standard_values_method_position",
                schema: "compendium",
                table: "ability_score_standard_values",
                columns: new[] { "ability_score_method_id", "position" },
                unique: true);

            migrationBuilder.Sql(
                """
                INSERT INTO compendium.ability_score_methods (id, rule_source_id, source_version_id, code, name, type, created_at_utc, updated_at_utc)
                VALUES
                ('00000000-0000-4000-8000-000000010001', '00000000-0000-4000-8000-000000000522', '00000000-0000-4000-8000-000000000523', 'STANDARD_ARRAY', 'Standard Array', 'StandardArray', TIMESTAMPTZ '2026-06-12 00:00:00+00', TIMESTAMPTZ '2026-06-12 00:00:00+00'),
                ('00000000-0000-4000-8000-000000010002', '00000000-0000-4000-8000-000000000522', '00000000-0000-4000-8000-000000000523', 'POINT_BUY', 'Point Buy', 'PointBuy', TIMESTAMPTZ '2026-06-12 00:00:00+00', TIMESTAMPTZ '2026-06-12 00:00:00+00'),
                ('00000000-0000-4000-8000-000000010003', '00000000-0000-4000-8000-000000000522', '00000000-0000-4000-8000-000000000523', 'RANDOM_GENERATION', 'Random Generation', 'RandomRoll', TIMESTAMPTZ '2026-06-12 00:00:00+00', TIMESTAMPTZ '2026-06-12 00:00:00+00')
                ON CONFLICT (code) DO NOTHING;

                INSERT INTO compendium.ability_score_method_rules (id, ability_score_method_id, code, numeric_value, text_value)
                VALUES
                ('00000000-0000-4000-8000-000000011001', '00000000-0000-4000-8000-000000010001', 'GENERATED_SCORE_COUNT', 6, NULL),
                ('00000000-0000-4000-8000-000000011002', '00000000-0000-4000-8000-000000010002', 'POINT_BUDGET', 27, NULL),
                ('00000000-0000-4000-8000-000000011003', '00000000-0000-4000-8000-000000010002', 'MIN_SCORE', 8, NULL),
                ('00000000-0000-4000-8000-000000011004', '00000000-0000-4000-8000-000000010002', 'MAX_SCORE', 15, NULL),
                ('00000000-0000-4000-8000-000000011005', '00000000-0000-4000-8000-000000010003', 'GENERATED_SCORE_COUNT', 6, NULL),
                ('00000000-0000-4000-8000-000000011006', '00000000-0000-4000-8000-000000010003', 'SCORING_RULE', NULL, 'Record the total of the highest three dice.')
                ON CONFLICT (ability_score_method_id, code) DO NOTHING;

                INSERT INTO compendium.ability_score_standard_values (id, ability_score_method_id, position, score)
                VALUES
                ('00000000-0000-4000-8000-000000012001', '00000000-0000-4000-8000-000000010001', 1, 15),
                ('00000000-0000-4000-8000-000000012002', '00000000-0000-4000-8000-000000010001', 2, 14),
                ('00000000-0000-4000-8000-000000012003', '00000000-0000-4000-8000-000000010001', 3, 13),
                ('00000000-0000-4000-8000-000000012004', '00000000-0000-4000-8000-000000010001', 4, 12),
                ('00000000-0000-4000-8000-000000012005', '00000000-0000-4000-8000-000000010001', 5, 10),
                ('00000000-0000-4000-8000-000000012006', '00000000-0000-4000-8000-000000010001', 6, 8)
                ON CONFLICT (ability_score_method_id, position) DO NOTHING;

                INSERT INTO compendium.ability_score_point_buy_costs (id, ability_score_method_id, score, cost)
                VALUES
                ('00000000-0000-4000-8000-000000013001', '00000000-0000-4000-8000-000000010002', 8, 0),
                ('00000000-0000-4000-8000-000000013002', '00000000-0000-4000-8000-000000010002', 9, 1),
                ('00000000-0000-4000-8000-000000013003', '00000000-0000-4000-8000-000000010002', 10, 2),
                ('00000000-0000-4000-8000-000000013004', '00000000-0000-4000-8000-000000010002', 11, 3),
                ('00000000-0000-4000-8000-000000013005', '00000000-0000-4000-8000-000000010002', 12, 4),
                ('00000000-0000-4000-8000-000000013006', '00000000-0000-4000-8000-000000010002', 13, 5),
                ('00000000-0000-4000-8000-000000013007', '00000000-0000-4000-8000-000000010002', 14, 7),
                ('00000000-0000-4000-8000-000000013008', '00000000-0000-4000-8000-000000010002', 15, 9)
                ON CONFLICT (ability_score_method_id, score) DO NOTHING;

                INSERT INTO compendium.ability_score_roll_rules (id, ability_score_method_id, dice_quantity, die_size, keep_highest_quantity, drop_lowest_quantity, repetitions)
                VALUES ('00000000-0000-4000-8000-000000014001', '00000000-0000-4000-8000-000000010003', 4, 6, 3, 1, 6)
                ON CONFLICT (ability_score_method_id) DO NOTHING;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ability_score_method_rules",
                schema: "compendium");

            migrationBuilder.DropTable(
                name: "ability_score_point_buy_costs",
                schema: "compendium");

            migrationBuilder.DropTable(
                name: "ability_score_roll_rules",
                schema: "compendium");

            migrationBuilder.DropTable(
                name: "ability_score_standard_values",
                schema: "compendium");

            migrationBuilder.DropTable(
                name: "ability_score_methods",
                schema: "compendium");
        }
    }
}

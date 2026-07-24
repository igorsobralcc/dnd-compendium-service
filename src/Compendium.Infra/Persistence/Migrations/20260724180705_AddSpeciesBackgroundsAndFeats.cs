using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Compendium.Infra.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddSpeciesBackgroundsAndFeats : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "backgrounds",
                schema: "compendium",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    rule_source_id = table.Column<Guid>(type: "uuid", nullable: false),
                    source_version_id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    name = table.Column<string>(type: "character varying(180)", maxLength: 180, nullable: false),
                    description = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_backgrounds", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "feats",
                schema: "compendium",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    rule_source_id = table.Column<Guid>(type: "uuid", nullable: false),
                    source_version_id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    name = table.Column<string>(type: "character varying(180)", maxLength: 180, nullable: false),
                    description = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    category = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    repeatable = table.Column<bool>(type: "boolean", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_feats", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "species",
                schema: "compendium",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    rule_source_id = table.Column<Guid>(type: "uuid", nullable: false),
                    source_version_id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    name = table.Column<string>(type: "character varying(180)", maxLength: 180, nullable: false),
                    description = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_species", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "background_ability_boost_rules",
                schema: "compendium",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    background_id = table.Column<Guid>(type: "uuid", nullable: false),
                    boost_amount = table.Column<int>(type: "integer", nullable: false),
                    ability_count = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_background_ability_boost_rules", x => x.id);
                    table.ForeignKey(
                        name: "FK_background_ability_boost_rules_backgrounds_background_id",
                        column: x => x.background_id,
                        principalSchema: "compendium",
                        principalTable: "backgrounds",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "background_ability_options",
                schema: "compendium",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    background_id = table.Column<Guid>(type: "uuid", nullable: false),
                    ability_id = table.Column<Guid>(type: "uuid", nullable: false),
                    sort_order = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_background_ability_options", x => x.id);
                    table.ForeignKey(
                        name: "FK_background_ability_options_backgrounds_background_id",
                        column: x => x.background_id,
                        principalSchema: "compendium",
                        principalTable: "backgrounds",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "background_feat_grants",
                schema: "compendium",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    background_id = table.Column<Guid>(type: "uuid", nullable: false),
                    feat_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_background_feat_grants", x => x.id);
                    table.ForeignKey(
                        name: "FK_background_feat_grants_backgrounds_background_id",
                        column: x => x.background_id,
                        principalSchema: "compendium",
                        principalTable: "backgrounds",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "background_skill_proficiencies",
                schema: "compendium",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    background_id = table.Column<Guid>(type: "uuid", nullable: false),
                    proficiency_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_background_skill_proficiencies", x => x.id);
                    table.ForeignKey(
                        name: "FK_background_skill_proficiencies_backgrounds_background_id",
                        column: x => x.background_id,
                        principalSchema: "compendium",
                        principalTable: "backgrounds",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "background_starting_equipment_rules",
                schema: "compendium",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    background_id = table.Column<Guid>(type: "uuid", nullable: false),
                    reference_id = table.Column<Guid>(type: "uuid", nullable: false),
                    reference_type = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_background_starting_equipment_rules", x => x.id);
                    table.ForeignKey(
                        name: "FK_background_starting_equipment_rules_backgrounds_background_~",
                        column: x => x.background_id,
                        principalSchema: "compendium",
                        principalTable: "backgrounds",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "background_tool_proficiencies",
                schema: "compendium",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    background_id = table.Column<Guid>(type: "uuid", nullable: false),
                    proficiency_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_background_tool_proficiencies", x => x.id);
                    table.ForeignKey(
                        name: "FK_background_tool_proficiencies_backgrounds_background_id",
                        column: x => x.background_id,
                        principalSchema: "compendium",
                        principalTable: "backgrounds",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ux_background_ability_boost_rules_background_amount",
                schema: "compendium",
                table: "background_ability_boost_rules",
                columns: new[] { "background_id", "boost_amount" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ux_background_ability_options_background_ability",
                schema: "compendium",
                table: "background_ability_options",
                columns: new[] { "background_id", "ability_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ux_background_feat_grants_background_feat",
                schema: "compendium",
                table: "background_feat_grants",
                columns: new[] { "background_id", "feat_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ux_background_skill_proficiencies_background_proficiency",
                schema: "compendium",
                table: "background_skill_proficiencies",
                columns: new[] { "background_id", "proficiency_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ux_background_starting_equipment_rules_reference",
                schema: "compendium",
                table: "background_starting_equipment_rules",
                columns: new[] { "background_id", "reference_id", "reference_type" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ux_background_tool_proficiencies_background_proficiency",
                schema: "compendium",
                table: "background_tool_proficiencies",
                columns: new[] { "background_id", "proficiency_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_backgrounds_source_version_id",
                schema: "compendium",
                table: "backgrounds",
                column: "source_version_id");

            migrationBuilder.CreateIndex(
                name: "ux_backgrounds_code",
                schema: "compendium",
                table: "backgrounds",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_feats_source_version_id",
                schema: "compendium",
                table: "feats",
                column: "source_version_id");

            migrationBuilder.CreateIndex(
                name: "ux_feats_code",
                schema: "compendium",
                table: "feats",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_species_source_version_id",
                schema: "compendium",
                table: "species",
                column: "source_version_id");

            migrationBuilder.CreateIndex(
                name: "ux_species_code",
                schema: "compendium",
                table: "species",
                column: "code",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_background_features_backgrounds_background_id",
                schema: "compendium",
                table: "background_features",
                column: "background_id",
                principalSchema: "compendium",
                principalTable: "backgrounds",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_feat_features_feats_feat_id",
                schema: "compendium",
                table: "feat_features",
                column: "feat_id",
                principalSchema: "compendium",
                principalTable: "feats",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_species_features_species_species_id",
                schema: "compendium",
                table: "species_features",
                column: "species_id",
                principalSchema: "compendium",
                principalTable: "species",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.Sql(
                """
                DO $$
                DECLARE
                    seeded_version_id uuid;
                    seeded_source_id uuid;
                BEGIN
                    SELECT id, rule_source_id
                    INTO seeded_version_id, seeded_source_id
                    FROM compendium.source_versions
                    WHERE version_number = '5.2.1'
                    ORDER BY is_current DESC, id
                    LIMIT 1;

                    IF seeded_version_id IS NOT NULL THEN
                        INSERT INTO compendium.species
                            (id, rule_source_id, source_version_id, code, name, description, created_at_utc, updated_at_utc)
                        VALUES
                            ('70000000-0000-7000-8000-000000000001', seeded_source_id, seeded_version_id, 'DRAGONBORN', 'Dragonborn', 'Playable species from SRD 5.2.1.', CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
                            ('70000000-0000-7000-8000-000000000002', seeded_source_id, seeded_version_id, 'DWARF', 'Dwarf', 'Playable species from SRD 5.2.1.', CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
                            ('70000000-0000-7000-8000-000000000003', seeded_source_id, seeded_version_id, 'ELF', 'Elf', 'Playable species from SRD 5.2.1.', CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
                            ('70000000-0000-7000-8000-000000000004', seeded_source_id, seeded_version_id, 'GNOME', 'Gnome', 'Playable species from SRD 5.2.1.', CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
                            ('70000000-0000-7000-8000-000000000005', seeded_source_id, seeded_version_id, 'GOLIATH', 'Goliath', 'Playable species from SRD 5.2.1.', CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
                            ('70000000-0000-7000-8000-000000000006', seeded_source_id, seeded_version_id, 'HALFLING', 'Halfling', 'Playable species from SRD 5.2.1.', CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
                            ('70000000-0000-7000-8000-000000000007', seeded_source_id, seeded_version_id, 'HUMAN', 'Human', 'Playable species from SRD 5.2.1.', CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
                            ('70000000-0000-7000-8000-000000000008', seeded_source_id, seeded_version_id, 'ORC', 'Orc', 'Playable species from SRD 5.2.1.', CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
                            ('70000000-0000-7000-8000-000000000009', seeded_source_id, seeded_version_id, 'TIEFLING', 'Tiefling', 'Playable species from SRD 5.2.1.', CURRENT_TIMESTAMP, CURRENT_TIMESTAMP)
                        ON CONFLICT (code) DO NOTHING;
                    END IF;
                END $$;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_background_features_backgrounds_background_id",
                schema: "compendium",
                table: "background_features");

            migrationBuilder.DropForeignKey(
                name: "FK_feat_features_feats_feat_id",
                schema: "compendium",
                table: "feat_features");

            migrationBuilder.DropForeignKey(
                name: "FK_species_features_species_species_id",
                schema: "compendium",
                table: "species_features");

            migrationBuilder.DropTable(
                name: "background_ability_boost_rules",
                schema: "compendium");

            migrationBuilder.DropTable(
                name: "background_ability_options",
                schema: "compendium");

            migrationBuilder.DropTable(
                name: "background_feat_grants",
                schema: "compendium");

            migrationBuilder.DropTable(
                name: "background_skill_proficiencies",
                schema: "compendium");

            migrationBuilder.DropTable(
                name: "background_starting_equipment_rules",
                schema: "compendium");

            migrationBuilder.DropTable(
                name: "background_tool_proficiencies",
                schema: "compendium");

            migrationBuilder.DropTable(
                name: "feats",
                schema: "compendium");

            migrationBuilder.DropTable(
                name: "species",
                schema: "compendium");

            migrationBuilder.DropTable(
                name: "backgrounds",
                schema: "compendium");
        }
    }
}

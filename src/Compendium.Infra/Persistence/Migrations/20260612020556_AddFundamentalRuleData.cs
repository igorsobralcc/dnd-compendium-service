using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Compendium.Infra.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddFundamentalRuleData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "abilities",
                schema: "compendium",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    rule_source_id = table.Column<Guid>(type: "uuid", nullable: false),
                    source_version_id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    name = table.Column<string>(type: "character varying(180)", maxLength: 180, nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_abilities", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "armor_training_categories",
                schema: "compendium",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    rule_source_id = table.Column<Guid>(type: "uuid", nullable: false),
                    source_version_id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    name = table.Column<string>(type: "character varying(180)", maxLength: 180, nullable: false),
                    sort_order = table.Column<int>(type: "integer", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_armor_training_categories", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "hit_dice",
                schema: "compendium",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    rule_source_id = table.Column<Guid>(type: "uuid", nullable: false),
                    source_version_id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    name = table.Column<string>(type: "character varying(180)", maxLength: 180, nullable: false),
                    die = table.Column<int>(type: "integer", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_hit_dice", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "languages",
                schema: "compendium",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    rule_source_id = table.Column<Guid>(type: "uuid", nullable: false),
                    source_version_id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    name = table.Column<string>(type: "character varying(180)", maxLength: 180, nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_languages", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "proficiencies",
                schema: "compendium",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    rule_source_id = table.Column<Guid>(type: "uuid", nullable: false),
                    source_version_id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    name = table.Column<string>(type: "character varying(180)", maxLength: 180, nullable: false),
                    type = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    related_entity_id = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_proficiencies", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "skills",
                schema: "compendium",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    rule_source_id = table.Column<Guid>(type: "uuid", nullable: false),
                    source_version_id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    name = table.Column<string>(type: "character varying(180)", maxLength: 180, nullable: false),
                    default_ability_id = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_skills", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_abilities_source_version_id",
                schema: "compendium",
                table: "abilities",
                column: "source_version_id");

            migrationBuilder.CreateIndex(
                name: "ux_abilities_code",
                schema: "compendium",
                table: "abilities",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_armor_training_categories_sort_order",
                schema: "compendium",
                table: "armor_training_categories",
                column: "sort_order");

            migrationBuilder.CreateIndex(
                name: "ux_armor_training_categories_code",
                schema: "compendium",
                table: "armor_training_categories",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ux_hit_dice_die",
                schema: "compendium",
                table: "hit_dice",
                column: "die",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_languages_source_version_id",
                schema: "compendium",
                table: "languages",
                column: "source_version_id");

            migrationBuilder.CreateIndex(
                name: "ux_languages_code",
                schema: "compendium",
                table: "languages",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_proficiencies_related_entity_id",
                schema: "compendium",
                table: "proficiencies",
                column: "related_entity_id");

            migrationBuilder.CreateIndex(
                name: "ix_proficiencies_type",
                schema: "compendium",
                table: "proficiencies",
                column: "type");

            migrationBuilder.CreateIndex(
                name: "ux_proficiencies_code",
                schema: "compendium",
                table: "proficiencies",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_skills_default_ability_id",
                schema: "compendium",
                table: "skills",
                column: "default_ability_id");

            migrationBuilder.CreateIndex(
                name: "ix_skills_source_version_id",
                schema: "compendium",
                table: "skills",
                column: "source_version_id");

            migrationBuilder.CreateIndex(
                name: "ux_skills_code",
                schema: "compendium",
                table: "skills",
                column: "code",
                unique: true);

            migrationBuilder.Sql(
                """
                INSERT INTO compendium.rulesets (id, code, name, version, status, created_at_utc, updated_at_utc)
                VALUES ('00000000-0000-4000-8000-000000000521', 'SRD_5.2.1', 'System Reference Document 5.2.1', '5.2.1', 'Active', TIMESTAMPTZ '2026-06-12 00:00:00+00', TIMESTAMPTZ '2026-06-12 00:00:00+00')
                ON CONFLICT (code) DO NOTHING;

                INSERT INTO compendium.rule_sources (id, ruleset_id, code, name, type, status, created_at_utc, updated_at_utc)
                VALUES ('00000000-0000-4000-8000-000000000522', '00000000-0000-4000-8000-000000000521', 'SRD', 'System Reference Document', 'Srd', 'Active', TIMESTAMPTZ '2026-06-12 00:00:00+00', TIMESTAMPTZ '2026-06-12 00:00:00+00')
                ON CONFLICT (ruleset_id, code) DO NOTHING;

                INSERT INTO compendium.source_versions (id, rule_source_id, version_number, publication_date, import_status, is_current, created_at_utc, updated_at_utc)
                VALUES ('00000000-0000-4000-8000-000000000523', '00000000-0000-4000-8000-000000000522', '5.2.1', DATE '2025-04-22', 'Imported', true, TIMESTAMPTZ '2026-06-12 00:00:00+00', TIMESTAMPTZ '2026-06-12 00:00:00+00')
                ON CONFLICT (rule_source_id, version_number) DO NOTHING;

                INSERT INTO compendium.abilities (id, rule_source_id, source_version_id, code, name, created_at_utc, updated_at_utc)
                VALUES
                ('00000000-0000-4000-8000-000000001001', '00000000-0000-4000-8000-000000000522', '00000000-0000-4000-8000-000000000523', 'STR', 'Strength', TIMESTAMPTZ '2026-06-12 00:00:00+00', TIMESTAMPTZ '2026-06-12 00:00:00+00'),
                ('00000000-0000-4000-8000-000000001002', '00000000-0000-4000-8000-000000000522', '00000000-0000-4000-8000-000000000523', 'DEX', 'Dexterity', TIMESTAMPTZ '2026-06-12 00:00:00+00', TIMESTAMPTZ '2026-06-12 00:00:00+00'),
                ('00000000-0000-4000-8000-000000001003', '00000000-0000-4000-8000-000000000522', '00000000-0000-4000-8000-000000000523', 'CON', 'Constitution', TIMESTAMPTZ '2026-06-12 00:00:00+00', TIMESTAMPTZ '2026-06-12 00:00:00+00'),
                ('00000000-0000-4000-8000-000000001004', '00000000-0000-4000-8000-000000000522', '00000000-0000-4000-8000-000000000523', 'INT', 'Intelligence', TIMESTAMPTZ '2026-06-12 00:00:00+00', TIMESTAMPTZ '2026-06-12 00:00:00+00'),
                ('00000000-0000-4000-8000-000000001005', '00000000-0000-4000-8000-000000000522', '00000000-0000-4000-8000-000000000523', 'WIS', 'Wisdom', TIMESTAMPTZ '2026-06-12 00:00:00+00', TIMESTAMPTZ '2026-06-12 00:00:00+00'),
                ('00000000-0000-4000-8000-000000001006', '00000000-0000-4000-8000-000000000522', '00000000-0000-4000-8000-000000000523', 'CHA', 'Charisma', TIMESTAMPTZ '2026-06-12 00:00:00+00', TIMESTAMPTZ '2026-06-12 00:00:00+00')
                ON CONFLICT (code) DO NOTHING;

                INSERT INTO compendium.skills (id, rule_source_id, source_version_id, code, name, default_ability_id, created_at_utc, updated_at_utc)
                VALUES
                ('00000000-0000-4000-8000-000000002001', '00000000-0000-4000-8000-000000000522', '00000000-0000-4000-8000-000000000523', 'ACROBATICS', 'Acrobatics', '00000000-0000-4000-8000-000000001002', TIMESTAMPTZ '2026-06-12 00:00:00+00', TIMESTAMPTZ '2026-06-12 00:00:00+00'),
                ('00000000-0000-4000-8000-000000002002', '00000000-0000-4000-8000-000000000522', '00000000-0000-4000-8000-000000000523', 'ANIMAL_HANDLING', 'Animal Handling', '00000000-0000-4000-8000-000000001005', TIMESTAMPTZ '2026-06-12 00:00:00+00', TIMESTAMPTZ '2026-06-12 00:00:00+00'),
                ('00000000-0000-4000-8000-000000002003', '00000000-0000-4000-8000-000000000522', '00000000-0000-4000-8000-000000000523', 'ARCANA', 'Arcana', '00000000-0000-4000-8000-000000001004', TIMESTAMPTZ '2026-06-12 00:00:00+00', TIMESTAMPTZ '2026-06-12 00:00:00+00'),
                ('00000000-0000-4000-8000-000000002004', '00000000-0000-4000-8000-000000000522', '00000000-0000-4000-8000-000000000523', 'ATHLETICS', 'Athletics', '00000000-0000-4000-8000-000000001001', TIMESTAMPTZ '2026-06-12 00:00:00+00', TIMESTAMPTZ '2026-06-12 00:00:00+00'),
                ('00000000-0000-4000-8000-000000002005', '00000000-0000-4000-8000-000000000522', '00000000-0000-4000-8000-000000000523', 'DECEPTION', 'Deception', '00000000-0000-4000-8000-000000001006', TIMESTAMPTZ '2026-06-12 00:00:00+00', TIMESTAMPTZ '2026-06-12 00:00:00+00'),
                ('00000000-0000-4000-8000-000000002006', '00000000-0000-4000-8000-000000000522', '00000000-0000-4000-8000-000000000523', 'HISTORY', 'History', '00000000-0000-4000-8000-000000001004', TIMESTAMPTZ '2026-06-12 00:00:00+00', TIMESTAMPTZ '2026-06-12 00:00:00+00'),
                ('00000000-0000-4000-8000-000000002007', '00000000-0000-4000-8000-000000000522', '00000000-0000-4000-8000-000000000523', 'INSIGHT', 'Insight', '00000000-0000-4000-8000-000000001005', TIMESTAMPTZ '2026-06-12 00:00:00+00', TIMESTAMPTZ '2026-06-12 00:00:00+00'),
                ('00000000-0000-4000-8000-000000002008', '00000000-0000-4000-8000-000000000522', '00000000-0000-4000-8000-000000000523', 'INTIMIDATION', 'Intimidation', '00000000-0000-4000-8000-000000001006', TIMESTAMPTZ '2026-06-12 00:00:00+00', TIMESTAMPTZ '2026-06-12 00:00:00+00'),
                ('00000000-0000-4000-8000-000000002009', '00000000-0000-4000-8000-000000000522', '00000000-0000-4000-8000-000000000523', 'INVESTIGATION', 'Investigation', '00000000-0000-4000-8000-000000001004', TIMESTAMPTZ '2026-06-12 00:00:00+00', TIMESTAMPTZ '2026-06-12 00:00:00+00'),
                ('00000000-0000-4000-8000-000000002010', '00000000-0000-4000-8000-000000000522', '00000000-0000-4000-8000-000000000523', 'MEDICINE', 'Medicine', '00000000-0000-4000-8000-000000001005', TIMESTAMPTZ '2026-06-12 00:00:00+00', TIMESTAMPTZ '2026-06-12 00:00:00+00'),
                ('00000000-0000-4000-8000-000000002011', '00000000-0000-4000-8000-000000000522', '00000000-0000-4000-8000-000000000523', 'NATURE', 'Nature', '00000000-0000-4000-8000-000000001004', TIMESTAMPTZ '2026-06-12 00:00:00+00', TIMESTAMPTZ '2026-06-12 00:00:00+00'),
                ('00000000-0000-4000-8000-000000002012', '00000000-0000-4000-8000-000000000522', '00000000-0000-4000-8000-000000000523', 'PERCEPTION', 'Perception', '00000000-0000-4000-8000-000000001005', TIMESTAMPTZ '2026-06-12 00:00:00+00', TIMESTAMPTZ '2026-06-12 00:00:00+00'),
                ('00000000-0000-4000-8000-000000002013', '00000000-0000-4000-8000-000000000522', '00000000-0000-4000-8000-000000000523', 'PERFORMANCE', 'Performance', '00000000-0000-4000-8000-000000001006', TIMESTAMPTZ '2026-06-12 00:00:00+00', TIMESTAMPTZ '2026-06-12 00:00:00+00'),
                ('00000000-0000-4000-8000-000000002014', '00000000-0000-4000-8000-000000000522', '00000000-0000-4000-8000-000000000523', 'PERSUASION', 'Persuasion', '00000000-0000-4000-8000-000000001006', TIMESTAMPTZ '2026-06-12 00:00:00+00', TIMESTAMPTZ '2026-06-12 00:00:00+00'),
                ('00000000-0000-4000-8000-000000002015', '00000000-0000-4000-8000-000000000522', '00000000-0000-4000-8000-000000000523', 'RELIGION', 'Religion', '00000000-0000-4000-8000-000000001004', TIMESTAMPTZ '2026-06-12 00:00:00+00', TIMESTAMPTZ '2026-06-12 00:00:00+00'),
                ('00000000-0000-4000-8000-000000002016', '00000000-0000-4000-8000-000000000522', '00000000-0000-4000-8000-000000000523', 'SLEIGHT_OF_HAND', 'Sleight of Hand', '00000000-0000-4000-8000-000000001002', TIMESTAMPTZ '2026-06-12 00:00:00+00', TIMESTAMPTZ '2026-06-12 00:00:00+00'),
                ('00000000-0000-4000-8000-000000002017', '00000000-0000-4000-8000-000000000522', '00000000-0000-4000-8000-000000000523', 'STEALTH', 'Stealth', '00000000-0000-4000-8000-000000001002', TIMESTAMPTZ '2026-06-12 00:00:00+00', TIMESTAMPTZ '2026-06-12 00:00:00+00'),
                ('00000000-0000-4000-8000-000000002018', '00000000-0000-4000-8000-000000000522', '00000000-0000-4000-8000-000000000523', 'SURVIVAL', 'Survival', '00000000-0000-4000-8000-000000001005', TIMESTAMPTZ '2026-06-12 00:00:00+00', TIMESTAMPTZ '2026-06-12 00:00:00+00')
                ON CONFLICT (code) DO NOTHING;

                INSERT INTO compendium.languages (id, rule_source_id, source_version_id, code, name, created_at_utc, updated_at_utc)
                VALUES
                ('00000000-0000-4000-8000-000000003001', '00000000-0000-4000-8000-000000000522', '00000000-0000-4000-8000-000000000523', 'COMMON', 'Common', TIMESTAMPTZ '2026-06-12 00:00:00+00', TIMESTAMPTZ '2026-06-12 00:00:00+00'),
                ('00000000-0000-4000-8000-000000003002', '00000000-0000-4000-8000-000000000522', '00000000-0000-4000-8000-000000000523', 'COMMON_SIGN_LANGUAGE', 'Common Sign Language', TIMESTAMPTZ '2026-06-12 00:00:00+00', TIMESTAMPTZ '2026-06-12 00:00:00+00'),
                ('00000000-0000-4000-8000-000000003003', '00000000-0000-4000-8000-000000000522', '00000000-0000-4000-8000-000000000523', 'DRACONIC', 'Draconic', TIMESTAMPTZ '2026-06-12 00:00:00+00', TIMESTAMPTZ '2026-06-12 00:00:00+00'),
                ('00000000-0000-4000-8000-000000003004', '00000000-0000-4000-8000-000000000522', '00000000-0000-4000-8000-000000000523', 'DWARVISH', 'Dwarvish', TIMESTAMPTZ '2026-06-12 00:00:00+00', TIMESTAMPTZ '2026-06-12 00:00:00+00'),
                ('00000000-0000-4000-8000-000000003005', '00000000-0000-4000-8000-000000000522', '00000000-0000-4000-8000-000000000523', 'ELVISH', 'Elvish', TIMESTAMPTZ '2026-06-12 00:00:00+00', TIMESTAMPTZ '2026-06-12 00:00:00+00'),
                ('00000000-0000-4000-8000-000000003006', '00000000-0000-4000-8000-000000000522', '00000000-0000-4000-8000-000000000523', 'GIANT', 'Giant', TIMESTAMPTZ '2026-06-12 00:00:00+00', TIMESTAMPTZ '2026-06-12 00:00:00+00'),
                ('00000000-0000-4000-8000-000000003007', '00000000-0000-4000-8000-000000000522', '00000000-0000-4000-8000-000000000523', 'GNOMISH', 'Gnomish', TIMESTAMPTZ '2026-06-12 00:00:00+00', TIMESTAMPTZ '2026-06-12 00:00:00+00'),
                ('00000000-0000-4000-8000-000000003008', '00000000-0000-4000-8000-000000000522', '00000000-0000-4000-8000-000000000523', 'GOBLIN', 'Goblin', TIMESTAMPTZ '2026-06-12 00:00:00+00', TIMESTAMPTZ '2026-06-12 00:00:00+00'),
                ('00000000-0000-4000-8000-000000003009', '00000000-0000-4000-8000-000000000522', '00000000-0000-4000-8000-000000000523', 'HALFLING', 'Halfling', TIMESTAMPTZ '2026-06-12 00:00:00+00', TIMESTAMPTZ '2026-06-12 00:00:00+00'),
                ('00000000-0000-4000-8000-000000003010', '00000000-0000-4000-8000-000000000522', '00000000-0000-4000-8000-000000000523', 'ORC', 'Orc', TIMESTAMPTZ '2026-06-12 00:00:00+00', TIMESTAMPTZ '2026-06-12 00:00:00+00'),
                ('00000000-0000-4000-8000-000000003011', '00000000-0000-4000-8000-000000000522', '00000000-0000-4000-8000-000000000523', 'ABYSSAL', 'Abyssal', TIMESTAMPTZ '2026-06-12 00:00:00+00', TIMESTAMPTZ '2026-06-12 00:00:00+00'),
                ('00000000-0000-4000-8000-000000003012', '00000000-0000-4000-8000-000000000522', '00000000-0000-4000-8000-000000000523', 'CELESTIAL', 'Celestial', TIMESTAMPTZ '2026-06-12 00:00:00+00', TIMESTAMPTZ '2026-06-12 00:00:00+00'),
                ('00000000-0000-4000-8000-000000003013', '00000000-0000-4000-8000-000000000522', '00000000-0000-4000-8000-000000000523', 'DEEP_SPEECH', 'Deep Speech', TIMESTAMPTZ '2026-06-12 00:00:00+00', TIMESTAMPTZ '2026-06-12 00:00:00+00'),
                ('00000000-0000-4000-8000-000000003014', '00000000-0000-4000-8000-000000000522', '00000000-0000-4000-8000-000000000523', 'DRUIDIC', 'Druidic', TIMESTAMPTZ '2026-06-12 00:00:00+00', TIMESTAMPTZ '2026-06-12 00:00:00+00'),
                ('00000000-0000-4000-8000-000000003015', '00000000-0000-4000-8000-000000000522', '00000000-0000-4000-8000-000000000523', 'INFERNAL', 'Infernal', TIMESTAMPTZ '2026-06-12 00:00:00+00', TIMESTAMPTZ '2026-06-12 00:00:00+00'),
                ('00000000-0000-4000-8000-000000003016', '00000000-0000-4000-8000-000000000522', '00000000-0000-4000-8000-000000000523', 'PRIMORDIAL', 'Primordial', TIMESTAMPTZ '2026-06-12 00:00:00+00', TIMESTAMPTZ '2026-06-12 00:00:00+00'),
                ('00000000-0000-4000-8000-000000003017', '00000000-0000-4000-8000-000000000522', '00000000-0000-4000-8000-000000000523', 'SYLVAN', 'Sylvan', TIMESTAMPTZ '2026-06-12 00:00:00+00', TIMESTAMPTZ '2026-06-12 00:00:00+00'),
                ('00000000-0000-4000-8000-000000003018', '00000000-0000-4000-8000-000000000522', '00000000-0000-4000-8000-000000000523', 'THIEVES_CANT', 'Thieves'' Cant', TIMESTAMPTZ '2026-06-12 00:00:00+00', TIMESTAMPTZ '2026-06-12 00:00:00+00'),
                ('00000000-0000-4000-8000-000000003019', '00000000-0000-4000-8000-000000000522', '00000000-0000-4000-8000-000000000523', 'UNDERCOMMON', 'Undercommon', TIMESTAMPTZ '2026-06-12 00:00:00+00', TIMESTAMPTZ '2026-06-12 00:00:00+00')
                ON CONFLICT (code) DO NOTHING;

                INSERT INTO compendium.armor_training_categories (id, rule_source_id, source_version_id, code, name, sort_order, created_at_utc, updated_at_utc)
                VALUES
                ('00000000-0000-4000-8000-000000004001', '00000000-0000-4000-8000-000000000522', '00000000-0000-4000-8000-000000000523', 'LIGHT_ARMOR', 'Light Armor', 10, TIMESTAMPTZ '2026-06-12 00:00:00+00', TIMESTAMPTZ '2026-06-12 00:00:00+00'),
                ('00000000-0000-4000-8000-000000004002', '00000000-0000-4000-8000-000000000522', '00000000-0000-4000-8000-000000000523', 'MEDIUM_ARMOR', 'Medium Armor', 20, TIMESTAMPTZ '2026-06-12 00:00:00+00', TIMESTAMPTZ '2026-06-12 00:00:00+00'),
                ('00000000-0000-4000-8000-000000004003', '00000000-0000-4000-8000-000000000522', '00000000-0000-4000-8000-000000000523', 'HEAVY_ARMOR', 'Heavy Armor', 30, TIMESTAMPTZ '2026-06-12 00:00:00+00', TIMESTAMPTZ '2026-06-12 00:00:00+00'),
                ('00000000-0000-4000-8000-000000004004', '00000000-0000-4000-8000-000000000522', '00000000-0000-4000-8000-000000000523', 'SHIELDS', 'Shields', 40, TIMESTAMPTZ '2026-06-12 00:00:00+00', TIMESTAMPTZ '2026-06-12 00:00:00+00')
                ON CONFLICT (code) DO NOTHING;

                INSERT INTO compendium.hit_dice (id, rule_source_id, source_version_id, code, name, die, created_at_utc, updated_at_utc)
                VALUES
                ('00000000-0000-4000-8000-000000005001', '00000000-0000-4000-8000-000000000522', '00000000-0000-4000-8000-000000000523', 'D6', 'd6', 6, TIMESTAMPTZ '2026-06-12 00:00:00+00', TIMESTAMPTZ '2026-06-12 00:00:00+00'),
                ('00000000-0000-4000-8000-000000005002', '00000000-0000-4000-8000-000000000522', '00000000-0000-4000-8000-000000000523', 'D8', 'd8', 8, TIMESTAMPTZ '2026-06-12 00:00:00+00', TIMESTAMPTZ '2026-06-12 00:00:00+00'),
                ('00000000-0000-4000-8000-000000005003', '00000000-0000-4000-8000-000000000522', '00000000-0000-4000-8000-000000000523', 'D10', 'd10', 10, TIMESTAMPTZ '2026-06-12 00:00:00+00', TIMESTAMPTZ '2026-06-12 00:00:00+00'),
                ('00000000-0000-4000-8000-000000005004', '00000000-0000-4000-8000-000000000522', '00000000-0000-4000-8000-000000000523', 'D12', 'd12', 12, TIMESTAMPTZ '2026-06-12 00:00:00+00', TIMESTAMPTZ '2026-06-12 00:00:00+00')
                ON CONFLICT (die) DO NOTHING;

                INSERT INTO compendium.proficiencies (id, rule_source_id, source_version_id, code, name, type, related_entity_id, created_at_utc, updated_at_utc)
                VALUES
                ('00000000-0000-4000-8000-000000006001', '00000000-0000-4000-8000-000000000522', '00000000-0000-4000-8000-000000000523', 'SAVE_STR', 'Strength Saving Throw', 'SavingThrow', '00000000-0000-4000-8000-000000001001', TIMESTAMPTZ '2026-06-12 00:00:00+00', TIMESTAMPTZ '2026-06-12 00:00:00+00'),
                ('00000000-0000-4000-8000-000000006002', '00000000-0000-4000-8000-000000000522', '00000000-0000-4000-8000-000000000523', 'SAVE_DEX', 'Dexterity Saving Throw', 'SavingThrow', '00000000-0000-4000-8000-000000001002', TIMESTAMPTZ '2026-06-12 00:00:00+00', TIMESTAMPTZ '2026-06-12 00:00:00+00'),
                ('00000000-0000-4000-8000-000000006003', '00000000-0000-4000-8000-000000000522', '00000000-0000-4000-8000-000000000523', 'SAVE_CON', 'Constitution Saving Throw', 'SavingThrow', '00000000-0000-4000-8000-000000001003', TIMESTAMPTZ '2026-06-12 00:00:00+00', TIMESTAMPTZ '2026-06-12 00:00:00+00'),
                ('00000000-0000-4000-8000-000000006004', '00000000-0000-4000-8000-000000000522', '00000000-0000-4000-8000-000000000523', 'SAVE_INT', 'Intelligence Saving Throw', 'SavingThrow', '00000000-0000-4000-8000-000000001004', TIMESTAMPTZ '2026-06-12 00:00:00+00', TIMESTAMPTZ '2026-06-12 00:00:00+00'),
                ('00000000-0000-4000-8000-000000006005', '00000000-0000-4000-8000-000000000522', '00000000-0000-4000-8000-000000000523', 'SAVE_WIS', 'Wisdom Saving Throw', 'SavingThrow', '00000000-0000-4000-8000-000000001005', TIMESTAMPTZ '2026-06-12 00:00:00+00', TIMESTAMPTZ '2026-06-12 00:00:00+00'),
                ('00000000-0000-4000-8000-000000006006', '00000000-0000-4000-8000-000000000522', '00000000-0000-4000-8000-000000000523', 'SAVE_CHA', 'Charisma Saving Throw', 'SavingThrow', '00000000-0000-4000-8000-000000001006', TIMESTAMPTZ '2026-06-12 00:00:00+00', TIMESTAMPTZ '2026-06-12 00:00:00+00')
                ON CONFLICT (code) DO NOTHING;

                INSERT INTO compendium.proficiencies (id, rule_source_id, source_version_id, code, name, type, related_entity_id, created_at_utc, updated_at_utc)
                SELECT ('00000000-0000-4000-8000-' || lpad((7000 + row_number() over (order by code))::text, 12, '0'))::uuid,
                       rule_source_id, source_version_id, 'LANG_' || code, name, 'Language', id, created_at_utc, updated_at_utc
                FROM compendium.languages
                WHERE source_version_id = '00000000-0000-4000-8000-000000000523'
                ON CONFLICT (code) DO NOTHING;

                INSERT INTO compendium.proficiencies (id, rule_source_id, source_version_id, code, name, type, related_entity_id, created_at_utc, updated_at_utc)
                SELECT ('00000000-0000-4000-8000-' || lpad((8000 + row_number() over (order by code))::text, 12, '0'))::uuid,
                       rule_source_id, source_version_id, 'SKILL_' || code, name, 'Skill', id, created_at_utc, updated_at_utc
                FROM compendium.skills
                WHERE source_version_id = '00000000-0000-4000-8000-000000000523'
                ON CONFLICT (code) DO NOTHING;

                INSERT INTO compendium.proficiencies (id, rule_source_id, source_version_id, code, name, type, related_entity_id, created_at_utc, updated_at_utc)
                SELECT ('00000000-0000-4000-8000-' || lpad((9000 + row_number() over (order by code))::text, 12, '0'))::uuid,
                       rule_source_id, source_version_id, 'ARMOR_' || code, name, 'Armor', id, created_at_utc, updated_at_utc
                FROM compendium.armor_training_categories
                WHERE source_version_id = '00000000-0000-4000-8000-000000000523'
                ON CONFLICT (code) DO NOTHING;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "abilities",
                schema: "compendium");

            migrationBuilder.DropTable(
                name: "armor_training_categories",
                schema: "compendium");

            migrationBuilder.DropTable(
                name: "hit_dice",
                schema: "compendium");

            migrationBuilder.DropTable(
                name: "languages",
                schema: "compendium");

            migrationBuilder.DropTable(
                name: "proficiencies",
                schema: "compendium");

            migrationBuilder.DropTable(
                name: "skills",
                schema: "compendium");

            migrationBuilder.Sql(
                """
                DELETE FROM compendium.source_versions WHERE id = '00000000-0000-4000-8000-000000000523';
                DELETE FROM compendium.rule_sources WHERE id = '00000000-0000-4000-8000-000000000522';
                DELETE FROM compendium.rulesets WHERE id = '00000000-0000-4000-8000-000000000521';
                """);
        }
    }
}

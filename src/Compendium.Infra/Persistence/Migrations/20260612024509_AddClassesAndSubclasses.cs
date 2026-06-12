using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Compendium.Infra.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddClassesAndSubclasses : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "classes",
                schema: "compendium",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    rule_source_id = table.Column<Guid>(type: "uuid", nullable: false),
                    source_version_id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    name = table.Column<string>(type: "character varying(180)", maxLength: 180, nullable: false),
                    description = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_classes", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "subclasses",
                schema: "compendium",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    character_class_id = table.Column<Guid>(type: "uuid", nullable: false),
                    rule_source_id = table.Column<Guid>(type: "uuid", nullable: false),
                    source_version_id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    name = table.Column<string>(type: "character varying(180)", maxLength: 180, nullable: false),
                    description = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_subclasses", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "class_core_traits",
                schema: "compendium",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    character_class_id = table.Column<Guid>(type: "uuid", nullable: false),
                    hit_die_id = table.Column<Guid>(type: "uuid", nullable: false),
                    armor_training_category_id = table.Column<Guid>(type: "uuid", nullable: true),
                    skill_choice_count = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_class_core_traits", x => x.id);
                    table.ForeignKey(
                        name: "FK_class_core_traits_classes_character_class_id",
                        column: x => x.character_class_id,
                        principalSchema: "compendium",
                        principalTable: "classes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "class_levels",
                schema: "compendium",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    character_class_id = table.Column<Guid>(type: "uuid", nullable: false),
                    level = table.Column<int>(type: "integer", nullable: false),
                    proficiency_bonus = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_class_levels", x => x.id);
                    table.ForeignKey(
                        name: "FK_class_levels_classes_character_class_id",
                        column: x => x.character_class_id,
                        principalSchema: "compendium",
                        principalTable: "classes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "class_primary_abilities",
                schema: "compendium",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    character_class_id = table.Column<Guid>(type: "uuid", nullable: false),
                    ability_id = table.Column<Guid>(type: "uuid", nullable: false),
                    sort_order = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_class_primary_abilities", x => x.id);
                    table.ForeignKey(
                        name: "FK_class_primary_abilities_classes_character_class_id",
                        column: x => x.character_class_id,
                        principalSchema: "compendium",
                        principalTable: "classes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "class_spellcasting_progressions",
                schema: "compendium",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    character_class_id = table.Column<Guid>(type: "uuid", nullable: false),
                    type = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    spellcasting_ability_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_class_spellcasting_progressions", x => x.id);
                    table.ForeignKey(
                        name: "FK_class_spellcasting_progressions_classes_character_class_id",
                        column: x => x.character_class_id,
                        principalSchema: "compendium",
                        principalTable: "classes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "subclass_features",
                schema: "compendium",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    character_subclass_id = table.Column<Guid>(type: "uuid", nullable: false),
                    source_version_id = table.Column<Guid>(type: "uuid", nullable: false),
                    feature_id = table.Column<Guid>(type: "uuid", nullable: false),
                    level = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_subclass_features", x => x.id);
                    table.ForeignKey(
                        name: "FK_subclass_features_subclasses_character_subclass_id",
                        column: x => x.character_subclass_id,
                        principalSchema: "compendium",
                        principalTable: "subclasses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "class_level_spell_slots",
                schema: "compendium",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    class_level_id = table.Column<Guid>(type: "uuid", nullable: false),
                    spell_level = table.Column<int>(type: "integer", nullable: false),
                    slots = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_class_level_spell_slots", x => x.id);
                    table.ForeignKey(
                        name: "FK_class_level_spell_slots_class_levels_class_level_id",
                        column: x => x.class_level_id,
                        principalSchema: "compendium",
                        principalTable: "class_levels",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "class_proficiency_grants",
                schema: "compendium",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    class_level_id = table.Column<Guid>(type: "uuid", nullable: false),
                    proficiency_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_class_proficiency_grants", x => x.id);
                    table.ForeignKey(
                        name: "FK_class_proficiency_grants_class_levels_class_level_id",
                        column: x => x.class_level_id,
                        principalSchema: "compendium",
                        principalTable: "class_levels",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "class_weapon_mastery_count_by_level",
                schema: "compendium",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    class_level_id = table.Column<Guid>(type: "uuid", nullable: false),
                    count = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_class_weapon_mastery_count_by_level", x => x.id);
                    table.ForeignKey(
                        name: "FK_class_weapon_mastery_count_by_level_class_levels_class_leve~",
                        column: x => x.class_level_id,
                        principalSchema: "compendium",
                        principalTable: "class_levels",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "class_spellcasting_level_rules",
                schema: "compendium",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    class_spellcasting_progression_id = table.Column<Guid>(type: "uuid", nullable: false),
                    class_level = table.Column<int>(type: "integer", nullable: false),
                    caster_level = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_class_spellcasting_level_rules", x => x.id);
                    table.ForeignKey(
                        name: "FK_class_spellcasting_level_rules_class_spellcasting_progressi~",
                        column: x => x.class_spellcasting_progression_id,
                        principalSchema: "compendium",
                        principalTable: "class_spellcasting_progressions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ux_class_core_traits_class",
                schema: "compendium",
                table: "class_core_traits",
                column: "character_class_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ux_class_level_spell_slots_level_spell_level",
                schema: "compendium",
                table: "class_level_spell_slots",
                columns: new[] { "class_level_id", "spell_level" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ux_class_levels_class_level",
                schema: "compendium",
                table: "class_levels",
                columns: new[] { "character_class_id", "level" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ux_class_primary_abilities_class_ability",
                schema: "compendium",
                table: "class_primary_abilities",
                columns: new[] { "character_class_id", "ability_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ux_class_primary_abilities_class_sort",
                schema: "compendium",
                table: "class_primary_abilities",
                columns: new[] { "character_class_id", "sort_order" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ux_class_proficiency_grants_level_proficiency",
                schema: "compendium",
                table: "class_proficiency_grants",
                columns: new[] { "class_level_id", "proficiency_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ux_class_spellcasting_level_rules_progression_level",
                schema: "compendium",
                table: "class_spellcasting_level_rules",
                columns: new[] { "class_spellcasting_progression_id", "class_level" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ux_class_spellcasting_progressions_class",
                schema: "compendium",
                table: "class_spellcasting_progressions",
                column: "character_class_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ux_class_weapon_mastery_count_by_level_level",
                schema: "compendium",
                table: "class_weapon_mastery_count_by_level",
                column: "class_level_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_classes_source_version_id",
                schema: "compendium",
                table: "classes",
                column: "source_version_id");

            migrationBuilder.CreateIndex(
                name: "ux_classes_code",
                schema: "compendium",
                table: "classes",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ux_subclass_features_subclass_feature_level",
                schema: "compendium",
                table: "subclass_features",
                columns: new[] { "character_subclass_id", "feature_id", "level" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ux_subclasses_class_code",
                schema: "compendium",
                table: "subclasses",
                columns: new[] { "character_class_id", "code" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "class_core_traits",
                schema: "compendium");

            migrationBuilder.DropTable(
                name: "class_level_spell_slots",
                schema: "compendium");

            migrationBuilder.DropTable(
                name: "class_primary_abilities",
                schema: "compendium");

            migrationBuilder.DropTable(
                name: "class_proficiency_grants",
                schema: "compendium");

            migrationBuilder.DropTable(
                name: "class_spellcasting_level_rules",
                schema: "compendium");

            migrationBuilder.DropTable(
                name: "class_weapon_mastery_count_by_level",
                schema: "compendium");

            migrationBuilder.DropTable(
                name: "subclass_features",
                schema: "compendium");

            migrationBuilder.DropTable(
                name: "class_spellcasting_progressions",
                schema: "compendium");

            migrationBuilder.DropTable(
                name: "class_levels",
                schema: "compendium");

            migrationBuilder.DropTable(
                name: "subclasses",
                schema: "compendium");

            migrationBuilder.DropTable(
                name: "classes",
                schema: "compendium");
        }
    }
}

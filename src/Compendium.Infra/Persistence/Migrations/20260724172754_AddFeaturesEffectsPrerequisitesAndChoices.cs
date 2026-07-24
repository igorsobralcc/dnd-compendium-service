using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Compendium.Infra.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddFeaturesEffectsPrerequisitesAndChoices : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "background_features",
                schema: "compendium",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    background_id = table.Column<Guid>(type: "uuid", nullable: false),
                    feature_id = table.Column<Guid>(type: "uuid", nullable: false),
                    source_version_id = table.Column<Guid>(type: "uuid", nullable: false),
                    level = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_background_features", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "choice_sets",
                schema: "compendium",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    source_entity_kind = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    source_entity_id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    minimum_choices = table.Column<int>(type: "integer", nullable: false),
                    maximum_choices = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_choice_sets", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "class_level_features",
                schema: "compendium",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    class_level_id = table.Column<Guid>(type: "uuid", nullable: false),
                    feature_id = table.Column<Guid>(type: "uuid", nullable: false),
                    source_version_id = table.Column<Guid>(type: "uuid", nullable: false),
                    level = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_class_level_features", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "effect_schemas",
                schema: "compendium",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    name = table.Column<string>(type: "character varying(180)", maxLength: 180, nullable: false),
                    type = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_effect_schemas", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "entity_prerequisites",
                schema: "compendium",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    entity_kind = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    entity_id = table.Column<Guid>(type: "uuid", nullable: false),
                    type = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    @operator = table.Column<string>(name: "operator", type: "character varying(80)", maxLength: 80, nullable: false),
                    target = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    value_type = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    text_value = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    numeric_value = table.Column<decimal>(type: "numeric", nullable: true),
                    boolean_value = table.Column<bool>(type: "boolean", nullable: true),
                    ref_id = table.Column<Guid>(type: "uuid", nullable: true),
                    enum_value = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_entity_prerequisites", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "feat_features",
                schema: "compendium",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    feat_id = table.Column<Guid>(type: "uuid", nullable: false),
                    feature_id = table.Column<Guid>(type: "uuid", nullable: false),
                    source_version_id = table.Column<Guid>(type: "uuid", nullable: false),
                    level = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_feat_features", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "features",
                schema: "compendium",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    rule_source_id = table.Column<Guid>(type: "uuid", nullable: false),
                    source_version_id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    name = table.Column<string>(type: "character varying(180)", maxLength: 180, nullable: false),
                    description = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    level_requirement = table.Column<int>(type: "integer", nullable: true),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_features", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "species_features",
                schema: "compendium",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    species_id = table.Column<Guid>(type: "uuid", nullable: false),
                    feature_id = table.Column<Guid>(type: "uuid", nullable: false),
                    source_version_id = table.Column<Guid>(type: "uuid", nullable: false),
                    level = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_species_features", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "choice_options",
                schema: "compendium",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    choice_set_id = table.Column<Guid>(type: "uuid", nullable: false),
                    type = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    ref_id = table.Column<Guid>(type: "uuid", nullable: true),
                    display_text = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    sort_order = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_choice_options", x => x.id);
                    table.ForeignKey(
                        name: "FK_choice_options_choice_sets_choice_set_id",
                        column: x => x.choice_set_id,
                        principalSchema: "compendium",
                        principalTable: "choice_sets",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "choice_set_filters",
                schema: "compendium",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    choice_set_id = table.Column<Guid>(type: "uuid", nullable: false),
                    type = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    value_type = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    text_value = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    numeric_value = table.Column<decimal>(type: "numeric", nullable: true),
                    boolean_value = table.Column<bool>(type: "boolean", nullable: true),
                    ref_id = table.Column<Guid>(type: "uuid", nullable: true),
                    enum_value = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_choice_set_filters", x => x.id);
                    table.ForeignKey(
                        name: "FK_choice_set_filters_choice_sets_choice_set_id",
                        column: x => x.choice_set_id,
                        principalSchema: "compendium",
                        principalTable: "choice_sets",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "effect_schema_fields",
                schema: "compendium",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    effect_schema_id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    value_type = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    is_required = table.Column<bool>(type: "boolean", nullable: false),
                    sort_order = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_effect_schema_fields", x => x.id);
                    table.ForeignKey(
                        name: "FK_effect_schema_fields_effect_schemas_effect_schema_id",
                        column: x => x.effect_schema_id,
                        principalSchema: "compendium",
                        principalTable: "effect_schemas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "feature_effects",
                schema: "compendium",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    feature_id = table.Column<Guid>(type: "uuid", nullable: false),
                    effect_schema_id = table.Column<Guid>(type: "uuid", nullable: false),
                    type = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    target = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_feature_effects", x => x.id);
                    table.ForeignKey(
                        name: "FK_feature_effects_features_feature_id",
                        column: x => x.feature_id,
                        principalSchema: "compendium",
                        principalTable: "features",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "feature_effect_conditions",
                schema: "compendium",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    feature_effect_id = table.Column<Guid>(type: "uuid", nullable: false),
                    type = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    value_type = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    text_value = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    numeric_value = table.Column<decimal>(type: "numeric", nullable: true),
                    boolean_value = table.Column<bool>(type: "boolean", nullable: true),
                    ref_id = table.Column<Guid>(type: "uuid", nullable: true),
                    enum_value = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_feature_effect_conditions", x => x.id);
                    table.ForeignKey(
                        name: "FK_feature_effect_conditions_feature_effects_feature_effect_id",
                        column: x => x.feature_effect_id,
                        principalSchema: "compendium",
                        principalTable: "feature_effects",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "feature_effect_field_values",
                schema: "compendium",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    feature_effect_id = table.Column<Guid>(type: "uuid", nullable: false),
                    effect_schema_field_id = table.Column<Guid>(type: "uuid", nullable: false),
                    value_type = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    text_value = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    numeric_value = table.Column<decimal>(type: "numeric", nullable: true),
                    boolean_value = table.Column<bool>(type: "boolean", nullable: true),
                    ref_id = table.Column<Guid>(type: "uuid", nullable: true),
                    enum_value = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_feature_effect_field_values", x => x.id);
                    table.ForeignKey(
                        name: "FK_feature_effect_field_values_feature_effects_feature_effect_~",
                        column: x => x.feature_effect_id,
                        principalSchema: "compendium",
                        principalTable: "feature_effects",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ux_background_features_source_feature_level",
                schema: "compendium",
                table: "background_features",
                columns: new[] { "background_id", "feature_id", "level" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_choice_options_choice_set_id",
                schema: "compendium",
                table: "choice_options",
                column: "choice_set_id");

            migrationBuilder.CreateIndex(
                name: "IX_choice_set_filters_choice_set_id",
                schema: "compendium",
                table: "choice_set_filters",
                column: "choice_set_id");

            migrationBuilder.CreateIndex(
                name: "ix_choice_sets_source_entity",
                schema: "compendium",
                table: "choice_sets",
                columns: new[] { "source_entity_kind", "source_entity_id" });

            migrationBuilder.CreateIndex(
                name: "ux_choice_sets_code",
                schema: "compendium",
                table: "choice_sets",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ux_class_level_features_source_feature_level",
                schema: "compendium",
                table: "class_level_features",
                columns: new[] { "class_level_id", "feature_id", "level" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ux_effect_schema_fields_schema_code",
                schema: "compendium",
                table: "effect_schema_fields",
                columns: new[] { "effect_schema_id", "code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ux_effect_schemas_code",
                schema: "compendium",
                table: "effect_schemas",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_entity_prerequisites_entity",
                schema: "compendium",
                table: "entity_prerequisites",
                columns: new[] { "entity_kind", "entity_id" });

            migrationBuilder.CreateIndex(
                name: "ux_feat_features_source_feature_level",
                schema: "compendium",
                table: "feat_features",
                columns: new[] { "feat_id", "feature_id", "level" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_feature_effect_conditions_feature_effect_id",
                schema: "compendium",
                table: "feature_effect_conditions",
                column: "feature_effect_id");

            migrationBuilder.CreateIndex(
                name: "ux_feature_effect_field_values_effect_field",
                schema: "compendium",
                table: "feature_effect_field_values",
                columns: new[] { "feature_effect_id", "effect_schema_field_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_feature_effects_feature_id",
                schema: "compendium",
                table: "feature_effects",
                column: "feature_id");

            migrationBuilder.CreateIndex(
                name: "ix_features_source_version_id",
                schema: "compendium",
                table: "features",
                column: "source_version_id");

            migrationBuilder.CreateIndex(
                name: "ux_features_code",
                schema: "compendium",
                table: "features",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ux_species_features_source_feature_level",
                schema: "compendium",
                table: "species_features",
                columns: new[] { "species_id", "feature_id", "level" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "background_features",
                schema: "compendium");

            migrationBuilder.DropTable(
                name: "choice_options",
                schema: "compendium");

            migrationBuilder.DropTable(
                name: "choice_set_filters",
                schema: "compendium");

            migrationBuilder.DropTable(
                name: "class_level_features",
                schema: "compendium");

            migrationBuilder.DropTable(
                name: "effect_schema_fields",
                schema: "compendium");

            migrationBuilder.DropTable(
                name: "entity_prerequisites",
                schema: "compendium");

            migrationBuilder.DropTable(
                name: "feat_features",
                schema: "compendium");

            migrationBuilder.DropTable(
                name: "feature_effect_conditions",
                schema: "compendium");

            migrationBuilder.DropTable(
                name: "feature_effect_field_values",
                schema: "compendium");

            migrationBuilder.DropTable(
                name: "species_features",
                schema: "compendium");

            migrationBuilder.DropTable(
                name: "choice_sets",
                schema: "compendium");

            migrationBuilder.DropTable(
                name: "effect_schemas",
                schema: "compendium");

            migrationBuilder.DropTable(
                name: "feature_effects",
                schema: "compendium");

            migrationBuilder.DropTable(
                name: "features",
                schema: "compendium");
        }
    }
}

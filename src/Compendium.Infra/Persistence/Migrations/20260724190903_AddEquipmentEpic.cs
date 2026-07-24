using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Compendium.Infra.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddEquipmentEpic : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "armors",
                schema: "compendium",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    equipment_item_id = table.Column<Guid>(type: "uuid", nullable: false),
                    armor_training_category_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_armors", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "equipment_items",
                schema: "compendium",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    rule_source_id = table.Column<Guid>(type: "uuid", nullable: false),
                    source_version_id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    name = table.Column<string>(type: "character varying(180)", maxLength: 180, nullable: false),
                    category = table.Column<int>(type: "integer", nullable: false),
                    weight = table.Column<decimal>(type: "numeric(10,3)", precision: 10, scale: 3, nullable: false),
                    cost_amount = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false),
                    cost_currency = table.Column<int>(type: "integer", nullable: false),
                    description = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_equipment_items", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "equipment_packs",
                schema: "compendium",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    equipment_item_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_equipment_packs", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "starting_equipment_rules",
                schema: "compendium",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    owner_type = table.Column<int>(type: "integer", nullable: false),
                    owner_entity_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_starting_equipment_rules", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "tools",
                schema: "compendium",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    equipment_item_id = table.Column<Guid>(type: "uuid", nullable: false),
                    proficiency_id = table.Column<Guid>(type: "uuid", nullable: true),
                    ability_code = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_tools", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "weapon_mastery_properties",
                schema: "compendium",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    name = table.Column<string>(type: "character varying(180)", maxLength: 180, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_weapon_mastery_properties", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "weapon_properties",
                schema: "compendium",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    name = table.Column<string>(type: "character varying(180)", maxLength: 180, nullable: false),
                    value_type = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_weapon_properties", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "weapons",
                schema: "compendium",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    equipment_item_id = table.Column<Guid>(type: "uuid", nullable: false),
                    category = table.Column<int>(type: "integer", nullable: false),
                    damage_dice = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    damage_type = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_weapons", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "armor_ac_rules",
                schema: "compendium",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    armor_id = table.Column<Guid>(type: "uuid", nullable: false),
                    base_ac = table.Column<int>(type: "integer", nullable: false),
                    adds_dexterity = table.Column<bool>(type: "boolean", nullable: false),
                    maximum_dexterity_bonus = table.Column<int>(type: "integer", nullable: true),
                    bonus = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_armor_ac_rules", x => x.id);
                    table.ForeignKey(
                        name: "FK_armor_ac_rules_armors_armor_id",
                        column: x => x.armor_id,
                        principalSchema: "compendium",
                        principalTable: "armors",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "armor_drawbacks",
                schema: "compendium",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    armor_id = table.Column<Guid>(type: "uuid", nullable: false),
                    type = table.Column<int>(type: "integer", nullable: false),
                    threshold = table.Column<int>(type: "integer", nullable: true),
                    description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_armor_drawbacks", x => x.id);
                    table.ForeignKey(
                        name: "FK_armor_drawbacks_armors_armor_id",
                        column: x => x.armor_id,
                        principalSchema: "compendium",
                        principalTable: "armors",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "equipment_pack_items",
                schema: "compendium",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    equipment_pack_id = table.Column<Guid>(type: "uuid", nullable: false),
                    equipment_item_id = table.Column<Guid>(type: "uuid", nullable: false),
                    quantity = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_equipment_pack_items", x => x.id);
                    table.ForeignKey(
                        name: "FK_equipment_pack_items_equipment_packs_equipment_pack_id",
                        column: x => x.equipment_pack_id,
                        principalSchema: "compendium",
                        principalTable: "equipment_packs",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "starting_equipment_groups",
                schema: "compendium",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    starting_equipment_rule_id = table.Column<Guid>(type: "uuid", nullable: false),
                    ordinal = table.Column<int>(type: "integer", nullable: false),
                    selection_count = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_starting_equipment_groups", x => x.id);
                    table.ForeignKey(
                        name: "FK_starting_equipment_groups_starting_equipment_rules_starting~",
                        column: x => x.starting_equipment_rule_id,
                        principalSchema: "compendium",
                        principalTable: "starting_equipment_rules",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "weapon_mastery_effects",
                schema: "compendium",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    weapon_mastery_property_id = table.Column<Guid>(type: "uuid", nullable: false),
                    type = table.Column<int>(type: "integer", nullable: false),
                    value = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_weapon_mastery_effects", x => x.id);
                    table.ForeignKey(
                        name: "FK_weapon_mastery_effects_weapon_mastery_properties_weapon_mas~",
                        column: x => x.weapon_mastery_property_id,
                        principalSchema: "compendium",
                        principalTable: "weapon_mastery_properties",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "weapon_mastery_requirements",
                schema: "compendium",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    weapon_mastery_property_id = table.Column<Guid>(type: "uuid", nullable: false),
                    type = table.Column<int>(type: "integer", nullable: false),
                    value = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_weapon_mastery_requirements", x => x.id);
                    table.ForeignKey(
                        name: "FK_weapon_mastery_requirements_weapon_mastery_properties_weapo~",
                        column: x => x.weapon_mastery_property_id,
                        principalSchema: "compendium",
                        principalTable: "weapon_mastery_properties",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "weapon_property_rules",
                schema: "compendium",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    weapon_property_id = table.Column<Guid>(type: "uuid", nullable: false),
                    field = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    @operator = table.Column<string>(name: "operator", type: "character varying(40)", maxLength: 40, nullable: false),
                    value = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_weapon_property_rules", x => x.id);
                    table.ForeignKey(
                        name: "FK_weapon_property_rules_weapon_properties_weapon_property_id",
                        column: x => x.weapon_property_id,
                        principalSchema: "compendium",
                        principalTable: "weapon_properties",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "weapon_property_links",
                schema: "compendium",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    weapon_id = table.Column<Guid>(type: "uuid", nullable: false),
                    weapon_property_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_weapon_property_links", x => x.id);
                    table.ForeignKey(
                        name: "FK_weapon_property_links_weapons_weapon_id",
                        column: x => x.weapon_id,
                        principalSchema: "compendium",
                        principalTable: "weapons",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "starting_equipment_options",
                schema: "compendium",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    starting_equipment_group_id = table.Column<Guid>(type: "uuid", nullable: false),
                    ordinal = table.Column<int>(type: "integer", nullable: false),
                    type = table.Column<int>(type: "integer", nullable: false),
                    reference_id = table.Column<Guid>(type: "uuid", nullable: false),
                    quantity = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_starting_equipment_options", x => x.id);
                    table.ForeignKey(
                        name: "FK_starting_equipment_options_starting_equipment_groups_starti~",
                        column: x => x.starting_equipment_group_id,
                        principalSchema: "compendium",
                        principalTable: "starting_equipment_groups",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "weapon_property_link_values",
                schema: "compendium",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    weapon_property_link_id = table.Column<Guid>(type: "uuid", nullable: false),
                    ordinal = table.Column<int>(type: "integer", nullable: false),
                    value = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_weapon_property_link_values", x => x.id);
                    table.ForeignKey(
                        name: "FK_weapon_property_link_values_weapon_property_links_weapon_pr~",
                        column: x => x.weapon_property_link_id,
                        principalSchema: "compendium",
                        principalTable: "weapon_property_links",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_armor_ac_rules_armor_id",
                schema: "compendium",
                table: "armor_ac_rules",
                column: "armor_id");

            migrationBuilder.CreateIndex(
                name: "IX_armor_drawbacks_armor_id",
                schema: "compendium",
                table: "armor_drawbacks",
                column: "armor_id");

            migrationBuilder.CreateIndex(
                name: "IX_armors_equipment_item_id",
                schema: "compendium",
                table: "armors",
                column: "equipment_item_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_equipment_items_category",
                schema: "compendium",
                table: "equipment_items",
                column: "category");

            migrationBuilder.CreateIndex(
                name: "ux_equipment_items_code",
                schema: "compendium",
                table: "equipment_items",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_equipment_pack_items_equipment_pack_id_equipment_item_id",
                schema: "compendium",
                table: "equipment_pack_items",
                columns: new[] { "equipment_pack_id", "equipment_item_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_equipment_packs_equipment_item_id",
                schema: "compendium",
                table: "equipment_packs",
                column: "equipment_item_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_starting_equipment_groups_starting_equipment_rule_id",
                schema: "compendium",
                table: "starting_equipment_groups",
                column: "starting_equipment_rule_id");

            migrationBuilder.CreateIndex(
                name: "IX_starting_equipment_options_starting_equipment_group_id",
                schema: "compendium",
                table: "starting_equipment_options",
                column: "starting_equipment_group_id");

            migrationBuilder.CreateIndex(
                name: "IX_starting_equipment_rules_owner_type_owner_entity_id",
                schema: "compendium",
                table: "starting_equipment_rules",
                columns: new[] { "owner_type", "owner_entity_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tools_equipment_item_id",
                schema: "compendium",
                table: "tools",
                column: "equipment_item_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_weapon_mastery_effects_weapon_mastery_property_id",
                schema: "compendium",
                table: "weapon_mastery_effects",
                column: "weapon_mastery_property_id");

            migrationBuilder.CreateIndex(
                name: "IX_weapon_mastery_properties_code",
                schema: "compendium",
                table: "weapon_mastery_properties",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_weapon_mastery_requirements_weapon_mastery_property_id",
                schema: "compendium",
                table: "weapon_mastery_requirements",
                column: "weapon_mastery_property_id");

            migrationBuilder.CreateIndex(
                name: "IX_weapon_properties_code",
                schema: "compendium",
                table: "weapon_properties",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_weapon_property_link_values_weapon_property_link_id",
                schema: "compendium",
                table: "weapon_property_link_values",
                column: "weapon_property_link_id");

            migrationBuilder.CreateIndex(
                name: "IX_weapon_property_links_weapon_id_weapon_property_id",
                schema: "compendium",
                table: "weapon_property_links",
                columns: new[] { "weapon_id", "weapon_property_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_weapon_property_rules_weapon_property_id",
                schema: "compendium",
                table: "weapon_property_rules",
                column: "weapon_property_id");

            migrationBuilder.CreateIndex(
                name: "IX_weapons_equipment_item_id",
                schema: "compendium",
                table: "weapons",
                column: "equipment_item_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "armor_ac_rules",
                schema: "compendium");

            migrationBuilder.DropTable(
                name: "armor_drawbacks",
                schema: "compendium");

            migrationBuilder.DropTable(
                name: "equipment_items",
                schema: "compendium");

            migrationBuilder.DropTable(
                name: "equipment_pack_items",
                schema: "compendium");

            migrationBuilder.DropTable(
                name: "starting_equipment_options",
                schema: "compendium");

            migrationBuilder.DropTable(
                name: "tools",
                schema: "compendium");

            migrationBuilder.DropTable(
                name: "weapon_mastery_effects",
                schema: "compendium");

            migrationBuilder.DropTable(
                name: "weapon_mastery_requirements",
                schema: "compendium");

            migrationBuilder.DropTable(
                name: "weapon_property_link_values",
                schema: "compendium");

            migrationBuilder.DropTable(
                name: "weapon_property_rules",
                schema: "compendium");

            migrationBuilder.DropTable(
                name: "armors",
                schema: "compendium");

            migrationBuilder.DropTable(
                name: "equipment_packs",
                schema: "compendium");

            migrationBuilder.DropTable(
                name: "starting_equipment_groups",
                schema: "compendium");

            migrationBuilder.DropTable(
                name: "weapon_mastery_properties",
                schema: "compendium");

            migrationBuilder.DropTable(
                name: "weapon_property_links",
                schema: "compendium");

            migrationBuilder.DropTable(
                name: "weapon_properties",
                schema: "compendium");

            migrationBuilder.DropTable(
                name: "starting_equipment_rules",
                schema: "compendium");

            migrationBuilder.DropTable(
                name: "weapons",
                schema: "compendium");
        }
    }
}

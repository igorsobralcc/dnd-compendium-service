# Internal HTTP Contract Versioning

Internal public DTOs use the `V1` suffix and include `apiVersion: "v1"` in the body when the
response represents a composite projection. Consumers depend on these contracts, never on domain
entities or EF Core models.

Optional additive changes may remain in the current version. Renaming or removing fields, or
changing their type, semantics, nullability, or cardinality requires a new DTO and a new route or
media type version. The previous version remains covered by contract tests during the migration
window agreed upon with the BFF, Character Builder, and Rules Engine.

The contracts currently covered include sources, classes, creation options, mechanical details,
the change feed, and translations. Contracts for spells, species, backgrounds, and feats will be
added when those aggregates exist; collections that have not yet been implemented remain empty in
`v1`, without arbitrary JSON payloads.

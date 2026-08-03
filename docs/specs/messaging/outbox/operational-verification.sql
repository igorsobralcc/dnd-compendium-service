-- Outbox production-like verification worksheet.
-- Run with timing enabled and retain the output in the deployment review.
-- Replace no values in UPDATE/DELETE statements: this script is read-only.

-- 1. PostgreSQL statement-statistics availability.
SELECT EXISTS (
    SELECT 1 FROM pg_extension WHERE extname = 'pg_stat_statements'
) AS pg_stat_statements_available;

-- 2. Outbox statement frequency and cost over the statistics window.
SELECT calls,
       round(mean_exec_time::numeric, 3) AS mean_exec_time_ms,
       round(total_exec_time::numeric, 3) AS total_exec_time_ms,
       rows,
       left(query, 500) AS query_sample
FROM pg_stat_statements
WHERE query ILIKE '%integration_outbox%'
ORDER BY total_exec_time DESC;

-- 3. Status, age, and relation-size baseline.
SELECT status,
       count(*) AS row_count,
       min(created_at_utc) AS oldest_created_at_utc,
       max(created_at_utc) AS newest_created_at_utc
FROM compendium.integration_outbox
GROUP BY status
ORDER BY status;

SELECT pg_size_pretty(pg_total_relation_size('compendium.integration_outbox')) AS total_size,
       pg_size_pretty(pg_relation_size('compendium.integration_outbox')) AS table_size,
       pg_size_pretty(pg_indexes_size('compendium.integration_outbox')) AS indexes_size;

SELECT indexrelid::regclass AS index_name,
       pg_size_pretty(pg_relation_size(indexrelid)) AS index_size,
       indisvalid AS is_valid
FROM pg_index
WHERE indrelid = 'compendium.integration_outbox'::regclass
ORDER BY indexrelid::regclass::text;

-- 4. Query-plan evidence. Use representative parameter values for the target
-- environment. EXPLAIN ANALYZE executes the read-only SELECTs below.
EXPLAIN (ANALYZE, BUFFERS, SETTINGS)
SELECT id
FROM compendium.integration_outbox
WHERE status IN ('PENDING', 'FAILED')
  AND available_at_utc <= clock_timestamp()
ORDER BY created_at_utc
LIMIT 50;

EXPLAIN (ANALYZE, BUFFERS, SETTINGS)
SELECT count(*)
FROM compendium.integration_outbox
WHERE status IN ('PENDING', 'FAILED', 'PROCESSING');

EXPLAIN (ANALYZE, BUFFERS, SETTINGS)
SELECT id
FROM compendium.integration_outbox
WHERE status = 'PUBLISHED'
  AND published_at_utc < clock_timestamp() - interval '30 days'
ORDER BY published_at_utc
LIMIT 1000;

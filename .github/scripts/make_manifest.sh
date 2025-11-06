#!/usr/bin/env bash
set -euo pipefail

TEMPLATE_PATH="${1:-manifest.template.json}"
OUT_PATH="${2:-public/manifest.json}"

if [[ ! -f "$TEMPLATE_PATH" ]]; then
  echo "Template not found: $TEMPLATE_PATH" >&2
  exit 1
fi

: "${OWNER:?OWNER environment variable is required}"
: "${VERSIONS_JSON:?VERSIONS_JSON environment variable is required (JSON array)}"

mkdir -p "$(dirname "$OUT_PATH")"

# Escape characters that would confuse sed replacement (notably & and /)
ESCAPED_VERSIONS=$(printf '%s' "$VERSIONS_JSON" | sed -e 's/[&/]/\\&/g')

sed -e "s|__OWNER__|$OWNER|g" \
    -e "s|__VERSIONS__|$ESCAPED_VERSIONS|" \
    "$TEMPLATE_PATH" > "$OUT_PATH"

echo "Wrote manifest to $OUT_PATH"


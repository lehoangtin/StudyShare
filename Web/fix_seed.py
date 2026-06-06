import re

with open('/Users/mac/Documents/PBL3/Web/seed_data.sql', 'r') as f:
    content = f.read()

# Find all unique GUIDs
guids = list(set(re.findall(r'[0-9A-Fa-f]{8}-[0-9A-Fa-f]{4}-[0-9A-Fa-f]{4}-[0-9A-Fa-f]{4}-[0-9A-Fa-f]{12}', content)))

# Generate DECLARE statements
declares = ""
for i, guid in enumerate(guids):
    declares += f"DECLARE @U{i} NVARCHAR(450) = (SELECT Id FROM AspNetUsers ORDER BY Id OFFSET {i} ROWS FETCH NEXT 1 ROWS ONLY);\n"
    # Replace in content (case-insensitive)
    content = re.sub(f"'{guid}'", f"@U{i}", content, flags=re.IGNORECASE)

# Remove USE and GO at the top
content = re.sub(r'USE PBL3;\nGO\n', '', content)

final_content = declares + "\n" + content

with open('/Users/mac/Documents/PBL3/Web/seed_data_azure.sql', 'w') as f:
    f.write(final_content)

print("Fixed seed data created successfully!")

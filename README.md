# CSV to JSON Taxonomy Converter

This script (`ConvertToJSON.py`) reads a structured biological dataset from a CSV file and generates a deeply nested JSON file representing taxonomic hierarchies (Kingdom → Phylum → Class → Order → Family → Genus → Species). This is useful for powering data visualizations, taxonomic browsers, and Unity-based ecological applications.

---

## File Structure

- `docs/Data.csv` — Input file containing the tabular dataset
- `docs/Data.json` — Output file with the nested JSON structure
- `ConvertToJSON.py` — Script that performs the conversion

---

## CSV Requirements

The CSV file **must contain the following column headers**:

| Column Name        | Description                                                                 |
|--------------------|-----------------------------------------------------------------------------|
| `Kingdom`          | Top-level classification                                                    |
| `Phylum`           | Second-level classification                                                 |
| `Class`            | Third-level classification                                                  |
| `Order`            | Fourth-level classification                                                 |
| `Family`           | Fifth-level classification                                                  |
| `Genus`            | Sixth-level classification                                                  |
| `Species`          | Seventh-level classification                                                |
| `Common Name`      | Human-readable species name                                                 |
| `Model`            | File path or name of 3D model used to represent the taxon                   |
| `Model Level`      | One of: `Kingdom`, `Phylum`, `Class`, `Order`, `Family`, `Genus`, or `Species` |
| `Sedentary`        | `TRUE` or `FALSE` (not case-sensitive)                                      |
| `Max Depth (m)`    | Maximum depth (in meters) where the organism can be found                   |
| `Min Depth (m)`    | Minimum depth (in meters) where the organism can be found                   |
| `count`            | Number of observed specimens                                                |

---

## How It Works

The script:
1. Reads the CSV row-by-row.
2. Builds a nested JSON tree based on taxonomy.
3. Adds models only at the specified `Model Level`.
4. Tracks counts and min/max depths across taxonomic levels.
5. Outputs a formatted `Data.json` file.

---

## Running the Script

Make sure your working directory contains the required files. Then run:

```bash
python ConvertToJSON.py
```

This will:
* Read docs/Data.csv
* Generate a pretty-formatted docs/Data.json

## Example CSV Row
Kingdom,Phylum,Class,Order,Family,Genus,Species,Common Name,Model,Model Level,Sedentary,Max Depth (m),Min Depth (m),count
Animalia,Chordata,Actinopterygii,Perciformes,Cichlidae,Tropheus,Tropheus duboisi,White-Spotted Cichlid,Tropheus.glb,Species,TRUE,15,5,20

## Output Format (Example)
{
  "totalCount": 20,
  "numKingdoms": 1,
  "Kingdoms": [
    {
      "name": "Animalia",
      "model": "",
      "count": 20,
      "sedentary": true,
      "maxDepth": 15,
      "minDepth": 5,
      "numPhyla": 1,
      "Phyla": [
        ...
      ]
    }
  ]
}

---

## Notes
* Booleans like Sedentary must be TRUE or FALSE (case-insensitive).
* Depth values must be numeric.
* All fields should be filled — missing data may cause unexpected behavior or missing branches.
* If a model is listed at the wrong level or nested under another model for a higher taxon level, it may not appear in the JSON.

---

## Contact
For issues or help with data formatting, reach out to [Justin Morera](mailto:mustinjorera@gmail.com).
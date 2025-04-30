# Project Summary
We have developed a virtual reality experience that allows users to immerse themselves directly inside a marine biodiversity dataset based on real-life ecological survey data. The experience offers hands-on data visualization and immersion for both educational and entertainment purposes. The goal of the project is to expand on traditional two-dimensional views of large datasets by offering unique angles and more personal interaction with the data. 

The environment features three main scenes for data visualization:
* An aquarium with a scrolling side view to examine organisms in the data at the depths they were observed in
* A habitat scene that allows users to get up-close-and-personal with the data by literally swimming around with the data
* A dock with a fun fishing `mini-game' where users catch organisms based on their actual proportion in the dataset and can see their progress on a graph that filters by taxonomic level

The organisms modeled in the environment come from a structured JSON data file that can be automatically generated from a Python script using a CSV file with each organism’s taxonomic info, common name, observation count, observed depth range, and 3D model location. During the experience, these data can be seen in a straightforward window by selecting specimens with the right controller. Additionally, the user can filter the population density and the types of organisms shown based on depth range, maximum observation count, and taxonomy.

More features and fixes are planned in the future!

# Usage for VR
## Running the Application (VR) - For Users and Data Scientist

1. Download the latest build [here](https://drive.google.com/drive/folders/1Mi9cVYlLdTlemFYia55Ezm7GIt1kFDYH?usp=drive_link).
2. Make sure your VR headset (e.g., Oculus Rift, Quest via Link, HTC Vive) is connected and recognized by SteamVR or OpenXR.
3. Double-click `CAP6119Project-DataVisualization.exe` to launch the VR experience.
4. Put on your headset and begin exploring!

**Note:** This app has only been tested on Meta Quest 2.

**Note:** This app requires a compatible OpenXR runtime (SteamVR, Oculus, etc.).

---

## Building the VR Project in Unity - For Developers

1. Open `CAP6119Project-DataVisualization` in Unity (tested with Unity 2023.1.0f1 or newer).
2. Ensure your VR SDK is installed:
   - Unity XR Plug-in Management
   - OpenXR (with necessary feature groups enabled)
   - XR Interaction Toolkit
3. Connect your VR headset (Oculus, SteamVR, etc.) and ensure it’s active.
4. Go to `File > Build Settings`.
   - Select `PC, Mac & Linux Standalone > Windows`
   - Add the main scene(s) to the build list
   - Click **Build and Run**
5. Unity will generate a `.exe` you can use to launch the experience.

---

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
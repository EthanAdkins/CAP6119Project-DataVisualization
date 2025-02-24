import csv
import json

with open("docs/Data.csv", "r", encoding="utf-8") as observations:
    with open("docs/Data.json", "w", encoding="utf-8") as output:
        reader = csv.DictReader(observations)
        data = []
        for row in reader:
            data.append(row)
        print(data)
        json.dump(data, output, indent=4)  # Pretty formatting with 4 spaces

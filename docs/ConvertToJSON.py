import csv
import json

with open("docs/Data.csv", "r", encoding="utf-8") as observations:
    with open("docs/Data.json", "w", encoding="utf-8") as output:
        reader = csv.DictReader(observations)
        data = {
            "totalCount": 0,
            "numKingdoms": 0,
            "Kingdoms": []
        }
        
        for row in reader:
            modelLevel = row["Model Level"]
            kingdom = row["Kingdom"]
            phylum = row["Phylum"]
            dataClass = row["Class"]
            order = row["Order"]
            family = row["Family"]
            genus = row["Genus"]
            species = row["Species"]
            commonName = row["Common Name"]
            modelLocation = row["Model"]
            modelLevel = row["Model Level"]
            sedentary = bool(row["Sedentary"].strip().lower() == "TRUE")
            maxDepth = round(float(row["Max Depth (m)"]))
            minDepth = round(float(row["Min Depth (m)"]))
            count = int(row["count"])

            data["totalCount"] += count
            if not any(k["name"] == kingdom for k in data["Kingdoms"]):
                data["numKingdoms"] += 1
                data["Kingdoms"].append(
                    {
                        "name": kingdom,
                        "model": modelLocation if modelLevel == "Kingdom" else "",
                        "count": count,
                        "sedentary": sedentary,
                        "maxDepth": maxDepth,
                        "minDepth": minDepth,
                        "numPhyla": 1,
                        "Phyla": 
                        [
                            {
                                "name": phylum,
                                "model": modelLocation if modelLevel == "Phylum" else "",
                                "count": count,
                                "sedentary": sedentary,
                                "maxDepth": maxDepth,
                                "minDepth": minDepth,
                                "numClasses": 1,
                                "Classes": 
                                [
                                    {
                                        "name": dataClass,
                                        "model": modelLocation if modelLevel == "Class" else "",
                                        "count": count,
                                        "sedentary": sedentary,
                                        "maxDepth": maxDepth,
                                        "minDepth": minDepth,
                                        "numOrders": 1,
                                        "Orders": 
                                        [
                                            {
                                                "name": order,
                                                "model": modelLocation if modelLevel == "Order" else "",
                                                "count": count,
                                                "sedentary": sedentary,
                                                "maxDepth": maxDepth,
                                                "minDepth": minDepth,
                                                "numFamilies": 1,
                                                "Families": 
                                                [
                                                    {
                                                        "name": family,
                                                        "model": modelLocation if modelLevel == "Family" else "",
                                                        "count": count,
                                                        "sedentary": sedentary,
                                                        "maxDepth": maxDepth,
                                                        "minDepth": minDepth,
                                                        "numGenera": 1,
                                                        "Genera": 
                                                        [
                                                            {
                                                                "name": genus,
                                                                "model": modelLocation if modelLevel == "Genus" else "",
                                                                "count": count,
                                                                "sedentary": sedentary,
                                                                "maxDepth": maxDepth,
                                                                "minDepth": minDepth,
                                                                "numSpecies": 1,
                                                                "Species": 
                                                                [
                                                                    {
                                                                        "name": species,
                                                                        "commonName": commonName,
                                                                        "model": modelLocation if modelLevel == "Species" else "",
                                                                        "count": count,
                                                                        "sedentary": sedentary,
                                                                        "maxDepth": maxDepth,
                                                                        "minDepth": minDepth
                                                                    }
                                                                ]
                                                            }
                                                        ]
                                                    }
                                                ]
                                            }
                                        ]
                                    } 
                                ]
                            }
                        ]
                    }
                )
            else:
                k = next(k for k in data["Kingdoms"] if k["name"] == kingdom)
                k["count"] += count
                if maxDepth > k["maxDepth"]:
                    k["maxDepth"] = maxDepth
                if minDepth < k["minDepth"]:
                    k["minDepth"] = minDepth

                if not any(p["name"] == phylum for p in k["Phyla"]):
                    k["numPhyla"] += 1
                    k["Phyla"].append(
                        {
                            "name": phylum,
                            "model": modelLocation if modelLevel == "Phylum" else "",
                            "count": count,
                            "sedentary": sedentary,
                            "maxDepth": maxDepth,
                            "minDepth": minDepth,
                            "numClasses": 1,
                            "Classes": 
                            [
                                {
                                    "name": dataClass,
                                    "model": modelLocation if modelLevel == "Class" else "",
                                    "count": count,
                                    "sedentary": sedentary,
                                    "maxDepth": maxDepth,
                                    "minDepth": minDepth,
                                    "numOrders": 1,
                                    "Orders": 
                                    [
                                        {
                                            "name": order,
                                            "model": modelLocation if modelLevel == "Order" else "",
                                            "count": count,
                                            "sedentary": sedentary,
                                            "maxDepth": maxDepth,
                                            "minDepth": minDepth,
                                            "numFamilies": 1,
                                            "Families": 
                                            [
                                                {
                                                    "name": family,
                                                    "model": modelLocation if modelLevel == "Family" else "",
                                                    "count": count,
                                                    "sedentary": sedentary,
                                                    "maxDepth": maxDepth,
                                                    "minDepth": minDepth,
                                                    "numGenera": 1,
                                                    "Genera": 
                                                    [
                                                        {
                                                            "name": genus,
                                                            "model": modelLocation if modelLevel == "Genus" else "",
                                                            "count": count,
                                                            "sedentary": sedentary,
                                                            "maxDepth": maxDepth,
                                                            "minDepth": minDepth,
                                                            "numSpecies": 1,
                                                            "Species": 
                                                            [
                                                                {
                                                                    "name": species,
                                                                    "commonName": commonName,
                                                                    "model": modelLocation if modelLevel == "Species" else "",
                                                                    "count": count,
                                                                    "sedentary": sedentary,
                                                                    "maxDepth": maxDepth,
                                                                    "minDepth": minDepth
                                                                }
                                                            ]
                                                        }
                                                    ]
                                                }
                                            ]
                                        }
                                    ]
                                } 
                            ]
                        }
                    )
                else:
                    p = next(p for p in k["Phyla"] if p["name"] == phylum)
                    p["count"] += count
                    if maxDepth > p["maxDepth"]:
                        p["maxDepth"] = maxDepth
                    if minDepth < p["minDepth"]:
                        p["minDepth"] = minDepth

                    if not any(c["name"] == dataClass for c in p["Classes"]):
                        p["numClasses"] += 1
                        p["Classes"].append(
                            {
                                "name": dataClass,
                                "model": modelLocation if modelLevel == "Class" else "",
                                "count": count,
                                "sedentary": sedentary,
                                "maxDepth": maxDepth,
                                "minDepth": minDepth,
                                "numOrders": 1,
                                "Orders": 
                                [
                                    {
                                        "name": order,
                                        "model": modelLocation if modelLevel == "Order" else "",
                                        "count": count,
                                        "sedentary": sedentary,
                                        "maxDepth": maxDepth,
                                        "minDepth": minDepth,
                                        "numFamilies": 1,
                                        "Families": 
                                        [
                                            {
                                                "name": family,
                                                "model": modelLocation if modelLevel == "Family" else "",
                                                "count": count,
                                                "sedentary": sedentary,
                                                "maxDepth": maxDepth,
                                                "minDepth": minDepth,
                                                "numGenera": 1,
                                                "Genera": 
                                                [
                                                    {
                                                        "name": genus,
                                                        "model": modelLocation if modelLevel == "Genus" else "",
                                                        "count": count,
                                                        "sedentary": sedentary,
                                                        "maxDepth": maxDepth,
                                                        "minDepth": minDepth,
                                                        "numSpecies": 1,
                                                        "Species": 
                                                        [
                                                            {
                                                                "name": species,
                                                                "commonName": commonName,
                                                                "model": modelLocation if modelLevel == "Species" else "",
                                                                "count": count,
                                                                "sedentary": sedentary,
                                                                "maxDepth": maxDepth,
                                                                "minDepth": minDepth
                                                            }
                                                        ]
                                                    }
                                                ]
                                            }
                                        ]
                                    }
                                ]
                            }
                        )
                    else:
                        c = next(c for c in p["Classes"] if c["name"] == dataClass)
                        c["count"] += count
                        if maxDepth > c["maxDepth"]:
                            c["maxDepth"] = maxDepth
                        if minDepth < c["minDepth"]:
                            c["minDepth"] = minDepth

                        if not any(o["name"] == order for o in c["Orders"]):
                            c["numOrders"] += 1
                            c["Orders"].append(
                                {
                                    "name": order,
                                    "model": modelLocation if modelLevel == "Order" else "",
                                    "count": count,
                                    "sedentary": sedentary,
                                    "maxDepth": maxDepth,
                                    "minDepth": minDepth,
                                    "numFamilies": 1,
                                    "Families": 
                                    [
                                        {
                                            "name": family,
                                            "model": modelLocation if modelLevel == "Family" else "",
                                            "count": count,
                                            "sedentary": sedentary,
                                            "maxDepth": maxDepth,
                                            "minDepth": minDepth,
                                            "numGenera": 1,
                                            "Genera": 
                                            [
                                                {
                                                    "name": genus,
                                                    "model": modelLocation if modelLevel == "Genus" else "",
                                                    "count": count,
                                                    "sedentary": sedentary,
                                                    "maxDepth": maxDepth,
                                                    "minDepth": minDepth,
                                                    "numSpecies": 1,
                                                    "Species": 
                                                    [
                                                        {
                                                            "name": species,
                                                            "commonName": commonName,
                                                            "model": modelLocation if modelLevel == "Species" else "",
                                                            "count": count,
                                                            "sedentary": sedentary,
                                                            "maxDepth": maxDepth,
                                                            "minDepth": minDepth
                                                        }
                                                    ]
                                                }
                                            ]
                                        }
                                    ]
                                }
                            )
                        else:
                            o = next(o for o in c["Orders"] if o["name"] == order)
                            o["count"] += count
                            if maxDepth > o["maxDepth"]:
                                o["maxDepth"] = maxDepth
                            if minDepth < o["minDepth"]:
                                o["minDepth"] = minDepth

                            if not any(f["name"] == family for f in o["Families"]):
                                o["numFamilies"] += 1
                                o["Families"].append(
                                    {
                                        "name": family,
                                        "model": modelLocation if modelLevel == "Family" else "",
                                        "count": count,
                                        "sedentary": sedentary,
                                        "maxDepth": maxDepth,
                                        "minDepth": minDepth,
                                        "numGenera": 1,
                                        "Genera": 
                                        [
                                            {
                                                "name": genus,
                                                "model": modelLocation if modelLevel == "Genus" else "",
                                                "count": count,
                                                "sedentary": sedentary,
                                                "maxDepth": maxDepth,
                                                "minDepth": minDepth,
                                                "numSpecies": 1,
                                                "Species": 
                                                [
                                                    {
                                                        "name": species,
                                                        "commonName": commonName,
                                                        "model": modelLocation if modelLevel == "Species" else "",
                                                        "count": count,
                                                        "sedentary": sedentary,
                                                        "maxDepth": maxDepth,
                                                        "minDepth": minDepth
                                                    }
                                                ]
                                            }
                                        ]
                                    }
                                )
                            else:
                                f = next(f for f in o["Families"] if f["name"] == family)
                                f["count"] += count
                                if maxDepth > f["maxDepth"]:
                                    f["maxDepth"] = maxDepth
                                if minDepth < f["minDepth"]:
                                    f["minDepth"] = minDepth
                                if not any(g["name"] == genus for g in f["Genera"]):
                                    f["numGenera"] += 1
                                    f["Genera"].append(
                                        {
                                            "name": genus,
                                            "model": modelLocation if modelLevel == "Genus" else "",
                                            "count": count,
                                            "sedentary": sedentary,
                                            "maxDepth": maxDepth,
                                            "minDepth": minDepth,
                                            "numSpecies": 1,
                                            "Species": 
                                            [
                                                {
                                                    "name": species,
                                                    "commonName": commonName,
                                                    "model": modelLocation if modelLevel == "Species" else "",
                                                    "count": count,
                                                    "sedentary": sedentary,
                                                    "maxDepth": maxDepth,
                                                    "minDepth": minDepth
                                                }
                                            ]
                                        }
                                    )
                                else:
                                    g = next(g for g in f["Genera"] if g["name"] == genus)
                                    g["count"] += count
                                    if maxDepth > g["maxDepth"]:
                                        g["maxDepth"] = maxDepth
                                    if minDepth < g["minDepth"]:
                                        g["minDepth"] = minDepth
                                    if not any(s["name"] == species for s in g["Species"]):
                                        g["numSpecies"] += 1
                                        g["Species"].append(
                                            {
                                                "name": species,
                                                "commonName": commonName,
                                                "model": modelLocation if modelLevel == "Species" else "",
                                                "count": count,
                                                "sedentary": sedentary,
                                                "maxDepth": maxDepth,
                                                "minDepth": minDepth
                                            }
                                        )
          
        print(data)
        
        json.dump(data, output, indent=4)  # Pretty formatting with 4 spaces
